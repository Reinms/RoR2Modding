﻿namespace ILHelper
{
    using System;
    using System.Reflection;

    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using Mono.Collections.Generic;

    using MonoMod.Cil;

    using Object = System.Object;


    public readonly struct ILShiftR<TVal1, TVal2, TResult>
    {
        internal readonly Boolean unsigned;
    }
}
