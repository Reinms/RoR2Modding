﻿#if ROGUEWISP
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour.HookGen;
using R2API.Utils;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;
using Mono.Cecil;

namespace RogueWispPlugin
{
    internal partial class Main
    {
        private const Single shieldRegenFrac = 0.03f;
        private const Single rootNumber = 4f;
        internal HashSet<CharacterBody> RW_BlockSprintCrosshair = new HashSet<CharacterBody>();
        partial void RW_Hook()
        {
            this.Enable += this.RW_AddHooks;
            this.Disable += this.RW_RemoveHooks;

            bodyloadouttoxmlbase = MethodBase.GetMethodFromHandle( typeof( R2API.LoadoutAPI ).GetMethod( "BodyLoadout_ToXml", BindingFlags.Static | BindingFlags.NonPublic ).MethodHandle );
        }

        internal delegate System.Xml.Linq.XElement loadoutapibodyloadouttoxml( On.RoR2.Loadout.BodyLoadoutManager.orig_ToXml orig, System.Object self, String elementName );
        private static MethodBase bodyloadouttoxmlbase;
        internal static event ILContext.Manipulator ilhook_R2API_LoadoutAPI_BodyLoadout_ToXml
        {
            add
            {
                HookEndpointManager.Modify<loadoutapibodyloadouttoxml>(bodyloadouttoxmlbase,value);
            }
            remove
            {
                HookEndpointManager.Unmodify<loadoutapibodyloadouttoxml>(bodyloadouttoxmlbase, value );
            }
        }

        private void RW_RemoveHooks()
        {
            IL.RoR2.GlobalEventManager.OnHitEnemy -= this.RemoveStunOnHit;//
            On.RoR2.GlobalEventManager.OnHitEnemy -= this.AddStunOnHit;//
            On.RoR2.CharacterBody.Start -= this.AddBurnManager;
            On.RoR2.CharacterBody.RecalculateStats -= this.ArmorBoost;
            IL.RoR2.CharacterBody.RecalculateStats -= this.ModifyMSBoost;
            On.RoR2.CharacterBody.FixedUpdate -= this.ShieldRegenStuff;
            //On.RoR2.CharacterModel.InstanceUpdate -= this.CharacterModel_InstanceUpdate;
            //On.RoR2.CharacterModel.UpdateOverlays -= this.CharacterModel_UpdateOverlays;
            IL.RoR2.UI.CrosshairManager.UpdateCrosshair -= this.CrosshairManager_UpdateCrosshair;
            IL.RoR2.CameraRigController.Update -= this.CameraRigController_Update;
            ilhook_R2API_LoadoutAPI_BodyLoadout_ToXml -= this.Main_ilhook_R2API_LoadoutAPI_BodyLoadout_ToXml;
            //On.RoR2.Projectile.SlowDownProjectiles.OnTriggerEnter += this.SlowDownProjectiles_OnTriggerEnter1;
            IL.RoR2.SetStateOnHurt.OnTakeDamageServer -= this.SetStateOnHurt_OnTakeDamageServer;
        }



