﻿#if ROGUEWISP
using System;
using System.Collections.Generic;
using System.Reflection;

using BepInEx.Configuration;

using Mono.Cecil.Cil;

using MonoMod.Cil;
using MonoMod.RuntimeDetour.HookGen;

using ReinCore;

using RoR2;

using UnityEngine;
using UnityEngine.Networking;

namespace Rein.RogueWispPlugin
{
    internal partial class Main
    {
        private const Single barrierDecayMult = 0.5f;
        private const Single shieldRegenFrac = 0.02f;
        private const Single rootNumber = 6f;
        internal HashSet<CharacterBody> RW_BlockSprintCrosshair = new HashSet<CharacterBody>();
        private ConfigEntry<Boolean> chargeBarEnabled;
        partial void RW_Hook()
        {
            this.Enable += this.RW_AddHooks;
            this.Disable += this.RW_RemoveHooks;
            this.chargeBarEnabled = base.Config.Bind<Boolean>("Visual (Client)", "SkillBarChargeIndicator", true, "Should a charge bar be displayed above the skill bar in addition to around the crosshair?");
            //this.deathMarkStuff = base.Config.Bind<Boolean>( "Gameplay (Server)", "DeathMarkDebuffChange", true, "Should Death Mark use a new system that favors non-stacking debuffs over stacking Dots?" );
        }




        private void RW_RemoveHooks()
        {
            HooksCore.RoR2.CameraRigController.Start.On -= this.Start_On1;
            HooksCore.RoR2.CharacterBody.Start.On -= this.Start_On2;
            HooksCore.RoR2.CharacterBody.FixedUpdate.On -= this.FixedUpdate_On;
            HooksCore.RoR2.CharacterBody.RecalculateStats.Il -= this.RecalculateStats_Il;
            HooksCore.RoR2.CharacterBody.RecalculateStats.On -= this.RecalculateStats_On;
            HooksCore.RoR2.UI.CrosshairManager.UpdateCrosshair.Il -= this.UpdateCrosshair_Il;
            HooksCore.RoR2.CameraRigController.Update.Il -= this.Update_Il;
            HooksCore.RoR2.SetStateOnHurt.OnTakeDamageServer.Il -= this.OnTakeDamageServer_Il;
            HooksCore.RoR2.CharacterModel.UpdateRendererMaterials.Il -= this.UpdateRendererMaterials_Il;
        }

        //private static StaticAccessor<Dictionary<String,Dictionary<String,String>>> languageDictionaries = new StaticAccessor<Dictionary<string, Dictionary<string, string>>>( typeof(Language), "languageDictionaries" );


        private void RW_AddHooks()
        {
            HooksCore.RoR2.UI.QuickPlayButtonController.Start.On += this.Start_On4;

            HooksCore.RoR2.CameraRigController.Start.On += this.Start_On1;
            HooksCore.RoR2.CharacterBody.Start.On += this.Start_On2;
            HooksCore.RoR2.CharacterBody.FixedUpdate.On += this.FixedUpdate_On;
            HooksCore.RoR2.CharacterBody.RecalculateStats.Il += this.RecalculateStats_Il;
            HooksCore.RoR2.CharacterBody.RecalculateStats.On += this.RecalculateStats_On;
            HooksCore.RoR2.UI.CrosshairManager.UpdateCrosshair.Il += this.UpdateCrosshair_Il;
            HooksCore.RoR2.CameraRigController.Update.Il += this.Update_Il;
            HooksCore.RoR2.SetStateOnHurt.OnTakeDamageServer.Il += this.OnTakeDamageServer_Il;
            HooksCore.RoR2.CharacterModel.UpdateRendererMaterials.Il += this.UpdateRendererMaterials_Il;
        }

