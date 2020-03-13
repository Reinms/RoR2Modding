﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RogueWispPlugin.Helpers;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;
using ReinCore;

namespace RogueWispPlugin
{
    internal partial class Main
    {
        internal static String skinnedProjExplosionString = "SKINNEDTHING";
        private static HashSet<GameObject> skinnedEffects = new HashSet<GameObject>();
        private static Dictionary<EffectIndex,Dictionary<UInt32,EffectDef>> skinnedEffectCache = new Dictionary<EffectIndex, Dictionary<UInt32, EffectDef>>();
        partial void SetupEffectSkinning()
        {
            this.Enable += this.Main_Enable;
            this.Disable += this.Main_Disable;
            this.FirstFrame += this.Main_FirstFrame1;
        }

        internal static void RegisterEffect( GameObject effect )
        {
            var bitSkin = effect.GetComponent<BitSkinController>();
            if( bitSkin != null )
            {
                skinnedEffects.Add( effect );
            }

            // TODO: Effect
            EffectsCore.AddEffect( effect );
        }

        internal static void RegisterProjectile( GameObject projectile )
        {
            var controller = projectile.GetComponent<ProjectileController>();
            if( controller != null )
            {
                var ghost = controller.ghostPrefab;
                if( ghost != null )
                {
                    var bitSkin = ghost.GetComponent<BitSkinController>();
                    if( bitSkin != null )
                    {
                        skinnedProjectiles.Add( projectile );
                    }
                }

                ProjectileCatalog.getAdditionalEntries += ( list ) => list.Add( projectile );
            } else
            {
                Main.LogE( "No ProjectileController on: " + projectile.name );
            }
        }

        private void Main_FirstFrame1()
        {
            foreach( var effect in skinnedEffects )
            {
                var ind = EffectCatalog.FindEffectIndexFromPrefab(effect);
                if( ind == EffectIndex.Invalid )
                {
                    Main.LogE( "Invalid effect: " + effect.name );
                    continue;
                }

                skinnedEffectCache[ind] = new Dictionary<UInt32, EffectDef>();
            }

            foreach( var projectile in skinnedProjectiles )
            {
                var ind = ProjectileCatalog.GetProjectileIndex( projectile );
                if( ind == -1 )
                {
                    Main.LogE( "Invalid projectile: " + projectile.name );
                    continue;
                }

                skinnedGhostCache[ind] = new Dictionary<UInt32, GameObject>();
            }
        }

        private void Main_Disable()
        {
            HooksCore.on_RoR2_Projectile_ProjectileController_Start -= this.HooksCore_on_RoR2_Projectile_ProjectileController_Start;
            HooksCore.il_RoR2_EffectManager_SpawnEffect___EffectIndex_EffectData_Boolean -= this.HooksCore_il_RoR2_EffectManager_SpawnEffect___EffectIndex_EffectData_Boolean;
            HooksCore.il_RoR2_Projectile_ProjectileImpactExplosion_FixedUpdate -= this.HooksCore_il_RoR2_Projectile_ProjectileImpactExplosion_FixedUpdate;
        }
        private void Main_Enable()
        {
            HooksCore.on_RoR2_Projectile_ProjectileController_Start += this.HooksCore_on_RoR2_Projectile_ProjectileController_Start;
            HooksCore.il_RoR2_EffectManager_SpawnEffect___EffectIndex_EffectData_Boolean += this.HooksCore_il_RoR2_EffectManager_SpawnEffect___EffectIndex_EffectData_Boolean;
            HooksCore.il_RoR2_Projectile_ProjectileImpactExplosion_FixedUpdate += this.HooksCore_il_RoR2_Projectile_ProjectileImpactExplosion_FixedUpdate;

            //On.RoR2.Projectile.ProjectileController.Start += this.ProjectileController_Start1;
            //IL.RoR2.EffectManager.SpawnEffect_EffectIndex_EffectData_bool += this.EffectManager_SpawnEffect_EffectIndex_EffectData_bool1;
            //IL.RoR2.Projectile.ProjectileImpactExplosion.FixedUpdate += this.ProjectileImpactExplosion_FixedUpdate;



            var instanceParam = Expression.Parameter( typeof( ProjectileImpactExplosion ), "instance" );
            var field = Expression.Field( instanceParam, "projectileController" );
            getController = Expression.Lambda<Func<ProjectileImpactExplosion, ProjectileController>>( field, instanceParam ).Compile();
        }

