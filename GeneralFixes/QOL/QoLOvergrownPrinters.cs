﻿namespace ReinGeneralFixes
{
    using BepInEx;
    using RoR2;
    using UnityEngine;
    using System.Collections.Generic;
    using RoR2.Navigation;
    using Mono.Cecil.Cil;
    using MonoMod.Cil;
    using System;
    using System.Reflection;
    using EntityStates;
    using RoR2.Skills;

    internal partial class Main
    {
        partial void QoLOvergrownPrinters() => this.Load += this.DoOvergrownPrinterEdits;

        private void DoOvergrownPrinterEdits()
        {
            ShopTerminalBehavior wildPrint = Resources.Load<GameObject>("Prefabs/NetworkedObjects/DuplicatorWild").GetComponent<ShopTerminalBehavior>();
            wildPrint.bannedItemTag = ItemTag.Any;
        }
    }
}
