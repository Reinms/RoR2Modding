﻿using RoR2;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RogueWispPlugin.Helpers
{
    internal class WispBitSkin : IBitSkin
    {
        #region Static
        internal static HashSet<UInt32> validColorInds = new HashSet<UInt32>();
        internal static HashSet<UInt32> validFlameStyles = new HashSet<UInt32>();
        internal static HashSet<UInt32> validArmorMaterials = new HashSet<UInt32>();

        #region Color
        private static Color[] colors = new Color[]
        {
            new Color( 0.8f, 0.3f, 0.9f ),          //0u    Ancient
            new Color( 0.906f, 0.420f, 0.235f ),    //1u    Lesser
            new Color( 0.4f, 0.769f, 0.192f ),      //2u    Greater
            new Color( 1f, 0.59f, 0.806f ),         //3u    Archaic
            new Color( 0.5f, 0.75f, 1f ),           //4u    Lunar
            new Color( 0.95f, 0.95f, 0.05f ),       //5u    Solar
            new Color( 0.95f, 0.05f, 0.05f ),       //6u    Abyssal
            new Color( 0f, 0f, 0f ),                //7u    Blighted
            new Color( 1f, 1f, 1f ),                //8u    Pure
        };
        #endregion
        #region FLGrad
        private static Func<Color,Gradient>[] flameGradStyles = new Func<Color, Gradient>[]
        {
            (col) =>                                //0u    Standard
            {
                var grad = new Gradient();
                var aKeys = new GradientAlphaKey[]
                {
                    new GradientAlphaKey( 0f, 0f ),
                    new GradientAlphaKey( 0f, 0.5f ),
                    new GradientAlphaKey( 1f, 1f ),
                };

                var cKeys = new GradientColorKey[]
                {
                    new GradientColorKey( Color.black, 0f ),
                    new GradientColorKey( col, 0.1f ),
                    new GradientColorKey( col, 1f ),
                };

                grad.alphaKeys = aKeys;
                grad.colorKeys = cKeys;
                return grad;
            },

            (col) =>                                //1u    Black Outline
            {
                var grad = new Gradient();
                var aKeys = new GradientAlphaKey[]
                {
                    new GradientAlphaKey( 0f, 0f ),
                    new GradientAlphaKey( 1f, 1f ),
                };

                var cKeys = new GradientColorKey[]
                {
                    new GradientColorKey( Color.black, 0f ),
                    new GradientColorKey( Color.black, 0.9f ),
                    new GradientColorKey( col, 1f ),
                };

                grad.alphaKeys = aKeys;
                grad.colorKeys = cKeys;
                return grad;
            },

            (col) =>                                //2u    White Outline
            {
                var grad = new Gradient();
                var aKeys = new GradientAlphaKey[]
                {
                    new GradientAlphaKey( 0f, 0f ),
                    new GradientAlphaKey( 0f, 0.5f ),
                    new GradientAlphaKey( 1f, 1f ),
                };

                var cKeys = new GradientColorKey[]
                {
                    new GradientColorKey( Color.black, 0f ),
                    new GradientColorKey( Color.white, 0.1f ),
                    new GradientColorKey( Color.white, 0.65f ),
                    new GradientColorKey( col, 0.7f ),
                    new GradientColorKey( col, 1f ),
                };

                grad.alphaKeys = aKeys;
                grad.colorKeys = cKeys;
                return grad;
            },

            (col) =>                                //3u    Black Inline
            {
                var grad = new Gradient();
                var aKeys = new GradientAlphaKey[]
                {
                    new GradientAlphaKey( 0f, 0f ),
                    new GradientAlphaKey( 0f, 0.5f ),
                    new GradientAlphaKey( 1f, 1f ),
                };

                var cKeys = new GradientColorKey[]
                {
                    new GradientColorKey( Color.black, 0f ),
                    new GradientColorKey( col, 0.1f ),
                    new GradientColorKey( col, 0.6f ),
                    new GradientColorKey( Color.black, 0.7f ),
                    new GradientColorKey( Color.black, 1f ),
                };

                grad.alphaKeys = aKeys;
                grad.colorKeys = cKeys;
                return grad;
            },

            (col) =>                                //4u    White Inline
            {
                var grad = new Gradient();
                var aKeys = new GradientAlphaKey[]
                {
                    new GradientAlphaKey( 0f, 0f ),
                    new GradientAlphaKey( 0f, 0.5f ),
                    new GradientAlphaKey( 1f, 1f ),
                };

                var cKeys = new GradientColorKey[]
                {
                    new GradientColorKey( Color.black, 0f ),
                    new GradientColorKey( col, 0.1f ),
                    new GradientColorKey( col, 0.95f ),
                    new GradientColorKey( Color.white, 1f ),
                };

                grad.alphaKeys = aKeys;
                grad.colorKeys = cKeys;
                return grad;
            },
        };
        #endregion
        #region ARMat
        private static Action<Color, StandardMaterial>[] armorMaterials = new Action<Color, StandardMaterial>[]
        {
            (col, mat) =>        //0u           Standard
            {
                mat.mainColor = new Color( 0.1f, 0.1f, 0.1f, 0.9f );
                mat.specularStrength = 0.1f;
                mat.specularExponent = 0.95f;
            },
            (col, mat) =>       //1u            Bone
            {
                mat.mainColor = new Color( 0.6f, 0.6f, 0.57f, 0.9f );
                mat.normalMap.texture = AssetLibrary<Texture>.i[TextureIndex.refWaves_N];
                mat.normalStrength = 1f;
                mat.smoothness = 1f;
            },
            (col, mat) =>       //2u            Metal
            {
                mat.mainTexture.texture = AssetLibrary<Texture>.i[TextureIndex.refTexCloudColor1];
                mat.mainColor = new Color( 0.3f, 0.3f, 0.3f, 0f );
                mat.normalMap.texture = AssetLibrary<Texture>.i[TextureIndex.refWaves_N];
                mat.normalStrength = 0.25f;
                mat.smoothness = 1f;
                mat.rampChoice = StandardMaterial.RampInfo.SmoothedTwoTone;
                mat.specularExponent = 0.75f;
                mat.specularStrength = 0.9f;
            },
        };
        #endregion
        #region Static Helpers
        private static Gradient CreateFlowGradient( Color color )
        {
            var grad = new Gradient();
            var aKeys = new GradientAlphaKey[]
            {
                new GradientAlphaKey( 1f, 0f ),
                new GradientAlphaKey( 1f, 1f ),
            };

            var cKeys = new GradientColorKey[]
            {
                new GradientColorKey( new Color( 0.05f, 0.05f, 0.05f ), 0f ),
                new GradientColorKey( new Color( 0.05f, 0.05f, 0.05f ), 0.4f ),
                new GradientColorKey( color, 0.6f ),
                new GradientColorKey( color, 0.7f ),
                new GradientColorKey( Color.white, 1f ),
            };

            grad.alphaKeys = aKeys;
            grad.colorKeys = cKeys;

            return grad;
        }
        private static Gradient CreateFEGradient( Color color )
        {
            var grad = new Gradient();
            var aKeys = new GradientAlphaKey[]
            {
                new GradientAlphaKey( 0f, 0f ),
                new GradientAlphaKey( 0f, 0.3f ),
                new GradientAlphaKey( 1f, 0.6f ),
            };

            var cKeys = new GradientColorKey[]
            {
                new GradientColorKey( color / 2f, 0f ),
                new GradientColorKey( color, 0.5f ),
                new GradientColorKey( Color.white, 0.6f ),
                new GradientColorKey( color, 0.8f ),
                new GradientColorKey( Color.black, 0.9f ),
            };

            grad.alphaKeys = aKeys;
            grad.colorKeys = cKeys;

            return grad;
        }

        private static Material CreatePlaceholderMaterial()
        {
            return new Material(Shader.Find( "Standard" ) );
        }
        #endregion

        #region Static Placeholders
        //internal static Material armorMain_placeholder = CreatePlaceholderMaterial();
        //internal static Material flameMain_placeholder = CreatePlaceholderMaterial();

        #endregion

        private static Gradient iridescentFlameGradient = new Gradient();
        private static Dictionary<UInt32,WispBitSkin> skinLookup = new Dictionary<UInt32, WispBitSkin>();
        static WispBitSkin()
        {
            var aKeys = new GradientAlphaKey[]
            {
                new GradientAlphaKey( 0f, 0f ),
                new GradientAlphaKey( 1f, 1f )
            };

            var cKeys = new GradientColorKey[]
            {
                new GradientColorKey( Color.black, 0.05f ),
                new GradientColorKey( new Color( 1f, 0f, 1f ), 0.165f ),
                new GradientColorKey( new Color( 0f, 0f, 1f ), 0.33f ),
                new GradientColorKey( new Color( 0f, 1f, 1f ), 0.495f ),
                new GradientColorKey( new Color( 0f, 1f, 0f ), 0.66f ),
                new GradientColorKey( new Color( 1f, 1f, 0f ), 0.825f ),
                new GradientColorKey( new Color( 1f, 0f, 0f ), 0.99f ),
                new GradientColorKey( new Color( 1f, 1f, 1f ), 1f ),
            };
            iridescentFlameGradient.alphaKeys = aKeys;
            iridescentFlameGradient.colorKeys = cKeys;


            //for( Int32 i = 0; i < Int32.MaxValue; ++i )
            //{
            //    if( i < colors.Length ) validColorInds.Add( i );
            //    if( i < flameGradStyles.Length ) validFlameStyles.Add( i );
            //    if( i < armorMaterials.Length ) validArmorMaterials.Add( i );
            //}
        }
        internal static WispBitSkin GetWispSkin( UInt32 ind )
        {
            Main.LogI( "Getting skin for: " + ind );
            if( skinLookup.ContainsKey( ind ) )
            {
                return skinLookup[ind];
            }



            var flags = (ind &          0b1111_0000_0000_0000_0000_0000_0000_0000u) >> 28;
            Boolean useCustomColor = ( (flags & 0b0001u ) >> 0 ) != 0u;
            Boolean isIridescent = ( (flags & 0b0010u ) >> 1 ) != 0u;
            //Boolean isTransparent = ( (flags & 0b0100u ) >> 2 ) != 0u;
            Boolean hasCracks = ( (flags & 0b1000u ) >> 3 ) != 0u;

            WispColorIndex color = WispColorIndex.MAX;
            UInt32 encodedCustomColor = 0u;
            if( useCustomColor )
            {
                encodedCustomColor = (ind & 0b0000_0000_0000_0011_1111_1111_1111_1111u);
            } else
            {
                color = (WispColorIndex)(ind & 0b0000_0000_0000_0000_0000_0000_0000_1111u);
            }

            FlameGradientType flameGrad = (FlameGradientType)( (ind & 0b0000_0000_0001_1100_0000_0000_0000_0000u ) >> 18 );
            ArmorMaterialType armorMat = (ArmorMaterialType)(  (ind & 0b0000_1110_0000_0000_0000_0000_0000_0000u ) >> 25 );

            var skin = new WispBitSkin( isIridescent, /*isTransparent ,*/ hasCracks, useCustomColor, color, flameGrad, armorMat, encodedCustomColor );
            skinLookup[ind] = skin;
            return skin;
        }
        internal static void ClearCachedSkins()
        {
            skinLookup.Clear();
        }
        private static void SkinDef_Awake( On.RoR2.SkinDef.orig_Awake orig, RoR2.SkinDef self ) { }

        #region Equality
        public static Boolean operator ==( WispBitSkin obj1, WispBitSkin obj2 )
        {
            return obj1.EncodeToSkinIndex() == obj2.EncodeToSkinIndex();
        }
        public static Boolean operator !=( WispBitSkin obj1, WispBitSkin obj2 )
        {
            return obj1.EncodeToSkinIndex() != obj2.EncodeToSkinIndex();
        }

        #endregion
        #endregion
        internal Color mainColor { get; private set; }
        internal Gradient flameGradient { get; private set; }
        //internal Gradient armorGradient { get; private set; }
        internal Material armorMainMaterial { get; private set; }
        //internal Material armorSecondMaterial { get; private set; }
        internal Material flameMainMaterial { get; private set; }





        private WispColorIndex color;
        private FlameGradientType flameGradientType;
        private ArmorMaterialType armorMaterialType;
        private Boolean isIridescent;
        //private Boolean isTransparent;
        private Boolean hasCracks;
        private Boolean useCustomColor;
        private UInt32 encodedCustomColor;

        private Boolean encoded;
        private UInt32 encodeValue;

        internal WispBitSkin( Boolean isIridescent, /*Boolean isTransparent,*/ Boolean hasCracks, Boolean useCustomColor, WispColorIndex color, FlameGradientType flameType, ArmorMaterialType armorMatType, UInt32 encodedCustomColor = 0u )
        {
            this.encoded = false;
            this.encodeValue = 0u;

            this.isIridescent = isIridescent;
            //this.isTransparent = isTransparent;
            this.hasCracks = hasCracks;
            this.useCustomColor = useCustomColor;
            this.color = color;
            this.encodedCustomColor = encodedCustomColor;
            this.flameGradientType = flameType;
            this.armorMaterialType = armorMatType;



            if( this.useCustomColor )
            {
                var encColor = this.encodedCustomColor & 0b0000_0000_0011_1111_1111_1111_1111u;
                Single r = ((encColor & 0b11_1111_0000_0000_0000u)>>12) / (Single)(0b11_1111u);
                Single g = ((encColor & 0b00_0000_1111_1100_0000u)>>6) / (Single)(0b11_1111u);
                Single b = ((encColor & 0b00_0000_0000_0011_1111u) ) / (Single)(0b11_1111u);

                Main.LogI( r );
                Main.LogI( g );
                Main.LogI( b );

                this.mainColor = new Color( r, g, b, 1f );
            } else
            {
                this.mainColor = colors[(UInt32)this.color];
            }

            if( this.isIridescent )
            {
                this.flameGradient = iridescentFlameGradient;
            } else
            {
                this.flameGradient = flameGradStyles[(UInt32)this.flameGradientType]( this.mainColor );
            }

            //var baseMat = UnityEngine.Object.Instantiate<Material>( )
            var flamesMain = new CloudMaterial( "FlamesMain" );
            //flamesMain.sourceBlend = 5f;
            flamesMain.destinationBlend = 10f;
            //flamesMain.internalSimpleBlendMode = 1f;
            flamesMain.tintColor = new Color( 1f, 1f, 1f, 1f );
            flamesMain.disableRemapping = false;
            flamesMain.mainTexture.texture = AssetLibrary<Texture>.i[TextureIndex.refTexBanditExplosionMask];
            flamesMain.remapTexture.texture = RampTextureGenerator.GenerateRampTexture( this.flameGradient );
            flamesMain.softFactor = 1f;
            flamesMain.brightnessBoost = 1f;
            flamesMain.alphaBoost = 3f;
            flamesMain.alphaBias = -0.1f;
            flamesMain.useUV1 = false;
            flamesMain.fadeClose = false;
            flamesMain.fadeCloseDistance = 0.5f;
            flamesMain.cull = MaterialBase.CullMode.Off;
            flamesMain.cloudRemappingOn = true;
            flamesMain.cloudDistortionOn = false;
            flamesMain.distortionStrength = 0.1f;
            flamesMain.cloudTexture1.texture = AssetLibrary<Texture>.i[TextureIndex.refTexCloudDifferenceBW2];
            flamesMain.cloudTexture2.texture = AssetLibrary<Texture>.i[TextureIndex.refTexCloudLightning1];
            flamesMain.cutoffScrollSpeed = new Vector4( 15f, 17f, 11f, 13f );
            flamesMain.vertexColorOn = false;
            flamesMain.vertexAlphaOn = false;
            flamesMain.luminanceForTextureAlpha = false;
            flamesMain.vertexOffset = false;
            flamesMain.fresnelFade = true;
            flamesMain.fresnelPower = 0.01f;
            flamesMain.vertexOffsetAmount = 0f;
            flamesMain.externalAlpha = 1f;

            this.flameMainMaterial = flamesMain.material;

            /*
            if( this.isTransparent )
            {
            
                var main = new CloudMaterial( "ArmorMain" );
                var sec = new DistortionMaterial( "ArmorRefraction" );

                //main.sourceBlend = 5f;
                //main.destinationBlend = 10f;
                //main.internalSimpleBlendMode = 1f;
                main.tintColor = new Color( 0f, 0f, 0f, 0f );
                main.disableRemapping = false;
                main.mainTexture.texture = AssetLibrary<Texture>.i[TextureIndex.refTexCloudIce];
                main.remapTexture.texture = RampTextureGenerator.GenerateRampTexture( this.flameGradient );
                main.softFactor = 1f;
                main.brightnessBoost = 1f;
                main.alphaBoost = 1f;
                main.alphaBias = 0f;
                main.useUV1 = false;
                main.fadeClose = false;
                main.cull = MaterialBase.CullMode.Off;
                main.cloudRemappingOn = false;
                main.cloudDistortionOn = false;
                main.distortionStrength = 0.05f;
                main.cloudTexture1.texture = AssetLibrary<Texture>.i[TextureIndex.refCaustics];
                main.cloudTexture2.texture = AssetLibrary<Texture>.i[TextureIndex.refTexCloudIce];
                main.cutoffScrollSpeed = new Vector4( 0f, 0f, 1f, 3f );
                main.vertexColorOn = false;
                main.vertexAlphaOn = false;
                main.luminanceForTextureAlpha = false;
                main.vertexOffset = false;
                main.fresnelFade = false;
                main.fresnelPower = 0f;
                main.vertexOffsetAmount = 0f;
                main.externalAlpha = 1f;

                sec.color = new Color( 0f, 0f, 0f, 0f );
                sec.mainTexture.texture = null;
                sec.bumpTexture.texture = AssetLibrary<Texture>.i[TextureIndex.refWaves_N];
                sec.magnitude = 1f;


                if( this.hasCracks )
                {
                    main.cloudRemappingOn = true;
                }

                this.armorMainMaterial = main.material;
                this.armorSecondMaterial = sec.material;
                
            } else
            {
            */
                var main = new StandardMaterial( "ArmorMain" );
                //this.armorSecondMaterial = null;

                main.cutout = false;
                main.mainColor = Color.white;
                main.mainTexture.texture = null;
                main.normalStrength = 0f;
                main.normalMap.texture = null;
                main.emissionColor = Color.black;
                main.emissionPower = 0f;
                main.smoothness = 0f;
                main.ignoreDiffuseAlphaForSpecular = false;
                main.rampChoice = StandardMaterial.RampInfo.Unlitish;
                main.decalLayer = StandardMaterial.DecalLayer.Character;
                main.specularStrength = 0f;
                main.specularExponent = 1f;
                main.cull = MaterialBase.CullMode.Off;
                main.dither = false;
                main.fadeBias = 0f;
                main.fresnelEmission = false;
                main.fresnelRamp.texture = RampTextureGenerator.GenerateRampTexture( CreateFEGradient( this.mainColor ) );
                main.fresnelPower = 0f;
                main.fresnelMask.texture = null;
                main.fresnelBoost = 0f;
                main.printingEnabled = false;
                main.splatmapEnabled = false;
                main.flowmapEnabled = false;
                main.flowmapTexture.texture = AssetLibrary<Texture>.i[TextureIndex.refCaustics];
                main.flowmapHeightmap.texture = AssetLibrary<Texture>.i[TextureIndex.refCaustics];
                main.flowmapHeightRamp.texture = RampTextureGenerator.GenerateRampTexture( CreateFlowGradient( this.mainColor ) );
                //Ramp texture stuff
                main.flowHeightBias = 0.3f;
                main.flowHeightPower = 1f;
                main.flowHeightEmissionStrength = 1f;
                main.flowSpeed = 1f;
                main.flowMaskStrength = 0f;
                main.flowNormalStrength = 2f;
                main.flowTextureScaleFactor = 1f;

                main.limbRemovalEnabled = false;
                main.limbPrimeMask = 1f;
                main.flashColor = Color.black;

                if( this.hasCracks )
                {
                    main.flowmapEnabled = true;
                } else
                {
                    main.flowmapEnabled = false;
                }

                armorMaterials[(UInt32)this.armorMaterialType]( this.mainColor, main );


                this.armorMainMaterial = main.material;

            /*
            }
            */

            skinLookup[this.EncodeToSkinIndex()] = this;
        }

        internal UInt32 EncodeToSkinIndex()
        {
            if( this.encoded == false )
            {
                this.encodeValue = 0u;
                this.UpdateEncodedFlags();
                this.UpdateEncodedColor();
                this.UpdateEncodedFlameGradType();
                this.UpdateEncodedArmorMaterialType();

                this.encoded = true;
            }

            return this.encodeValue;
        }

        internal void Apply(GameObject obj )
        {
            On.RoR2.SkinDef.Awake += SkinDef_Awake;
            var skinDef = ScriptableObject.CreateInstance<SkinDef>();
            On.RoR2.SkinDef.Awake -= SkinDef_Awake;

        }

        #region Encoding
        private void UpdateEncodedFlags()
        {
            this.encodeValue &=     0b0000_1111_1111_1111_1111_1111_1111_1111u;
            UInt32 flag = 0u;
            flag |= (this.hasCracks ? 1u : 0u) << 31;
            //flag |= (this.isTransparent ? 1u : 0u) << 30;
            flag |= (this.isIridescent ? 1u : 0u) << 29;
            flag |= (this.useCustomColor ? 1u : 0u) << 28;
            this.encodeValue |= flag;
        }
        private void UpdateEncodedColor()
        {
            this.encodeValue &=     0b1111_1111_1111_1100_0000_0000_0000_0000u;
            var flag = 0u;
            if( this.useCustomColor )
            {
                flag = this.encodedCustomColor;
            } else
            {
                if( this.color >= WispColorIndex.MAX )
                {
                    throw new IndexOutOfRangeException( "Too large a value for flameColorIndex" );
                }
                flag = (UInt32)this.color;
            }

            this.encodeValue |= flag;
        }
        private void UpdateEncodedFlameGradType()
        {
            this.encodeValue &= 0b1111_1111_1110_0011_1111_1111_1111_1111u;
            if( this.flameGradientType >= FlameGradientType.MAX )
            {
                throw new IndexOutOfRangeException( "Too large a value for flameGradientType" );
            }

            var flag = ((UInt32)this.flameGradientType) << 18;
            this.encodeValue |= flag;
        }
        private void UpdateEncodedArmorMaterialType()
        {
            this.encodeValue &= 0b1111_0001_1111_1111_1111_1111_1111_1111u;
            if( this.armorMaterialType >= ArmorMaterialType.MAX )
            {
                throw new IndexOutOfRangeException( "Too large a value for armorMateriallType" );
            }

            var flag = ((UInt32)this.armorMaterialType) << 25;
            this.encodeValue |= flag;
        }
        #endregion
        #region Enums
        internal enum WispColorIndex : UInt32
        {
            Ancient = 0u,
            Lesser = 1u,
            Greater = 2u,
            Archaic = 3u,
            Lunar = 4u,
            Solar = 5u,
            Abyssal = 6u,
            Blighted = 7u,
            Pure = 8u,
            MAX = 16u,
        }
        internal enum FlameGradientType : UInt32
        {
            Standard = 0u,
            BlackOutline = 1u,
            WhiteOutline = 2u,
            BlackInline = 3u,
            WhiteInline = 4u,
            MAX = 8u,
        }
        internal enum ArmorMaterialType : UInt32
        {
            Standard = 0u,
            Bone = 1u,
            Metal = 2u,
            MAX = 8u,
        }
        #endregion
    }
}


