﻿using System;
using System.Collections.Generic;
using System.Text;
using BepInEx;
using ReinCore;

namespace Rein.AlternateArtificer
{
    [BepInDependency( AssemblyLoad.guid )]
    [BepInPlugin( Main.guid, Main.name, Main.ver )]
    internal partial class Main : BaseUnityPlugin
    {
        public const String guid = "com.Rein.AltArti";
        public const String name = "Alternate Artificer";


    }
}
