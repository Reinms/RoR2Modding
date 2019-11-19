﻿using RoR2;
using RoR2.Orbs;
using System;
using UnityEngine;

namespace WispSurvivor.Orbs
{
    internal class IgnitionOrb : RoR2.Orbs.Orb, IOrbFixedUpdateBehavior
    {
        //Ignite settings
        public Single igniteTime = 1f;
        public Single igniteTickDmg = 1f;
        public Single igniteTickFreq = 1f;
        public Single igniteProcCoef = 1f;
        public Single igniteStacksPerTick = 1f;
        public Single igniteBaseStacksOnDeath = 0f;
        public Single igniteDeathStacksMult = 1f;
        public Single igniteExpireStacksMult = 1f;

        public DamageColorIndex igniteDamageColor = DamageColorIndex.Default;

        public BlazeOrb parent;

        //Global settings
        public Boolean crit = false;
        public Boolean isActive = true;

        public UInt32 skin = 0;

        public TeamIndex team;

        public Vector3 normal;
        public Vector3 targetPos;

        public GameObject attacker;

        //Private values
        private Single damageInterval;
        private Single damageTimer;
        private Single durationTimer;
        private Single chargeMult;

        private Boolean dead = false;
        private Boolean wasDead = false;
        private Boolean boosted = false;
        private Boolean firstTick = true;

        public override void Begin()
        {
            this.duration = this.igniteTime;
            this.isActive = true;

            Vector3 tangent = Vector3.forward;
            Vector3.OrthoNormalize( ref this.normal, ref tangent );

            //EffectData effectData = new EffectData
            //{
            //    origin = origin,
            //    genericFloat = duration,
            //    rotation = Quaternion.LookRotation(tangent, this.normal)
            //};
            //effectData.SetHurtBoxReference( this.target );

            //EffectManager.instance.SpawnEffect( Modules.WispEffectModule.utilityBurns[this.skin], effectData, true );

            this.damageInterval = 1f / this.igniteTickFreq;
            this.damageTimer = this.damageInterval;

            CharacterBody targetBody = this.target.healthComponent.GetComponent<CharacterBody>();
            Inventory targetInv = (targetBody ? targetBody.inventory : null) ;

            this.chargeMult = GetChargeMult(targetInv ? targetInv.GetItemCount( ItemIndex.BoostDamage ): 0, targetBody.baseNameToken);

            //this.isLonk = this.target.healthComponent.gameObject.GetComponent<CharacterBody>().hullClassification == HullClassification.Golem || this.target.healthComponent.gameObject.GetComponent<CharacterBody>().hullClassification == HullClassification.BeetleQueen;
        }

        public void FixedUpdate()
        {
            try
            {
                this.boosted = this.parent.isActive && this.parent.isOwnerInside;
            } catch( NullReferenceException e )
            {
                this.boosted = false;
            }

            this.durationTimer += Time.fixedDeltaTime;

            if( !this.target ) this.dead = true;
            if( !this.target.healthComponent ) this.dead = true;
            if( !this.target.healthComponent.alive ) this.dead = true;
            if( this.dead && !this.wasDead ) this.OnDead();
            if( this.dead ) return;

            targetPos = target.transform.position;

            this.damageTimer += Time.fixedDeltaTime * (this.boosted ? 1f : 0.5f);

            while( this.damageTimer >= this.damageInterval )
            {
                this.TickDamage( this.target );
                this.damageTimer -= this.damageInterval;
            }
        }

        public override void OnArrival()
        {
            if( this.isActive )
            {
                this.OnDead( this.igniteExpireStacksMult );
            }
        }

