﻿namespace Rein.Sniper.Modules
{
    using System;
    using System.Collections.Generic;

    using EntityStates;

    using ReinCore;

    using Rewired;

    using RoR2;
    using RoR2.Skills;

    using Rein.Sniper.Components;
    using Rein.Sniper.Data;
    using Rein.Sniper.Expansions;
    using Rein.Sniper.Properties;
    using Rein.Sniper.SkillDefs;
    using Rein.Sniper.SkillDefTypes.Bases;
    using Rein.Sniper.States.Bases;
    using Rein.Sniper.States.Primary.Reload;
    using Rein.Sniper.States.Secondary;
    using Rein.Sniper.States.Special;
    using Rein.Sniper.States.Utility;

    using UnityEngine;
    using UnityEngine.Networking;
    using Rein.Sniper.DotTypes;

    internal static class SkillsModule
    {
        private static (SkillDef, String) wip
        {
            get
            {
                var def = ScriptableObject.CreateInstance<SkillDef>();
                return (def, Unlockables.WIPUnlockable.unlockable_Identifier);
            }
        }

        private abstract class StandardContextBase : IAmmoStateContext
        {
            public abstract GameObject tracerEffectPrefab { get; }
            protected virtual Single durationMultiplier { get => 1f; }
            protected virtual SoundModule.FireType fireSoundType { get => SoundModule.FireType.Normal; }
            protected virtual GameObject? hitEffectPrefab { get => null; }
            protected virtual Boolean hitEffectNormal { get => true; }
            protected virtual UInt32 bulletCount { get => 1; }
            protected virtual Single maxDistance { get => 1000f; }
            protected virtual Single minSpread { get => 0f; }
            protected virtual Single maxSpread { get => 0f; }
            protected virtual Single procCoefficient { get => 1f; }
            protected virtual Single bulletRadius { get => 0.2f; }
            protected virtual LayerMask hitMask { get => LayerIndex.entityPrecise.mask | LayerIndex.world.mask; }
            protected virtual LayerMask stopperMask { get => LayerIndex.entityPrecise.mask | LayerIndex.world.mask; }
            protected virtual Boolean smartCollision { get => true; }
            protected virtual Boolean headshotCapable { get => false; }
            protected virtual Single spreadPitchScale { get => 1f; }
            protected virtual Single spreadYawScale { get => 1f; }
            protected virtual DamageColorIndex damageColor { get => DamageColorIndex.Default; }
            protected virtual DamageType damageType { get => DamageType.Generic; }
            protected virtual BulletAttack.FalloffModel bulletFalloff { get => BulletAttack.FalloffModel.None; }
            protected virtual Single baseDamageMultiplier { get => 1f; }
            protected virtual Single baseForceMultiplier { get => 1f; }
            protected virtual Single recoilMultiplier { get => 1f; }
            protected virtual Single animPlayRate { get => 1f; }
            protected virtual Single aimModeLinger { get => 5f; }
            protected virtual Boolean chargeIncreasesRecoil { get => true; }


            protected Int32 bulletsFired = 0;
            protected Single duration;
            public virtual void OnEnter<T>(SnipeState<T> state) where T : struct, ISniperPrimaryDataProvider
            {
                this.duration = this.durationMultiplier * state.baseDuration / state.characterBody.attackSpeed;
                this.FireBullet(state);           
            }
            protected void FireBullet<T>(SnipeState<T> state) where T : struct, ISniperPrimaryDataProvider
            {
                this.bulletsFired++;
                var aim = state.GetAimRay();
                state.StartAimMode(aim, this.duration + this.aimModeLinger, true);
                if(state.isAuthority)
                {
                    var bullet = this.CreateBullet();
                    this.ModifyBullet(bullet, state, aim);
                    bullet.Fire();
                    var recoil = state.recoilStrength * this.recoilMultiplier * (1f + (this.chargeIncreasesRecoil ? 0.5f * state.chargeBoost : 0f));
                    state.AddRecoil(-1f * recoil, -3f * recoil, -0.2f * recoil, 0.2f * recoil);
                }
                state.PlayAnimation("Gesture, Additive", "Shoot", "rateShoot", this.duration * animPlayRate);
                SoundModule.PlayFire(state.gameObject, state.soundFrac, this.fireSoundType);
            }
            public virtual ExpandableBulletAttack CreateBullet() => new ExpandableBulletAttack<NoData>();
            public virtual void ModifyBullet<T>(ExpandableBulletAttack bullet, SnipeState<T> state, Ray aimRay)
                 where T : struct, ISniperPrimaryDataProvider
            {
                var body = state.characterBody;

                bullet.aimVector = aimRay.direction;
                bullet.origin = aimRay.origin;
                bullet.attackerBody = body;
                bullet.isCrit = body.RollCrit();
                bullet.procChainMask = default;
                bullet.owner = body.gameObject;
                bullet.chargeBoost = state.chargeBoost;
                bullet.reloadBoost = state.reloadBoost;
                bullet.muzzleName = state.muzzleName;
                bullet.queryTriggerInteraction = QueryTriggerInteraction.UseGlobal;
                bullet.weapon = null;

                bullet.hitEffectPrefab = this.hitEffectPrefab;
                bullet.tracerEffectPrefab = this.tracerEffectPrefab;
                bullet.bulletCount = this.bulletCount;
                bullet.HitEffectNormal = this.hitEffectNormal;
                bullet.maxDistance = this.maxDistance;
                bullet.maxSpread = this.maxSpread;
                bullet.minSpread = this.minSpread;
                bullet.procCoefficient = this.procCoefficient;
                bullet.radius = this.bulletRadius;
                bullet.hitMask = this.hitMask;
                bullet.stopperMask = this.stopperMask;
                bullet.sniper = this.headshotCapable;
                bullet.smartCollision = this.smartCollision;
                bullet.spreadPitchScale = this.spreadPitchScale;
                bullet.spreadYawScale = this.spreadYawScale;
                bullet.damageColorIndex = this.damageColor;
                bullet.damageType = this.damageType;
                bullet.falloffModel = this.bulletFalloff;

                bullet.damage = body.damage * state.damageMultiplier * this.baseDamageMultiplier;
                bullet.force = state.forceMultiplier * this.baseForceMultiplier;
            }
            public virtual void FixedUpdate<T>(SnipeState<T> state) where T : struct, ISniperPrimaryDataProvider
            {
                if(state.isAuthority && state.fixedAge >= this.duration)
                {
                    state.outer.SetNextStateToMain();
                }
            }

            public virtual void OnExit<T>(SnipeState<T> state) where T : struct, ISniperPrimaryDataProvider
            {
            }

            public virtual void OnDeserialize<T>(SnipeState<T> state, NetworkReader reader) where T : struct, ISniperPrimaryDataProvider { }
            public virtual void OnSerialize<T>(SnipeState<T> state, NetworkWriter writer) where T : struct, ISniperPrimaryDataProvider { }
        }

        private abstract class OnHitContextBase<TData> : StandardContextBase
            where TData : struct
        {
            public virtual TData InitData() => default;
            protected virtual OnBulletDelegate<TData> onHit { get => null; }
            protected virtual OnBulletDelegate<TData> onStop { get => null; }
            public sealed override ExpandableBulletAttack CreateBullet() => new ExpandableBulletAttack<TData>();
            public sealed override void ModifyBullet<T>(ExpandableBulletAttack bullet, SnipeState<T> state, Ray aim)
            {
                base.ModifyBullet(bullet, state, aim);
                var bul = bullet as ExpandableBulletAttack<TData>;
                bul.onHit = this.onHit;
                bul.onStop = this.onStop;
                bul.data = this.InitData();
                this.InitBullet(bul, state);
            }
            public virtual void InitBullet<T>(ExpandableBulletAttack<TData> bullet, SnipeState<T> state)
                where T : struct, ISniperPrimaryDataProvider
            {

            }
        }

        private class FMJContext : OnHitContextBase<RicochetData>
        {
            private static readonly GameObject _tracer;
            static FMJContext()
            {
                _tracer = VFXModule.GetStandardAmmoTracer();
                RicochetController.ricochetEffectPrefab = VFXModule.GetRicochetEffectPrefab();
            }
            public override GameObject tracerEffectPrefab => _tracer;
            protected override LayerMask stopperMask => base.stopperMask & ~LayerIndex.entityPrecise.mask;
            protected override OnBulletDelegate<RicochetData> onStop => (bullet, hit) =>
            {
                if(hit.collider)
                {
                    Vector3 v1 = hit.direction;
                    Vector3 v2 = hit.surfaceNormal;
                    Single dot = (v1.x * v2.x) + (v1.y * v2.y) + (v1.z * v2.z);
                    Single chance = Mathf.Clamp( Mathf.Acos( dot ) / 0.02f / Mathf.PI, 0f, 100f);
                    if(Util.CheckRoll(100f - chance, bullet.attackerBody.master))
                    {
                        Vector3 newDir = (-2f * dot * v2) + v1;
                        var newBul = bullet.Clone();// as ExpandableBulletAttack<RicochetData>;
                        newBul.origin = hit.point;
                        newBul.aimVector = newDir;
                        var wepObj = newBul.weapon = new GameObject("temp", typeof(NetworkIdentity));
                        
                        if(newBul.data.counter++ == 0) newBul.radius *= 10f;

                        if(hit.damageModifier == HurtBox.DamageModifier.SniperTarget)
                        {
                            newBul.damage *= 1.5f;
                        }

                        RicochetController.QueueRicochet(newBul, (UInt32)(hit.distance / 6f) + 1u);
                    }
                }
            };

            public override void InitBullet<T>(ExpandableBulletAttack<RicochetData> bullet, SnipeState<T> state)
            {
                base.InitBullet(bullet, state);
                bullet.damage *= state.reloadBoost;
                bullet.damage *= (1f + state.chargeBoost);
                if(bullet.isCrit)
                {
                    bullet.damage *= 1f + (state.chargeBoost / 20f);
                }
            }

        }

        private class ExplosiveContext : OnHitContextBase<NoData>
        {
            private static readonly GameObject _tracer = VFXModule.GetExplosiveAmmoTracer();
            public override GameObject tracerEffectPrefab => _tracer;
            protected override Single baseDamageMultiplier => 0.2f;
            protected override Single procCoefficient => 1f;
            protected override Single bulletRadius => 0.5f;
            protected override OnBulletDelegate<NoData> onHit => (bullet, hit) =>
            {
                Single rad = 8f * (1f + bullet.chargeBoost);
                EffectManager.SpawnEffect(VFXModule.GetExplosiveAmmoExplosionIndex(), new EffectData
                {
                    origin = hit.point,
                    scale = rad,
                    rotation = Util.QuaternionSafeLookRotation(hit.direction)
                }, true);
                var blast = new BlastAttack
                {
                    attacker = bullet.owner,
                    attackerFiltering = AttackerFiltering.Default,
                    baseDamage = bullet.damage * 3.5f,
                    baseForce = 1f,
                    bonusForce = Vector3.zero,
                    crit = bullet.isCrit,
                    damageColorIndex = DamageColorIndex.Default,
                    damageType = bullet.damageType,
                    falloffModel = BlastAttack.FalloffModel.None,
                    impactEffect = EffectIndex.Invalid, // FUTURE: Explosive Ammo Impact Effect
                    inflictor = null,
                    losType = BlastAttack.LoSType.None,
                    position = hit.point,
                    procChainMask = bullet.procChainMask,
                    procCoefficient = bullet.procCoefficient * 0.5f,
                    radius = rad,
                    teamIndex = TeamComponent.GetObjectTeam(bullet.owner),
                };
                _ = blast.Fire();
            };

            public override void InitBullet<T>(ExpandableBulletAttack<NoData> bullet, SnipeState<T> state)
            {
                base.InitBullet(bullet, state);
                bullet.damage *= state.reloadBoost;
                bullet.damage *= (1f + state.chargeBoost * 0.8f);
            }
        }

        private class BurstContext : StandardContextBase
        {
            private static readonly GameObject _tracer = VFXModule.GetBurstAmmoTracer();

            public override GameObject tracerEffectPrefab => _tracer;
            protected override Single durationMultiplier => base.durationMultiplier * 4f;
            protected override Single baseDamageMultiplier => 0.4f;
            protected override Single procCoefficient => 1f;
            protected override Single bulletRadius => 0.5f;
            protected override Single recoilMultiplier => base.recoilMultiplier * 0.5f * this._recoilMultiplier;
            protected override Single animPlayRate => this._animPlayRate;
            protected override SoundModule.FireType fireSoundType => SoundModule.FireType.Burst;
            protected override Boolean chargeIncreasesRecoil => false;



            private Int32 shotsToFire;
            private Single fireInterval;
            private Single _recoilMultiplier = 0.75f;
            private Single _animPlayRate;
            private Single timer = 0f;

            const Single firingFrac = 0.75f;
            

            public override void OnEnter<T>(SnipeState<T> state)
            {
                this.shotsToFire = (Int32)(3 * (1 + state.chargeBoost));
                this._animPlayRate = this.shotsToFire;
                //this._recoilMultiplier = base.recoilMultiplier * 0.;

                base.OnEnter(state);

                this.fireInterval = (this.duration * firingFrac) / (this.shotsToFire + 1);
            }

            public override void FixedUpdate<T>(SnipeState<T> state)
            {
                while(this.bulletsFired < this.shotsToFire && (state.fixedAge / this.fireInterval) >= this.bulletsFired)
                {
                    this._recoilMultiplier *= this._recoilMultiplier * this._recoilMultiplier;
                    this.FireBullet(state);
                }

                base.FixedUpdate(state);
            }

            public override void ModifyBullet<T>(ExpandableBulletAttack bullet, SnipeState<T> state, Ray aimRay)
            {
                base.ModifyBullet(bullet, state, aimRay);
                bullet.damage *= state.reloadBoost;
            }
        }


        internal const Single plasmaTotalMult = 2f;
        internal const Single plasmaProcPerTick = 0.5f;
        internal const Single plasmaTotalDuration = 10f;
        internal const Single plasmaTickFreq = 2.5f;


        internal const Single plasmaTotalTicks = plasmaTotalDuration * plasmaTickFreq + 1f;
        internal const Single plasmaDamagePerTick = plasmaTotalMult / plasmaTotalTicks;
        private class PlasmaContext : OnHitContextBase<NoData>
        {
            private static readonly GameObject _tracer = VFXModule.GetPlasmaAmmoTracer();
            private const Single baseDotDuration = 10f;

            public override GameObject tracerEffectPrefab => _tracer;
            protected override Single baseDamageMultiplier => plasmaDamagePerTick;
            protected override Single procCoefficient => plasmaProcPerTick;
            protected override Single bulletRadius => 0.5f;
            protected override DamageColorIndex damageColor => CatalogModule.plasmaDamageColor;
            protected override SoundModule.FireType fireSoundType => SoundModule.FireType.Plasma;
            protected override OnBulletDelegate<NoData> onHit => (bullet, hit) =>
            {
                var box = hit.hitHurtBox;
                if(!box) return;
                var target = box.healthComponent?.body;
                if(!target) return;
                if(!FriendlyFireManager.ShouldDirectHitProceed(box.healthComponent, bullet.team)) return;

                PlasmaDot.Apply(target, bullet.attackerBody, bullet.damage / bullet.attackerBody.damage, plasmaTotalDuration * Mathf.Sqrt(bullet.chargeBoost + 1f), bullet.procCoefficient, bullet.isCrit, box.transform.InverseTransformPoint(hit.point), bullet.aimVector * -1f, box);
            };

            public override void InitBullet<T>(ExpandableBulletAttack<NoData> bullet, SnipeState<T> state)
            {
                base.InitBullet(bullet, state);
                bullet.damage *= state.reloadBoost;
                bullet.damage *= Mathf.Sqrt(1f + state.chargeBoost);
            }
        }




        internal static void CreateAmmoSkills()
        {
            var skills = new List<(SkillDef,String)>();


            var standardAmmo = SniperAmmoSkillDef.Create<FMJContext>();
            standardAmmo.icon = Properties.Icons.StandardAmmoIcon;
            standardAmmo.skillName = "StandardAmmo";
            standardAmmo.skillNameToken = Tokens.SNIPER_AMMO_STANDARD_NAME;
            standardAmmo.skillDescriptionToken = Tokens.SNIPER_AMMO_STANDARD_DESC;
            standardAmmo.fireSoundType = SoundModule.FireType.Normal;
            standardAmmo.keywordTokens = new[]
            {    
                Tokens.SNIPER_KEYWORD_PIERCING,
                Tokens.SNIPER_KEYWORD_RICOCHET,
                Tokens.SNIPER_KEYWORD_PRIMARYDMG,
                Tokens.SNIPER_KEYWORD_BOOST,
            };
            skills.Add((standardAmmo, ""));



            var explosive = SniperAmmoSkillDef.Create<ExplosiveContext>();
            
            explosive.icon = Properties.Icons.ExplosiveAmmoIcon;
            explosive.skillName = "ExplosiveAmmo";
            explosive.skillNameToken = Tokens.SNIPER_AMMO_EXPLOSIVE_NAME;
            explosive.skillDescriptionToken = Tokens.SNIPER_AMMO_EXPLOSIVE_DESC;
            explosive.fireSoundType = SoundModule.FireType.Normal;
            explosive.keywordTokens = new[]
            {
                Tokens.SNIPER_KEYWORD_EXPLOSIVE,
                Tokens.SNIPER_KEYWORD_PRIMARYDMG,
                Tokens.SNIPER_KEYWORD_BOOST,
            };
            skills.Add((explosive, ""));
 


            var burstAmmo = SniperAmmoSkillDef.Create<BurstContext>();
            burstAmmo.icon = Properties.Icons.BurstAmmoIcon;
            burstAmmo.skillName = "Burst Ammo";
            burstAmmo.skillNameToken = Tokens.SNIPER_AMMO_BURST_NAME;
            burstAmmo.skillDescriptionToken = Tokens.SNIPER_AMMO_BURST_DESC;
            burstAmmo.fireSoundType = SoundModule.FireType.Burst;
            burstAmmo.keywordTokens = new[]
            {
                Tokens.SNIPER_KEYWORD_PRIMARYDMG,
                Tokens.SNIPER_KEYWORD_BOOST,
            };
            skills.Add((burstAmmo, ""));
            //skills.Add(wip);


            var plasmaAmmo = SniperAmmoSkillDef.Create<PlasmaContext>();
            plasmaAmmo.icon = Properties.Icons.PlasmaAmmoIcon;
            plasmaAmmo.skillName = "PlasmaAmmo";
            plasmaAmmo.skillNameToken = Tokens.SNIPER_AMMO_PLASMA_NAME;
            plasmaAmmo.skillDescriptionToken = Tokens.SNIPER_AMMO_PLASMA_DESC;
            plasmaAmmo.fireSoundType = SoundModule.FireType.Plasma;
            skills.Add((plasmaAmmo, ""));

            var shockAmmo = SniperAmmoSkillDef.Create<BurstContext>();
            shockAmmo.icon = Properties.Icons.ShockAmmoIcon;
            shockAmmo.skillName = "ShockAmmo";
            shockAmmo.skillNameToken = Tokens.SNIPER_AMMO_SHOCK_NAME;
            shockAmmo.skillDescriptionToken = Tokens.SNIPER_AMMO_SHOCK_DESC;
            shockAmmo.fireSoundType = SoundModule.FireType.Burst;
            skills.Add((shockAmmo, Unlockables.WIPUnlockable.unlockable_Identifier));


            //skills.Add(wip);
            //skills.Add(wip);
            //skills.Add(wip);

            SkillFamiliesModule.ammoSkills = skills;
        }
        /* 




            //#endregion


            #region Scatter
            //GameObject scatterTracer = VFXModule.GetScatterAmmoTracer();
            //BulletAttack.FalloffModel scatterFalloff = BulletFalloffCore.AddFalloffModel( (dist) => Mathf.Pow(Mathf.InverseLerp( 200f, 10f, dist ),2f) );
            //var scatterCreate = new BulletCreationDelegate( (body, reload, aim, muzzle) =>
            //{
            //    var bullet = new ExpandableBulletAttack
            //    {
            //        aimVector = aim.direction,
            //        attackerBody = body,
            //        bulletCount = (UInt32)( 3 + ( 2 * (Int32)reload ) ),
            //        chargeLevel = 0f,
            //        damage = body.damage * 0.25f,
            //        damageColorIndex = DamageColorIndex.Default,
            //        damageType = DamageType.Generic,
            //        falloffModel = scatterFalloff,
            //        force = 25f,
            //        HitEffectNormal = true,
            //        hitEffectPrefab = null, // TODO: Explosive Ammo Hit Effect
            //        hitMask = LayerIndex.entityPrecise.mask | LayerIndex.world.mask,
            //        isCrit = body.RollCrit(),
            //        maxDistance = 200f,
            //        maxSpread = 2.6f,
            //        minSpread = 1f,
            //        muzzleName = muzzle,
            //        onHit = null,
            //        onStop = null,
            //        origin = aim.origin,
            //        owner = body.gameObject,
            //        procChainMask = default,
            //        procCoefficient = 0.6f,
            //        queryTriggerInteraction = QueryTriggerInteraction.UseGlobal,
            //        radius = 0.15f,
            //        smartCollision = true,
            //        sniper = false,
            //        spreadPitchScale = 1f,
            //        spreadYawScale = 1f,
            //        stopperMask = LayerIndex.world.mask | LayerIndex.entityPrecise.mask,
            //        tracerEffectPrefab = scatterTracer,
            //        weapon = null,
            //    };
            //    return bullet;
            //});
            //var scatterCharge = new ChargeBulletModifierDelegate( (bullet) =>
            //{
            //    bullet.bulletCount += (UInt32)(4f * bullet.chargeLevel);
            //});
            //var scatter = SniperAmmoSkillDef.Create( scatterCreate );
            //scatter.icon = UIModule.GetScatterAmmoIcon();
            //scatter.skillName = "Scatter Ammo";
            //scatter.skillNameToken = Tokens.SNIPER_AMMO_SCATTER_NAME;
            //scatter.skillDescriptionToken = Tokens.SNIPER_AMMO_SCATTER_DESC;
            //scatter.fireSoundType = SoundModule.FireType.Scatter;
            //skills.Add( scatter );



            


            #region Shock


            #endregion

         * 
         * 
         */

        //internal static void CreatePassiveSkills()
        //{
        //    var skills = new List<(SkillDef,String)>();

        //    var critPassive = SniperPassiveSkillDef.Create( default, false, 1.2f, 1f );
        //    critPassive.icon = UIModule.GetCritPassiveIcon();
        //    critPassive.skillName = "Precise Aim";
        //    critPassive.skillNameToken = Tokens.SNIPER_PASSIVE_CRITICAL_NAME;
        //    critPassive.skillDescriptionToken = Tokens.SNIPER_PASSIVE_CRITICAL_DESC;
        //    skills.Add( (critPassive,"") );

        //    //var headshot = SniperPassiveSkillDef.Create( default, true, 1.0f );
        //    //headshot.icon = UIModule.GetHeadshotPassiveIcon();
        //    //headshot.skillName = "Headshot";
        //    //headshot.skillNameToken = Tokens.SNIPER_PASSIVE_HEADSHOT_NAME;
        //    //headshot.skillDescriptionToken = Tokens.SNIPER_PASSIVE_HEADSHOT_DESC;
        //    //skills.Add( headshot );

        //    skills.Add( wip );

        //    SkillFamiliesModule.passiveSkills = skills;
        //}

        private struct DefaultSnipe : ISniperPrimaryDataProvider
        {
            public Single baseDuration => 0.15f;

            public Single recoilStrength => 6f;

            public Single damageMultiplier => 6f;

            public Single forceMultiplier => 200f;

            public String muzzleName => "MuzzleRailgun";

            public Single upBoostForce => 500f;
        }
        private struct MagSnipe : ISniperPrimaryDataProvider
        {
            public Single baseDuration => 0.15f;

            public Single recoilStrength => 4f;

            public Single damageMultiplier => 2.5f;

            public Single forceMultiplier => 100f;

            public String muzzleName => "MuzzleRailgun";

            public Single upBoostForce => 200f;
        }
        internal static void CreatePrimarySkills()
        {
            var skills = new List<(SkillDef,String)>();


            var snipe = SniperReloadableFireSkillDef.Create<DefaultSnipe,DefaultReload>("Weapon", "Weapon");
            snipe.actualMaxStock = 1;
            snipe.icon = Properties.Icons.SnipeIcon;
            snipe.interruptPriority = InterruptPriority.Skill;
            snipe.isBullets = false;
            snipe.rechargeStock = 0;
            snipe.reloadIcon = Properties.Icons.ReloadIcon;
            snipe.reloadInterruptPriority = InterruptPriority.Skill;
            snipe.reloadParams = new ReloadParams
            {
                attackSpeedCap = 1.25f,
                attackSpeedDecayCoef = 10f,
                baseDuration = 1.5f,
                badMult = 1.0f,
                goodMult = 1.2f,
                perfectMult = 1.5f,
                reloadDelay = 0.4f,
                reloadEndDelay = 0.6f,
                perfectStart = 0.25f,
                perfectEnd = 0.4f,
                goodStart = 0.4f,
                goodEnd = 0.6f,
            };
            snipe.requiredStock = 1;
            snipe.shootDelay = 0.15f;
            snipe.skillDescriptionToken = Tokens.SNIPER_PRIMARY_SNIPE_DESC;
            snipe.skillNameToken = Tokens.SNIPER_PRIMARY_SNIPE_NAME;
            snipe.stockToConsume = 1;
            snipe.stockToReload = 1;
            snipe.skillName = "Snipe";
            snipe.noSprint = true;
            snipe.noSprintReload = false;
            skills.Add((snipe, ""));

            var mag = SniperReloadableFireSkillDef.Create<MagSnipe,MagReload>("Weapon", "Weapon");
            mag.actualMaxStock = 4;
            mag.icon = Properties.Icons.SnipeMag;
            mag.interruptPriority = InterruptPriority.Skill;
            mag.isBullets = false;
            mag.rechargeStock = 0;
            mag.reloadIcon = Properties.Icons.SnipeMagReload;
            mag.reloadInterruptPriority = InterruptPriority.Skill;
            mag.reloadParams = new ReloadParams
            {
                attackSpeedCap = 1.5f,
                attackSpeedDecayCoef = 10f,
                badMult = 1.0f,
                baseDuration = 1.5f,
                goodMult = 1.2f,
                perfectMult = 1.5f,
                reloadDelay = 1f,
                reloadEndDelay = 1f,
                perfectStart = 0.25f,
                perfectEnd = 0.4f,
                goodStart = 0.4f,
                goodEnd = 0.6f,
            };
            mag.requiredStock = 1;
            mag.shootDelay = 0.4f;
            mag.skillDescriptionToken = Tokens.SNIPER_PRIMARY_MAG_DESC;
            mag.skillNameToken = Tokens.SNIPER_PRIMARY_MAG_NAME;
            mag.stockToConsume = 1;
            mag.stockToReload = 4;
            mag.skillName = "MagSnipe";
            mag.noSprint = true;
            mag.noSprintReload = false;
            skills.Add((mag, ""));
            //skills.Add(wip);

            //var slide = SniperReloadableFireSkillDef.Create<SlideSnipe,SlideReload>("Weapon", "Body");
            //slide.actualMaxStock = 1;
            //slide.icon = null; // TODO: Slide snipe icon
            //slide.interruptPriority = InterruptPriority.Skill;
            //slide.isBullets = false;
            //slide.rechargeStock = 0;
            //slide.reloadIcon = null; // TODO: Slide Snipe Reload icon
            //slide.reloadInterruptPriority = InterruptPriority.Skill;
            //slide.reloadParams = new ReloadParams
            //{
            //    attackSpeedCap = 1.25f,
            //    attackSpeedDecayCoef = 10f,
            //    badMult = 0.4f,
            //    baseDuration = 1.5f,
            //    goodMult = 1f,
            //    perfectMult = 2f,
            //    reloadDelay = 0.5f,
            //    reloadEndDelay = 0.75f,
            //    perfectStart = 0.25f,
            //    perfectEnd = 0.4f,
            //    goodStart = 0.4f,
            //    goodEnd = 0.6f,
            //};
            //slide.requiredStock = 1;
            //slide.shootDelay = 0.15f;
            //slide.skillDescriptionToken = Tokens.SNIPER_PRIMARY_DASH_DESC;
            //slide.skillNameToken = Tokens.SNIPER_PRIMARY_DASH_NAME;
            //slide.stockToConsume = 1;
            //slide.stockToReload = 1;
            //slide.skillName = "Slide";
            //skills.Add( slide );

            skills.Add(wip);

            SkillFamiliesModule.primarySkills = skills;
        }

        internal static void CreateSecondarySkills()
        {
            var skills = new List<(SkillDef,String)>();

            var charge = SniperScopeSkillDef.Create<DefaultScope>( new ZoomParams(shoulderStart: 1f, shoulderEnd: 5f,
                                                                                                             scopeStart: 3f, scopeEnd: 8f,
                                                                                                             shoulderFrac: 0.25f, defaultZoom: 0f,
                                                                                                             inputScale: 0.03f, baseFoV: 60f) );
            charge.baseMaxStock = 1;
            charge.baseRechargeInterval = 10f;
            charge.icon = Properties.Icons.SteadyAimIcon;
            charge.isBullets = false;
            charge.rechargeStock = 1;
            charge.requiredStock = 0;
            charge.skillDescriptionToken = Tokens.SNIPER_SECONDARY_STEADY_DESC;
            charge.skillName = "SteadyAim";
            charge.skillNameToken = Tokens.SNIPER_SECONDARY_STEADY_NAME;
            charge.stockToConsumeOnFire = 1;
            charge.stockRequiredToKeepZoom = 0;
            charge.stockRequiredToModifyFire = 1;
            charge.beginSkillCooldownOnSkillEnd = false;
            charge.initialCarryoverLoss = 0.0f;
            charge.decayType = SniperScopeSkillDef.DecayType.Exponential;
            charge.decayValue = 0.1f;
            charge.chargeCanCarryOver = false;
            charge.keywordTokens = new[]
            {
                Tokens.SNIPER_KEYWORD_SCOPED,
                Tokens.SNIPER_KEYWORD_BOOST,
            };
            charge.consumeChargeOnFire = true;
            skills.Add((charge, ""));

            var quick = SniperScopeSkillDef.Create<QuickScope>( new ZoomParams(shoulderStart: 1f, shoulderEnd: 5f,
                                                                                                             scopeStart: 3f, scopeEnd: 8f,
                                                                                                             shoulderFrac: 0.25f, defaultZoom: 0f,
                                                                                                             inputScale: 0.03f, baseFoV: 60f) );
            quick.baseMaxStock = 4;
            quick.baseRechargeInterval = 6f;
            quick.icon = Properties.Icons.QuickscopeIcon;
            quick.isBullets = false;
            quick.rechargeStock = 1;
            quick.requiredStock = 0;
            quick.skillDescriptionToken = Tokens.SNIPER_SECONDARY_QUICK_DESC;
            quick.skillName = "Quickscope";
            quick.skillNameToken = Tokens.SNIPER_SECONDARY_QUICK_NAME;
            quick.stockToConsumeOnFire = 1;
            quick.stockRequiredToKeepZoom = 0;
            quick.stockRequiredToModifyFire = 1;
            quick.beginSkillCooldownOnSkillEnd = false;
            quick.keywordTokens = new[]
            {
                Tokens.SNIPER_KEYWORD_SCOPED,
                Tokens.SNIPER_KEYWORD_BOOST,
            };
            quick.consumeChargeOnFire = true;
            skills.Add((quick, ""));


            skills.Add(wip);



            SkillFamiliesModule.secondarySkills = skills;
        }

        internal static void CreateUtilitySkills()
        {
            var skills = new List<(SkillDef,String)>();

            var backflip = SniperSkillDef.Create<Backflip>("Body");
            backflip.baseMaxStock = 1;
            backflip.baseRechargeInterval = 5f;
            backflip.beginSkillCooldownOnSkillEnd = true;
            backflip.canceledFromSprinting = false;
            backflip.fullRestockOnAssign = true;
            backflip.icon = Properties.Icons.BackflipIcon;
            backflip.interruptPriority = InterruptPriority.PrioritySkill;
            backflip.isBullets = false;
            backflip.isCombatSkill = false;
            backflip.mustKeyPress = true;
            backflip.noSprint = true;
            backflip.rechargeStock = 1;
            backflip.requiredStock = 1;
            backflip.shootDelay = 0.1f;
            backflip.skillDescriptionToken = Tokens.SNIPER_UTILITY_BACKFLIP_DESC;
            backflip.skillName = "Military Training";
            backflip.skillNameToken = Tokens.SNIPER_UTILITY_BACKFLIP_NAME;
            backflip.stockToConsume = 1;
            backflip.keywordTokens = new[]
            {
                Tokens.SNIPER_KEYWORD_RELOADS,
                "KEYWORD_STUNNING",
            };
            skills.Add((backflip, ""));

            SkillFamiliesModule.utilitySkills = skills;
        }

        internal static void CreateSpecialSkills()
        {
            var skills = new List<(SkillDef,String)>();

            var decoy = DecoySkillDef.Create<DecoyActivation,DecoyReactivation>( "Weapon", "Weapon" );
            decoy.baseMaxStock = 1;
            decoy.baseRechargeInterval = 12f;
            decoy.beginSkillCooldownOnSkillEnd = true;
            decoy.fullRestockOnAssign = true;
            decoy.icon = Properties.Icons.DecoyIcon;
            decoy.interruptPriority = InterruptPriority.PrioritySkill;
            decoy.isCombatSkill = false;
            decoy.maxReactivationTimer = -1f;
            decoy.minReactivationTimer = 0.75f;
            decoy.noSprint = false;
            decoy.reactivationIcon = Properties.Icons.DecoyReactivateIcon;
            decoy.reactivationInterruptPriority = InterruptPriority.PrioritySkill;
            decoy.reactivationRequiredStock = 1;
            decoy.reactivationStockToConsume = 1;
            decoy.consumeOnInvalidate = true;
            decoy.cdRefundOnInvalidate = 0.0f;
            decoy.rechargeStock = 1;
            decoy.requiredStock = 1;
            decoy.skillDescriptionToken = Tokens.SNIPER_SPECIAL_DECOY_DESC;
            decoy.skillName = "Decoy";
            decoy.skillNameToken = Tokens.SNIPER_SPECIAL_DECOY_NAME;
            decoy.startCooldownAfterReactivation = true;
            decoy.stockToConsume = 0;
            decoy.keywordTokens = new[]
            {
                Tokens.SNIPER_KEYWORD_REACTIVATION,
                Tokens.SNIPER_KEYWORD_PHASED,
                "KEYWORD_WEAK",
                "KEYWORD_STUNNING",
            };
            skills.Add((decoy, ""));

            var knife = KnifeSkillDef.Create<KnifeActivation,KnifeReactivation>( "Weapon", "Body" );
            knife.baseMaxStock = 1;
            knife.baseRechargeInterval = 18f;
            knife.beginSkillCooldownOnSkillEnd = true;
            knife.fullRestockOnAssign = true;
            knife.icon = Properties.Icons.KnifeIcon;
            knife.interruptPriority = InterruptPriority.PrioritySkill;
            knife.isCombatSkill = true;
            knife.maxReactivationTimer = 8f;
            knife.minReactivationTimer = 0.2f;
            knife.noSprint = true;
            knife.reactivationIcon = Properties.Icons.KnifeReactivateIcon;
            knife.reactivationInterruptPriority = InterruptPriority.PrioritySkill;
            knife.reactivationRequiredStock = 1;
            knife.reactivationStockToConsume = 1;
            knife.rechargeStock = 1;
            knife.requiredStock = 1;
            knife.skillDescriptionToken = Tokens.SNIPER_SPECIAL_KNIFE_DESC;
            knife.skillName = "BlinkKnife";
            knife.skillNameToken = Tokens.SNIPER_SPECIAL_KNIFE_NAME;
            knife.startCooldownAfterReactivation = true;
            knife.stockToConsume = 0;      
            KnifeSkillData.interruptPriority = InterruptPriority.PrioritySkill;
            KnifeSkillData.targetMachineName = "Weapon";
            KnifeSkillData.slashState = SkillsCore.StateType<KnifePickupSlash>();
            skills.Add((knife, Unlockables.WIPUnlockable.unlockable_Identifier));
            //skills.Add(wip);

            SkillFamiliesModule.specialSkills = skills;
        }
    }
}
