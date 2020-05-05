﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using BepInEx.Logging;
using ReinCore;
using RoR2;
using RoR2.Networking;
using UnityEngine;
using KinematicCharacterController;
using EntityStates;
using RoR2.Skills;
using RoR2.UI;
using UnityEngine.UI;
using Sniper.Components;
using System.Linq.Expressions;

namespace Sniper.Modules
{
    internal static class UIModule
    {
        private static Sprite GetUnfinishedIcon()
        {
            var texJob = TexturesCore.GenerateCrossTextureBatch( 512, 512, 64, 16, 2, Color.black, Color.red, Color.white, Color.white, Color.white, Color.white );

            return Sprite.Create( texJob.OutputTextureAndDispose(), new Rect( 0f, 0f, 512f, 512f ), new Vector2( 0.5f, 0.5f ) );
        }


        internal static Sprite GetStandardAmmoIcon()
        {
            return AssetModule.GetSniperAssetBundle().LoadAsset<Sprite>( Properties.Resources.StandardAmmoIcon );
        }
        internal static Sprite GetExplosiveAmmoIcon() => AssetModule.GetSniperAssetBundle().LoadAsset<Sprite>( Properties.Resources.ExplosiveAmmoIcon );
        internal static Sprite GetSnipeIcon()
        {
            return AssetModule.GetSniperAssetBundle().LoadAsset<Sprite>( Properties.Resources.SnipeIcon );
        }
        internal static Sprite GetSnipeReloadIcon()
        {
            return AssetModule.GetSniperAssetBundle().LoadAsset<Sprite>( Properties.Resources.ReloadIcon );
        }
        internal static Sprite GetSteadyAimIcon()
        {
            return AssetModule.GetSniperAssetBundle().LoadAsset<Sprite>( Properties.Resources.SteadyAimIcon );
        }
        internal static Sprite GetCritPassiveIcon()
        {
            return AssetModule.GetSniperAssetBundle().LoadAsset<Sprite>( Properties.Resources.CritPassiveIcon );
        }
        internal static Sprite GetHeadshotPassiveIcon()
        {
            return AssetModule.GetSniperAssetBundle().LoadAsset<Sprite>( Properties.Resources.HeadshotIcon );
        }

        internal static Sprite GetBackflipIcon()
        {
            return AssetModule.GetSniperAssetBundle().LoadAsset<Sprite>( Properties.Resources.BackflipIcon );
        }

        internal static Sprite GetQuickScopeIcon()
        {
            return GetUnfinishedIcon();
        }

        internal static Sprite GetKnifeIcon()
        {
            return GetUnfinishedIcon();
        }

        internal static Sprite GetKnifeReactivationIcon()
        {
            return GetUnfinishedIcon();
        }

        internal static Sprite GetDecoyIcon()
        {
            return GetUnfinishedIcon();
        }

        internal static Sprite GetDecoyReactivationIcon()
        {
            return GetUnfinishedIcon();
        }











        internal static Color reloadHandleColor { get; } = new Color( 1f, 1f, 1f, 1f );

        internal static GameObject GetDefaultDrosshair()
        {
            // TODO: Create default crosshair
            return Resources.Load<GameObject>( "Prefabs/CharacterBodies/CrocoBody" ).GetComponent<CharacterBody>().crosshairPrefab;
        }

        private static GameObject scopeCrosshair;
        internal static GameObject GetScopeCrosshair()
        {
            if( scopeCrosshair == null )
            {
                scopeCrosshair = UICreationModule.CreateScopeCrosshair();
            }
            return scopeCrosshair;
        }

        internal static Texture GetPortraitIcon()
        {
            return AssetModule.GetSniperAssetBundle().LoadAsset<Texture2D>( Properties.Resources.PortraitIcon );
            //return null;
        }

        internal static GameObject GetQuickScope()
        {
            // TODO: Quick scope UI
            return GetScopeCrosshair();
        }

        internal static GameObject GetChargeScope()
        {
            // TODO: Charge scope UI
            return GetScopeCrosshair();
        }