        private void FromMaster_Il(ILContext il)
        {
            var c = new ILCursor( il );
            _ = c.GotoNext(MoveType.After, x => x.MatchCallOrCallvirt<RoR2.Loadout>(nameof(Loadout.Copy)));

            _ = c.Emit(OpCodes.Ldloc_1);
            _ = c.Emit(OpCodes.Ldfld, typeof(RoR2.CharacterSpawnCard).GetField("runtimeLoadout", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance));
            _ = c.Emit(OpCodes.Ldarg_0);
            _ = c.EmitDelegate<Action<RoR2.Loadout, RoR2.CharacterMaster>>((dest, source) =>
             {
                 if(dest == null || source == null || !source) return;
                 var sourceLoadout = source.loadout;
                 if(sourceLoadout == null) return;

                 var sourceSkin = sourceLoadout.bodyLoadoutManager.GetSkinIndex(Main.rogueWispBodyIndex);
                 var invertedSkin = (~Helpers.WispBitSkin.GetWispSkin(sourceSkin)).EncodeToSkinIndex();
                 dest.bodyLoadoutManager.SetSkinIndex(Main.rogueWispBodyIndex, invertedSkin);
             });

        }

        private void CreateDoppelganger_Il(ILContext il)
        {
            ILCursor c = new ILCursor( il );
            _ = c.GotoNext(MoveType.After, x => x.MatchCallOrCallvirt<RoR2.DirectorCore>(nameof(DirectorCore.TrySpawnObject)), x => x.MatchPop());
            c.Index--;
            _ = c.Remove();
            _ = c.EmitDelegate<Action<GameObject>>((obj) =>
             {
                 if(obj != null && obj)
                 {
                     var master = obj.GetComponent<CharacterMaster>();
                     if(master != null && master)
                     {
                         var loadout = master.loadout;
                         if(loadout != null)
                         {
                             var bodyInd = BodyCatalog.FindBodyIndex(master.bodyPrefab);
                             if(bodyInd >= 0 && bodyInd == Main.rogueWispBodyIndex)
                             {
                                 var bodyLoadoutManager = loadout.bodyLoadoutManager;
                                 var skinInd = bodyLoadoutManager.GetSkinIndex( bodyInd );
                                 UInt32 newSkinInd;
                                 try
                                 {
                                     var skin = ~Helpers.WispBitSkin.GetWispSkin( skinInd );
                                     newSkinInd = skin.EncodeToSkinIndex();
                                 } catch
                                 {
                                     Main.LogE("Error inverting skin for wisp clone, please copy the line of 1s and 0s below and send them to me.");
                                     Main.LogE(Convert.ToString(skinInd, 2).PadLeft(32, '0'));
                                     newSkinInd = 0b0000_0000_0000_0000_0000_0000_0000_1000u;
                                 }

                                 bodyLoadoutManager.SetSkinIndex(bodyInd, newSkinInd);

                                 if(master.hasBody)
                                 {
                                     var body = master.GetBody();
                                     body.skinIndex = newSkinInd;
                                     if(body != null && body)
                                     {
                                         var ml = body.modelLocator;
                                         if(ml != null && ml)
                                         {
                                             var model = ml.modelTransform;
                                             if(model != null && model)
                                             {
                                                 var skinController = model.GetComponent<Helpers.WispModelBitSkinController>();
                                                 skinController?.Apply(Helpers.WispBitSkin.GetWispSkin(newSkinInd));
                                             }
                                         }
                                     }
                                 }
                             }
                         }
                     }
                 }
            });


        }

        private void UpdateRendererMaterials_Il(ILContext il)
        {
            var c = new ILCursor( il );
            _ = c.GotoNext(MoveType.After, x => x.MatchCallOrCallvirt<RoR2.CharacterModel>("get_isDoppelganger"));
            _ = c.Emit(OpCodes.Ldarg_0);
            _ = c.EmitDelegate<Func<CharacterModel, Boolean>>((model) => model.body != null && model.body.baseNameToken != Rein.Properties.Tokens.WISP_SURVIVOR_BODY_NAME);
            _ = c.Emit(OpCodes.And);
        }