        private void RW_AddHooks()
        {
            IL.RoR2.GlobalEventManager.OnHitEnemy += this.RemoveStunOnHit;
            On.RoR2.GlobalEventManager.OnHitEnemy += this.AddStunOnHit;
            On.RoR2.CharacterBody.Start += this.AddBurnManager;
            On.RoR2.CharacterBody.RecalculateStats += this.ArmorBoost;
            IL.RoR2.CharacterBody.RecalculateStats += this.ModifyMSBoost;
            On.RoR2.CharacterBody.FixedUpdate += this.ShieldRegenStuff;
            //On.RoR2.CharacterModel.InstanceUpdate += this.CharacterModel_InstanceUpdate;
            //On.RoR2.CharacterModel.UpdateOverlays += this.CharacterModel_UpdateOverlays;
            //On.RoR2.EffectManager.TransmitEffect += this.DoVeryVeryBadThings;
            IL.RoR2.UI.CrosshairManager.UpdateCrosshair += this.CrosshairManager_UpdateCrosshair;
            IL.RoR2.CameraRigController.Update += this.CameraRigController_Update;
            ilhook_R2API_LoadoutAPI_BodyLoadout_ToXml += this.Main_ilhook_R2API_LoadoutAPI_BodyLoadout_ToXml;
            //On.RoR2.Projectile.SlowDownProjectiles.OnTriggerEnter += this.SlowDownProjectiles_OnTriggerEnter1;
            IL.RoR2.SetStateOnHurt.OnTakeDamageServer += this.SetStateOnHurt_OnTakeDamageServer;
            On.RoR2.CameraRigController.Start += this.CameraRigController_Start1;

            //var instance = Expression.Parameter(typeof(RoR2.Projectile.SlowDownProjectiles), "instance" );
            //var body = Expression.Field( instance, "affectedRigidBodies" );
            //getRbList = Expression.Lambda<Func<RoR2.Projectile.SlowDownProjectiles, List<Rigidbody>>>( body, instance ).Compile();
        }

        private void CameraRigController_Start1( On.RoR2.CameraRigController.orig_Start orig, CameraRigController self )
        {
            orig( self );

            if( self.hud != null )
            {
                var par = self.hud.transform.Find( "MainContainer/MainUIArea/BottomRightCluster/Scaler" );
                var inst = Instantiate<GameObject>( wispHudPrefab, par );
                var bar3Rect = inst.GetComponent<RectTransform>();
                bar3Rect.anchoredPosition = new Vector2( 14f, 140f );
                bar3Rect.sizeDelta = new Vector2( 310f, 32f );
                bar3Rect.anchorMin = new Vector2( 0.5f, 0.5f );
                bar3Rect.anchorMax = new Vector2( 0.5f, 0.5f );
                bar3Rect.pivot = new Vector2( 0.5f, 0.5f );
                bar3Rect.localEulerAngles = Vector3.zero;
                bar3Rect.localScale = Vector3.one;

            }
        }




        //private static Func<RoR2.Projectile.SlowDownProjectiles, List<Rigidbody> > getRbList;
        //private void SlowDownProjectiles_OnTriggerEnter1( On.RoR2.Projectile.SlowDownProjectiles.orig_OnTriggerEnter orig, RoR2.Projectile.SlowDownProjectiles self, Collider other )
        //{
        //    var otherController = other.GetComponent<ProjectileController>();
        //    if( !otherController ) return;
        //    var otherOwner = otherController.owner;
        //    if( !otherOwner || TeamComponent.GetObjectTeam(otherOwner) == TeamIndex.Player ) return;
        //    var rb = other.attachedRigidbody;
        //    if( rb == null ) return;
        //    getRbList( self ).Add( rb );
        //}

        private void Main_ilhook_R2API_LoadoutAPI_BodyLoadout_ToXml( ILContext il )
        {
            ILCursor c = new ILCursor( il );
            ILLabel label = default;
            c.GotoNext( MoveType.After, x => x.MatchLdloc( 1 ) );
            c.Remove();
            ++c.Index;
            c.Remove();
            c.EmitDelegate<Func<ModelSkinController,UInt32,SkinDef>>( (modelSkin, skinInd ) =>
            {
                if( modelSkin == null ) return null;
                if( skinInd >= modelSkin.skins.Length ) return null;
                return modelSkin.skins[skinInd];
            });
        }

        private void CameraRigController_Update( ILContext il )
        {
            ILCursor c = new ILCursor( il );

            c.GotoNext( MoveType.After, x => x.MatchCallOrCallvirt<RoR2.CharacterBody>( "get_isSprinting" ) );
            c.Emit( OpCodes.Ldarg_0 );
            c.Emit<RoR2.CameraRigController>( OpCodes.Ldfld, "targetBody" );
            c.EmitDelegate<Func<CharacterBody, Boolean>>( ( body ) => !this.RW_BlockSprintCrosshair.Contains( body ) );
            c.Emit( OpCodes.And );
        }

