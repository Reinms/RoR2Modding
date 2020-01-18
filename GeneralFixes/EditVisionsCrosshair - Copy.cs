﻿using BepInEx;
using RoR2;
using UnityEngine;
using System.Collections.Generic;
using RoR2.Navigation;
using R2API;
using R2API.Utils;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Reflection;
using EntityStates;
using RoR2.Skills;

namespace ReinGeneralFixes
{
    internal partial class Main
    {
        partial void EditOvergrownPrinters()
        {
            this.Load += this.DoOvergrownPrinterEdits;
        }

        private void DoOvergrownPrinterEdits()
        {
            var wildPrint = Resources.Load<GameObject>("Prefabs/NetworkedObjects/DuplicatorWild").GetComponent<ShopTerminalBehavior>();
            wildPrint.bannedItemTag = ItemTag.Any;
        }
    }
}