        private void Start_On4(HooksCore.RoR2.UI.QuickPlayButtonController.Start.Orig orig, RoR2.UI.QuickPlayButtonController self)
        {
            self.gameObject.SetActive(false);
            orig(self);
            self.gameObject.SetActive(false);
        }

        private void OnHitEnemy_On(HooksCore.RoR2.GlobalEventManager.OnHitEnemy.Orig orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
        {
            orig(self, damageInfo, victim);
            if(damageInfo.procCoefficient <= 0f || damageInfo.rejected || !NetworkServer.active || !damageInfo.attacker) return;
            CharacterBody body = damageInfo.attacker.GetComponent<CharacterBody>();
            if(!body) return;
            Inventory inventory = body.inventory;
            if(!inventory) return;
            Int32 stunCount = inventory.GetItemCount(ItemIndex.StunChanceOnHit);
            if(stunCount <= 0) return;
            Single sqCoef = Mathf.Sqrt(damageInfo.procCoefficient);
            if(!RoR2.Util.CheckRoll(RoR2.Util.ConvertAmplificationPercentageIntoReductionPercentage(sqCoef * 5f * stunCount), body.master)) return;
            SetStateOnHurt stateOnHurt = victim.GetComponent<SetStateOnHurt>();
            if(!stateOnHurt) return;
            stateOnHurt.SetStun(sqCoef * 2f);
        }
        private void OnTakeDamageServer_Il(ILContext il)
        {
            var c = new ILCursor( il );
            _ = c.GotoNext(MoveType.Before, x => x.MatchCallOrCallvirt<SetStateOnHurt>("SetStun"));
            _ = c.Emit(OpCodes.Ldloc_0);
            _ = c.EmitDelegate<Func<DamageInfo, Single>>((info) => info.procCoefficient);
            _ = c.Emit(OpCodes.Mul);
        }
        private void Update_Il(ILContext il)
        {
            var c = new ILCursor( il );

            _ = c.GotoNext(MoveType.After, x => x.MatchCallOrCallvirt<RoR2.CharacterBody>("get_isSprinting"));
            _ = c.Emit(OpCodes.Ldarg_0);
            _ = c.Emit<RoR2.CameraRigController>(OpCodes.Ldfld, "targetBody");
            _ = c.EmitDelegate<Func<CharacterBody, Boolean>>((body) => !this.RW_BlockSprintCrosshair.Contains(body));
            _ = c.Emit(OpCodes.And);
        }
        private void UpdateCrosshair_Il(ILContext il)
        {
            var c = new ILCursor( il );
            _ = c.GotoNext(MoveType.After, x => x.MatchCallOrCallvirt<RoR2.CharacterBody>("get_isSprinting"));
            _ = c.Emit(OpCodes.Ldarg_1);
            _ = c.EmitDelegate<Func<CharacterBody, Boolean>>((body) => !this.RW_BlockSprintCrosshair.Contains(body));
            _ = c.Emit(OpCodes.And);
        }
        private void RecalculateStats_On(HooksCore.RoR2.CharacterBody.RecalculateStats.Orig orig, CharacterBody self)
        {
            orig(self);
            if(self && self.inventory)
            {
                if(self.HasBuff(RW_armorBuff))
                {
                    self.armor += 75f;
                }
                if(self.HasBuff(RW_flameChargeBuff))
                {
                    self.barrierDecayRate *= barrierDecayMult;
                }
            }
        }
        private void RecalculateStats_Il(ILContext il) => new ILCursor(il)
            .GotoNext(
                x => x.MatchLdarg(0),
                x => x.MatchLdcI4((Int32)BuffIndex.EnrageAncientWisp)
            ).GotoNext(MoveType.Before, x => x.MatchLdcR4(out _))
            .Remove()
            .LdC_(0.25f);
        private void FixedUpdate_On( HooksCore.RoR2.CharacterBody.FixedUpdate.Orig orig, CharacterBody self )
        {
            orig( self );
            if( NetworkServer.active )
            {
                Int32 count = self.GetBuffCount( RW_flameChargeBuff );
                if( count > 0 )
                {
                    self.healthComponent.AddBarrier( Time.fixedDeltaTime * shieldRegenFrac * Mathf.Pow( count, 1f / rootNumber ) * self.healthComponent.fullCombinedHealth );
                }
            }
        }
        private void Start_On2( HooksCore.RoR2.CharacterBody.Start.Orig orig, CharacterBody self )
        {
            orig( self );
            self.gameObject.AddComponent<WispBurnManager>();
        }
        private void Start_On1( HooksCore.RoR2.CameraRigController.Start.Orig orig, CameraRigController self )
        {
            orig( self );

            if( self.hud != null )
            {
                var par = self.hud.transform.Find( "MainContainer/MainUIArea/BottomRightCluster/Scaler" );
                if( this.chargeBarEnabled.Value )
                {

                    GameObject inst = Instantiate<GameObject>( wispHudPrefab, par );
                    RectTransform bar3Rect = inst.GetComponent<RectTransform>();
                    bar3Rect.anchoredPosition = new Vector2( 14f, 140f );
                    bar3Rect.sizeDelta = new Vector2( 310f, 32f );
                    bar3Rect.anchorMin = new Vector2( 0.5f, 0.5f );
                    bar3Rect.anchorMax = new Vector2( 0.5f, 0.5f );
                    bar3Rect.pivot = new Vector2( 0.5f, 0.5f );
                    bar3Rect.localEulerAngles = Vector3.zero;
                    bar3Rect.localScale = Vector3.one;
                }



                Transform par2 = self.hud.transform.Find( "MainContainer/MainUIArea/CrosshairCanvas");

                GameObject cross1 = Instantiate<GameObject>( wispCrossBar1, par2 );

                RectTransform bar1Rect = cross1.GetComponent<RectTransform>();

                GameObject cross2 = Instantiate<GameObject>( wispCrossBar2, par2 );

                RectTransform bar2Rect = cross2.GetComponent<RectTransform>();


                bar1Rect.anchoredPosition = new Vector2( 96f, 0f );
                bar2Rect.anchoredPosition = new Vector2( -96f, 0f );

                bar1Rect.sizeDelta = new Vector2( 256f, 128f );
                bar2Rect.sizeDelta = new Vector2( 256f, 128f );

                bar1Rect.anchorMin = new Vector2( 0.5f, 0.5f );
                bar2Rect.anchorMin = new Vector2( 0.5f, 0.5f );

                bar1Rect.anchorMax = new Vector2( 0.5f, 0.5f );
                bar2Rect.anchorMax = new Vector2( 0.5f, 0.5f );

                bar1Rect.pivot = new Vector2( 0.5f, 0.5f );
                bar2Rect.pivot = new Vector2( 0.5f, 0.5f );

                bar1Rect.localEulerAngles = new Vector3( 0f, 0f, -90f );
                bar2Rect.localEulerAngles = new Vector3( 0f, 0f, 90f );

                bar1Rect.localScale = Vector3.one * 0.125f;
                bar2Rect.localScale = Vector3.one * 0.125f;
            }
        }
        private void Main_ilhook_R2API_LoadoutAPI_BodyLoadout_ToXml( ILContext il )
        {
            var c = new ILCursor( il );
            _ = c.GotoNext( MoveType.After, x => x.MatchLdloc( 1 ) );
            _ = c.Remove();
            ++c.Index;
            _ = c.Remove();
            _ = c.EmitDelegate<Func<ModelSkinController, UInt32, SkinDef>>( ( modelSkin, skinInd ) => modelSkin == null ? null : skinInd >= modelSkin.skins.Length ? null : modelSkin.skins[skinInd] );
        }
    }
}
#endif
