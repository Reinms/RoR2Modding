﻿using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Sniper.ScriptableObjects.Custom
{
    [CreateAssetMenu( fileName = "TextureSet", menuName = "Rein/Sniper/SniperTextureSet", order = 1 )]
    public class SniperTextureSet : ScriptableObject
    {
        [SerializeField]
        public List<SniperTexture> sniperTextures = new List<SniperTexture>();

        public Texture2D this[String key] => this.lookup[key].texture;

        public void Merge( SniperTextureSet source )
        {
            for( Int32 i = 0; i < source.sniperTextures.Count; i++ )
            {
                this.sniperTextures.Add( source.sniperTextures[i] );
            }

            this.CheckForDuplicates();
            this.Build();
        }

        public void MergeAndReplace( SniperTextureSet source )
        {
            this.Build();
            for( Int32 i = 0; i < source.sniperTextures.Count; ++i )
            {
                SniperTexture tex = source.sniperTextures[i];
                if( this.lookup.TryGetValue( tex.textureName, out var existingEntry ) )
                {
                    this.sniperTextures[existingEntry.index] = tex;
                } else
                {
                    this.sniperTextures.Add( tex );
                }
            }
            this.CheckForDuplicates();
            this.Build();
        }

        private Dictionary<String,(Int32 index,Texture2D texture)> lookup = new Dictionary<String, (Int32 index, Texture2D texture)>();

        private void Build()
        {
            for( Int32 i = 0; i < this.sniperTextures.Count; ++i )
            {
                var tex = this.sniperTextures[i];
                this.lookup[tex.textureName] = (i, tex.texture);
            }
        }

        private void CheckForDuplicates()
        {
            var hash = new HashSet<String>();
            for( Int32 i = 0; i < this.sniperTextures.Count; ++i )
            {
                if( !hash.Add( this.sniperTextures[i].textureName ) ) throw new ArgumentException( "Cannot have duplicate names for textures in a set" );
            }
        }

        private void Awake() => this.Build();



        private void OnValidate() => this.CheckForDuplicates();
    }
}
