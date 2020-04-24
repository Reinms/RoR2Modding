﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using BepInEx.Logging;
using ReinCore;
using RoR2;
using UnityEngine;

namespace Sniper.Expansions
{
    internal delegate void OnBulletDelegate( ExpandableBulletAttack bullet, BulletAttack.BulletHit hitInfo );
    internal class ExpandableBulletAttack : BulletAttack, ICloneable
    {

        internal event OnBulletDelegate onHit;
        internal event OnBulletDelegate onStop;
        internal CharacterBody attackerBody;

        internal ExpandableBulletAttack() : base()
        {
            base.hitCallback = this.ExpandableHitCallback;
        }

        private Boolean ExpandableHitCallback( ref BulletHit hitInfo )
        {
            var result = base.DefaultHitCallback( ref hitInfo );
            this.onHit?.Invoke( this, hitInfo );
            if( !result && this.onStop != null )
            {
                this.onStop( this, hitInfo );
            }
            return result;
        }

        public System.Object Clone()
        {
            var res = new ExpandableBulletAttack
            {
                aimVector = this.aimVector,
                attackerBody = this.attackerBody,
                bulletCount = this.bulletCount,
                damage = this.damage,
                damageColorIndex = this.damageColorIndex,
                damageType = this.damageType,
                falloffModel = this.falloffModel,
                filterCallback = this.filterCallback,
                force = this.force,
                HitEffectNormal = this.HitEffectNormal,
                hitEffectPrefab = this.hitEffectPrefab,
                hitMask = this.hitMask,
                isCrit = this.isCrit,
                maxSpread = this.maxSpread,
                maxDistance = this.maxDistance,
                minSpread = this.minSpread,
                muzzleName = this.muzzleName,
                origin = this.origin,
                owner = this.owner,
                procChainMask = this.procChainMask,
                procCoefficient = this.procCoefficient,
                queryTriggerInteraction = this.queryTriggerInteraction,
                radius = this.radius,
                smartCollision = this.smartCollision,
                sniper = this.sniper,
                spreadPitchScale = this.spreadPitchScale,
                spreadYawScale = this.spreadYawScale,
                stopperMask = this.stopperMask,
                tracerEffectPrefab = this.tracerEffectPrefab,
                weapon = this.weapon,
            };

            if( this.onHit != null )
            {
                foreach( var v in this.onHit.GetInvocationList() )
                {
                    var del = v as OnBulletDelegate;
                    if( del != null ) res.onHit += del;
                }
            }

            if( this.onStop != null )
            {
                foreach( var v in this.onStop.GetInvocationList() )
                {
                    var del = v as OnBulletDelegate;
                    if( del != null ) res.onStop += del;
                }
            }
            return res;
        }
    }
}