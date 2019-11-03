﻿using UnityEngine;
using System;
using System.Collections.Generic;
using static WispSurvivor.Util.OrbHelper;

namespace WispSurvivor.Modules
{
    public static class WispOrbModule
    {
        public static void DoModule( GameObject body , Dictionary<Type,Component> dic)
        {
            AddOrb(typeof(Orbs.RestoreOrb));
            AddOrb(typeof(Orbs.SnapOrb));
            AddOrb(typeof(Orbs.SparkOrb));
            AddOrb(typeof(Orbs.BlazeOrb));
            AddOrb(typeof(Orbs.IgnitionOrb));
        }

        private static T C<T>( this Dictionary<Type,Component> dic ) where T : Component
        {
            return dic[typeof(T)] as T;
        }
    }
}
