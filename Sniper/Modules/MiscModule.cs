﻿namespace Rein.Sniper.Modules
{
    using RoR2;

    using Rein.Sniper.ScriptableObjects;

    using UnityEngine;

    internal static class MiscModule
    {
        internal static GameObject GetPodPrefab()
        {
            if( _podPrefab == null )
            {
                _podPrefab = CreatePodPrefab();
            }
            return _podPrefab;
        }
        private static GameObject _podPrefab;
        private static GameObject CreatePodPrefab() =>
            Resources.Load<GameObject>("Prefabs/CharacterBodies/CommandoBody").GetComponent<CharacterBody>().preferredPodPrefab;
            // FUTURE: Create sniper survivor pod prefab
            //null;

        internal static CharacterCameraParams GetCharCameraParams()
        {
            if( _sniperCharCameraParams == null )
            {
                _sniperCharCameraParams = CreateSniperCharCameraParams();
            }
            return _sniperCharCameraParams;
        }
        private static CharacterCameraParams _sniperCharCameraParams;
        private static CharacterCameraParams CreateSniperCharCameraParams()
        {
            var param = SniperCameraParams.Create( new Vector3( 1f, 0.8f, -3f ), 0.5f, 0.2f, 1.0f, 0.2f );
            param.name = "ccpSniper";
            param.minPitch = -70f;
            param.maxPitch = 70f;
            param.wallCushion = 0.1f;
            param.pivotVerticalOffset = 1.37f;
            param.standardLocalCameraPos = new Vector3( 0f, 0f, -8.18f );
            return param;
        }
    }
}
