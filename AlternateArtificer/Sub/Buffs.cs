﻿namespace AlternativeArtificer
{
    using AlternativeArtificer.States.Main;

    using R2API;

    using RoR2;

    using UnityEngine;

    public partial class Main
    {
        public BuffIndex fireBuff;
        public BuffIndex burnBuff;
        public DotController.DotIndex burnDot;

        private void DoBuffs() => this.AddFireBuff();

        private void AddFireBuff()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            this.fireBuff = (BuffIndex)ItemAPI.AddCustomBuff( new CustomBuff( "AltArtiFireBuff", new BuffDef
            {
                buffColor = new Color( 0.9f, 0.2f, 0.2f ),
                canStack = true,
                iconPath = "Textures/BuffIcons/texBuffAffixRed",
                isDebuff = false,
                name = "AltArtiFireBuff"
            } ) );
#pragma warning restore CS0618 // Type or member is obsolete

            this.burnBuff = BuffIndex.OnFire;
            this.burnDot = DotController.DotIndex.Burn;
            AltArtiPassive.fireBuff = this.fireBuff;
        }
    }
}
