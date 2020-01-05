﻿using BepInEx;
using R2API.Utils;
using RoR2;
using RoR2.Networking;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using RogueWispPlugin.Helpers;
using RogueWispPlugin.Modules;
//using static RogueWispPlugin.Helpers.APIInterface;

namespace RogueWispPlugin
{
    internal partial class Main
    {
        private GameObject AW_body;
        private GameObject AW_master;

        partial void AW_Test();
        partial void AW_General();
        partial void AW_Hook();

        partial void CreateAncientWisp()
        {
            this.AW_Test();
            this.AW_General();
            this.AW_Hook();
        }
    }

}
