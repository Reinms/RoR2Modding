﻿namespace Sniper.States.Special
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using BepInEx.Logging;
    using ReinCore;
    using RoR2;
    using RoR2.Networking;
    using UnityEngine;
    using KinematicCharacterController;
    using EntityStates;
    using RoR2.Skills;
    using System.Reflection;
    using Sniper.Expansions;
    using Sniper.Enums;
    using Sniper.SkillDefs;
    using Sniper.States.Bases;

    internal class KnifeActivation : ActivationBaseState<KnifeSkillData>
    {
        // TODO: Implement Knife Activation
        internal override KnifeSkillData CreateSkillData()
        {
            base.data = new KnifeSkillData();
            return base.data;
        }
    }
}
