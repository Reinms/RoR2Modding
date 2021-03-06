﻿using System;
using System.Collections.Generic;

using Rein.RogueWispPlugin.Helpers;

using RoR2;
using RoR2.Orbs;

using UnityEngine;

namespace Rein.RogueWispPlugin
{
    internal partial class Main
    {
        internal class BlazeOrb : RoR2.Orbs.Orb, IOrbFixedUpdateBehavior
        {
            public GameObject obj;

            //Blaze settings
            public Single blazeTime = 1f;
            public Single blazeFreq = 1f;
            public Single blazeRadius = 1f;
            public Single blazeExpireStacksMult = 1f;
            public Single blazeBuffDuration = 1f;
            public Single blazeOrbBundleSendFreq = 1f;
            public Single blazeBonusBundlePercent = 1f;

            public UInt32 blazeOrbStackBundleSize = 1;

            //Ignite settings
            public Single igniteDuration = 1f;
            public Single igniteStacksPerTick = 1f;
            public Single igniteTickDamage = 1f;
            public Single igniteProcCoef = 1f;
            public Single igniteTickFreq = 1f;
            public Single igniteBaseStacksOnDeath = 1f;
            public Single igniteDeathStacksMult = 1f;
            public Single igniteExpireStacksMult = 1f;

            public DamageColorIndex igniteDamageColor = DamageColorIndex.Default;

            //Global settings
            public Boolean crit = false;

            public UInt32 skin = 0;

            public Boolean isActive = true;
            public Boolean isOwnerInside = false;

            public TeamIndex team;
            public Vector3 normal;

            public GameObject attacker;

            public List<IgnitionOrb> children;

            //Private stuff
            private Single blazeResetInt;
            private Single bundleSendInt;
            private Single curTime;
            private Single bundleSendTimer = 0f;
            private Single stacks = 0f;

            private HurtBox attackerBox;

            private CharacterBody attackerBody;

            private BuffIndex b;

            private readonly Dictionary<HealthComponent, Single> mask = new Dictionary<HealthComponent, Single>();
            private readonly Dictionary<HealthComponent, Main.WispBurnManager> burnManagers = new Dictionary<HealthComponent, WispBurnManager>();

            private Dictionary<HealthComponent,Hit> bestHits = new Dictionary<HealthComponent, Hit>();

            private HashSet<Collider> collidersInZone = new HashSet<Collider>();
            private Dictionary<HealthComponent, TargetContext> targetContexts = new Dictionary<HealthComponent, TargetContext>();
            private TriggerCallbackController hitDetection;
            private AssociationMap<HealthComponent,HurtBox> targetBoxes = new AssociationMap<HealthComponent, HurtBox>();

            private struct Hit
            {
                public Single distance;
                public HurtBox box;

                public Hit( HurtBox box, Single distance )
                {
                    this.distance = distance;
                    this.box = box;
                }
            }

            private class TargetContext
            {
                public Single timer;
                public Main.WispBurnManager burnManager;
                public HealthComponent healthComponent;
                public HurtBox mainHurtBox;

                public TargetContext( Main.WispBurnManager manager, HealthComponent hc, Single initTimer = 0f )
                {
                    this.timer = initTimer;
                    this.burnManager = manager;
                    this.healthComponent = hc;
                    this.mainHurtBox = hc.body.mainHurtBox;
                }
            }