        private void HooksCore_il_RoR2_Projectile_ProjectileImpactExplosion_FixedUpdate( ILContext il )
        {
            ILCursor c = new ILCursor( il );
            c.GotoNext( MoveType.Before,
                x => x.MatchLdcI4( 1 ),
                x => x.MatchCall( typeof( RoR2.EffectManager ), nameof( EffectManager.SpawnEffect ) )
            );
            c.Emit( OpCodes.Dup );
            c.Emit( OpCodes.Ldarg_0 );
            c.EmitDelegate<Action<EffectData, ProjectileImpactExplosion>>( ( data, self ) =>
            {
                //Main.LogI( "Check1" );
                if( self.explosionSoundString == Main.skinnedProjExplosionString )
                {
                    //Main.LogI( "sound matched" );
                    var ownerBody = getController(self)?.owner?.GetComponent<CharacterBody>();
                    if( ownerBody != null ) data.genericUInt = ownerBody.skinIndex;
                }
            } );
        }
        private void HooksCore_il_RoR2_EffectManager_SpawnEffect___EffectIndex_EffectData_Boolean( ILContext il )
        {
            ILCursor c = new ILCursor( il );

            c.GotoNext( MoveType.Before, x => x.MatchCall( typeof( RoR2.EffectCatalog ), "GetEffectDef" ) );
            c.Remove();
            c.Emit( OpCodes.Ldarg_1 );
            c.EmitDelegate<Func<EffectIndex, EffectData, EffectDef>>( ( index, data ) =>
            {
                if( skinnedEffectCache.ContainsKey( index ) )
                {
                    var cache = skinnedEffectCache[index];
                    var skinInd = data.genericUInt;
                    if( cache.ContainsKey( skinInd ) )
                    {
                        return cache[skinInd];
                    } else
                    {
                        var origDef = EffectCatalog.GetEffectDef( index );
                        var skinnedPrefab = origDef.prefab.ClonePrefab( "CachedEffect", false );
                        var skinController = skinnedPrefab.GetComponent<BitSkinController>();
                        skinController.Apply( WispBitSkin.GetWispSkin( skinInd ) );
                        Destroy( skinController );
                        var newDef = new EffectDef()
                        {
                            cullMethod = origDef.cullMethod,
                            index = index,
                            prefab = skinnedPrefab,
                            prefabEffectComponent = skinnedPrefab.GetComponent<EffectComponent>(),
                            prefabName = origDef.prefabName,
                            prefabVfxAttributes = skinnedPrefab.GetComponent<VFXAttributes>(),
                            spawnSoundEventName = origDef.spawnSoundEventName
                        };
                        

                        cache[skinInd] = newDef;
                        return newDef;
                    }
                } else
                {
                    return EffectCatalog.GetEffectDef( index );
                }
            } );
        }
        private void HooksCore_on_RoR2_Projectile_ProjectileController_Start( HooksCore.orig_RoR2_Projectile_ProjectileController_Start orig, ProjectileController self )
        {
            //Main.LogI( "ProjController start" );
            //Main.LogI( "isPrediction: " + self.isPrediction );
            //Main.LogI( "allowPrediction: " + self.allowPrediction );
            //Main.LogI( "networkserveractive: " + NetworkServer.active );
            //Main.LogI( "authority: " + self.hasAuthority );
            if( skinnedGhostCache.ContainsKey( self.catalogIndex ) && self.owner != null )
            {
                //Main.LogI( "SkinnedProjectile" );
                var skinInd = self.owner.GetComponent<CharacterBody>().skinIndex;
                var cache = skinnedGhostCache[self.catalogIndex];
                if( cache.ContainsKey( skinInd ) )
                {
                    //Main.LogI( "Cached ghost found" );
                    self.ghostPrefab = cache[skinInd];
                } else
                {
                    //Main.LogI( "Generating ghost" );
                    var origGhost = self.ghostPrefab;
                    var newGhost = origGhost.ClonePrefab( "CachedProjectileGhost", false );
                    var controller = newGhost.GetComponent<BitSkinController>();
                    controller.Apply( WispBitSkin.GetWispSkin( skinInd ) );
                    UnityEngine.Object.Destroy( controller );
                    cache[skinInd] = newGhost;
                    self.ghostPrefab = newGhost;
                }
            }
            if( self.ghostPrefab == null )
            {
                //Main.LogI( "Null ghost prefab after skinning" );
            }

            orig( self );

            if( self.ghost == null )
            {
                //Main.LogI( "No ghost after start" );
            }
        }

        private static Func<ProjectileImpactExplosion,ProjectileController> getController;


