﻿using RoR2;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace WispSurvivor.Modules
{
    public static class WispCameraModule
    {
        public static void DoModule( GameObject body , Dictionary<Type,Component> dic )
        {
            AddChildren(body , dic);
            CamSettings(body, dic);
        }

        private static void AddChildren( GameObject body , Dictionary<Type,Component> dic )
        {
            GameObject cameraPivot = new GameObject("CameraPivot");
            GameObject aimOrigin = new GameObject("AimOrigin");
            MonoBehaviour.DontDestroyOnLoad(cameraPivot);
            MonoBehaviour.DontDestroyOnLoad(aimOrigin);
            cameraPivot.transform.SetParent(body.transform, false);
            aimOrigin.transform.SetParent(body.transform, false);
            cameraPivot.transform.localPosition = new Vector3(0f, 3.25f, 0f);
            aimOrigin.transform.localPosition = new Vector3(0f, 2.35f, 0f);
            dic.C<CameraTargetParams>().cameraPivotTransform = cameraPivot.transform;
            dic.C<CharacterBody>().aimOriginTransform = aimOrigin.transform;
        }

        private static void CamSettings( GameObject body , Dictionary<Type,Component> dic )
        {
            CharacterCameraParams camSettings = ScriptableObject.CreateInstance<CharacterCameraParams>();
            camSettings.minPitch = -70f;
            camSettings.maxPitch = 70f;
            camSettings.wallCushion = 0.05f;
            camSettings.pivotVerticalOffset = 1.5f;
            camSettings.standardLocalCameraPos = new Vector3(0f, 0f, -10.86f);
            dic.C<CameraTargetParams>().cameraParams = camSettings;
            dic.C<CameraTargetParams>().idealLocalCameraPos = new Vector3(0f, 0f, -4.71f);
        }

        private static T C<T>( this Dictionary<Type,Component> dic ) where T : Component
        {
            return dic[typeof(T)] as T;
        }
    }
}
