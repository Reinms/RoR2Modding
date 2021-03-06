﻿namespace Rein.Sniper.Modules
{
    using ReinCore;

    using RoR2;
    using RoR2.Projectile;

    using UnityEngine;

    internal static class ProjectileGhostModule
    {
#pragma warning disable IDE1006 // Naming Styles
        private static GameObject _baseKnifeGhost;
#pragma warning restore IDE1006 // Naming Styles
        private static GameObject GetBaseKnifeGhost()
        {
            if( _baseKnifeGhost == null )
            {
                _baseKnifeGhost = CreateBaseKnifeGhost();
            }

            return _baseKnifeGhost;
        }
        private static GameObject CreateBaseKnifeGhost()
        {
            GameObject obj = AssetModule.GetSniperAssetBundle().LoadAsset<GameObject>( Properties.Resources.prefab__KnifeGhostPrefab );

            _ = obj.AddOrGetComponent<ProjectileGhostController>();

            VFXAttributes vfx = obj.AddOrGetComponent<VFXAttributes>();
            vfx.vfxPriority = VFXAttributes.VFXPriority.Always;
            vfx.vfxIntensity = VFXAttributes.VFXIntensity.Low;
            vfx.optionalLights = null;
            vfx.secondaryParticleSystem = null;

            obj.transform.localScale = new Vector3( 2f, 2f, 2f );

            return obj;
        }

        internal static GameObject GetKnifeGhost( Material meshMaterial, Material trailMaterial )
        {
            GameObject obj = GetBaseKnifeGhost().ClonePrefab( "KnifeGhost", false );

            obj.GetComponentInChildren<MeshRenderer>().sharedMaterial = meshMaterial;
          
            obj.GetComponentInChildren<TrailRenderer>().sharedMaterial = trailMaterial;

            return obj;
        }
    }
}
