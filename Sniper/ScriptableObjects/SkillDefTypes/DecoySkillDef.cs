﻿namespace Rein.Sniper.SkillDefs
{
    using System;

    using Rein.Sniper.SkillDefTypes.Bases;
    using Rein.Sniper.States.Bases;

    using UnityEngine;

    internal class DecoySkillDef : ReactivatedSkillDef<DecoySkillData>
    {
        internal static DecoySkillDef Create<TActivation, TReactivation>( String activationMachine, String reactivationMachine )
            where TActivation : ActivationBaseState<DecoySkillData>
            where TReactivation : ReactivationBaseState<DecoySkillData>
        {
            DecoySkillDef def = ScriptableObject.CreateInstance<DecoySkillDef>();
            def.InitStates<TActivation, TReactivation>( activationMachine, reactivationMachine );
            return def;
        }
    }
}