        //private void ProjectileImpactExplosion_FixedUpdate( ILContext il )
        //{
        //    ILCursor c = new ILCursor( il );
        //    c.GotoNext( MoveType.Before,
        //        x => x.MatchLdcI4( 1 ),
        //        x => x.MatchCall( typeof( RoR2.EffectManager ), nameof( EffectManager.SpawnEffect ) )
        //    );
        //    c.Emit( OpCodes.Dup );
        //    c.Emit( OpCodes.Ldarg_0 );
        //    c.EmitDelegate<Action<EffectData,ProjectileImpactExplosion>>( (data, self) =>
        //    {
        //        //Main.LogI( "Check1" );
        //        if( self.explosionSoundString == Main.skinnedProjExplosionString )
        //        {
        //            //Main.LogI( "sound matched" );
        //            var ownerBody = getController(self)?.owner?.GetComponent<CharacterBody>();
        //            if( ownerBody != null ) data.genericUInt = ownerBody.skinIndex;
        //        }
        //    });
        //}

        //private void EffectManager_SpawnEffect_EffectIndex_EffectData_bool1( MonoMod.Cil.ILContext il )
        //{
        //    ILCursor c = new ILCursor( il );

        //    c.GotoNext( MoveType.Before, x => x.MatchCall( typeof( RoR2.EffectCatalog ), "GetEffectDef" ) );
        //    c.Remove();
        //    c.Emit( OpCodes.Ldarg_1 );
        //    c.EmitDelegate<Func<EffectIndex, EffectData, EffectDef>>( ( index, data ) =>
        //    {
        //        if( skinnedEffectCache.ContainsKey( index ) )
        //        {
        //            var cache = skinnedEffectCache[index];
        //            var skinInd = data.genericUInt;
        //            if( cache.ContainsKey( skinInd ) )
        //            {
        //                return cache[skinInd];
        //            } else
        //            {
        //                var origDef = EffectCatalog.GetEffectDef( index );
        //                var skinnedPrefab = origDef.prefab.ClonePrefab( "CachedEffect", true );
        //                var skinController = skinnedPrefab.GetComponent<BitSkinController>();
        //                skinController.Apply( WispBitSkin.GetWispSkin( skinInd ) );
        //                Destroy( skinController );
        //                var newDef = new EffectDef()
        //                {
        //                    cullMethod = origDef.cullMethod,
        //                    index = index,
        //                    prefab = skinnedPrefab,
        //                    prefabEffectComponent = skinnedPrefab.GetComponent<EffectComponent>(),
        //                    prefabName = origDef.prefabName,
        //                    prefabVfxAttributes = skinnedPrefab.GetComponent<VFXAttributes>(),
        //                    spawnSoundEventName = origDef.spawnSoundEventName
        //                };

        //                cache[skinInd] = newDef;
        //                return newDef;
        //            }
        //        } else
        //        {
        //            return EffectCatalog.GetEffectDef( index );
        //        }
        //    } );
        //}

        private static HashSet<GameObject> skinnedProjectiles = new HashSet<GameObject>();
        private static Dictionary<Int32,Dictionary<UInt32,GameObject>> skinnedGhostCache = new Dictionary<Int32, Dictionary<UInt32, GameObject>>();

        //private void ProjectileController_Start1( On.RoR2.Projectile.ProjectileController.orig_Start orig, RoR2.Projectile.ProjectileController self )
        //{
        //    //Main.LogI( "ProjController start" );
        //    //Main.LogI( "isPrediction: " + self.isPrediction );
        //    //Main.LogI( "allowPrediction: " + self.allowPrediction );
        //    //Main.LogI( "networkserveractive: " + NetworkServer.active );
        //    //Main.LogI( "authority: " + self.hasAuthority );
        //    if( skinnedGhostCache.ContainsKey( self.catalogIndex ) && self.owner != null )
        //    {
        //        //Main.LogI( "SkinnedProjectile" );
        //        var skinInd = self.owner.GetComponent<CharacterBody>().skinIndex;
        //        var cache = skinnedGhostCache[self.catalogIndex];
        //        if( cache.ContainsKey( skinInd ) )
        //        {
        //            //Main.LogI( "Cached ghost found" );
        //            self.ghostPrefab = cache[skinInd];
        //        } else
        //        {
        //            //Main.LogI( "Generating ghost" );
        //            var origGhost = self.ghostPrefab;
        //            var newGhost = origGhost.ClonePrefab( "CachedProjectileGhost", false );
        //            var controller = newGhost.GetComponent<BitSkinController>();
        //            controller.Apply(WispBitSkin.GetWispSkin(skinInd));
        //            UnityEngine.Object.Destroy( controller );
        //            cache[skinInd] = newGhost;
        //            self.ghostPrefab = newGhost;
        //        }
        //    }
        //    if( self.ghostPrefab == null )
        //    {
        //        //Main.LogI( "Null ghost prefab after skinning" );
        //    }

        //    orig( self );

        //    if( self.ghost == null )
        //    {
        //        //Main.LogI( "No ghost after start" );
        //    }
        //}
    }
}