        private void CrosshairManager_UpdateCrosshair( ILContext il )
        {
            ILCursor c = new ILCursor( il );
            c.GotoNext( MoveType.After, x => x.MatchCallOrCallvirt<RoR2.CharacterBody>( "get_isSprinting" ) );
            c.Emit( OpCodes.Ldarg_1 );
            c.EmitDelegate<Func<CharacterBody, Boolean>>( ( body ) => !this.RW_BlockSprintCrosshair.Contains( body ) );
            c.Emit( OpCodes.And );
        }

        //private void DoVeryVeryBadThings( On.RoR2.EffectManager.orig_TransmitEffect orig, EffectIndex effectPrefabIndex, EffectData effectData, NetworkConnection netOrigin )
        //{
        //    orig( effectPrefabIndex, effectData, netOrigin );

        //    if( NetworkServer.active && CheckIndex( (UInt32)effectPrefabIndex, this.restoreIndex ) && (effectData.color.r != 0 || effectData.color.g != 0 || effectData.color.b != 0 || effectData.color.a != 0) )
        //    {
        //        GameObject temp = effectData.ResolveHurtBoxReference();
        //        if( temp )
        //        {
        //            CharacterBody comp = temp.GetComponent<CharacterBody>();
        //            if( comp && comp.baseNameToken == "WISP_SURVIVOR_BODY_NAME" )
        //            {
        //                Single value = HeatwaveClientOrb.DoMoreBadThings(effectData.color);
        //                if( value > 0f )
        //                {
        //                    this.StartCoroutine( this.DelayBuff( this.RW_flameChargeBuff, comp, value, effectData.genericUInt, effectData.genericFloat ) );
        //                }
        //            }
        //        }
        //    }
        //}

        //private IEnumerator DelayBuff( BuffIndex buff, CharacterBody body, Single duration, UInt32 stacks, Single delay )
        //{
        //    yield return new WaitForSeconds( delay );

        //    for( UInt32 i = 0; i < stacks; i++ )
        //    {
        //        body.AddTimedBuff( buff, duration );
        //    }
        //}

        //private static Boolean CheckIndex( UInt32 index, UInt32[] indicies )
        //{
        //    for( Int32 i = 0; i < indicies.Length; i++ )
        //    {
        //        if( index == indicies[i] ) return true;
        //    }
        //    return false;
        //}


        private void SetStateOnHurt_OnTakeDamageServer( ILContext il )
        {
            ILCursor c = new ILCursor( il );
            //c.GotoNext( MoveType.After, x => x.MatchLdfld<SetStateOnHurt>( nameof( SetStateOnHurt.canBeStunned ) ) );
            //Main.LogI( c );
            //c.Index += 8;
            //Main.LogI( c );
            c.GotoNext( MoveType.Before, x => x.MatchCallOrCallvirt<SetStateOnHurt>( "SetStun" ) );
            c.Emit( OpCodes.Ldloc_0 );
            c.EmitDelegate<Func<DamageInfo, Single>>( ( info ) => info.procCoefficient );
            c.Emit( OpCodes.Mul );
        }


        private void RemoveStunOnHit( ILContext il )
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext( MoveType.After,
                x => x.MatchCallOrCallvirt( "RoR2.SetStateOnHurt", "SetStun" )
                );
            c.Index += -3;
            c.RemoveRange( 3 );
        }

        private void AddStunOnHit( On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo info, GameObject victim )
        {
            orig( self, info, victim );
            if( info.procCoefficient <= 0f || info.rejected || !NetworkServer.active || !info.attacker ) return;
            CharacterBody body = info.attacker.GetComponent<CharacterBody>();
            if( !body ) return;
            Inventory inventory = body.inventory;
            if( !inventory ) return;
            Int32 stunCount = inventory.GetItemCount(ItemIndex.StunChanceOnHit);
            if( stunCount <= 0 ) return;
            Single sqCoef = Mathf.Sqrt(info.procCoefficient);
            if( !RoR2.Util.CheckRoll( RoR2.Util.ConvertAmplificationPercentageIntoReductionPercentage( sqCoef * 5f * stunCount ), body.master ) ) return;
            SetStateOnHurt stateOnHurt = victim.GetComponent<SetStateOnHurt>();
            if( !stateOnHurt ) return;
            stateOnHurt.SetStun( sqCoef * 2f );
        }

