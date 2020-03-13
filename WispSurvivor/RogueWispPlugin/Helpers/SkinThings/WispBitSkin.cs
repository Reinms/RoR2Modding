﻿using RoR2;
using System;
using System.Collections.Generic;
using UnityEngine;
using ReinCore;

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
            new Color( 0f, 1f, 0.6f ),              //9u    Aquatic
            new Color( 0.302f, 0f, 0.302f ),        //10u   Faded
            new Color( 0.302f, 0f, 0f ),            //11u   Blood
            new Color( 0f, 0f, 0.2f ),              //12u   Midnight
            new Color( 0f, 0.302f, 0f ),            //13u   Forest

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

                var p1Col = col * 0.75f;
                var p2Col = col;
                var p3Col = col * 1.1f;
                var p4Col = col * 1.25f;

                p3Col.r = Mathf.Min( p3Col.r, 1f );
                p3Col.g = Mathf.Min( p3Col.g, 1f );
                p3Col.b = Mathf.Min( p3Col.b, 1f );
                p3Col.a = Mathf.Min( p3Col.a, 1f );

                p4Col.r = Mathf.Min( p4Col.r, 1f );
                p4Col.g = Mathf.Min( p4Col.g, 1f );
                p4Col.b = Mathf.Min( p4Col.b, 1f );
                p4Col.a = Mathf.Min( p4Col.a, 1f );

                var cKeys = new GradientColorKey[]
                {
                    new GradientColorKey( Color.black, 0f ),
                    new GradientColorKey( p1Col, 0.5f ),
                    new GradientColorKey( p2Col, 0.8f ),
                    new GradientColorKey( p3Col, 0.85f ),
                    new GradientColorKey( p4Col, 1f ),
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
                    new GradientColorKey( Color.black, 0.99f ),
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
                    new GradientColorKey( Color.grey, 0.5f ),
                    new GradientColorKey( col, 0.6f ),
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
                    new GradientAlphaKey( 1f, 1f ),
                };

                var cKeys = new GradientColorKey[]
                {
                    new GradientColorKey( Color.black, 0f ),
                    new GradientColorKey( col, 0.35f ),
                    new GradientColorKey( col, 0.5f ),
                    new GradientColorKey( Color.black, 0.6f ),
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
                    new GradientAlphaKey( 1f, 1f ),
                };

                var cKeys = new GradientColorKey[]
                {
                    new GradientColorKey( Color.black, 0f ),
                    new GradientColorKey( col, 0.5f ),
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
                mat.mainColor = new Color( 0.55f, 0.55f, 0.51f, 0.9f );
                mat.normalMap.texture = AssetsCore.LoadAsset<Texture>(Texture2DIndex.refWaves_N);
                mat.normalStrength = 1f;
                mat.smoothness = 1f;
            },
            (col, mat) =>       //2u            Metal
            {
                mat.mainTexture.texture = null;
                mat.mainColor = new Color( 0.3f, 0.3f, 0.3f, 0.9f );
                mat.ignoreDiffuseAlphaForSpecular = true;
                mat.normalMap.texture = AssetsCore.LoadAsset<Texture>(Texture2DIndex.refWaves_N);
                mat.normalStrength = 0.25f;
                mat.smoothness = 1f;
                mat.rampChoice = StandardMaterial.RampInfo.SmoothedTwoTone;
                mat.specularExponent = 1.2f;
                mat.specularStrength = 1f;
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
                new GradientColorKey( new Color( 0.05f, 0.05f, 0.05f ), 0.35f ),
                new GradientColorKey( color, 0.5f ),
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
                new GradientColorKey( Color.black, 0f ),
                new GradientColorKey( new Color( 0.5f, 0f, 0f ), 0.05f ),
                new GradientColorKey( new Color( 0.5f, 0.5f, 0f ), 0.1f ),
                new GradientColorKey( new Color( 0f, 0.5f, 0f ), 0.25f ),
                new GradientColorKey( new Color( 0f, 0.5f, 0.5f ), 0.4f ),
                new GradientColorKey( new Color( 0f, 0f, 0.5f ), 0.825f ),
                new GradientColorKey( new Color( 0.5f, 0f, 0.5f ), 0.99f ),
                new GradientColorKey( new Color( 0.1f, 0.1f, 0.1f ), 1f ),
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
            //Main.LogI( "Getting skin for: " + ind );
            if( skinLookup.ContainsKey( ind ) )
            {
                return skinLookup[ind];
            }



            var flags = (ind &          0b1111_0000_0000_0000_0000_0000_0000_0000u) >> 28;
            Boolean useCustomColor = ( (flags & 0b0001u ) >> 0 ) != 0u;
            Boolean isIridescent = ( (flags & 0b0010u ) >> 1 ) != 0u;
            //Boolean isTransparent = ( (flags & 0b0100u ) >> 2 ) != 0u;
            Boolean hasCracks = ( (flags & 0b1000u ) >> 3 ) != 0u;

            WispColorIndex color = WispColorIndex.Ancient;
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

        #region Equality
        public static Boolean operator ==( WispBitSkin obj1, WispBitSkin obj2 )
        {
            if( obj1 is null && obj2 is null ) return true;
            if( obj1 is null || obj2 is null ) return false;
            return obj1.EncodeToSkinIndex() == obj2.EncodeToSkinIndex();
        }
        public static Boolean operator !=( WispBitSkin obj1, WispBitSkin obj2 )
        {
            return !(obj1 == obj2);
        }

        #endregion
        #endregion
        internal Color mainColor { get; private set; }
        internal Gradient flameGradient { get; private set; }
        //internal Gradient armorGradient { get; private set; }
        internal Material armorMainMaterial { get; private set; }
        //internal Material armorSecondMaterial { get; private set; }
        internal Material flameMainMaterial { get; private set; }
        internal Material tracerMaterial { get; private set; }
        internal Material flamePillarMaterial { get; private set; }
        internal Material areaIndicatorMaterial { get; private set; }
        internal Material explosionMaterial { get; private set; }
        internal Material beamMaterial { get; private set; }
        internal Material distortionLightMaterial { get; private set; }
        internal Material distortionMaterial { get; private set; }
        internal Material distortionHeavyMaterial { get; private set; }
        internal Material arcaneCircleMaterial { get; private set; }
        internal Material flameTornadoMaterial { get; private set; }
        internal Material bossAreaIndicatorMaterial { get; private set; }
        internal Material bossExplosionAreaMaterial { get; private set; }

        internal BurnEffectController.EffectParams burnParams { get; private set; }





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

            var flameRampTex = TextureGenerator.GenerateRampTexture( this.flameGradient );
            //Main.debugTexture = flameRampTex;




            var flamesMain = new CloudMaterial( "FlamesMain" );
            flamesMain.sourceBlend = UnityEngine.Rendering.BlendMode.One;
            flamesMain.destinationBlend = UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha;
            flamesMain.internalSimpleBlendMode = 0f;
            flamesMain.tintColor = new Color( 1f, 1f, 1f, 1f );
            flamesMain.disableRemapping = false;
            flamesMain.mainTexture.texture = AssetsCore.LoadAsset<Texture>(Texture2DIndex.refTexGlowSoftCenterMask);
            flamesMain.remapTexture.texture = flameRampTex;
            flamesMain.softFactor = 0.5f;
            flamesMain.brightnessBoost = 1.1f;
            flamesMain.alphaBoost = 5f;
            flamesMain.alphaBias = 0f;
            flamesMain.useUV1 = false;
            flamesMain.fadeClose = true;
            flamesMain.fadeCloseDistance = 0.5f;
            flamesMain.cull = MaterialBase.CullMode.Off;
            flamesMain.cloudRemappingOn = true;
            flamesMain.cloudDistortionOn = true;
            flamesMain.distortionStrength = 0.15f;
            flamesMain.cloudTexture1.texture = AssetsCore.LoadAsset<Texture>(Texture2DIndex.refTexCloudDifferenceBW1);
            flamesMain.cloudTexture2.texture = AssetsCore.LoadAsset<Texture>(Texture2DIndex.refTexCloudIce);
            flamesMain.cutoffScrollSpeed = new Vector4( 15f, 17f, 11f, 13f );
            flamesMain.vertexColorOn = false;
            flamesMain.vertexAlphaOn = false;
            flamesMain.luminanceForTextureAlpha = false;
            flamesMain.vertexOffset = false;
            flamesMain.fresnelFade = false;
            flamesMain.fresnelPower = 0.1f;
            flamesMain.vertexOffsetAmount = 0f;
            flamesMain.externalAlpha = 1f;

            this.flameMainMaterial = flamesMain.material;




            var tracerMat = new CloudMaterial( "TracerMaterial" );
            tracerMat.sourceBlend = UnityEngine.Rendering.BlendMode.One;
            tracerMat.destinationBlend = UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha;
            tracerMat.internalSimpleBlendMode = 0f;
            tracerMat.tintColor = Color.white;
            tracerMat.disableRemapping = false;
            tracerMat.mainTexture.texture = AssetsCore.LoadAsset<Texture>(Texture2DIndex.refTexParticleDust1Mask);
            tracerMat.remapTexture.texture = flameRampTex;
            tracerMat.softFactor = 1f;
            tracerMat.brightnessBoost = 4f;
            tracerMat.alphaBoost = 5.01f;
            tracerMat.alphaBias = 0f;
            tracerMat.useUV1 = false;
            tracerMat.fadeClose = false;
            tracerMat.fadeCloseDistance = 0.5f;
            tracerMat.cull = MaterialBase.CullMode.Off;
            tracerMat.cloudRemappingOn = false;
            tracerMat.cloudDistortionOn = false;
            tracerMat.distortionStrength = 0.1f;
            tracerMat.cloudTexture1.texture = null;
            tracerMat.cloudTexture2.texture = null;
            tracerMat.cutoffScrollSpeed = Vector4.zero;
            tracerMat.vertexColorOn = true;
            tracerMat.vertexAlphaOn = false;
            tracerMat.luminanceForTextureAlpha = false;
            tracerMat.vertexOffset = false;
            tracerMat.fresnelFade = false;
            tracerMat.fresnelPower = 0f;
            tracerMat.vertexOffsetAmount = 0f;
            tracerMat.externalAlpha = 1f;

            this.tracerMaterial = tracerMat.material;




            var pillarMat = new CloudMaterial( "FlamePillar" );
            pillarMat.sourceBlend = UnityEngine.Rendering.BlendMode.SrcAlpha;
            pillarMat.destinationBlend = UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha;
            pillarMat.internalSimpleBlendMode = 1f;
            pillarMat.tintColor = Color.white;
            pillarMat.disableRemapping = false;
            pillarMat.mainTexture.texture = AssetsCore.LoadAsset<Texture>(Texture2DIndex.refTexWillowispSpiral);
            pillarMat.remapTexture.texture = flameRampTex;
            pillarMat.softFactor = 1.038f;
            pillarMat.brightnessBoost = 2f;
            pillarMat.alphaBoost = 6.57f;
            pillarMat.alphaBias = 0f;
            pillarMat.useUV1 = false;
            pillarMat.fadeClose = false;
            pillarMat.fadeCloseDistance = 0.5f;
            pillarMat.cull = MaterialBase.CullMode.Off;
            pillarMat.cloudRemappingOn = true;
            pillarMat.cloudDistortionOn = true;
            pillarMat.distortionStrength = 0.12f;
            pillarMat.cloudTexture1.texture = AssetsCore.LoadAsset<Texture>(Texture2DIndex.refTexCloudDifferenceBW2);
            pillarMat.cloudTexture2.texture = AssetsCore.LoadAsset<Texture>(Texture2DIndex.refTexCloudOrganicNormal);
            pillarMat.cutoffScrollSpeed = new Vector4( 4f, 8f, 5f, 2f );
            pillarMat.vertexColorOn = true;
            pillarMat.vertexAlphaOn = false;
            pillarMat.luminanceForTextureAlpha = false;
            pillarMat.vertexOffset = false;
            pillarMat.fresnelFade = false;
            pillarMat.fresnelPower = 0f;
            pillarMat.vertexOffsetAmount = 0f;
            pillarMat.externalAlpha = 1f;

            this.flamePillarMaterial = pillarMat.material;


            var areaMat = new IntersectionCloudMaterial( "AreaIndicatorMaterial");
            areaMat.sourceBlend = UnityEngine.Rendering.BlendMode.One;
            areaMat.destinationBlend = UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha;
            areaMat.tintColor = Color.white;
            areaMat.mainTexture.texture = null;
            areaMat.cloudTexture1.texture = AssetsCore.LoadAsset<Texture>(Texture2DIndex.refCaustics);
            areaMat.cloudTexture1.tiling = new Vector2( 0.1f, 0.1f );
            areaMat.cloudTexture2.texture = AssetsCore.LoadAsset<Texture>(Texture2DIndex.refTexArcaneCircle1Mask);
            areaMat.cloudTexture2.tiling = new Vector2( 0.075f, 0.075f );
            areaMat.remapTexture.texture = flameRampTex;
            areaMat.cutoffScrollSpeed = new Vector4( 11f, -13f, -17f, 15f );
            areaMat.softFactor = 2f;
            areaMat.softPower = 1f;
            areaMat.brightnessBoost = 5f;
            areaMat.rimPower = 0.5f;
            areaMat.rimStrength = 0.25f;
            areaMat.alphaBoost = 3f;
            areaMat.intersectionStrength = 18f;
            areaMat.cull = MaterialBase.CullMode.Off;
            areaMat.externalAlpha = 1f;
            areaMat.vertexColorsOn = false;
            areaMat.triplanarOn = true;

            this.areaIndicatorMaterial = areaMat.material;



            var explMat = new CloudMaterial( "ExplosionMaterial" );
            explMat.sourceBlend = UnityEngine.Rendering.BlendMode.SrcAlpha;
            explMat.destinationBlend = UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha;
            //explMat.internalSimpleBlendMode = 1f;
            explMat.tintColor = new Color( 1f, 1f, 1f, 1f );
            explMat.disableRemapping = false;
            explMat.mainTexture.texture = AssetsCore.LoadAsset<Texture>(Texture2DIndex.refTexGlowSoftCenterMask);
            explMat.remapTexture.texture = flameRampTex;
            explMat.softFactor = 0f;
            explMat.brightnessBoost = 1.5f;
            explMat.alphaBoost = 5f;
            explMat.alphaBias = 0f;
            explMat.useUV1 = false;
            explMat.fadeClose = false;
            explMat.fadeCloseDistance = 0.5f;
            explMat.cull = MaterialBase.CullMode.Off;
            explMat.cloudRemappingOn = true;
            explMat.cloudDistortionOn = false;
            explMat.distortionStrength = 0.15f;
            explMat.cloudTexture1.texture = AssetsCore.LoadAsset<Texture>(Texture2DIndex.refTexCloudDifferenceBW2);
            explMat.cloudTexture1.tiling = new Vector2( 3f, 3f );
            explMat.cloudTexture2.texture = AssetsCore.LoadAsset<Texture>(Texture2DIndex.refTexCloudDifferenceBW1);
            explMat.cloudTexture2.tiling = new Vector2( 4f, 4f );
            explMat.cutoffScrollSpeed = new Vector4( 15f, 17f, 11f, 13f );
            explMat.vertexColorOn = false;
            explMat.vertexAlphaOn = false;
            explMat.luminanceForTextureAlpha = false;
            explMat.vertexOffset = false;
            explMat.fresnelFade = false;
            explMat.fresnelPower = 0.1f;
            explMat.vertexOffsetAmount = 0f;
            explMat.externalAlpha = 1f;

            this.explosionMaterial = explMat.material;




            var beamMat = new CloudMaterial( "beamMaterial" );
            beamMat.sourceBlend = UnityEngine.Rendering.BlendMode.One;
            beamMat.destinationBlend = UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha;
            beamMat.internalSimpleBlendMode = 0f;
            beamMat.tintColor = Color.white;
            beamMat.disableRemapping = false;
            beamMat.mainTexture.texture = AssetsCore.LoadAsset<Texture>(Texture2DIndex.refTexAlphaGradient3Mask);
            beamMat.remapTexture.texture = flameRampTex;
            beamMat.softFactor = 1f;
            beamMat.brightnessBoost = 8f;
            beamMat.alphaBoost = 10f;
            beamMat.alphaBias = 0f;
            beamMat.useUV1 = false;
            beamMat.fadeClose = false;
            beamMat.fadeCloseDistance = 0.5f;
            beamMat.cull = MaterialBase.CullMode.Off;
            beamMat.cloudRemappingOn = true;
            beamMat.cloudDistortionOn = false;
            beamMat.distortionStrength = 0.1f;
            beamMat.cloudTexture1.texture = AssetsCore.LoadAsset<Texture>(Texture2DIndex.refTexCloudIce);
            beamMat.cloudTexture2.texture = AssetsCore.LoadAsset<Texture>(Texture2DIndex.refCaustics);
            beamMat.cloudTexture2.tiling = new Vector2( 1f, 0.25f );
            beamMat.cutoffScrollSpeed = new Vector4( -11f, 17f, 13f, -15f );
            beamMat.vertexColorOn = false;
            beamMat.vertexAlphaOn = false;
            beamMat.luminanceForTextureAlpha = false;
            beamMat.vertexOffset = false;
            beamMat.fresnelFade = false;
            beamMat.fresnelPower = 0f;
            beamMat.vertexOffsetAmount = 0f;
            beamMat.externalAlpha = 1f;

            this.beamMaterial = beamMat.material;



            var burnMat = new CloudMaterial( "BurnMaterial" );
            burnMat.sourceBlend = UnityEngine.Rendering.BlendMode.One;
            burnMat.destinationBlend = UnityEngine.Rendering.BlendMode.One;
            burnMat.internalSimpleBlendMode = 0f;
            burnMat.tintColor = Color.white;
            burnMat.disableRemapping = false;
            burnMat.mainTexture.texture = AssetsCore.LoadAsset<Texture>(Texture2DIndex.refTexCloudDifferenceBW1);
            burnMat.mainTexture.tiling = new Vector2( 12f, 12f );
            burnMat.remapTexture.texture = flameRampTex;
            burnMat.softFactor = 0f;
            burnMat.brightnessBoost = 2.98f;
            burnMat.alphaBoost = 11.51f;
            burnMat.alphaBias = 0.301f;
            burnMat.useUV1 = true;
            burnMat.fadeClose = false;
            burnMat.fadeCloseDistance = 0.5f;
            burnMat.cull = MaterialBase.CullMode.Back;
            burnMat.cloudRemappingOn = true;
            burnMat.cloudDistortionOn = false;
            burnMat.distortionStrength = 0.1f;
            burnMat.cloudTexture1.texture = AssetsCore.LoadAsset<Texture>(Texture2DIndex.refTexCloudDifferenceBW1);
            burnMat.cloudTexture1.tiling = new Vector2( 4f, 4f );
            burnMat.cloudTexture2.texture = AssetsCore.LoadAsset<Texture>(Texture2DIndex.refTexCloudDifferenceBW1);
            burnMat.cutoffScrollSpeed = new Vector4( 20f, -5f, 3f, 0f );
            burnMat.vertexColorOn = false;
            burnMat.vertexAlphaOn = false;
            burnMat.vertexOffset = false;
            burnMat.luminanceForTextureAlpha = false;
            burnMat.vertexOffsetAmount = 0f;
            burnMat.externalAlpha = 1f;

            var firePrefab = Resources.Load<GameObject>("Prefabs/HelfireEffect").ClonePrefab("FireEffect", false);
            firePrefab.transform.Find( "Point light" ).GetComponent<Light>().color = this.mainColor;
            firePrefab.transform.Find( "Point light Flash" ).GetComponent<Light>().color = this.mainColor;

            this.burnParams = new BurnEffectController.EffectParams
            {
                fireEffectPrefab = firePrefab,
                overlayMaterial = burnMat.material,
                startSound = "Play_item_proc_igniteOnKill_Loop",
                stopSound = "Stop_item_proc_igniteOnKill_Loop"
            };



            var arcCircle = new CloudMaterial( "ArcCircle" );
            arcCircle.sourceBlend = UnityEngine.Rendering.BlendMode.One;
            arcCircle.destinationBlend = UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha;
            arcCircle.internalSimpleBlendMode = 0f;
            arcCircle.tintColor = new Color( 1f, 1f, 1f, 1f );
            arcCircle.disableRemapping = false;
            arcCircle.mainTexture.texture = AssetsCore.LoadAsset<Texture>(Texture2DIndex.refTexArcaneCircle1Mask);
            arcCircle.remapTexture.texture = flameRampTex;
            arcCircle.softFactor = 0f;
            arcCircle.brightnessBoost = 3f;
            arcCircle.alphaBoost = 5f;
            arcCircle.alphaBias = 0f;
            arcCircle.useUV1 = false;
            arcCircle.fadeClose = true;
            arcCircle.fadeCloseDistance = 0.5f;
            arcCircle.cull = MaterialBase.CullMode.Off;
            arcCircle.cloudRemappingOn = true;
            arcCircle.cloudDistortionOn = false;
            arcCircle.distortionStrength = 0.15f;
            arcCircle.cloudTexture1.texture = null;
            arcCircle.cloudTexture2.texture = null;
            arcCircle.cutoffScrollSpeed = Vector4.zero;
            arcCircle.vertexColorOn = false;
            arcCircle.vertexAlphaOn = false;
            arcCircle.luminanceForTextureAlpha = false;
            arcCircle.vertexOffset = false;
            arcCircle.fresnelFade = false;
            arcCircle.fresnelPower = 0.1f;
            arcCircle.vertexOffsetAmount = 0f;
            arcCircle.externalAlpha = 1f;

            this.arcaneCircleMaterial = arcCircle.material;









            var flameTornadoMat = new CloudMaterial( "flameTornadoMat" );
            flameTornadoMat.sourceBlend = UnityEngine.Rendering.BlendMode.One;
            flameTornadoMat.destinationBlend = UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha;
            flameTornadoMat.internalSimpleBlendMode = 0f;
            flameTornadoMat.tintColor = new Color( 1f, 1f, 1f, 1f );
            flameTornadoMat.disableRemapping = false;
            flameTornadoMat.mainTexture.texture = null;
            flameTornadoMat.remapTexture.texture = flameRampTex;
            flameTornadoMat.softFactor = 0.299f;
            flameTornadoMat.brightnessBoost = 1f;
            flameTornadoMat.alphaBoost = 2.16f;
            flameTornadoMat.alphaBias = 0.207f;
            flameTornadoMat.useUV1 = false;
            flameTornadoMat.fadeClose = true;
            flameTornadoMat.fadeCloseDistance = 0.5f;
            flameTornadoMat.cull = MaterialBase.CullMode.Off;
            flameTornadoMat.cloudRemappingOn = true;
            flameTornadoMat.cloudDistortionOn = false;
            flameTornadoMat.distortionStrength = 0.15f;
            flameTornadoMat.cloudTexture1.texture = AssetsCore.LoadAsset<Texture>(Texture2DIndex.refTexCloudDifferenceBW2);
            flameTornadoMat.cloudTexture1.tiling = new Vector2( 6f, 6f );
            flameTornadoMat.cloudTexture2.texture = AssetsCore.LoadAsset<Texture>(Texture2DIndex.refTexBehemothTileMask);
            flameTornadoMat.cloudTexture2.tiling = new Vector2( 1f, 6f );
            flameTornadoMat.cutoffScrollSpeed = new Vector4( 12f, -20f, 3f, -40f );
            flameTornadoMat.vertexColorOn = false;
            flameTornadoMat.vertexAlphaOn = true;
            flameTornadoMat.luminanceForTextureAlpha = false;
            flameTornadoMat.vertexOffset = false;
            flameTornadoMat.fresnelFade = false;
            flameTornadoMat.fresnelPower = 0.1f;
            flameTornadoMat.vertexOffsetAmount = 0f;
            flameTornadoMat.externalAlpha = 1f;

            this.flameTornadoMaterial = flameTornadoMat.material;






            var bossAreaMat = new IntersectionCloudMaterial( "AreaIndicatorMaterial");
            bossAreaMat.sourceBlend = UnityEngine.Rendering.BlendMode.One;
            bossAreaMat.destinationBlend = UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha;
            bossAreaMat.tintColor = Color.white;
            bossAreaMat.mainTexture.texture = null;
            bossAreaMat.cloudTexture1.texture = AssetsCore.LoadAsset<Texture>(Texture2DIndex.refCaustics);
            bossAreaMat.cloudTexture1.tiling = new Vector2( 0.1f, 0.1f );
            bossAreaMat.cloudTexture2.texture = AssetsCore.LoadAsset<Texture>(Texture2DIndex.refCaustics);
            bossAreaMat.cloudTexture2.tiling = new Vector2( 0.05f, 0.05f );
            bossAreaMat.remapTexture.texture = flameRampTex;
            bossAreaMat.cutoffScrollSpeed = new Vector4( 11f, -13f, -17f, 15f );
            bossAreaMat.softFactor = 1f;
            bossAreaMat.softPower = 5f;
            bossAreaMat.brightnessBoost = 3f;
            bossAreaMat.rimPower = 3f;
            bossAreaMat.rimStrength = 1.5f;
            bossAreaMat.alphaBoost = 2.5f;
            bossAreaMat.intersectionStrength = 30f;
            bossAreaMat.cull = MaterialBase.CullMode.Off;
            bossAreaMat.externalAlpha = 1f;
            bossAreaMat.vertexColorsOn = false;
            bossAreaMat.triplanarOn = true;

            this.bossAreaIndicatorMaterial = bossAreaMat.material;



            var bossExplosionAreaMat = new CloudMaterial( "bossExplosionAreaMat" );
            bossExplosionAreaMat.sourceBlend = UnityEngine.Rendering.BlendMode.One;
            bossExplosionAreaMat.destinationBlend = UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha;
            bossExplosionAreaMat.internalSimpleBlendMode = 0f;
            bossExplosionAreaMat.tintColor = new Color( 1f, 1f, 1f, 1f );
            bossExplosionAreaMat.disableRemapping = false;
            bossExplosionAreaMat.mainTexture.texture = null;
            bossExplosionAreaMat.remapTexture.texture = flameRampTex;
            bossExplosionAreaMat.softFactor = 0.299f;
            bossExplosionAreaMat.brightnessBoost = 1f;
            bossExplosionAreaMat.alphaBoost = 2.16f;
            bossExplosionAreaMat.alphaBias = 0.207f;
            bossExplosionAreaMat.useUV1 = false;
            bossExplosionAreaMat.fadeClose = true;
            bossExplosionAreaMat.fadeCloseDistance = 0.5f;
            bossExplosionAreaMat.cull = MaterialBase.CullMode.Off;
            bossExplosionAreaMat.cloudRemappingOn = true;
            bossExplosionAreaMat.cloudDistortionOn = false;
            bossExplosionAreaMat.distortionStrength = 0.15f;
            bossExplosionAreaMat.cloudTexture1.texture = AssetsCore.LoadAsset<Texture>(Texture2DIndex.refTexCloudDifferenceBW2);
            bossExplosionAreaMat.cloudTexture1.tiling = new Vector2( 6f, 6f );
            bossExplosionAreaMat.cloudTexture2.texture = AssetsCore.LoadAsset<Texture>(Texture2DIndex.refTexCloudOrganic1);
            bossExplosionAreaMat.cloudTexture2.tiling = new Vector2( 1f, 6f );
            bossExplosionAreaMat.cutoffScrollSpeed = new Vector4( 12f, -20f, 3f, -40f );
            bossExplosionAreaMat.vertexColorOn = false;
            bossExplosionAreaMat.vertexAlphaOn = true;
            bossExplosionAreaMat.luminanceForTextureAlpha = false;
            bossExplosionAreaMat.vertexOffset = false;
            bossExplosionAreaMat.fresnelFade = false;
            bossExplosionAreaMat.fresnelPower = 0.1f;
            bossExplosionAreaMat.vertexOffsetAmount = 0f;
            bossExplosionAreaMat.externalAlpha = 1f;

            this.bossExplosionAreaMaterial = bossExplosionAreaMat.material;














            var distortL = new DistortionMaterial( "DistortionMat" );
            distortL.bumpTexture.texture = AssetsCore.LoadAsset<Texture>(Texture2DIndex.refTexNormalSphereFaded);
            distortL.mainTexture.texture = null;
            distortL.magnitude = 0.65f;
            var distLr = this.mainColor.r;
            var distLg = this.mainColor.g;
            var distLb = this.mainColor.b;
            var distLa = 0.25f;
            distortL.color = new Color( distLr, distLg, distLb, distLa );


            this.distortionLightMaterial = distortL.material;

            var distort = new DistortionMaterial( "DistortionMat" );
            distort.bumpTexture.texture = AssetsCore.LoadAsset<Texture>(Texture2DIndex.refTexNormalSphere);
            distort.mainTexture.texture = null;
            distort.magnitude = 1.2f;
            var distr = this.mainColor.r;
            var distg = this.mainColor.g;
            var distb = this.mainColor.b;
            var dista = 0.25f;
            distort.color = new Color( distr, distg, distb, dista );


            this.distortionMaterial = distort.material;

            var distortH = new DistortionMaterial( "DistortionMat" );
            distortH.bumpTexture.texture = AssetsCore.LoadAsset<Texture>(Texture2DIndex.refTexNormalSphere);
            distortH.mainTexture.texture = null;
            distortH.magnitude = 4f;
            var distHr = this.mainColor.r;
            var distHg = this.mainColor.g;
            var distHb = this.mainColor.b;
            var distHa = 0.25f;
            distortH.color = new Color( distHr, distHg, distHb, distHa );


            this.distortionHeavyMaterial = distortH.material;



            var main = new StandardMaterial( "ArmorMain" );

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
            main.decalLayer = StandardMaterial.DecalLayer.Default;
            main.specularStrength = 0f;
            main.specularExponent = 1f;
            main.cull = MaterialBase.CullMode.Back;
            main.dither = false;
            main.fadeBias = 0f;
            main.fresnelEmission = true;
            main.fresnelRamp.texture = TextureGenerator.GenerateRampTexture( CreateFEGradient( this.mainColor ) );
            main.fresnelPower = 0.2f;
            main.fresnelMask.texture = null;
            main.fresnelBoost = 20f;
            main.printingEnabled = false;
            main.splatmapEnabled = false;
            main.flowmapEnabled = false;
            main.flowmapTexture.texture = AssetsCore.LoadAsset<Texture>(Texture2DIndex.refCaustics);
            main.flowmapHeightmap.texture = AssetsCore.LoadAsset<Texture>(Texture2DIndex.refCaustics);
            main.flowmapHeightRamp.texture = TextureGenerator.GenerateRampTexture( CreateFlowGradient( this.mainColor ) );
            main.flowHeightBias = 0.3f;
            main.flowHeightPower = 1f;
            main.flowHeightEmissionStrength = 1f;
            main.flowSpeed = 1f;
            main.flowMaskStrength = 0f;
            main.flowNormalStrength = 2f;
            main.flowTextureScaleFactor = 1f;

            main.limbRemovalEnabled = false;
            main.limbPrimeMask = 1f;
            main.flashColor = Color.clear;

            if( this.hasCracks )
            {
                main.flowmapEnabled = true;
            } else
            {
                main.flowmapEnabled = false;
            }

            armorMaterials[(UInt32)this.armorMaterialType]( this.mainColor, main );


            this.armorMainMaterial = main.material;

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
                if( (UInt32)this.color >= 16u )
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
            if( (UInt32)this.flameGradientType >= 8u )
            {
                throw new IndexOutOfRangeException( "Too large a value for flameGradientType" );
            }

            var flag = ((UInt32)this.flameGradientType) << 18;
            this.encodeValue |= flag;
        }
        private void UpdateEncodedArmorMaterialType()
        {
            this.encodeValue &= 0b1111_0001_1111_1111_1111_1111_1111_1111u;
            if( (UInt32)this.armorMaterialType >= 8u )
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
            Aquatic = 9u,
            Faded = 10u,
            Blood = 11u,
            Midnight = 12u,
            Forest = 13u,
        }
        internal enum FlameGradientType : UInt32
        {
            Standard = 0u,
            BlackOut = 1u,
            WhiteOut = 2u,
            BlackIn = 3u,
            WhiteIn = 4u,
        }
        internal enum ArmorMaterialType : UInt32
        {
            Standard = 0u,
            Bone = 1u,
            Metal = 2u,
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