﻿namespace ILHelper
{
    using System;
    using System.Reflection;

    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using Mono.Collections.Generic;

    using MonoMod.Cil;

    using Object = System.Object;

    public class Boxed<T> : IStackRep
        where T : struct
    {
        public T unbox => (T)(Object)this;
        Type IStackRep.representedType => typeof(Object);
    }
}
