﻿namespace AlternativeArtificer.Helpers
{
    using System;
    using UnityEngine;
    public class InstancedRandom
    {
        private UnityEngine.Random.State localState;
        private UnityEngine.Random.State globalState;

        public InstancedRandom( Int32 seed )
        {
            this.globalState = UnityEngine.Random.state;
            UnityEngine.Random.InitState( seed );
            this.localState = UnityEngine.Random.state;
            UnityEngine.Random.state = globalState;
        }
        public Vector2 InsideUnitCircle()
        {
            this.CacheState();
            var temp = UnityEngine.Random.insideUnitCircle;
            this.RestoreState();
            return temp;
        }
        public Vector3 InsideUnitSphere()
        {
            this.CacheState();
            var temp = UnityEngine.Random.insideUnitSphere;
            this.RestoreState();
            return temp;
        }
        public Vector3 OnUnitSphere()
        {
            this.CacheState();
            var temp = UnityEngine.Random.onUnitSphere;
            this.RestoreState();
            return temp;
        }
        public Quaternion Rotation()
        {
            this.CacheState();
            var temp = UnityEngine.Random.rotation;
            this.RestoreState();
            return temp;
        }
        public Quaternion RotationUniform()
        {
            this.CacheState();
            var temp = UnityEngine.Random.rotationUniform;
            this.RestoreState();
            return temp;
        }
        public Single Value()
        {
            this.CacheState();
            var temp = UnityEngine.Random.value;
            this.RestoreState();
            return temp;
        }
        public Single Range( Single min, Single max )
        {
            this.CacheState();
            var temp = UnityEngine.Random.Range( min, max );
            this.RestoreState();
            return temp;
        }
        public Color ColorHSV()
        {
            this.CacheState();
            var temp = UnityEngine.Random.ColorHSV();
            this.RestoreState();
            return temp;
        }
        public Color ColorHSV( Single hueMin, Single hueMax )
        {
            this.CacheState();
            var temp = UnityEngine.Random.ColorHSV( hueMin, hueMax );
            this.RestoreState();
            return temp;
        }
        public Color ColorHSV( Single hueMin, Single hueMax, Single saturationMin, Single saturationMax )
        {
            this.CacheState();
            var temp = UnityEngine.Random.ColorHSV( hueMin, hueMax, saturationMin, saturationMax );
            this.RestoreState();
            return temp;
        }
        public Color ColorHSV( Single hueMin, Single hueMax, Single saturationMin, Single saturationMax, Single valueMin, Single valueMax )
        {
            this.CacheState();
            var temp = UnityEngine.Random.ColorHSV( hueMin, hueMax, saturationMin, saturationMax, valueMin, valueMax );
            this.RestoreState();
            return temp;
        }
        public Color ColorHSV( Single hueMin, Single hueMax, Single saturationMin, Single saturationMax, Single valueMin, Single valueMax, Single alphaMin, Single alphaMax )
        {
            this.CacheState();
            var temp = UnityEngine.Random.ColorHSV( hueMin, hueMax, saturationMin, saturationMax, valueMin, valueMax, alphaMin, alphaMax );
            this.RestoreState();
            return temp;
        }

        private void CacheState()
        {
            this.globalState = UnityEngine.Random.state;
            UnityEngine.Random.state = this.localState;
        }

        private void RestoreState()
        {
            this.localState = UnityEngine.Random.state;
            UnityEngine.Random.state = this.globalState;
        }
    }
}