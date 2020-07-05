﻿namespace Sniper.States.Bases
{
    using System;
    using System.Diagnostics;

    using Sniper.Data;
    using Sniper.Enums;
    using Sniper.Expansions;
    using Sniper.Modules;

    using UnityEngine;
    using UnityEngine.Networking;

    internal abstract class SnipeBaseState : SniperSkillBaseState
    {
        protected abstract Single baseDuration { get; }
        protected abstract Single recoilStrength { get; }

        internal ReloadParams reloadParams { get; set; }

        internal ReloadTier reloadTier { private get; set; }

        private Boolean bulletFired = false;
        private Single duration;

        private Single charge;

        private SoundModule.FireType fireSoundType = SoundModule.FireType.Normal;

        protected abstract void ModifyBullet( ExpandableBulletAttack bullet );

        private static Stopwatch timer = new Stopwatch();

        private void FireBullet()
        {
            if( this.bulletFired )
            {
                return;
            }
            Ray aimRay = this.GetAimRay();

            var ammo = base.characterBody.ammo;
            this.fireSoundType = ammo.fireSoundType;
            ExpandableBulletAttack bullet = ammo.CreateBullet( base.characterBody, this.reloadTier, aimRay, "MuzzleRailgun" );
            //var bullet = new ExpandableBulletAttack
            //{
            //    aimVector = aimRay.direction,
            //    attackerBody = characterBody,
            //    bulletCount = 1,
            //    damage = characterBody.damage,
            //    damageColorIndex = DamageColorIndex.Default,
            //    damageType = DamageType.Generic,
            //    falloffModel = BulletAttack.FalloffModel.None,
            //    force = 1f,
            //    HitEffectNormal = true,
            //    hitEffectPrefab = null,
            //    hitMask = LayerIndex.entityPrecise.mask | LayerIndex.world.mask,
            //    isCrit = RollCrit(),
            //    maxDistance = 1000f,
            //    maxSpread = 0f,
            //    minSpread = 0f,
            //    muzzleName = "MuzzleRailgun",
            //    origin = aimRay.origin,
            //    owner = gameObject,
            //    procChainMask = default,
            //    procCoefficient = 1f,
            //    queryTriggerInteraction = QueryTriggerInteraction.UseGlobal,
            //    radius = 1f,
            //    smartCollision = true,
            //    sniper = false,
            //    spreadPitchScale = 1f,
            //    spreadYawScale = 1f,
            //    stopperMask = LayerIndex.world.mask | LayerIndex.entityPrecise.mask,
            //    tracerEffectPrefab = null,
            //    weapon = null,
            //};
            this.ModifyBullet( bullet );
            this.reloadParams.ModifyBullet( bullet, this.reloadTier );
            this.characterBody.passive.ModifyBullet( bullet );
            SkillDefs.SniperScopeSkillDef.ScopeInstanceData data = this.characterBody.scopeInstanceData;

            if( data != null && data.shouldModify )
            {
                data.SendFired().Apply( bullet );
            }

            this.charge = bullet.chargeLevel;
            bullet.Fire();

            this.AddRecoil( -1f * this.recoilStrength, -3f * this.recoilStrength, -0.2f * this.recoilStrength, 0.2f * this.recoilStrength );

            this.bulletFired = true;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = this.baseDuration / this.characterBody.attackSpeed;
            base.StartAimMode( 2f, false );

            if( this.isAuthority )
            {
                this.FireBullet();
            }

            base.PlayAnimation( "Gesture, Additive", "Shoot", "rateShoot", this.duration );

            SoundModule.PlayFire( base.gameObject, this.charge, this.fireSoundType );

        }

        public override void OnSerialize( NetworkWriter writer )
        {
            base.OnSerialize( writer );
            writer.Write( this.charge );
            writer.Write( (Byte)this.fireSoundType );
        }

        public override void OnDeserialize( NetworkReader reader )
        {
            base.OnDeserialize( reader );
            this.charge = reader.ReadSingle();
            this.fireSoundType = (SoundModule.FireType)reader.ReadByte();
        }



        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if( this.isAuthority && this.fixedAge >= this.duration )
            {
                this.outer.SetNextStateToMain();
            }
        }
    }
}