        private void TickDamage( HurtBox enemy )
        {
            if( !enemy ) return;
            if( !enemy.healthComponent ) return;
            if( !enemy.healthComponent.gameObject ) return;
            if( !this.attacker ) return;
            //Damage info stuff here
            DamageInfo d = new DamageInfo();
            d.damage = this.igniteTickDmg;
            d.attacker = this.attacker;
            d.inflictor = null;
            d.force = Vector3.zero;
            d.crit = this.crit;
            d.procChainMask = new ProcChainMask();
            d.procCoefficient = (this.boosted ? this.igniteProcCoef : 0f) * (this.firstTick ? 1f : 1f);
            d.position = enemy.transform.position;
            d.damageColorIndex = this.igniteDamageColor;

            this.firstTick = false;

            enemy.healthComponent.TakeDamage( d );
            GlobalEventManager.instance.OnHitEnemy( d, enemy.healthComponent.gameObject );
            GlobalEventManager.instance.OnHitAll( d, enemy.healthComponent.gameObject );
            this.parent.AddStacks( this.igniteStacksPerTick * this.chargeMult, this);
        }

        private void OnDead( Single mult = 1f )
        {
            this.wasDead = true;

            this.isActive = false;

            Single value = mult * (this.igniteBaseStacksOnDeath + ((this.igniteTime - this.durationTimer) * this.igniteTickFreq * this.igniteStacksPerTick * this.igniteDeathStacksMult));

            if( this.parent.isActive )
            {
                this.parent.AddStacks( value * this.chargeMult, this );
                this.parent.children.Remove( this );
            } else
            {
                //Do a cool thing here based on value (explosion I guess? modified will o wisp explosion could be fun...)
            }
        }

        private float GetChargeMult(Int32 damageBoost, String name )
        {
            float temp = 1f;

            temp *= 1f + ((Single)damageBoost / 10f);

            switch( GetMonsterTier( name ) )
            {
                case MonsterTier.Basic:
                    temp *= 0.35f;
                    break;
                case MonsterTier.Small:
                    temp *= 0.5f;
                    break;
                case MonsterTier.Mid:
                    temp *= 0.65f;
                    break;
                case MonsterTier.Miniboss:
                    temp *= 0.85f;
                    break;
                case MonsterTier.Boss:
                    temp *= 1.0f;
                    break;
                case MonsterTier.SuperBoss:
                    temp *= 6f;
                    break;
                case MonsterTier.HyperBoss:
                    temp *= 1f;
                    break;
                case MonsterTier.Other:
                    temp *= 0.1f;
                    break;
            }
            return temp;
        }

        private enum MonsterTier
        {
            Basic = 0,
            Small = 1,
            Mid = 2,
            Miniboss = 3,
            Boss = 4,
            SuperBoss = 5,
            HyperBoss = 6,
            Other = 7
        }

