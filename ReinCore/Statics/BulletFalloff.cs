﻿namespace ReinCore
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using BepInEx;
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using MonoMod.Cil;
    using RoR2;
    using UnityEngine;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public static class BulletFalloffCore
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static Boolean loaded { get; internal set; } = false;
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public delegate Single FalloffDelegate( Single distance );
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static BulletAttack.FalloffModel AddFalloffModel( FalloffDelegate falloffDelegate )
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            if( !loaded )
            {
                throw new CoreNotLoadedException( nameof( FalloffDelegate ) );
            }

            delegates.Add( falloffDelegate );
            BulletAttack.FalloffModel ind = currentIndex++;

            if( hookApplied )
            {
                HooksCore.RoR2.BulletAttack.DefaultHitCallback.Il -= DefaultHitCallback_Il;
            }
            HooksCore.RoR2.BulletAttack.DefaultHitCallback.Il += DefaultHitCallback_Il;
            hookApplied = true;

            return ind;
        }



        static BulletFalloffCore()
        {
            loaded = true;
        }

        private static Boolean hookApplied = false;

        private static readonly BulletAttack.FalloffModel startingIndex = EnumExtensions.GetMax<BulletAttack.FalloffModel>();
        private static BulletAttack.FalloffModel currentIndex = startingIndex + 1;
        private static readonly List<FalloffDelegate> delegates = new List<FalloffDelegate>();

        private static void DefaultHitCallback_Il( ILContext il )
        {
            var cursor = new ILCursor( il );

            ILLabel[] baseLabels = null;
            ILLabel breakLabel = null;
            _ = cursor.GotoNext( MoveType.AfterLabel, 
                x => x.MatchSwitch( out baseLabels ),
                x => x.MatchBr( out breakLabel )
            );

            Int32 origCases = baseLabels.Length;
            Array.Resize<ILLabel>( ref baseLabels, origCases + delegates.Count );

            _ = cursor.GotoLabel( breakLabel, MoveType.Before, false );

            FieldInfo field = typeof( BulletAttack.BulletHit ).GetField( nameof(BulletAttack.BulletHit.distance), BindingFlags.Instance | BindingFlags.Public );

            for( Int32 i = 0; i < delegates.Count; ++i )
            {
                Int32 caseInd = origCases + i;

                _ = cursor.Emit( OpCodes.Ldarg_1 );
                cursor.Index--;
                ILLabel label = cursor.MarkLabel();
                cursor.Index++;
                _ = cursor.Emit( OpCodes.Ldfld, field );
                _ = cursor.EmitDelegate<FalloffDelegate>( delegates[i] );
                _ = cursor.Emit( OpCodes.Stloc, 4 );
                _ = cursor.Emit( OpCodes.Br, breakLabel );

                baseLabels[caseInd] = label;
            }

            _ = cursor.GotoPrev( MoveType.AfterLabel, x => x.MatchSwitch( out _ ) );
            _ = cursor.Remove();
            _ = cursor.Emit( OpCodes.Switch, baseLabels );
        }
    }
}
