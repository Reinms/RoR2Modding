﻿namespace Sniper.ScriptableObjects
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using RoR2;
    using UnityEngine;

    internal class SniperCameraParams : CharacterCameraParams
    {
        internal static SniperCameraParams Create( Vector3 throwPosition )
        {
            SniperCameraParams obj = ScriptableObject.CreateInstance<SniperCameraParams>();
            obj.throwLocalCameraPos = throwPosition;

            return obj;
        }

        [SerializeField]
        internal Vector3 throwLocalCameraPos;
    }
}