//Skin system should go first
//  Define what is being exposed as a setting.
//      Base color of flames (Purple, white, ect)
//      Gradient properties Color to white, white to color, color to black, black to color
//      Armor material type (Plain, Crystal, Metal, ect)
//
//
//
//  Find ways to extend the system
//      IL hook into onnetuserloadoutchanged
//      Check if body is wispy
//      if wispy, handle skin index in special way (generate a skindef from it dynamically) need static function for this
//          
//
//
//
//
//  Mapping to the UINT
//  0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
// |Color                 |FLGra|L|ARMat| 
//
//      3 3 2 2 2 2 2 2 2 2 2 2 1 1 1 1 1 1 1 1 1 1 0 0 0 0 0 0 0 0 0 0                                
//      1 0 9 8 7 6 5 4 3 2 1 0 9 8 7 6 5 4 3 2 1 0 9 8 7 6 5 4 3 2 1 0             
//
//      0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
//     |D|D|D|D|ARMat|Padding|FLGra|CustomColorBits            |FLColor| 
//
//  D1 = y/n armor with glowing cracks
//  D2 = y/n armor transparent
//  D3 = y/n flames Iridescent
//  D4 = y/n custom color
//
//  FLGra = Flame gradient (8x options)
//  ARMat = Armor material (8x options)
//  Pad   = Unused padding space
//  FLColor = Flame color (16x options)
//
//
//  4096x (12) Color = The base color for flames and gradients
//  8x (3) FLGra = The style of the gradient for fire
//  2x (1) L = Should armor have glow lines?
//  8x (3) ARMat = The material of the armor
//  16x ARGrad = The style of the gradient for armor
//  16x ArMat = The "Material" of the armor (Normal, transparency, ect)
//  16x FLType flame type (unknown uses)
//  
// Color
// Grad type for fire
//      Standard
//      C to black
//      C to white
//      Black to C
//      White to C
//      ----
//      ----
//      ----
//
//
//
// Armor material
//      Standard
//      Metal
//      Pale
//      ----
//      ----
//      ----
//      ----
//      ----
//
//
//
//
//
//
//
//
//