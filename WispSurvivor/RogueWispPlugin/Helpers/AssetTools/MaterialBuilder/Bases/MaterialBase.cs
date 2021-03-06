﻿//using System;
//using System.Collections.Generic;
//using System.Text;
//using UnityEngine;
//using ReinCore;

//namespace RogueWispPlugin.Helpers
//{
//    internal abstract class MaterialBase
//    {
//        /// <summary>
//        /// The name of the material
//        /// </summary>
//        internal String name
//        {
//            get
//            {
//                return this.material?.name;
//            }
//            set
//            {
//                if( this.material ) this.material.name = value;
//            }
//        }

//        /// <summary>
//        /// The shader to use
//        /// </summary>
//        internal ShaderIndex shader { get; set; }

//        /// <summary>
//        /// The Material that is being created
//        /// </summary>
//        internal Material material { get; private set; }

//        internal MaterialBase( String name, ShaderIndex index )
//        {
//            this.shader = index;
//            this.material = new Material( AssetsCore.LoadAsset<Shader>(index) );
//            this.name = name;
//        }

//        internal MaterialBase( Material mat )
//        {
//            this.material = mat;
//            this.name = mat.name;
//        }

//        internal enum CullMode
//        {
//            Off = 0,
//            Front = 1,
//            Back = 2
//        }

//        internal enum RampInfo
//        {
//            TwoTone = 0,
//            SmoothedTwoTone = 1,
//            Unlitish = 3,
//            SubSurface = 4,
//        }

//        internal void SetSingle( String name, Single value )
//        {
//            this.material.SetFloat( name, value );
//        }
//        internal Single GetSingle( String name )
//        {
//            return this.material.GetFloat( name );
//        }
//        internal void SetColor( String name, Color value )
//        {
//            this.material.SetColor( name, value );
//        }
//        internal Color GetColor( String name )
//        {
//            return this.material.GetColor( name );
//        }
//        internal void SetKeyword( String keyword, Boolean value )
//        {
//            if( value )
//            {
//                this.material.EnableKeyword( keyword );
//            } else
//            {
//                this.material.DisableKeyword( keyword );
//            }
//        }
//        internal Boolean GetKeyword( String keyword )
//        {
//            return this.material.IsKeywordEnabled( keyword );
//        }
//        internal Vector4 GetVector4( String name )
//        {
//            return this.material.GetVector( name );
//        }
//        internal void SetVector4( String name, Vector4 value )
//        {
//            this.material.SetVector( name, value );
//        }


//        internal class TextureData
//        {
//            internal TextureData( Material mat, String propName )
//            {
//                this.mat = mat;
//                this.propName = propName;
//                this._texture = this.mat.GetTexture( this.propName );
//            }

//            internal Texture texture
//            {
//                get => this._texture;
//                set
//                {
//                    if( value != this._texture )
//                    {
//                        this._texture = value;
//                        this.mat.SetTexture( this.propName, this._texture );
//                    }
//                }
//            }

//            internal Material mat;
//            internal String propName;

//            private Texture _texture;
//        }

//        internal class ScaleOffsetTextureData : TextureData
//        {
//            internal ScaleOffsetTextureData( Material mat, String propName ) : base( mat, propName )
//            {

//                this._tiling = base.mat.GetTextureScale( base.propName );
//                this._offset = base.mat.GetTextureOffset( base.propName );
//            }
//            internal Vector2 tiling
//            {
//                get => this._tiling;
//                set
//                {
//                    if( value != this._tiling )
//                    {
//                        this._tiling = value;
//                        base.mat.SetTextureScale( base.propName, this._tiling );
//                    }
//                }
//            }
//            internal Vector2 offset
//            {
//                get => this._offset;
//                set
//                {
//                    if( value != this._offset )
//                    {
//                        this._offset = value;
//                        base.mat.SetTextureOffset( base.propName, this._offset );
//                    }
//                }
//            }

//            private Vector2 _tiling;
//            private Vector2 _offset;
//        }
//    }

//}
