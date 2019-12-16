﻿using R2API.Utils;
using RoR2;
using RoR2.Projectile;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Acrid.Helpers
{
    public static class CatalogHelpers
    {

        #region Catalogs
        /// <summary>
        /// Adds a GameObject to the BodyCatalog and returns true
        /// Returns false if the GameObject is null
        /// Will not work after BodyCatalog is initalized.
        /// </summary>
        /// <param Body Prefab="g"></param>
        /// <returns></returns>
        public static System.Boolean RegisterNewBody( GameObject g )
        {
            if( g != null )
            {
                RoR2.BodyCatalog.getAdditionalEntries += list =>
                {
                    list.Add( g );
                };

                return true;
            }
            return false;
        }

        /// <summary>
        /// Adds a GameObject to the projectile catalog and returns true
        /// GameObject must be non-null and have a ProjectileController component
        /// returns false if GameObject is null or is missing the component
        /// </summary>
        /// <param Projectile Prefab="g"></param>
        /// <returns></returns>
        public static System.Boolean RegisterNewProjectile( GameObject g )
        {
            if( g.HasComponent<ProjectileController>() )
            {
                RoR2.ProjectileCatalog.getAdditionalEntries += list =>
                {
                    list.Add( g );
                };

                return true;
            }
            return false;
        }

        /// <summary>
        /// Adds a GameObject to the effects catalog and returns true
        /// Returns false if GameObject is null
        /// </summary>
        /// <param Effect Prefab="prefab"></param>
        /// <returns></returns>
        public static System.Boolean RegisterNewEffect( GameObject prefab )
        {
            List<GameObject> effects = EffectManager.instance.GetFieldValue<List<GameObject>>("effectPrefabsList");
            Dictionary<GameObject, System.UInt32> effectLookup = EffectManager.instance.GetFieldValue<Dictionary<GameObject, System.UInt32>>("effectPrefabToIndexMap");

            if( !prefab )
            {
                return false;
            }

            System.Int32 index = effects.Count;

            effects.Add( prefab );
            effectLookup.Add( prefab, (System.UInt32)index );
            return true;
        }
        #endregion

        #region SkinDefs
        /// <summary>
        /// Creates and returns a new SkinDef
        /// Note that selecting modded skins will break a save file if the mod is uninstalled while it is selected
        /// </summary>
        /// <param name="skin"></param>
        /// <returns></returns>
        public static SkinDef CreateNewSkinDef( SkinDefInfo skin )
        {
            On.RoR2.SkinDef.Awake += SkinDef_Awake;

            SkinDef newSkin = ScriptableObject.CreateInstance<SkinDef>();

            newSkin.baseSkins = skin.baseSkins;
            newSkin.icon = skin.icon;
            newSkin.unlockableName = skin.unlockableName;
            newSkin.rootObject = skin.rootObject;
            newSkin.rendererInfos = skin.rendererInfos;
            newSkin.nameToken = skin.nameToken;
            newSkin.name = skin.name;



            On.RoR2.SkinDef.Awake -= SkinDef_Awake;
            return newSkin;
        }

        /// <summary>
        /// Creates and returns a RendererInfo with the specifified settings
        /// </summary>
        /// <param Renderer="r"></param>
        /// <param Default Material="m"></param>
        /// <returns></returns>
        public static CharacterModel.RendererInfo CreateRendererInfo( Renderer r, Material m, System.Boolean ignoreOverlays, UnityEngine.Rendering.ShadowCastingMode shadow )
        {
            CharacterModel.RendererInfo ren = new CharacterModel.RendererInfo();
            ren.renderer = r;
            ren.defaultMaterial = m;
            ren.ignoreOverlays = ignoreOverlays;
            ren.defaultShadowCastingMode = shadow;
            return ren;
        }

        /// <summary>
        /// Struct used to define the fields for a SkinDef before actually creating it
        /// </summary>
        public struct SkinDefInfo
        {
            public SkinDef[] baseSkins;
            public Sprite icon;
            public System.String nameToken;
            public System.String unlockableName;
            public GameObject rootObject;
            public CharacterModel.RendererInfo[] rendererInfos;
            public System.String name;
        }
        #endregion

        #region Internal
        private static void SkinDef_Awake( On.RoR2.SkinDef.orig_Awake orig, SkinDef self )
        {
            //Intentionally do nothing
        }
        #endregion
    }
}