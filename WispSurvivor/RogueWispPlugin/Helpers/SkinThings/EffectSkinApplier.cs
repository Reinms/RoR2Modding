﻿
using RoR2;

using UnityEngine;

namespace Rein.RogueWispPlugin.Helpers
{
    [RequireComponent( typeof( EffectComponent ), typeof( WispSkinnedEffect ) )]
    internal class EffectSkinApplier : MonoBehaviour
    {
        public WispSkinnedEffect skinController;
        public EffectComponent effectComponent;

        private void Start()
        {
            this.skinController.Apply( WispBitSkin.GetWispSkin( this.effectComponent.effectData.genericUInt ) );
        }

    }
}