            public override void Begin()
            {
                this.b = Main.RW_curseBurn;

                this.hitDetection = TriggerCallbackController.CreateSphere( this.blazeRadius, base.origin, true, LayerIndex.projectile );
                this.hitDetection.onTriggerEnter += this.ColliderEnter;
                this.hitDetection.onTriggerExit += this.ColliderExit;

                this.duration = this.blazeTime;

                this.isActive = true;

                Vector3 tangent = Vector3.forward;
                Vector3.OrthoNormalize( ref this.normal, ref tangent );

                EffectData effectData = new EffectData
                {
                    origin = base.origin,
                    genericFloat = base.duration,
                    rotation = Quaternion.identity,
                    scale = this.blazeRadius * 2f,
                    genericUInt = this.skin,
                };
                EffectManager.SpawnEffect( Main.utilityIndicator, effectData, true );

                this.blazeResetInt = 1f / this.blazeFreq;
                this.bundleSendInt = 1f / this.blazeOrbBundleSendFreq;

                this.attackerBody = this.attacker.GetComponent<CharacterBody>();
                this.attackerBox = this.attackerBody.mainHurtBox;
            }
            private void ColliderStay( Collider col )
            {
                if( this.collidersInZone.Contains( col ) ) return;
                this.ColliderEnter( col );
            }
            private void ColliderEnter( Collider col )
            {
                this.collidersInZone.Add( col );
                var hb = col?.GetComponent<HurtBox>();
                if( hb == null ) return;
                var hc = hb.healthComponent;
                if( hc == null ) return;
                if( !FriendlyFireManager.ShouldDirectHitProceed( hc, this.team ) ) return;
                if( hc == this.attackerBody.healthComponent ) return;

                this.targetBoxes.Add( hc, hb );
            }
            private void ColliderExit( Collider col )
            {
                if( this.collidersInZone.Contains( col ) ) this.collidersInZone.Remove( col );
                var hb = col?.GetComponent<HurtBox>();
                if( hb == null ) return;
                if( !this.targetBoxes.ContainsValue( hb ) ) return;
                var hc = hb.healthComponent;
                if( hc == null ) return;
                if( !FriendlyFireManager.ShouldDirectHitProceed( hc, this.team ) ) return;
                if( hc == this.attackerBody.healthComponent ) return;

                this.targetBoxes.RemoveValue( hb );
            }

            public void FixedUpdate()
            {
                this.isOwnerInside = this.attacker && Vector3.Distance( this.attacker.transform.position, this.origin ) <= this.blazeRadius;


                var dt = Time.fixedDeltaTime;
                foreach( var hc in this.targetBoxes.keys )
                {
                    if( hc == null || !hc.alive ) continue;
                    if( !this.targetContexts.ContainsKey( hc ) )
                    {
                        var burn = hc?.AddOrGetComponent<WispBurnManager>();
                        this.targetContexts[hc] = new TargetContext( burn, hc, this.blazeResetInt );
                    }
                    var context = this.targetContexts[hc];
                    context.timer += dt;

                    while( context.timer >= this.blazeResetInt )
                    {
                        context.timer -= this.blazeResetInt;
                        this.TickDamage( context );
                    }
                }

                if( this.isOwnerInside ) this.bundleSendTimer += Time.fixedDeltaTime;
                if( this.isOwnerInside && this.stacks >= this.blazeOrbStackBundleSize && this.bundleSendTimer >= this.bundleSendInt ) this.SendStackBundle();
                if( this.isOwnerInside && !this.attackerBody.HasBuff( BuffIndex.EnrageAncientWisp ) ) this.attackerBody.AddBuff( BuffIndex.EnrageAncientWisp );
                if( !this.isOwnerInside && this.attackerBody.HasBuff( BuffIndex.EnrageAncientWisp ) ) this.attackerBody.RemoveBuff( BuffIndex.EnrageAncientWisp );
            }

            public override void OnArrival()
            {
                base.OnArrival();
                //Ceil stacks to int and send if owner is in radius

                if( this.isOwnerInside && this.stacks > 0 )
                {
                    RestoreOrb orb = new RestoreOrb();
                    orb.stacks = (UInt32)Mathf.CeilToInt( ( this.stacks ) / this.blazeExpireStacksMult );
                    orb.length = this.blazeBuffDuration;
                    orb.origin = this.origin;
                    orb.skin = this.skin;
                    orb.target = this.attackerBox;

                    OrbManager.instance.AddOrb( orb );
                }

                if( this.attackerBody.HasBuff( BuffIndex.EnrageAncientWisp ) ) this.attackerBody.RemoveBuff( BuffIndex.EnrageAncientWisp );

                this.isActive = false;

                this.hitDetection.Cleanup();
                UnityEngine.Object.Destroy( this.obj );
            }

            public void AddStacks( Single stacks, IgnitionOrb i )
            {
                if( !this.isActive ) return;
                this.stacks += stacks;
                var distance = ( i.targetPos - this.origin ).magnitude;
                var delay = distance / 60f;

                EffectData fx = new EffectData
                {
                    origin = i.targetPos,
                    start = origin,
                    genericFloat = delay,
                    genericUInt = this.skin,
                    scale = 0.25f,
                    genericBool = false
                };
                EffectManager.SpawnEffect( Main.utilityLeech, fx, true );
            }