        private static GameObject reloadBarPrefab;
        internal static void CreateReloadBarPrefab()
        {
            var obj = PrefabsCore.CreateUIPrefab("ReloadBar", false );
            var objTrans = obj.transform as RectTransform;
            objTrans.sizeDelta = new Vector2( 640f, 80f );
            objTrans.anchorMin = new Vector2( 0.5f, 0.5f );
            objTrans.anchorMax = new Vector2( 0.5f, 0.5f );
            objTrans.pivot = new Vector2( 0.5f, 0.5f );
            objTrans.localPosition = Vector3.zero;

            var holder = PrefabsCore.CreateUIPrefab( "BarHolder", false );
            var holderTrans = holder.transform as RectTransform;
            holderTrans.SetParent( objTrans, false );
            holderTrans.sizeDelta = Vector2.zero;
            holderTrans.anchorMax = Vector2.one;
            holderTrans.anchorMin = Vector2.zero;
            holderTrans.pivot= new Vector2( 0.5f, 0.5f );
            holderTrans.localPosition = Vector3.zero;

            var background = PrefabsCore.CreateUIPrefab( "Background", false );
            var bgTrans = background.transform as RectTransform;
            bgTrans.SetParent( holderTrans, false );
            bgTrans.localPosition = Vector3.zero;
            bgTrans.sizeDelta = Vector2.zero;
            bgTrans.anchorMin = new Vector2( 0f, 0.1f );
            bgTrans.anchorMax = new Vector2( 1f, 0.9f );
            bgTrans.pivot = new Vector2( 0.5f, 0.5f );
            var bgRend = background.AddComponent<CanvasRenderer>();
            bgRend.cullTransparentMesh = false;
            var bgImg = background.AddComponent<Image>();
            bgImg.sprite = null;
            bgImg.color = Color.white;
            bgImg.material = null;
            bgImg.raycastTarget = false;
            bgImg.type = Image.Type.Simple;
            bgImg.useSpriteMesh = false;
            bgImg.preserveAspect = false;


            var slideArea = PrefabsCore.CreateUIPrefab( "Handle Slide Area", false );
            var slideAreaTrans = slideArea.transform as RectTransform;
            slideAreaTrans.SetParent( holderTrans, false );
            slideAreaTrans.localPosition = Vector3.zero;
            slideAreaTrans.sizeDelta = Vector2.zero;
            slideAreaTrans.anchorMin = new Vector2( 0f, 0f );
            slideAreaTrans.anchorMax = new Vector2( 1f, 1f );
            slideAreaTrans.pivot = new Vector2( 0.5f, 0.5f );

            var handle = PrefabsCore.CreateUIPrefab( "Handle", false );
            var handleTrans = handle.transform as RectTransform;
            handleTrans.SetParent( slideAreaTrans, false );
            handleTrans.localPosition = Vector3.zero;
            handleTrans.sizeDelta = new Vector2( 16f, 0f );
            handleTrans.pivot = new Vector2( 0.5f, 0.5f );
            var handleRend = handle.AddComponent<CanvasRenderer>();
            handleRend.cullTransparentMesh = false;
            var handleImg = handle.AddComponent<Image>();
            var tex = TexturesCore.GenerateBarTexture(128, 640, true, 64, 16, Color.black, reloadHandleColor, 4 );
            handleImg.sprite = Sprite.Create( tex, new Rect( 0f, 0f, tex.width, tex.height ), new Vector2( 0.5f, 0.5f ) );
            handleImg.color = Color.white;
            handleImg.material = null;
            handleImg.raycastTarget = false;
            handleImg.type = Image.Type.Simple;
            handleImg.useSpriteMesh = false;
            handleImg.preserveAspect = false;

            var slider = holder.AddComponent<Slider>();
            slider.interactable = false;
            slider.transition = Selectable.Transition.None;
            slider.navigation = new Navigation { mode = Navigation.Mode.None };
            slider.fillRect = null;
            slider.handleRect = handleTrans;
            slider.direction = Slider.Direction.LeftToRight;
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.wholeNumbers = false;
            slider.value = 0f;

            obj.AddComponent<ReloadUIController>();

            reloadBarPrefab = obj;
        }

        internal static GameObject GetRelodBar()
        {
            if( !reloadBarPrefab )
            {
                CreateReloadBarPrefab();
            }
            var bar = UnityEngine.Object.Instantiate<GameObject>( reloadBarPrefab );
            return bar;
        }

        //public static GameObject hudPrefab;
        internal static void EditHudPrefab()
        {
            //hudPrefab = Resources.Load<GameObject>( "Prefabs/HUDSimple" );
        }
    }
}
