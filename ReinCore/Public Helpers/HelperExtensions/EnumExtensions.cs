﻿namespace ReinCore
{
    using System;

    /// <summary>
    /// Various generic extensions for Enum types
    /// </summary>
    public static class GenericEnumExtensions
    {
        /// <summary>
        /// Faster version of HasFlag (?)
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="value"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public static Boolean Flag<TEnum>( this TEnum value, TEnum flag ) where TEnum : struct, Enum, IConvertible => ( Convert.ToUInt64( value ) & Convert.ToUInt64( flag ) ) != 0ul;
    }
}
