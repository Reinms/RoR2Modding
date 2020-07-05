﻿namespace Sniper.SkillDefs
{
    using System;

    using EntityStates;

    using ReinCore;

    using RoR2;

    using Sniper.Components;
    using Sniper.Data;
    using Sniper.Enums;
    using Sniper.Modules;
    using Sniper.SkillDefTypes.Bases;
    using Sniper.States.Bases;
    using Sniper.UI.Components;

    using UnityEngine;

    internal class SniperReloadableFireSkillDef : SniperSkillDef
    {
        internal static SniperReloadableFireSkillDef Create<TFire, TReload>( String fireStateMachineName, String reloadStateMachineName )
            where TFire : SnipeBaseState
            where TReload : EntityState, ISniperReloadState
        {
            SniperReloadableFireSkillDef def = ScriptableObject.CreateInstance<SniperReloadableFireSkillDef>();
            def.activationState = SkillsCore.StateType<TFire>();
            def.activationStateMachineName = fireStateMachineName;
            def.baseRechargeInterval = 1f;
            def.beginSkillCooldownOnSkillEnd = true;
            def.canceledFromSprinting = false;
            def.isCombatSkill = true;
            def.fullRestockOnAssign = false;
            def.mustKeyPress = true;
            def.noSprint = true;
            def.reloadActivationState = SkillsCore.StateType<TReload>();
            def.reloadStateMachineName = reloadStateMachineName;
            def.baseMaxStock = 0;
            return def;
        }

        [SerializeField]
        internal Int32 actualMaxStock;
        [SerializeField]
        internal Int32 stockToReload;
        [SerializeField]
        internal SerializableEntityStateType reloadActivationState;
        [SerializeField]
        internal InterruptPriority reloadInterruptPriority;
        [SerializeField]
        internal String reloadStateMachineName;
        [SerializeField]
        internal Sprite reloadIcon;
        [SerializeField]
        internal ReloadParams reloadParams;

        public sealed override BaseSkillInstanceData OnAssigned( GenericSkill skillSlot )
        {
            EntityStateMachine reloadTargetStatemachine = null;

            EntityStateMachine[] stateMachines = skillSlot.GetComponents<EntityStateMachine>();
            for( Int32 i = 0; i < stateMachines.Length; ++i )
            {
                EntityStateMachine mach = stateMachines[i];
                if( mach.customName == this.reloadStateMachineName )
                {
                    reloadTargetStatemachine = mach;
                }
            }

#if ASSERT
            if( reloadTargetStatemachine == null )
            {
                Log.Error( "No state machine found for reload" );
            }
#endif

            skillSlot.stock = this.actualMaxStock;

            return new SniperPrimaryInstanceData( this, reloadTargetStatemachine, this.reloadParams, skillSlot );
        }

        public sealed override Sprite GetCurrentIcon( GenericSkill skillSlot )
        {
            var data = skillSlot.skillInstanceData as SniperPrimaryInstanceData;
            return data.isReloading ? this.reloadIcon : base.icon;
        }

        public sealed override Boolean CanExecute( GenericSkill skillSlot )
        {
            var data = skillSlot.skillInstanceData as SniperPrimaryInstanceData;
            EntityStateMachine mach = data.isReloading ? data.reloadStatemachine : skillSlot.stateMachine;
            return this.IsReady( skillSlot ) &&
                mach &&
                !mach.HasPendingState() &&
                mach.CanInterruptState( data.isReloading ? this.reloadInterruptPriority : base.interruptPriority );
        }

        public sealed override Boolean IsReady( GenericSkill skillSlot )
        {
            var data = skillSlot.skillInstanceData as SniperPrimaryInstanceData;
            return data.isReloading ? data.CanReload() : data.CanShoot();
        }

        protected sealed override EntityState InstantiateNextState( GenericSkill skillSlot )
        {
            var data = skillSlot.skillInstanceData as SniperPrimaryInstanceData;
            var state = EntityState.Instantiate(data.isReloading ? this.reloadActivationState : base.activationState);
            if( state is BaseSkillState skillState )
            {
                skillState.activatorSkillSlot = skillSlot;
            }
            if( state is SnipeBaseState snipeState )
            {
                snipeState.reloadParams = this.reloadParams;
            }
            if( state is ISniperReloadState reloadState )
            {
                reloadState.reloadTier = data.ReadReload();
            }
            return state;
        }

        public sealed override void OnExecute( GenericSkill skillSlot )
        {
            var data = skillSlot.skillInstanceData as SniperPrimaryInstanceData;
            EntityState state = this.InstantiateNextState(skillSlot);
            if( data.isReloading )
            {
                //var reloadState = state as ISniperReloadState;
            } else
            {
                var fireState = state as SnipeBaseState;
                fireState.reloadTier = data.currentReloadTier;
            }

            EntityStateMachine machine = data.isReloading ? data.reloadStatemachine : skillSlot.stateMachine;

            if( machine.SetInterruptState( state, data.isReloading ? this.reloadInterruptPriority : base.interruptPriority ) )
            {
                CharacterBody body = skillSlot.characterBody;
                if( body )
                {
                    if( base.noSprint )
                    {
                        body.isSprinting = false;
                    }
                    body.OnSkillActivated( skillSlot );
                }
                if( data.isReloading )
                {
                    data.StopReload();
                } else
                {
                    skillSlot.stock -= base.stockToConsume;
                    data.delayTimer = 0f;
                    if( skillSlot.stock <= 0 )
                    {
                        data.StartReload();
                    }
                }
            }
        }

        public sealed override void OnFixedUpdate( GenericSkill skillSlot )
        {
            var data = skillSlot.skillInstanceData as SniperPrimaryInstanceData;
            data.delayTimer += Time.fixedDeltaTime * skillSlot.characterBody.attackSpeed;
        }


        internal class SniperPrimaryInstanceData : BaseSkillInstanceData
        {
            internal SniperPrimaryInstanceData( 
                SniperReloadableFireSkillDef def,
                EntityStateMachine reloadTargetStatemachine, 
                ReloadParams reloadParams,
                GenericSkill skillSlot
            )
            {
                this.def = def;
                this.reloadStatemachine = reloadTargetStatemachine;
                this.reloadParams = reloadParams;
                _ = ReloadUIController.GetReloadTexture( this.reloadParams );
                this.secondarySlot = this.reloadStatemachine.commonComponents.characterBody.skillLocator.secondary;
                this.isReloading = true;
                this.currentReloadTier = ReloadTier.Perfect;
                this.skillSlot = skillSlot;
                this.skillSlot.stock = this.def.stockToReload;
                this.body = this.skillSlot.characterBody as SniperCharacterBody;
            }

            internal void ForceReload( ReloadTier tier )
            {
                this.currentReloadTier = tier;
                SoundModule.PlayLoad( this.secondarySlot.gameObject, tier );
                this.isReloading = false;
                this.skillSlot.stock = Mathf.Max( this.skillSlot.stock, Mathf.Min( this.skillSlot.stock + this.def.stockToReload, this.def.actualMaxStock ) );
                this.body.ForceStopReload();
                this.UpdateCrosshair();
            }

            internal void StartReload()
            {
                this.isReloading = true;
                this.body.StartReload( this.reloadParams );
                this.UpdateCrosshair();
            }
            internal void StopReload()
            {
                this.body.StopReload( this );
                this.skillSlot.stock += this.def.stockToReload;
                this.UpdateCrosshair();
            }

            internal ReloadTier ReadReload()
            {
                this.currentReloadTier = this.body.ReadReload();
                this.UpdateCrosshair();
                return this.currentReloadTier;
            }


            internal SniperCrosshairController crosshair
            {
                private get => this._crosshair;
                set
                {
                    this._crosshair = value;
                    this.UpdateCrosshair();
                }
            }
            private SniperCrosshairController _crosshair;
            private void UpdateCrosshair()
            {
                if( !this.crosshair || this.crosshair is null ) return;
                this.crosshair.reloadMax = this.def.actualMaxStock;
                this.crosshair.reloadCurrent = this.skillSlot.stock;
                this.crosshair.reloadTier = this.currentReloadTier;
            }

            internal Boolean CanReload() => this.body.CanReload();

            internal Boolean CanShoot() => this.delayTimer >= this.def.shootDelay;

            internal SniperReloadableFireSkillDef def;
            internal EntityStateMachine reloadStatemachine;
            internal GenericSkill skillSlot;
            internal GenericSkill secondarySlot;
            internal SniperCharacterBody body;
            internal ReloadParams reloadParams;
            internal ReloadTier currentReloadTier;
            internal Boolean isReloading = true;
            internal Single delayTimer;
        }
    }
}