        private void AddBurnManager( On.RoR2.CharacterBody.orig_Start orig, CharacterBody self )
        {
            orig( self );
            self.gameObject.AddComponent<WispBurnManager>();
        }

        private void ArmorBoost( On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self )
        {
            orig( self );
            if( self && self.inventory )
            {
                if( self.HasBuff( this.RW_armorBuff ) )
                {
                    self.SetPropertyValue<Single>( "armor", self.armor + 75f );
                }
            }
        }

        private void ModifyMSBoost( ILContext il )
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext( MoveType.After,
                x => x.MatchLdarg( 0 ),
                x => x.MatchLdcI4( 6 ),
                x => x.MatchCallOrCallvirt<RoR2.CharacterBody>( "HasBuff" ),
                x => x.MatchBrfalse( out _ ),
                x => x.MatchLdloc( 47 )
                );

            c.Remove();
            c.Emit( Mono.Cecil.Cil.OpCodes.Ldc_R4, 0.25f );
        }

        private void ShieldRegenStuff( On.RoR2.CharacterBody.orig_FixedUpdate orig, CharacterBody self )
        {
            orig( self );
            if( NetworkServer.active )
            {
                Int32 count = self.GetBuffCount( this.RW_flameChargeBuff );
                if( count > 0 )
                {
                    self.healthComponent.AddBarrier( Time.fixedDeltaTime * shieldRegenFrac * Mathf.Pow(count,1f / rootNumber) * self.maxHealth );
                }
            }
        }

        //private void CharacterModel_InstanceUpdate( On.RoR2.CharacterModel.orig_InstanceUpdate orig, CharacterModel self )
        //{
        //    if( self.body && self.body.baseNameToken == "WISP_SURVIVOR_BODY_NAME" )
        //    {
        //        EliteIndex eliteInd = self.GetFieldValue<EliteIndex>( "myEliteIndex");
        //        if( eliteInd == EliteIndex.Poison )
        //        {
        //            self.SetFieldValue<Material>( "particleMaterialOverride", Main.eliteFlameMaterials[self.body.skinIndex][0] );
        //        } else if( eliteInd == EliteIndex.Haunted )
        //        {
        //            self.SetFieldValue<Material>( "particleMaterialOverride", Main.eliteFlameMaterials[self.body.skinIndex][1] );
        //        } else
        //        {
        //            self.SetFieldValue<Material>( "particleMaterialOverride", null );
        //        }

        //        self.InvokeMethod( "UpdateGoldAffix" );
        //        self.InvokeMethod( "UpdatePoisonAffix" );
        //        self.InvokeMethod( "UpdateHauntedAffix" );
        //        self.InvokeMethod( "UpdateLights" );
        //    } else
        //    {
        //        orig( self );
        //    }
        //}

        //private void CharacterModel_UpdateOverlays( On.RoR2.CharacterModel.orig_UpdateOverlays orig, CharacterModel self )
        //{
        //    orig( self );
        //    if( self.body && self.body.baseNameToken == "WISP_SURVIVOR_BODY_NAME" )
        //    {
        //        Material[] mats = self.GetFieldValue<Material[]>("currentOverlays" );
        //        UInt32 skin = self.body.skinIndex;
        //        for( Int32 i = 0; i < mats.Length; i++ )
        //        {
        //            if( mats[i] == CharacterModel.energyShieldMaterial )
        //            {
        //                mats[i] = Main.shieldOverlayMaterials[skin];
        //            }
        //        }
        //    }
        //}
    }
}
#endif