            private void SendStackBundle()
            {
                this.bundleSendTimer -= this.bundleSendInt;
                this.stacks -= this.blazeOrbStackBundleSize;
                Int32 bonus = Mathf.FloorToInt(this.stacks * this.blazeBonusBundlePercent);
                this.stacks -= bonus;

                if( this.bundleSendTimer > 0f ) this.bundleSendTimer *= 0.25f;

                RestoreOrb orb = new RestoreOrb();
                orb.stacks = (UInt32)Mathf.CeilToInt( this.blazeOrbStackBundleSize + bonus );
                orb.length = this.blazeBuffDuration;
                orb.origin = this.origin;
                orb.skin = this.skin;
                orb.target = this.attackerBox;

                OrbManager.instance.AddOrb( orb );
            }

            private void TickDamage( HurtBox enemy )
            {
                ( this.burnManagers.ContainsKey( enemy.healthComponent ) ? this.burnManagers[enemy.healthComponent] : ( this.burnManagers[enemy.healthComponent] = enemy.healthComponent.GetComponent<WispBurnManager>() ) ).SetSkinDuration( this.skin, this.igniteDuration );


                IgnitionOrb orb = new IgnitionOrb();

                orb.attacker = this.attacker;
                orb.crit = this.crit;
                orb.normal = Vector3.Normalize( enemy.transform.position - this.origin );
                orb.origin = enemy.transform.position;
                orb.skin = this.skin;
                orb.target = enemy;
                orb.team = this.team;
                orb.igniteStacksPerTick = this.igniteStacksPerTick;
                orb.igniteProcCoef = this.igniteProcCoef;
                orb.igniteTickDmg = this.igniteTickDamage;
                orb.igniteTickFreq = this.igniteTickFreq;
                orb.igniteTime = this.igniteDuration;
                orb.igniteDamageColor = this.igniteDamageColor;
                orb.igniteDeathStacksMult = this.igniteDeathStacksMult;
                orb.igniteBaseStacksOnDeath = this.igniteBaseStacksOnDeath;
                orb.igniteExpireStacksMult = this.igniteExpireStacksMult;
                orb.parent = this;

                RoR2.Orbs.OrbManager.instance.AddOrb( orb );
                this.children.Add( orb );

                enemy.healthComponent.gameObject.GetComponent<CharacterBody>().AddTimedBuff( this.b, this.igniteDuration );
            }

            private void TickDamage( TargetContext enemy )
            {
                if( enemy == null || enemy.healthComponent == null || enemy.mainHurtBox == null ) return;
                if( enemy.burnManager != null )
                {
                    enemy.burnManager.SetSkinDuration( this.skin, this.igniteDuration );
                }
                IgnitionOrb orb = new IgnitionOrb();

                orb.attacker = this.attacker;
                orb.crit = this.crit;
                orb.normal = Vector3.Normalize( enemy.mainHurtBox.transform.position - this.origin );
                orb.origin = enemy.mainHurtBox.transform.position;
                orb.skin = this.skin;
                orb.target = enemy.mainHurtBox;
                orb.team = this.team;
                orb.igniteStacksPerTick = this.igniteStacksPerTick;
                orb.igniteProcCoef = this.igniteProcCoef;
                orb.igniteTickDmg = this.igniteTickDamage;
                orb.igniteTickFreq = this.igniteTickFreq;
                orb.igniteTime = this.igniteDuration;
                orb.igniteDamageColor = this.igniteDamageColor;
                orb.igniteDeathStacksMult = this.igniteDeathStacksMult;
                orb.igniteBaseStacksOnDeath = this.igniteBaseStacksOnDeath;
                orb.igniteExpireStacksMult = this.igniteExpireStacksMult;
                orb.parent = this;

                RoR2.Orbs.OrbManager.instance.AddOrb( orb );
                this.children.Add( orb );

                enemy.healthComponent.body.AddTimedBuff( this.b, this.igniteDuration );
            }
        }
    }
}