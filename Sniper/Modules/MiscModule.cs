﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using BepInEx.Logging;
using ReinCore;
using RoR2;
using Sniper.ScriptableObjects;
using UnityEngine;

namespace Sniper.Modules
{
    internal static class MiscModule
    {
        internal static GameObject GetPodPrefab()
        {
            // TODO: Create Pod Prefab
            return null;
        }

        internal static CharacterCameraParams GetCharCameraParams()
        {
            var param = SniperCameraParams.Create( new Vector3( 1f, 0.8f, -3f ) );
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