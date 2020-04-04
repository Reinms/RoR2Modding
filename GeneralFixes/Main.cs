﻿using BepInEx;
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
using System.Runtime.CompilerServices;

namespace ReinGeneralFixes
{
    [BepInDependency( Rein.AssemblyLoad.guid, BepInDependency.DependencyFlags.HardDependency )]
    [BepInPlugin("com.Rein.GeneralBalance", "General Balance + Fixes", "2.1.0.32")]
    internal partial class Main : BaseUnityPlugin
    {
        internal Single gestureBreakChance = 0.025f;

        internal static Main instance;



        private event Action Load;
        private event Action Enable;
        private event Action Disable;
        private event Action FirstFrame;
        private event Action Frame;
        private event Action PostFrame;
        private event Action Tick;
        private event Action GUI;


        partial void BalanceCommandoCDs();
        partial void BalanceCorpsebloom();
        partial void BalanceOSP();
        partial void BalanceGesture();
        partial void BalancePreGameShake();

        partial void FixBandolier();
        partial void FixSelfDamage();
        partial void FixCircletModel();


        partial void QoLCommandoRoll();
        partial void QoLVisionsCrosshair();
        partial void QoLOvergrownPrinters();

        partial void PerformanceKinCharController();


#if PROFILER
        partial void Profiler();
#endif
#if DPSMETER
        partial void SetupDPSMeter();
#endif

        private Main()
        {
            instance = this;
#if PROFILER
            this.Profiler();
#endif



            this.BalanceCommandoCDs();
            this.BalanceCorpsebloom();
            this.BalanceOSP();
            this.BalanceGesture();
            this.BalancePreGameShake();

            this.FixBandolier();
            this.FixSelfDamage();
            this.FixCircletModel();

            this.QoLCommandoRoll();
            this.QoLVisionsCrosshair();
            this.QoLOvergrownPrinters();

            this.PerformanceKinCharController();

            //On.RoR2.Projectile.ProjectileController.Start += this.ProjectileController_Start;

#if DPSMETER
            this.SetupDPSMeter();
#endif
        }


#pragma warning disable IDE0051 // Remove unused private members
        private void Awake() => this.Load?.Invoke();
        private void Start() => this.FirstFrame?.Invoke();
        private void OnEnable() => this.Enable?.Invoke();
        private void OnDisable() => this.Disable?.Invoke();
        private void Update() => this.Frame?.Invoke();
        private void LateUpdate() => this.PostFrame?.Invoke();
        private void FixedUpdate() => this.Tick?.Invoke();
        private void OnGUI() => this.GUI?.Invoke();
#pragma warning restore IDE0051 // Remove unused private members



        internal static void Log( BepInEx.Logging.LogLevel level, System.Object data, String member, Int32 line )
        {
            if( data == null )
            {
                Main.instance.Logger.LogError( "Null data sent by: " + member + " line: " + line );
                return;
            }

            if( level == BepInEx.Logging.LogLevel.Fatal || level == BepInEx.Logging.LogLevel.Error || level == BepInEx.Logging.LogLevel.Warning | level == BepInEx.Logging.LogLevel.Message )
            {
                Main.instance.Logger.Log( level, data );
            } else
            {
#if LOGGING
                Main.instance.Logger.Log( level, data );
#endif
            }
#if FINDLOGS
            Main.instance.Logger.LogWarning( "Log: " + level.ToString() + " called by: " + member + " : " + line );
#endif
        }

        internal static void LogI( System.Object data, [CallerMemberName] String member = "", [CallerLineNumber] Int32 line = 0 ) => Main.Log( BepInEx.Logging.LogLevel.Info, data, member, line );
        internal static void LogM( System.Object data, [CallerMemberName] String member = "", [CallerLineNumber] Int32 line = 0 ) => Main.Log( BepInEx.Logging.LogLevel.Message, data, member, line );
        internal static void LogD( System.Object data, [CallerMemberName] String member = "", [CallerLineNumber] Int32 line = 0 ) => Main.Log( BepInEx.Logging.LogLevel.Debug, data, member, line );
        internal static void LogW( System.Object data, [CallerMemberName] String member = "", [CallerLineNumber] Int32 line = 0 ) => Main.Log( BepInEx.Logging.LogLevel.Warning, data, member, line );
        internal static void LogE( System.Object data, [CallerMemberName] String member = "", [CallerLineNumber] Int32 line = 0 ) => Main.Log( BepInEx.Logging.LogLevel.Error, data, member, line );
        internal static void LogF( System.Object data, [CallerMemberName] String member = "", [CallerLineNumber] Int32 line = 0 ) => Main.Log( BepInEx.Logging.LogLevel.Fatal, data, member, line );
        internal static Int32 logCounter = 0;
        internal static void LogC( [CallerMemberName] String member = "", [CallerLineNumber] Int32 line = 0 ) => Main.Log( BepInEx.Logging.LogLevel.Info, member + ": " + line + ":: " + logCounter++, member, line );
    }
}
/*
Changelist:
Commando Phase round 2s cooldown, phase blast 3s cooldown
Commando dodge roll now counts as sprinting
// TODO: Commando roll nosprint false
// TODO: Commando roll movespeed mult down

OSP now is applied after shields and barrier.
OSP can now no longer block more than 180% of your max health in damage from any single hit.

// TODO: Disable OP items
// TODO: Disable UP items


Gesture now has chance of breaking equipment on use.
Gesture now reduces cooldown by 50% per stack.

Visions now gives huntress and mercenary commandos crosshair to make aiming possible.

Overgrown printers can now have any boss tier item, including fancy pearls and halcyon seeds.



// TODO: Focus crystal + crowbar self damage
// TODO: Add model to spectral circlet





ITEMS:
Gesture             chance to overload equipment on use. While overloaded equip has low cd and fires automatically. Equip breaks on overload end.


Fireworks
Gasoline
Medkit
Monster Tooth
Stun Grenade
Warbanner           (Regen while in zone)

Berzerkers          (Gives base damage instead of attack speed)
Chronobauble
Razor Wire
Will o Wisp

Ceremonial Dagger
Happiest Mask
Resonance Disk
Tesla Coil

Queens Gland

Blast Shower        (make it reflect projectiles, instead of destroying)
Royal Capacitor

Effigy
*/