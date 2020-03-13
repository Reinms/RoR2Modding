﻿using System;
using System.Collections.Generic;
using BepInEx;

namespace ReinCore
{
    // TODO: LanguageCore docs

    /// <summary>
    /// 
    /// </summary>
    public static class LanguageCore
    {
        /// <summary>
        /// 
        /// </summary>
        public static Boolean loaded { get; internal set; } = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="language"></param>
        public static void AddLanguageToken( String key, String value, String language = "" )
        {
            RoR2.Language.onCurrentLanguageChanged += () =>
            {
                var cur = RoR2.Language.currentLanguage;
                if( cur == language || String.IsNullOrEmpty( language ) )
                {
                    var langDict = languageDictionaries.Get();
                    if( langDict.TryGetValue( cur, out var dict ) )
                    {
                        dict[key] = value;
                    }
                }
            };
        }


        static LanguageCore()
        {



            loaded = true;
        }

        private static StaticAccessor<Dictionary<String,Dictionary<String,String>>> languageDictionaries = new StaticAccessor<Dictionary<String, Dictionary<String, String>>>( typeof(RoR2.Language), "languageDictionaries" );
    }
}