        private MonsterTier GetMonsterTier( string name )
        {
            switch( name )
            {
                default:
                    Debug.Log( name + " is not a handled monster" );
                    return MonsterTier.Other;

                case "LEMURIAN_BODY_NAME":
                    return MonsterTier.Basic;
                case "WISP_BODY_NAME":
                    return MonsterTier.Basic;
                case "BEETLE_BODY_NAME":
                    return MonsterTier.Basic;
                case "JELLYFISH_BODY_NAME":
                    return MonsterTier.Basic;
                case "CLAY_BODY_NAME":
                    return MonsterTier.Basic;

                
                case "HERMIT_CRAB_BODY_NAME":
                    return MonsterTier.Small;
                case "IMP_BODY_NAME":
                    return MonsterTier.Small;
                case "ROBOBALLMINI_BODY_NAME":
                    return MonsterTier.Small;
                case "VULTURE_BODY_NAME":
                    return MonsterTier.Small;
                case "URCHINTURRET_BODY_NAME":
                    return MonsterTier.Small;


                case "GOLEM_BODY_NAME":
                    return MonsterTier.Mid;
                case "BEETLEGUARD_BODY_NAME":
                    return MonsterTier.Mid;
                case "BELL_BODY_NAME":
                    return MonsterTier.Mid;
                case "BISON_BODY_NAME":
                    return MonsterTier.Mid;


                case "LEMURIANBRUISER_BODY_NAME":
                    return MonsterTier.Miniboss;
                case "GREATERWISP_BODY_NAME":
                    return MonsterTier.Miniboss;
                case "CLAYBRUISER_BODY_NAME":
                    return MonsterTier.Miniboss;
                case "ARCHWISP_BODY_NAME":
                    return MonsterTier.Miniboss;


                case "BEETLEQUEEN_BODY_NAME":
                    return MonsterTier.Boss;
                case "TITAN_BODY_NAME":
                    return MonsterTier.Boss;
                case "MAGMAWORM_BODY_NAME":
                    return MonsterTier.Boss;
                case "CLAYBOSS_BODY_NAME":
                    return MonsterTier.Boss;
                case "GRAVEKEEPER_BODY_NAME":
                    return MonsterTier.Boss;
                case "IMPBOSS_BODY_NAME":
                    return MonsterTier.Boss;
                case "ROBOBALLBOSS_BODY_NAME":
                    return MonsterTier.Boss;
                case "VAGRANT_BODY_NAME":
                    return MonsterTier.Boss;


                case "ELECTRICWORM_BODY_NAME":
                    return MonsterTier.SuperBoss;
                case "SHOPKEEPER_BODY_NAME":
                    return MonsterTier.SuperBoss;


                case "ANCIENTWISP_BODY_NAME":
                    return MonsterTier.HyperBoss;
                case "TITANGOLD_BODY_NAME":
                    return MonsterTier.HyperBoss;
                case "SUPERROBOBALLBOSS_BODY_NAME":
                    return MonsterTier.HyperBoss;


                case "DRONE_BACKUP_BODY_NAME":
                    return MonsterTier.Basic;
                case "DRONEBACKUP_BODY_NAME":
                    return MonsterTier.Basic;
                case "DRONE_GUNNER_BODY_NAME":
                    return MonsterTier.Basic;
                case "DRONE_HEALING_BODY_NAME":
                    return MonsterTier.Basic;
                case "ENGITURET_BODY_NAME":
                    return MonsterTier.Basic;
                case "ENGI_BODY_NAME":
                    return MonsterTier.Mid;
                case "BANDIT_BODY_NAME":
                    return MonsterTier.Mid;
                case "BOMBER_BODY_NAME":
                    return MonsterTier.Mid;
                case "COMMANDO_BODY_NAME":
                    return MonsterTier.Mid;
                case "ENFORCER_BODY_NAME":
                    return MonsterTier.Mid;
                case "HAND_BODY_NAME":
                    return MonsterTier.Mid;
                case "HUNTRESS_BODY_NAME":
                    return MonsterTier.Mid;
                case "LOADER_BODY_NAME":
                    return MonsterTier.Mid;
                case "MAGE_BODY_NAME":
                    return MonsterTier.Mid;
                case "MERC_BODY_NAME":
                    return MonsterTier.Mid;
                case "SNIPER_BODY_NAME":
                    return MonsterTier.Mid;
                case "TOOLBOT_BODY_NAME":
                    return MonsterTier.Mid;
                case "TREEBOT_BODY_NAME":
                    return MonsterTier.Mid;
                case "TURRET1_BODY_NAME":
                    return MonsterTier.Basic;
                case "SQUIDTURRET_BODY_NAME":
                    return MonsterTier.Mid;
                case "EQUIPMENTDRONE_BODY_NAME":
                    return MonsterTier.Mid;
                case "FLAMEDRONE_BODY_NAME":
                    return MonsterTier.Mid;
                case "POTMOBILE_BODY_NAME":
                    return MonsterTier.Mid;
                case "DRONE_MEGA_BODY_NAME":
                    return MonsterTier.Mid;
                case "DRONE_MISSILE_BODY_NAME":
                    return MonsterTier.Mid;

                case "POT2_BODY_NAME":
                    return MonsterTier.Other;
                case "":
                    return MonsterTier.Other;
            }
        }
    }
}
