﻿using RoR2;
using UnityEngine;
using System;
using System.Reflection;
using System.Collections.Generic;
using static WispSurvivor.Util.PrefabUtilities;
using static WispSurvivor.Helpers.ParticleUtils;

namespace WispSurvivor.Modules
{
    public static class WispEffectModule
    {
        public static GameObject[][] genericImpactEffects = new GameObject[8][];

        public static GameObject[] primaryOrbEffects = new GameObject[8];
        public static GameObject[] primaryExplosionEffects = new GameObject[8];

        public static GameObject[] secondaryExplosions = new GameObject[8];

        public static GameObject[] utilityFlames = new GameObject[8];
        public static GameObject[] utilityBurns = new GameObject[8];
        public static GameObject[] utilityLeech = new GameObject[8];
        public static GameObject[] utilityAim = new GameObject[8];

        public static GameObject[] specialCharge = new GameObject[8];
        public static GameObject[] specialExplosion = new GameObject[8];

        public static void DoModule(GameObject body, Dictionary<Type, Component> dic)
        {
            CreateGenericImpactEffects();
            CreatePrimaryOrbEffects();
            CreatePrimaryExplosionEffects();
            CreateSecondaryExplosionEffect();
            CreateBlazeOrbEffects();
            CreateIgnitionOrbEffects();
            CreateLeechOrbEffects();
            CreateUtilityAimEffects();
            CreateSpecialExplosionEffects();
            CreateSpecialChargeEffects();
        }

        public static void Register()
        {
            foreach (GameObject[] gs in genericImpactEffects)
            {
                foreach (GameObject g in gs)
                {
                    RegisterNewEffect(g);
                }
            }
            foreach (GameObject g in primaryOrbEffects)
            {
                RegisterNewEffect(g);
            }
            foreach (GameObject g in primaryExplosionEffects)
            {
                RegisterNewEffect(g);
            }
            foreach (GameObject g in secondaryExplosions)
            {
                RegisterNewEffect(g);
            }
            foreach (GameObject g in utilityFlames)
            {
                RegisterNewEffect(g);
            }
            foreach (GameObject g in utilityBurns)
            {
                RegisterNewEffect(g);
            }
            foreach (GameObject g in utilityLeech)
            {
                RegisterNewEffect(g);
            }
            foreach (GameObject g in specialCharge)
            {
                RegisterNewEffect(g);
            }
            foreach (GameObject g in specialExplosion)
            {
                RegisterNewEffect(g);
            }
        }
        #region Generic Impact Effects
        private static void CreateGenericImpactEffects()
        {
            GameObject[] bases = new GameObject[1];
            bases[0] = Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/ImpactWispEmber");

            for (int i = 0; i < 8; i++)
            {
                genericImpactEffects[i] = CreateImpFx(bases, i);
            }

        }

        private static GameObject[] CreateImpFx(GameObject[] bases, int skinIndex)
        {
            //Material mat = WispMaterialModule.fireMaterials[skinIndex][0];
            //Color col = WispMaterialModule.fireColors[skinIndex];

            GameObject[] effects = new GameObject[bases.Length];

            effects[0] = CreateImpFx00(bases[0], skinIndex);

            return effects;
        }

        private static GameObject CreateImpFx00(GameObject baseObj, int skinIndex)
        {
            const int matIndex = 0;
            GameObject obj = baseObj.InstantiateClone("WispImpact0-" + skinIndex.ToString());
         

            GameObject flameObj = obj.transform.Find("Flames").gameObject;
            GameObject flashObj = obj.transform.Find("Flash").gameObject;

            ParticleSystem flamePS = flameObj.GetComponent<ParticleSystem>();
            ParticleSystemRenderer flamePSR = flameObj.GetComponent<ParticleSystemRenderer>();

            ParticleSystem flashPS = flashObj.GetComponent<ParticleSystem>();
            ParticleSystemRenderer flashPSR = flashObj.GetComponent<ParticleSystemRenderer>();

            
            var flameCOL = flamePS.colorOverLifetime;
            ParticleSystem.MinMaxGradient flameColMMGrad = new ParticleSystem.MinMaxGradient();
            flameColMMGrad.mode = ParticleSystemGradientMode.Gradient;
            flameColMMGrad.gradient = WispMaterialModule.fireGradients[skinIndex];
            flameCOL.color = flameColMMGrad;
            flamePSR.material = WispMaterialModule.fireMaterials[skinIndex][matIndex];

            //Gradient flameColGrad = new Gradient();
            //GradientColorKey[] flameColCols = new GradientColorKey[3];
            //flameColCols[0] = new GradientColorKey(new Color(5f, 5f, 5f), 0f);
            //flameColCols[1] = new GradientColorKey(new Color(1f, 1f, 1f), 0.1f);
            //flameColCols[2] = new GradientColorKey(new Color(0.6f, 0.6f, 0.6f), 1f);
            //flameColGrad.SetKeys(flameColCols, flameCOL.color.gradient.alphaKeys);
            //flameColMMGrad.gradient = flameColGrad;


            var flashCOL = flashPS.colorOverLifetime;
            ParticleSystem.MinMaxGradient flashColMMGrad = new ParticleSystem.MinMaxGradient();
            flashColMMGrad.mode = ParticleSystemGradientMode.Gradient;
            flashColMMGrad.gradient = WispMaterialModule.fireGradients[skinIndex];
            flashCOL.color = flashColMMGrad;
            flashPSR.material =WispMaterialModule.fireMaterials[skinIndex][matIndex];

            //Gradient flashColGrad = new Gradient();
            //GradientColorKey[] flashColCols = new GradientColorKey[3];
            //flashColCols[0] = new GradientColorKey(new Color(5f, 5f, 5f), 0f);
            //flashColCols[1] = new GradientColorKey(new Color(1f, 1f, 1f), 0.1f);
            //flashColCols[2] = new GradientColorKey(new Color(0.6f, 0.6f, 0.6f), 1f);
            //flashColGrad.SetKeys(flashColCols, flashCOL.color.gradient.alphaKeys);


            return obj;
        }
        #endregion
        #region Primary Orb
        private static void CreatePrimaryOrbEffects()
        {
            GameObject baseFX = Resources.Load<GameObject>("Prefabs/Effects/OrbEffects/WispOrbEffect");

            for (int i = 0; i < 8; i++)
            {
                primaryOrbEffects[i] = CreatePrimaryOrb(baseFX, i);
            }
        }

        private static GameObject CreatePrimaryOrb(GameObject baseFX, int skinIndex)
        {
            GameObject obj = baseFX.InstantiateClone("PrimaryOrb"+skinIndex.ToString());

            MonoBehaviour.DestroyImmediate(obj.GetComponent<RoR2.Orbs.OrbEffect>());

            var orbController = obj.AddComponent<Components.WispOrbEffectController>();
            orbController.startSound = "Play_gravekeeper_attack1_fire";
            orbController.endSound = "Play_lemurianBruiser_m1_explode";

            foreach(AkEvent ev in obj.GetComponents<AkEvent>())
            {
                MonoBehaviour.Destroy(ev);
            }


            Material flameMat = WispMaterialModule.fireMaterials[skinIndex][0];
            Color flameCol = WispMaterialModule.fireColors[skinIndex];

            Material distortion = Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/LightningStrikeImpact").transform.Find("Distortion").GetComponent<ParticleSystemRenderer>().material;

            GameObject parts1 = obj.transform.Find("Mesh").gameObject;
            GameObject parts2 = obj.transform.Find("Flames").gameObject;
            GameObject parts3 = new GameObject("Ball");
            parts3.transform.parent = obj.transform;
            parts3.transform.localPosition = Vector3.zero;
            parts3.transform.localScale = Vector3.one;


            parts1.name = "FlameParticles";
            parts2.name = "DistortionParticles";

            parts1.Strip();
            parts2.Strip();

            ParticleSystem ps1 = parts1.AddComponent<ParticleSystem>();
            ParticleSystemRenderer psr1 = parts1.AddOrGetComponent<ParticleSystemRenderer>();
            #region Particle System definitions 1
            BasicSetup(ps1);

            var ps1Main = ps1.main;
            ps1Main.duration = 1f;
            ps1Main.loop = true;
            ps1Main.prewarm = false;
            ps1Main.startDelay = 0f;
            ps1Main.startLifetime = 1f;
            ps1Main.startSpeed = 10f;
            ps1Main.startSize = 1.25f;
            ps1Main.startRotation = 0f;
            ps1Main.flipRotation = 0f;
            ps1Main.gravityModifier = 0f;
            ps1Main.simulationSpace = ParticleSystemSimulationSpace.World;
            ps1Main.simulationSpeed = 1f;
            ps1Main.useUnscaledTime = false;
            ps1Main.scalingMode = ParticleSystemScalingMode.Hierarchy;
            ps1Main.playOnAwake = true;
            ps1Main.emitterVelocityMode = ParticleSystemEmitterVelocityMode.Rigidbody;
            ps1Main.maxParticles = 1000;
            ps1Main.stopAction = ParticleSystemStopAction.None;
            ps1Main.cullingMode = ParticleSystemCullingMode.Automatic;
            ps1Main.ringBufferMode = ParticleSystemRingBufferMode.Disabled;

            var ps1Emis = ps1.emission;
            ps1Emis.enabled = true;
            ps1Emis.rateOverTime = new ParticleSystem.MinMaxCurve
            {
                mode = ParticleSystemCurveMode.Constant,
                constant = 0f
            };
            ps1Emis.rateOverDistance = new ParticleSystem.MinMaxCurve
            {
                mode = ParticleSystemCurveMode.Constant,
                constant = 1f
            };

            var ps1Shape = ps1.shape;
            ps1Shape.enabled = true;
            ps1Shape.shapeType = ParticleSystemShapeType.Donut;
            ps1Shape.radius = 5f;
            ps1Shape.donutRadius = 3f;
            ps1Shape.radiusThickness = 1f;
            ps1Shape.arc = 360f;
            ps1Shape.arcMode = ParticleSystemShapeMultiModeValue.Random;
            ps1Shape.arcSpread = 0f;
            ps1Shape.position = Vector3.zero;
            ps1Shape.rotation = new Vector3(0f, 90f, 0f);
            ps1Shape.scale = new Vector3(1f, 1f, 1f);
            ps1Shape.alignToDirection = false;
            ps1Shape.randomDirectionAmount = 0f;
            ps1Shape.sphericalDirectionAmount = 0f;
            ps1Shape.randomPositionAmount = 0f;

            var ps1COL = ps1.colorOverLifetime;
            ps1COL.enabled = true;
            ps1COL.color = new ParticleSystem.MinMaxGradient
            {
                mode = ParticleSystemGradientMode.Gradient,
                gradient = new Gradient
                {
                    alphaKeys = new GradientAlphaKey[3]
                    {
                        new GradientAlphaKey
                        {
                            time = 0f,
                            alpha = 0f
                        },
                        new GradientAlphaKey
                        {
                            time = 0.075f,
                            alpha = 1f
                        },
                        new GradientAlphaKey
                        {
                            time = 1f,
                            alpha = 0f
                        }
                    },
                    colorKeys = new GradientColorKey[3]
                    {
                        new GradientColorKey
                        {
                            time = 0f,
                            color = new Color
                            {
                                r = 5f,
                                g = 5f,
                                b = 5f
                            }
                        },
                        new GradientColorKey
                        {
                            time = 0.08f,
                            color = new Color
                            {
                                r = 1f,
                                g = 1f,
                                b = 1f
                            }
                        },
                        new GradientColorKey
                        {
                            time = 1f,
                            color = new Color
                            {
                                r = 0.7f,
                                g = 0.7f,
                                b = 0.7f
                            }
                        }
                    }
                }
            };

            var ps1SOL = ps1.sizeOverLifetime;
            ps1SOL.enabled = true;
            ps1SOL.size = new ParticleSystem.MinMaxCurve
            {
                mode = ParticleSystemCurveMode.Curve,
                curve = new AnimationCurve
                {
                    postWrapMode = WrapMode.Clamp,
                    preWrapMode = WrapMode.Clamp,
                    keys = new Keyframe[3]
                    {
                        new Keyframe
                        {
                            time = 0f,
                            value = 0.42f,
                            outTangent = 0.5f,
                            outWeight = 0.5f
                        },
                        new Keyframe
                        {
                            time = 0.4f,
                            value = 0.5f,
                            inTangent = 0.5f,
                            inWeight = 0.5f,
                            outTangent = 0.5f,
                            outWeight = 0.5f
                        },
                        new Keyframe
                        {
                            time = 1f,
                            value = 0f,
                            inTangent = 0.5f,
                            inWeight = 0.5f
                        }
                    }
                }
            };
            ps1SOL.separateAxes = false;
            ps1SOL.sizeMultiplier = 1f;

            var ps1ROL = ps1.rotationOverLifetime;
            ps1ROL.enabled = true;
            ps1ROL.x = new ParticleSystem.MinMaxCurve
            {
                mode = ParticleSystemCurveMode.Constant,
                constant = 89.99f
            };
            ps1ROL.y = new ParticleSystem.MinMaxCurve
            {
                mode = ParticleSystemCurveMode.Constant,
                constant = 89.99f
            };
            ps1ROL.z = new ParticleSystem.MinMaxCurve
            {
                mode = ParticleSystemCurveMode.Constant,
                constant = 89.99f
            };

            psr1.renderMode = ParticleSystemRenderMode.Billboard;
            psr1.normalDirection = 1f;
            psr1.material = flameMat;
            psr1.sortMode = ParticleSystemSortMode.None;
            psr1.sortingFudge = 0f;
            psr1.minParticleSize = 0f;
            psr1.maxParticleSize = 0.5f;
            psr1.alignment = ParticleSystemRenderSpace.View;
            psr1.allowRoll = true;
            psr1.maskInteraction = SpriteMaskInteraction.None;
            psr1.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            psr1.receiveShadows = false;
            psr1.shadowBias = 0f;
            psr1.motionVectorGenerationMode = MotionVectorGenerationMode.Object;
            psr1.sortingLayerID = LayerIndex.defaultLayer.intVal;
            psr1.sortingOrder = 0;
            psr1.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.BlendProbes;
            psr1.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
            #endregion


            ParticleSystem ps2 = parts2.AddComponent<ParticleSystem>();
            ParticleSystemRenderer psr2 = parts2.AddOrGetComponent<ParticleSystemRenderer>();

            #region Particle System definitions 2
            BasicSetup(ps2);

            var ps2Main = ps2.main;
            ps2Main.duration = 1f;
            ps2Main.loop = true;
            ps2Main.prewarm = false;
            ps2Main.startDelay = 0f;
            ps2Main.startLifetime = 1f;
            ps2Main.startSpeed = 0f;
            ps2Main.startSize = 1.25f;
            ps2Main.startRotation = 0f;
            ps2Main.flipRotation = 0.5f;
            ps2Main.gravityModifier = 0f;
            ps2Main.simulationSpace = ParticleSystemSimulationSpace.World;
            ps2Main.simulationSpeed = 1f;
            ps2Main.useUnscaledTime = false;
            ps2Main.scalingMode = ParticleSystemScalingMode.Hierarchy;
            ps2Main.playOnAwake = true;
            ps2Main.emitterVelocityMode = ParticleSystemEmitterVelocityMode.Rigidbody;
            ps2Main.maxParticles = 1000;
            ps2Main.stopAction = ParticleSystemStopAction.None;
            ps2Main.cullingMode = ParticleSystemCullingMode.Automatic;
            ps2Main.ringBufferMode = ParticleSystemRingBufferMode.Disabled;

            var ps2Emis = ps2.emission;
            ps2Emis.enabled = true;
            ps2Emis.rateOverTime = new ParticleSystem.MinMaxCurve
            {
                mode = ParticleSystemCurveMode.Constant,
                constant = 0f
            };
            ps2Emis.rateOverDistance = new ParticleSystem.MinMaxCurve
            {
                mode = ParticleSystemCurveMode.Constant,
                constant = 0.25f
            };

            var ps2Shape = ps2.shape;
            ps2Shape.enabled = true;
            ps2Shape.shapeType = ParticleSystemShapeType.BoxEdge;
            ps2Shape.radius = 0.5f;
            ps2Shape.position = Vector3.zero;
            ps2Shape.rotation = new Vector3(0f, 0f, 0f);
            ps2Shape.scale = new Vector3(1f, 1f, 1f);
            ps2Shape.alignToDirection = false;
            ps2Shape.randomDirectionAmount = 0f;
            ps2Shape.sphericalDirectionAmount = 0f;
            ps2Shape.randomPositionAmount = 0f;

            var ps2COL = ps2.colorOverLifetime;
            ps2COL.enabled = false;
            ps2COL.color = new ParticleSystem.MinMaxGradient
            {
                mode = ParticleSystemGradientMode.Gradient,
                gradient = new Gradient
                {
                    alphaKeys = new GradientAlphaKey[3]
                    {
                        new GradientAlphaKey
                        {
                            time = 0f,
                            alpha = 0f
                        },
                        new GradientAlphaKey
                        {
                            time = 0.075f,
                            alpha = 1f
                        },
                        new GradientAlphaKey
                        {
                            time = 1f,
                            alpha = 0f
                        }
                    },
                    colorKeys = new GradientColorKey[3]
                    {
                        new GradientColorKey
                        {
                            time = 0f,
                            color = new Color
                            {
                                r = 5f,
                                g = 5f,
                                b = 5f
                            }
                        },
                        new GradientColorKey
                        {
                            time = 0.08f,
                            color = new Color
                            {
                                r = 1f,
                                g = 1f,
                                b = 1f
                            }
                        },
                        new GradientColorKey
                        {
                            time = 1f,
                            color = new Color
                            {
                                r = 0.7f,
                                g = 0.7f,
                                b = 0.7f
                            }
                        }
                    }
                }
            };

            var ps2SOL = ps2.sizeOverLifetime;
            ps2SOL.enabled = true;
            ps2SOL.size = new ParticleSystem.MinMaxCurve
            {
                mode = ParticleSystemCurveMode.Curve,
                curve = new AnimationCurve
                {
                    postWrapMode = WrapMode.Clamp,
                    preWrapMode = WrapMode.Clamp,
                    keys = new Keyframe[3]
                    {
                        new Keyframe
                        {
                            time = 0f,
                            value = 0.42f,
                            outTangent = 0.5f,
                            outWeight = 0.5f
                        },
                        new Keyframe
                        {
                            time = 0.4f,
                            value = 0.5f,
                            inTangent = 0.5f,
                            inWeight = 0.5f,
                            outTangent = 0.5f,
                            outWeight = 0.5f
                        },
                        new Keyframe
                        {
                            time = 1f,
                            value = 0f,
                            inTangent = 0.5f,
                            inWeight = 0.5f
                        }
                    }
                }
            };
            ps2SOL.separateAxes = false;
            ps2SOL.sizeMultiplier = 4f;

            var ps2ROL = ps2.rotationOverLifetime;
            ps2ROL.enabled = true;
            ps2ROL.x = new ParticleSystem.MinMaxCurve
            {
                mode = ParticleSystemCurveMode.Constant,
                constant = 2f
            };
            ps2ROL.y = new ParticleSystem.MinMaxCurve
            {
                mode = ParticleSystemCurveMode.Constant,
                constant = 2f
            };
            ps2ROL.z = new ParticleSystem.MinMaxCurve
            {
                mode = ParticleSystemCurveMode.Constant,
                constant = 2f
            };

            psr2.renderMode = ParticleSystemRenderMode.Billboard;
            psr2.normalDirection = 1f;
            psr2.material = distortion;
            psr2.sortMode = ParticleSystemSortMode.None;
            psr2.sortingFudge = 0f;
            psr2.minParticleSize = 0f;
            psr2.maxParticleSize = 0.5f;
            psr2.alignment = ParticleSystemRenderSpace.View;
            psr2.allowRoll = true;
            psr2.maskInteraction = SpriteMaskInteraction.None;
            psr2.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            psr2.receiveShadows = false;
            psr2.shadowBias = 0f;
            psr2.motionVectorGenerationMode = MotionVectorGenerationMode.Object;
            psr2.sortingLayerID = LayerIndex.defaultLayer.intVal;
            psr2.sortingOrder = 0;
            psr2.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.BlendProbes;
            psr2.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
            #endregion

            ParticleSystem ps3 = parts3.AddComponent<ParticleSystem>();
            ParticleSystemRenderer psr3 = parts3.AddOrGetComponent<ParticleSystemRenderer>();

            #region Particle System definitions 3
            BasicSetup(ps3);

            var ps3Main = ps3.main;
            ps3Main.duration = 1f;
            ps3Main.loop = true;
            ps3Main.prewarm = false;
            ps3Main.startDelay = 0f;
            ps3Main.startLifetime = 1f;
            ps3Main.startSpeed = 0f;
            ps3Main.startSize = 1f;
            ps3Main.startRotation = 0f;
            ps3Main.flipRotation = 0.5f;
            ps3Main.gravityModifier = 0f;
            ps3Main.simulationSpace = ParticleSystemSimulationSpace.World;
            ps3Main.simulationSpeed = 1f;
            ps3Main.useUnscaledTime = false;
            ps3Main.scalingMode = ParticleSystemScalingMode.Hierarchy;
            ps3Main.playOnAwake = true;
            ps3Main.emitterVelocityMode = ParticleSystemEmitterVelocityMode.Rigidbody;
            ps3Main.maxParticles = 1000;
            ps3Main.stopAction = ParticleSystemStopAction.None;
            ps3Main.cullingMode = ParticleSystemCullingMode.Automatic;
            ps3Main.ringBufferMode = ParticleSystemRingBufferMode.Disabled;

            var ps3Emis = ps3.emission;
            ps3Emis.enabled = true;
            ps3Emis.rateOverTime = new ParticleSystem.MinMaxCurve
            {
                mode = ParticleSystemCurveMode.Constant,
                constant = 0f
            };
            ps3Emis.rateOverDistance = new ParticleSystem.MinMaxCurve
            {
                mode = ParticleSystemCurveMode.Constant,
                constant = 0.25f
            };

            var ps3Shape = ps3.shape;
            ps3Shape.enabled = true;
            ps3Shape.shapeType = ParticleSystemShapeType.BoxEdge;
            ps3Shape.radius = 0.5f;
            ps3Shape.position = Vector3.zero;
            ps3Shape.rotation = new Vector3(0f, 0f, 0f);
            ps3Shape.scale = new Vector3(1f, 1f, 1f);
            ps3Shape.alignToDirection = false;
            ps3Shape.randomDirectionAmount = 0f;
            ps3Shape.sphericalDirectionAmount = 0f;
            ps3Shape.randomPositionAmount = 0f;

            var ps3COL = ps3.colorOverLifetime;
            ps3COL.enabled = false;
            ps3COL.color = new ParticleSystem.MinMaxGradient
            {
                mode = ParticleSystemGradientMode.Gradient,
                gradient = new Gradient
                {
                    alphaKeys = new GradientAlphaKey[3]
                    {
                        new GradientAlphaKey
                        {
                            time = 0f,
                            alpha = 0f
                        },
                        new GradientAlphaKey
                        {
                            time = 0.075f,
                            alpha = 1f
                        },
                        new GradientAlphaKey
                        {
                            time = 1f,
                            alpha = 0f
                        }
                    },
                    colorKeys = new GradientColorKey[3]
                    {
                        new GradientColorKey
                        {
                            time = 0f,
                            color = new Color
                            {
                                r = 5f,
                                g = 5f,
                                b = 5f
                            }
                        },
                        new GradientColorKey
                        {
                            time = 0.08f,
                            color = new Color
                            {
                                r = 1f,
                                g = 1f,
                                b = 1f
                            }
                        },
                        new GradientColorKey
                        {
                            time = 1f,
                            color = new Color
                            {
                                r = 0.7f,
                                g = 0.7f,
                                b = 0.7f
                            }
                        }
                    }
                }
            };

            var ps3SOL = ps3.sizeOverLifetime;
            ps3SOL.enabled = true;
            ps3SOL.size = new ParticleSystem.MinMaxCurve
            {
                mode = ParticleSystemCurveMode.Curve,
                curve = new AnimationCurve
                {
                    postWrapMode = WrapMode.Clamp,
                    preWrapMode = WrapMode.Clamp,
                    keys = new Keyframe[3]
                    {
                        new Keyframe
                        {
                            time = 0f,
                            value = 0.42f,
                            outTangent = 0.5f,
                            outWeight = 0.5f
                        },
                        new Keyframe
                        {
                            time = 0.4f,
                            value = 0.5f,
                            inTangent = 0.5f,
                            inWeight = 0.5f,
                            outTangent = 0.5f,
                            outWeight = 0.5f
                        },
                        new Keyframe
                        {
                            time = 1f,
                            value = 0f,
                            inTangent = 0.5f,
                            inWeight = 0.5f
                        }
                    }
                }
            };
            ps3SOL.separateAxes = false;
            ps3SOL.sizeMultiplier = 4f;

            var ps3ROL = ps3.rotationOverLifetime;
            ps3ROL.enabled = true;
            ps3ROL.x = new ParticleSystem.MinMaxCurve
            {
                mode = ParticleSystemCurveMode.Constant,
                constant = 2f
            };
            ps3ROL.y = new ParticleSystem.MinMaxCurve
            {
                mode = ParticleSystemCurveMode.Constant,
                constant = 2f
            };
            ps3ROL.z = new ParticleSystem.MinMaxCurve
            {
                mode = ParticleSystemCurveMode.Constant,
                constant = 2f
            };

            psr3.renderMode = ParticleSystemRenderMode.Billboard;
            psr3.normalDirection = 1f;
            psr3.material = flameMat;
            psr3.sortMode = ParticleSystemSortMode.None;
            psr3.sortingFudge = 0f;
            psr3.minParticleSize = 0f;
            psr3.maxParticleSize = 0.5f;
            psr3.alignment = ParticleSystemRenderSpace.View;
            psr3.allowRoll = true;
            psr3.maskInteraction = SpriteMaskInteraction.None;
            psr3.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            psr3.receiveShadows = false;
            psr3.shadowBias = 0f;
            psr3.motionVectorGenerationMode = MotionVectorGenerationMode.Object;
            psr3.sortingLayerID = LayerIndex.defaultLayer.intVal;
            psr3.sortingOrder = 0;
            psr3.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.BlendProbes;
            psr3.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
            #endregion

            return obj;
        }
        #endregion
        #region Primary Explosion
        private static void CreatePrimaryExplosionEffects()
        {
            GameObject baseFX = Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/ExplosionGreaterWisp");
            for (int i = 0; i < 8; i++)
            {
                primaryExplosionEffects[i] = CreatePrimaryExplosion(baseFX, i);
            }
        }

        private static GameObject CreatePrimaryExplosion(GameObject baseFX, int skinIndex)
        {
            Material flamesMat = WispMaterialModule.fireMaterials[skinIndex][0];
            Color flamesColor = WispMaterialModule.fireColors[skinIndex];
            GameObject obj = baseFX.InstantiateClone("PrimaryExplosion"+skinIndex.ToString());
            GameObject obj2 = obj.transform.Find("Particles").gameObject;

            GameObject flamesObj = obj2.transform.Find("Flames").gameObject;
            GameObject sparksObj = obj2.transform.Find("Sparks").gameObject;
            GameObject flameSphObj = obj2.transform.Find("Flames,Sphere").gameObject;
            GameObject ringObj = obj2.transform.Find("Ring").gameObject;
            GameObject debrisObj = obj2.transform.Find("Debris").gameObject;
            GameObject flashObj = obj2.transform.Find("Flash").gameObject;
            GameObject distObj = obj2.transform.Find("Distortion").gameObject;



            ParticleSystem flamesPS = flamesObj.GetComponent<ParticleSystem>();
            ParticleSystemRenderer flamesPSR = flamesObj.GetComponent<ParticleSystemRenderer>();

            var flamesCOL = flamesPS.colorOverLifetime;
            flamesCOL.color = new ParticleSystem.MinMaxGradient
            {
                mode = ParticleSystemGradientMode.Gradient,
                gradient = new Gradient
                {
                    alphaKeys = flamesCOL.color.gradient.alphaKeys,
                    colorKeys = new GradientColorKey[3]
                    {
                        new GradientColorKey
                        {
                            time = 0f,
                            color = new Color
                            {
                                r = 1f,
                                g = 1f,
                                b = 1f
                            }
                        },
                        new GradientColorKey
                        {
                            time = 0.025f,
                            color = flamesColor
                        },
                        new GradientColorKey
                        {
                            time = 1f,
                            color = new Color
                            {
                                r = 0.1f,
                                g = 0.1f,
                                b = 0.1f
                            }
                        }
                    }
                }
            };

            ParticleSystem sparksPS = sparksObj.GetComponent<ParticleSystem>();
            ParticleSystemRenderer sparksPSR = sparksObj.GetComponent<ParticleSystemRenderer>();

            sparksPSR.material = flamesMat;

            var sparksCOL = sparksPS.colorOverLifetime;
            sparksCOL.color = new ParticleSystem.MinMaxGradient
            {
                mode = ParticleSystemGradientMode.Gradient,
                gradient = new Gradient
                {
                    alphaKeys = sparksCOL.color.gradient.alphaKeys,
                    colorKeys = new GradientColorKey[1]
                    {
                        new GradientColorKey
                        {
                            time = 0f,
                            color = new Color
                            {
                                r = 1f,
                                g = 1f,
                                b = 1f
                            }
                        }
                    }
                }
            };

            ParticleSystem flameSphPS = flameSphObj.GetComponent<ParticleSystem>();

            var flameSphCOL = flameSphPS.colorOverLifetime;
            flameSphCOL.color = new ParticleSystem.MinMaxGradient
            {
                mode = ParticleSystemGradientMode.Gradient,
                gradient = new Gradient
                {
                    alphaKeys = flameSphCOL.color.gradient.alphaKeys,
                    colorKeys = new GradientColorKey[3]
                    {
                        new GradientColorKey
                        {
                            time = 0f,
                            color = new Color
                            {
                                r = 1f,
                                g = 1f,
                                b = 1f
                            }
                        },
                        new GradientColorKey
                        {
                            time = 0.025f,
                            color = flamesColor
                        },
                        new GradientColorKey
                        {
                            time = 1f,
                            color = new Color
                            {
                                r = 0.1f,
                                g = 0.1f,
                                b = 0.1f
                            }
                        }
                    }
                }
            };


            ParticleSystem ringPS = ringObj.GetComponent<ParticleSystem>();

            var ringCOL = ringPS.colorOverLifetime;
            ringCOL.color = new ParticleSystem.MinMaxGradient
            {
                mode = ParticleSystemGradientMode.Gradient,
                gradient = new Gradient
                {
                    alphaKeys = ringCOL.color.gradient.alphaKeys,
                    colorKeys = new GradientColorKey[3]
                    {
                        new GradientColorKey
                        {
                            time = 0f,
                            color = new Color
                            {
                                r = 1f,
                                g = 1f,
                                b = 1f
                            }
                        },
                        new GradientColorKey
                        {
                            time = 0.025f,
                            color = flamesColor
                        },
                        new GradientColorKey
                        {
                            time = 1f,
                            color = new Color
                            {
                                r = 0.1f,
                                g = 0.1f,
                                b = 0.1f
                            }
                        }
                    }
                }
            };

            MonoBehaviour.Destroy(debrisObj);
            MonoBehaviour.Destroy(flashObj);
            //MonoBehaviour.Destroy(distObj);
            //MonoBehaviour.Destroy(ringObj);
            //MonoBehaviour.Destroy(flameSphObj);
            //MonoBehaviour.Destroy(sparksObj);
            //MonoBehaviour.Destroy(flamesObj);

            return obj;
        }


        #endregion
        #region Secondary Explosion
        private static void CreateSecondaryExplosionEffect()
        {
            GameObject baseFX = Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/MeteorStrikeImpact");
            GameObject refFX = Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/AncientWispPillar");

            //Transform refPST = Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/AncientWispPillar").transform.Find("Particles");

            for (int i = 0; i < 8; i++)
            {
                secondaryExplosions[i] = CreateSecondaryExplosion(baseFX, i, refFX);
            }

        }

        private static GameObject CreateSecondaryExplosion(GameObject baseFX, int skinIndex, GameObject refFX)
        {
            GameObject obj = baseFX.InstantiateClone("SecondaryExplosion"+skinIndex.ToString());

            Material flameMat = WispMaterialModule.fireMaterials[skinIndex][0];
            Color flameCol = WispMaterialModule.fireColors[skinIndex];

            obj.transform.localScale = new Vector3( 2f, 2f, 2f );

            obj.transform.Find("Flash").gameObject.name = "Flash2";

            //MonoBehaviour.Destroy(obj.transform.Find("Debris").gameObject);
            //MonoBehaviour.Destroy(obj.transform.Find("Dust").gameObject);
            MonoBehaviour.Destroy(obj.transform.Find("Dust, Directional").gameObject);
            //MonoBehaviour.Destroy(obj.transform.Find("Dust, Directional").gameObject);


            //Add a new particle system and set its properties
            GameObject tube = new GameObject("Tube");
            tube.transform.parent = obj.transform;
            tube.transform.localPosition = Vector3.zero;
            tube.transform.localRotation = Quaternion.LookRotation(Vector3.forward, Vector3.down);
            tube.transform.localScale = new Vector3(0.6f, 0.6f, 0.5f);

            ParticleSystem tubePS = tube.AddComponent<ParticleSystem>();
            ParticleSystemRenderer tubePSR = tube.GetComponent<ParticleSystemRenderer>();

            ParticleSystem tubeRefPS = refFX.transform.Find("Particles").Find("Flames, Tube, CenterHuge").GetComponent<ParticleSystem>();
            ParticleSystemRenderer tubeRefPSR = refFX.transform.Find("Particles").Find("Flames, Tube, CenterHuge").GetComponent<ParticleSystemRenderer>();

            SetParticleStruct<ParticleSystem.MainModule>(tubePS.main, tubeRefPS.main);
            SetParticleStruct<ParticleSystem.EmissionModule>(tubePS.emission, tubeRefPS.emission);
            SetParticleStruct<ParticleSystem.ShapeModule>(tubePS.shape, tubeRefPS.shape);
            SetParticleStruct<ParticleSystem.ColorOverLifetimeModule>(tubePS.colorOverLifetime, tubeRefPS.colorOverLifetime);
            SetParticleStruct<ParticleSystem.SizeOverLifetimeModule>(tubePS.sizeOverLifetime, tubeRefPS.sizeOverLifetime);
            SetParticleStruct<ParticleSystem.RotationOverLifetimeModule>(tubePS.rotationOverLifetime, tubeRefPS.rotationOverLifetime);

            var tubePSEmis = tubePS.emission;
            var tubeRefPSEmis = tubeRefPS.emission;
            ParticleSystem.Burst[] tempBursts = new ParticleSystem.Burst[1];
            tempBursts[0] = new ParticleSystem.Burst
            {
                count = 10,
                cycleCount = 1,
                probability = 1f,
                time = 0f,
                repeatInterval = 10f
            };
            tubePSEmis.SetBursts(tempBursts);
            
            foreach( PropertyInfo p in tubePSR.GetType().GetProperties() )
            {
                if( p.CanWrite && p.CanRead ) p.SetValue(tubePSR, p.GetValue(tubeRefPSR));
            }

            tubePSR.material = WispMaterialModule.fireMaterials[skinIndex][5];

            //Configure existing particle systems
            GameObject fire = obj.transform.Find("Fire").gameObject;
            ParticleSystemRenderer firePSR = fire.GetComponent<ParticleSystemRenderer>();
            firePSR.material = WispMaterialModule.fireMaterials[skinIndex][6];


            GameObject flFire = obj.transform.Find("Flash Lines, Fire").gameObject;
            ParticleSystem flFirePS = flFire.GetComponent<ParticleSystem>();
            ParticleSystemRenderer flFirePSR = flFire.GetComponent<ParticleSystemRenderer>();
            var flFirePScol = flFirePS.colorOverLifetime;
            flFirePScol.color = new ParticleSystem.MinMaxGradient
            {
                mode = ParticleSystemGradientMode.Gradient,
                gradient = WispMaterialModule.fireGradients[skinIndex]
            };
            flFirePSR.material = WispMaterialModule.fireMaterials[skinIndex][6];

            GameObject flBase = obj.transform.Find("Flash Lines").gameObject;
            ParticleSystem flBasePS = flBase.GetComponent<ParticleSystem>();
            var flBasePSmain = flBasePS.main;
            flBasePSmain.startColor = new ParticleSystem.MinMaxGradient
            {
                mode = ParticleSystemGradientMode.Color,
                color = WispMaterialModule.fireColors[skinIndex]
            };

            GameObject flash = obj.transform.Find("Flash").gameObject;
            ParticleSystem flashPS = flash.GetComponent<ParticleSystem>();
            var flashPScol = flashPS.colorOverLifetime;
            flashPScol.color = new ParticleSystem.MinMaxGradient
            {
                mode = ParticleSystemGradientMode.Gradient,
                gradient = WispMaterialModule.fireGradients[skinIndex]
            };

            GameObject flash2 = obj.transform.Find("Flash2").gameObject;
            ParticleSystem flash2PS = flash2.GetComponent<ParticleSystem>();
            var flash2PScol = flash2PS.colorOverLifetime;
            flash2PScol.color = new ParticleSystem.MinMaxGradient
            {
                mode = ParticleSystemGradientMode.Gradient,
                gradient = WispMaterialModule.fireGradients[skinIndex]
            };

            obj.transform.Find("Point light").GetComponent<Light>().color = WispMaterialModule.fireColors[skinIndex];

            GameObject sparks = obj.transform.Find("Sparks").gameObject;
            ParticleSystem sparksPS = sparks.GetComponent<ParticleSystem>();
            var sparksPSmain = sparksPS.main;
            sparksPSmain.startColor = new ParticleSystem.MinMaxGradient
            {
                mode = ParticleSystemGradientMode.Color,
                color = WispMaterialModule.fireColors[skinIndex]
            };



            return obj;
        }

        #endregion
        #region Blaze Orb
        private static void CreateBlazeOrbEffects()
        {
            GameObject baseFX = Resources.Load<GameObject>("Prefabs/Effects/MeteorStrikePredictionEffect");

            for( int i = 0; i < 8; i++ )
            {
                utilityFlames[i] = CreateBlazeEffect(baseFX, i);
            }
        }

        private static GameObject CreateBlazeEffect( GameObject baseFX, int skinIndex )
        {
            GameObject obj = baseFX.InstantiateClone("BlazeEffect"+skinIndex.ToString());

            MonoBehaviour.Destroy(obj.GetComponent<DestroyOnTimer>());

            obj.AddComponent<Components.WispBlazeEffectController>();
            obj.transform.localScale = Vector3.one;

            obj.GetComponent<EffectComponent>().applyScale = true;

            Transform indicator = obj.transform.Find("GroundSlamIndicator");
            MonoBehaviour.Destroy(indicator.gameObject);

            GameObject fireObj = new GameObject("Flames");
            fireObj.transform.parent = obj.transform;
            fireObj.transform.localPosition = Vector3.zero;
            fireObj.transform.localRotation = Quaternion.identity;
            fireObj.transform.localScale = Vector3.one;

            ParticleSystem firePS = fireObj.AddComponent<ParticleSystem>();
            ParticleSystemRenderer firePSR = fireObj.GetComponent<ParticleSystemRenderer>();

            var fireMain = firePS.main;
            fireMain.duration = 1f;
            fireMain.loop = true;
            fireMain.prewarm = false;
            fireMain.startLifetime = new ParticleSystem.MinMaxCurve
            {
                mode = ParticleSystemCurveMode.TwoConstants,
                constantMin = 0.75f,
                constantMax = 1f
            };
            fireMain.gravityModifier = new ParticleSystem.MinMaxCurve
            {
                mode = ParticleSystemCurveMode.Constant,
                constant = -0.5f
            };
            fireMain.scalingMode = ParticleSystemScalingMode.Shape;
            fireMain.startRotation = new ParticleSystem.MinMaxCurve
            {
                mode = ParticleSystemCurveMode.TwoConstants,
                constantMin = 0f,
                constantMax = 360f
            };
            fireMain.flipRotation = 0.5f;

            var fireEmis = firePS.emission;
            fireEmis.rateOverTime = new ParticleSystem.MinMaxCurve
            {
                mode = ParticleSystemCurveMode.Constant,
                constant = 10f
            };

            var fireShape = firePS.shape;
            fireShape.position = new Vector3(0f, 0f, 0f);
            fireShape.rotation = new Vector3(-90f, 0f, 0f);
            fireShape.scale = new Vector3(0.75f, 0.75f, 0.75f);
            fireShape.radiusThickness = 1f;
            fireShape.angle = 30f;

            var fireCOL = firePS.colorOverLifetime;
            fireCOL.enabled = true;
            fireCOL.color = new ParticleSystem.MinMaxGradient
            {
                mode = ParticleSystemGradientMode.Gradient,
                gradient = new Gradient
                {
                    alphaKeys = new GradientAlphaKey[4]
                    {
                        new GradientAlphaKey( 0f, 0f ),
                        new GradientAlphaKey( 0.6f, 0.05f),
                        new GradientAlphaKey( 0.1f, 0.65f),
                        new GradientAlphaKey( 0f, 1f )
                    },
                    colorKeys = new GradientColorKey[1]
                    {
                        new GradientColorKey( new Color( 1f, 1f, 1f ) , 0f )
                    },
                    mode = GradientMode.Blend
                }
            };

            var fireSOL = firePS.sizeOverLifetime;
            fireSOL.enabled = true;
            fireSOL.size = new ParticleSystem.MinMaxCurve
            {
                mode = ParticleSystemCurveMode.Curve,
                curve = new AnimationCurve
                {
                    postWrapMode = WrapMode.Clamp,
                    preWrapMode = WrapMode.Clamp,
                    keys = new Keyframe[3]
                    {
                        new Keyframe(0f,0.25f),
                        new Keyframe(0.43f,0.4f),
                        new Keyframe(1f,0.01f)
                    }
                }
            };
            fireSOL.sizeMultiplier = 3f;

            var fireROL = firePS.rotationOverLifetime;
            fireROL.enabled = true;
            fireROL.z = new ParticleSystem.MinMaxCurve
            {
                mode = ParticleSystemCurveMode.Constant,
                constant = 3f
            };

            firePSR.material = WispMaterialModule.fireMaterials[skinIndex][1];

            GameObject ringObj = new GameObject("Ring");
            ringObj.transform.parent = obj.transform;
            ringObj.transform.localPosition = Vector3.zero;
            ringObj.transform.localRotation = Quaternion.identity;
            ringObj.transform.localScale = new Vector3( 1f, 1f, 1f );

            ParticleSystem ringPS = ringObj.AddComponent<ParticleSystem>();
            ParticleSystemRenderer ringPSR = ringObj.GetComponent<ParticleSystemRenderer>();

            var ringMain = ringPS.main;
            ringMain.duration = 1f;
            ringMain.loop = true;
            ringMain.prewarm = false;
            ringMain.startLifetime = new ParticleSystem.MinMaxCurve
            {
                mode = ParticleSystemCurveMode.TwoConstants,
                constantMin = 0.75f,
                constantMax = 1f
            };
            ringMain.gravityModifier = 0f;
            ringMain.scalingMode = ParticleSystemScalingMode.Shape;
            ringMain.startRotation = new ParticleSystem.MinMaxCurve
            {
                mode = ParticleSystemCurveMode.TwoConstants,
                constantMin = 0f,
                constantMax = 360f
            };
            ringMain.flipRotation = 0.5f;
            ringMain.startSpeed = 0f;

            var ringEmis = ringPS.emission;
            ringEmis.rateOverTime = new ParticleSystem.MinMaxCurve
            {
                mode = ParticleSystemCurveMode.Constant,
                constant = 30f
            };

            var ringShape = ringPS.shape;
            ringShape.shapeType = ParticleSystemShapeType.Sphere;
            ringShape.position = new Vector3(0f, 0f, 0f);
            ringShape.rotation = new Vector3(-90f, 0f, 0f);
            ringShape.scale = new Vector3(1f, 1f, 1f);
            ringShape.radiusThickness = 0f;

            var ringCOL = ringPS.colorOverLifetime;
            ringCOL.enabled = true;
            ringCOL.color = new ParticleSystem.MinMaxGradient
            {
                mode = ParticleSystemGradientMode.Gradient,
                gradient = new Gradient
                {
                    alphaKeys = new GradientAlphaKey[4]
                    {
                        new GradientAlphaKey( 0f, 0f ),
                        new GradientAlphaKey( 0.6f, 0.05f),
                        new GradientAlphaKey( 0.1f, 0.65f),
                        new GradientAlphaKey( 0f, 1f )
                    },
                    colorKeys = new GradientColorKey[1]
                    {
                        new GradientColorKey( new Color( 1f, 1f, 1f ) , 0f )
                    },
                    mode = GradientMode.Blend
                }
            };

            var ringSOL = ringPS.sizeOverLifetime;
            ringSOL.enabled = true;
            ringSOL.size = new ParticleSystem.MinMaxCurve
            {
                mode = ParticleSystemCurveMode.Curve,
                curve = new AnimationCurve
                {
                    postWrapMode = WrapMode.Clamp,
                    preWrapMode = WrapMode.Clamp,
                    keys = new Keyframe[3]
                    {
                        new Keyframe(0f,0.1f),
                        new Keyframe(0.5f,0.5f),
                        new Keyframe(1f,0.01f)
                    }
                }
            };
            ringSOL.sizeMultiplier = 3f;

            var ringROL = ringPS.rotationOverLifetime;
            ringROL.enabled = true;
            ringROL.z = new ParticleSystem.MinMaxCurve
            {
                mode = ParticleSystemCurveMode.Constant,
                constant = 3f
            };

            ringPSR.material = WispMaterialModule.fireMaterials[skinIndex][1];

            return obj;
        }

        #endregion
        #region Ignition Orb
        private static void CreateIgnitionOrbEffects()
        {
            GameObject baseFX = Resources.Load<GameObject>("Prefabs/Effects/HelfireIgniteEffect");

            for( int i = 0; i < 8; i++ )
            {
                utilityBurns[i] = CreateIgniteEffect(baseFX, i);
            }
        }

        private static GameObject CreateIgniteEffect(GameObject baseFX, int skinIndex)
        {
            GameObject obj = baseFX.InstantiateClone("IgniteEffect"+skinIndex.ToString());
            MonoBehaviour.Destroy(obj.GetComponent<DestroyOnTimer>());
            obj.AddComponent<Components.WispIgnitionEffectController>();
            //obj.transform.Find("Point Light").GetComponent<Light>().color = WispMaterialModule.fireColors[skinIndex];
            MonoBehaviour.Destroy(obj.transform.Find("Point Light").gameObject);
            Transform flareObj = obj.transform.Find("Flare");
            Transform puffObj = obj.transform.Find("Puff");
            ParticleSystem flarePS = flareObj.GetComponent<ParticleSystem>();
            ParticleSystemRenderer flarePSR = flareObj.GetComponent<ParticleSystemRenderer>();
            flarePSR.material = WispMaterialModule.fireMaterials[skinIndex][7];

            ParticleSystem puffPS = puffObj.GetComponent<ParticleSystem>();
            ParticleSystemRenderer puffPSR = puffObj.GetComponent<ParticleSystemRenderer>();

            puffPSR.material = WispMaterialModule.fireMaterials[skinIndex][1];

            var puffMain = puffPS.main;
            puffMain.loop = true;
            puffMain.duration = 0.05f;
            puffMain.simulationSpace = ParticleSystemSimulationSpace.World;
            puffMain.startSize = new ParticleSystem.MinMaxCurve
            {
                mode = ParticleSystemCurveMode.TwoConstants,
                constantMin = 1f,
                constantMax = 2f
            };

            var puffEmis = puffPS.emission;
            puffEmis.SetBursts(Array.Empty<ParticleSystem.Burst>());
            puffEmis.rateOverTime = new ParticleSystem.MinMaxCurve
            {
                mode = ParticleSystemCurveMode.Constant,
                constant = 5f
            };

            return obj;
        }


        #endregion
        #region Leech Orb
        private static void CreateLeechOrbEffects()
        {
            GameObject baseFX = Resources.Load<GameObject>("Prefabs/Effects/OrbEffects/HauntOrbEffect");

            for( int i = 0; i < 8; i++ )
            {
                utilityLeech[i] = CreateLeechOrb(baseFX, i);
            }
        }

        private static GameObject CreateLeechOrb( GameObject baseFX, int skinIndex )
        {
            GameObject obj = baseFX.InstantiateClone("LeechEffect"+skinIndex.ToString());

            

            Transform vfx = obj.transform.Find("VFX");
            Transform core = vfx.Find("Core");

            ParticleSystemRenderer corePSR = core.GetComponent<ParticleSystemRenderer>();

            corePSR.material = WispMaterialModule.fireMaterials[skinIndex][1];
            
            return obj;
        }

        #endregion
        #region Utility Aim Effect
        private static void CreateUtilityAimEffects()
        {
            GameObject baseFX = new GameObject("Temp", new Type[2]
            {
                typeof(LineRenderer),
                typeof(Components.WispAimLineController),
            });

            for( int i = 0; i < 8; i++ )
            {
                utilityAim[i] = CreateUtilityAim(baseFX, i);
            }

            MonoBehaviour.Destroy(baseFX);

        }

        private static GameObject CreateUtilityAim( GameObject baseFX, int skinIndex)
        {
            GameObject obj = baseFX.InstantiateClone("WispUtilityAimEffect");

            LineRenderer lr = obj.GetComponent<LineRenderer>();
            GameObject g = new GameObject("lineEnd");
            g.transform.parent = obj.transform;
            g.transform.localPosition = Vector3.zero;
            g.transform.localRotation = Quaternion.identity;
            g.transform.localScale = Vector3.one;

            lr.alignment = LineAlignment.View;
            lr.colorGradient = WispMaterialModule.fireGradients[skinIndex];
            lr.generateLightingData = false;
            lr.positionCount = 2;
            lr.startWidth = 0.1f;
            lr.endWidth = 0.1f;
            lr.useWorldSpace = true;
            //lr.material = WispMaterialModule.fireMaterials[skinIndex][5];
            

            return obj;
        }

        #endregion
        #region Special Explosion
        private static void CreateSpecialExplosionEffects()
        {
            GameObject baseFX = Resources.Load<GameObject>("Prefabs/Effects/BeamSphereExplosion");

            for( int i = 0; i < 8; i++ )
            {
                specialExplosion[i] = CreateSpecialExplosion(baseFX, i);
            }
        }

        private static GameObject CreateSpecialExplosion( GameObject baseFX, int skinIndex )
        {
            GameObject obj = baseFX.InstantiateClone("SpecialExplosion"+skinIndex.ToString());
            Transform burst = obj.transform.Find("InitialBurst");
            Transform ring = burst.Find("Ring");
            Transform chunks = burst.Find("Chunks, Sharp");
            Transform flames = burst.Find("Flames");
            Transform flash = burst.Find("Flash");
            Transform light = burst.Find("Point light");
            Transform zap = burst.Find("Lightning");

            var ringPS = ring.GetComponent<ParticleSystem>();
            var ringPSR = ring.GetComponent<ParticleSystemRenderer>();

            var ringCol = ringPS.colorOverLifetime;
            ringCol.color = new ParticleSystem.MinMaxGradient
            {
                mode = ParticleSystemGradientMode.Gradient,
                gradient = WispMaterialModule.fireGradients[skinIndex]
            };

            ringPSR.material = WispMaterialModule.fireMaterials[skinIndex][8];


            var chunkPS = chunks.GetComponent<ParticleSystem>();

            var chunkCol = chunkPS.colorOverLifetime;
            chunkCol.color = new ParticleSystem.MinMaxGradient
            {
                mode = ParticleSystemGradientMode.Gradient,
                gradient = WispMaterialModule.fireGradients[skinIndex]
            };


            var flamesPSR = flames.GetComponent<ParticleSystemRenderer>();
            flamesPSR.material = WispMaterialModule.fireMaterials[skinIndex][9];


            var flashPS = flash.GetComponent<ParticleSystem>();

            var flashCol = flashPS.colorOverLifetime;
            flashCol.color = new ParticleSystem.MinMaxGradient
            {
                mode = ParticleSystemGradientMode.Gradient,
                gradient = new Gradient
                {
                    alphaKeys = new GradientAlphaKey[1]
                    {
                        new GradientAlphaKey( 1f, 0f )
                    },
                    colorKeys = new GradientColorKey[2]
                    {
                        new GradientColorKey( new Color( 1f, 1f, 1f ) , 0f ),
                        new GradientColorKey( WispMaterialModule.fireColors[skinIndex] , 0.25f )
                    },
                    mode = GradientMode.Blend
                }
            };

            light.GetComponent<Light>().color = WispMaterialModule.fireColors[skinIndex];

            var zapPSR = zap.GetComponent<ParticleSystemRenderer>();
            zapPSR.material = WispMaterialModule.fireMaterials[skinIndex][10];

            return obj;
        }


        #endregion
        #region Special Charge
        private static void CreateSpecialChargeEffects()
        {
            GameObject baseFX = Resources.Load<GameObject>("Prefabs/Effects/ChargeMageFireBomb");

            for( int i = 0; i < 8; i++ )
            {
                specialCharge[i] = CreateSpecialCharge(baseFX, i);
            }
        }

        private static GameObject CreateSpecialCharge( GameObject baseFX, int skinIndex )
        {
            GameObject obj = baseFX.InstantiateClone("SpecialCharge"+skinIndex.ToString());

            obj.name = "WispSpecialChargeEffect";

            Transform baseChild = obj.transform.Find("Base");
            Transform orbCore = obj.transform.Find("OrbCore");
            Transform light = obj.transform.Find("Point light");

            light.GetComponent<Light>().color = WispMaterialModule.fireColors[skinIndex];

            ParticleSystem basePS = baseChild.GetComponent<ParticleSystem>();
            ParticleSystemRenderer basePSR = baseChild.GetComponent<ParticleSystemRenderer>();

            var basePSmain = basePS.main; 
            basePSmain.startColor = new Color(1f, 1f, 1f, 1f);

            basePSR.material = WispMaterialModule.fireMaterials[skinIndex][9];

            MonoBehaviour.Destroy(orbCore.GetComponent<MeshRenderer>());
            MonoBehaviour.Destroy(orbCore.GetComponent<MeshFilter>());

            ParticleSystem orbPS = orbCore.AddOrGetComponent<ParticleSystem>();
            ParticleSystemRenderer orbPSR = orbCore.AddOrGetComponent<ParticleSystemRenderer>();

            orbPSR.material = WispMaterialModule.fireMaterials[skinIndex][9];

            BasicSetup(orbPS);

            var orbMain = orbPS.main;
            orbMain.duration = 5f;
            orbMain.loop = true;
            orbMain.startLifetime = 0.5f;
            orbMain.startSpeed = 0f;
            orbMain.startSize = 3f;
            orbMain.startColor = new Color(1f, 1f, 1f, 1f);
            orbMain.gravityModifier = 0f;
            orbMain.cullingMode = ParticleSystemCullingMode.AlwaysSimulate;

            var orbEmis = orbPS.emission;
            orbEmis.enabled = true;
            orbEmis.rateOverTime = 100f;
            orbEmis.rateOverDistance = 0f;

            var orbShape = orbPS.shape;
            orbShape.enabled = true;
            orbShape.shapeType = ParticleSystemShapeType.Sphere;
            orbShape.radius = 1f;

            var orbCOL = orbPS.colorOverLifetime;
            orbCOL.enabled = true;
            orbCOL.color = new ParticleSystem.MinMaxGradient
            {
                mode = ParticleSystemGradientMode.Gradient,
                gradient = new Gradient
                {
                    mode = GradientMode.Blend,
                    alphaKeys = new GradientAlphaKey[2]
                    {
                        new GradientAlphaKey( 1f, 0f ),
                        new GradientAlphaKey( 0f, 1f )
                    },
                    colorKeys = new GradientColorKey[1]
                    {
                        new GradientColorKey( new Color( 1f, 1f, 1f) , 0f )
                    }
                }
            };

            var orbSOL = orbPS.sizeOverLifetime;
            orbSOL.enabled = true;
            orbSOL.size = new ParticleSystem.MinMaxCurve
            {
                mode = ParticleSystemCurveMode.Curve,
                curve = new AnimationCurve
                {
                    postWrapMode = WrapMode.Clamp,
                    preWrapMode = WrapMode.Clamp,
                    keys = new Keyframe[2]
                    {
                        new Keyframe( 0f, 0f ),
                        new Keyframe( 1f, 1f )
                    }
                }
            };




            GameObject sparks = new GameObject("Sparks");
            sparks.transform.parent = obj.transform;
            sparks.transform.localPosition = Vector3.zero;
            sparks.transform.localRotation = Quaternion.identity;

            ParticleSystem sparkPS = sparks.AddComponent<ParticleSystem>();
            ParticleSystemRenderer sparkPSR = sparks.AddOrGetComponent<ParticleSystemRenderer>();

            sparkPSR.material = WispMaterialModule.fireMaterials[skinIndex][1];

            sparkPS.BasicSetup();

            var sparkMain = sparkPS.main;
            sparkMain.duration = 5f;
            sparkMain.loop = true;
            sparkMain.startLifetime = 1f;
            sparkMain.startSpeed = -5f;
            sparkMain.startSize = 0.5f;
            sparkMain.cullingMode = ParticleSystemCullingMode.AlwaysSimulate;

            var sparkEmis = sparkPS.emission;
            sparkEmis.enabled = true;
            sparkEmis.rateOverTime = 100f;
            sparkEmis.rateOverDistance = 0f;
            //sparkEmis.rateOverTimeMultiplier = 1f;
            //sparkEmis.rateOverDistanceMultiplier = 0f;

            var sparkShape = sparkPS.shape;
            sparkShape.enabled = true;
            sparkShape.shapeType = ParticleSystemShapeType.Sphere;
            sparkShape.radius = 5f;
            sparkShape.radiusThickness = 0.01f;

            var sparkCOL = sparkPS.colorOverLifetime;
            sparkCOL.enabled = true;
            sparkCOL.color = new ParticleSystem.MinMaxGradient
            {
                mode = ParticleSystemGradientMode.Gradient,
                gradient = new Gradient
                {
                    mode = GradientMode.Blend,
                    alphaKeys = new GradientAlphaKey[4]
                    {
                        new GradientAlphaKey(0f, 0f ),
                        new GradientAlphaKey(1f, 0.1f),
                        new GradientAlphaKey(0.7f, 0.7f),
                        new GradientAlphaKey(0f, 1f)
                    },
                    colorKeys = new GradientColorKey[1]
                    {
                        new GradientColorKey( new Color( 1f, 1f, 1f ) , 0f )
                    }
                }
            };

            var sparkSOL = sparkPS.sizeOverLifetime;
            sparkSOL.enabled = true;
            sparkSOL.size = new ParticleSystem.MinMaxCurve
            {
                mode = ParticleSystemCurveMode.Curve,
                curve = new AnimationCurve
                {
                    preWrapMode = WrapMode.Clamp,
                    postWrapMode = WrapMode.Clamp,
                    keys = new Keyframe[2]
                    {
                        new Keyframe(0f, 0f ),
                        new Keyframe(1f, 1f )
                    }
                }
            };
            sparkSOL.sizeMultiplier = 1f;


            return obj;
        }



        #endregion
        private static void ExFunction(GameObject body, Dictionary<Type, Component> dic)
        {

        }

        private static T C<T>(this Dictionary<Type, Component> dic) where T : Component
        {
            return dic[typeof(T)] as T;
        }

        private static void Strip(this GameObject g)
        {
            foreach (Component c in g.GetComponents<Component>())
            {
                if (!c) continue;
                if (c.GetType() == typeof(Transform)) continue;

                MonoBehaviour.DestroyImmediate(c);
            }
        }

        private static void BasicSetup(this ParticleSystem ps1)
        {
            var ps1Emission = ps1.emission;
            ps1Emission.enabled = false;

            var ps1Shape = ps1.shape;
            ps1Shape.enabled = false;

            var ps1VOL = ps1.velocityOverLifetime;
            ps1VOL.enabled = false;

            var ps1LimVOL = ps1.limitVelocityOverLifetime;
            ps1LimVOL.enabled = false;

            var ps1InhVel = ps1.inheritVelocity;
            ps1InhVel.enabled = false;

            var ps1FOL = ps1.forceOverLifetime;
            ps1FOL.enabled = false;

            var ps1COL = ps1.colorOverLifetime;
            ps1COL.enabled = false;

            var ps1CBS = ps1.colorBySpeed;
            ps1CBS.enabled = false;

            var ps1SOL = ps1.sizeOverLifetime;
            ps1SOL.enabled = false;

            var ps1SBS = ps1.sizeBySpeed;
            ps1SBS.enabled = false;

            var ps1ROL = ps1.rotationOverLifetime;
            ps1ROL.enabled = false;

            var ps1RBS = ps1.rotationBySpeed;
            ps1RBS.enabled = false;

            var ps1ExtFor = ps1.externalForces;
            ps1ExtFor.enabled = false;

            var ps1Noise = ps1.noise;
            ps1Noise.enabled = false;

            var ps1Collis = ps1.collision;
            ps1Collis.enabled = false;

            var ps1Trig = ps1.trigger;
            ps1Trig.enabled = false;

            var ps1SubEmit = ps1.subEmitters;
            ps1SubEmit.enabled = false;

            var ps1TexAnim = ps1.textureSheetAnimation;
            ps1TexAnim.enabled = false;

            var ps1Light = ps1.lights;
            ps1Light.enabled = false;

            var ps1Trails = ps1.trails;
            ps1Trails.enabled = false;

            var ps1Cust = ps1.customData;
            ps1Cust.enabled = false;
        }

        private static void DebugMaterialInfo(Material m)
        {
            Debug.Log("Material name: " + m.name);
            string[] s = m.shaderKeywords;
            Debug.Log("Shader keywords");
            for (int i = 0; i < s.Length; i++)
            {
                Debug.Log(s[i]);
            }

            Debug.Log("Shader name: " + m.shader.name);

            Debug.Log("Texture Properties");
            string[] s2 = m.GetTexturePropertyNames();
            for (int i = 0; i < s2.Length; i++)
            {
                Debug.Log(s2[i] + " : " + m.GetTexture(s2[i]));
            }
        }
    }
}

/* Sounds

(Special fire?)    Play_item_use_BFG_fire
(impact)    Play_item_use_BFG_explode
    Stop_item_use_BFG_loop
    Play_item_use_BFG_zaps
    Play_item_use_BFG_charge

    Play_gravekeeper_attack1_fly_loop
    Play_gravekeeper_attack2_shoot_singleChain	
    Play_gravekeeper_attack1_fire
    Play_gravekeeper_impact_body
    Play_gravekeeper_attack1_close	
    Play_gravekeeper_idle_twitch
    Play_gravekeeper_spawn_01
    Play_gravekeeper_attack2_charge		
    Play_gravekeeper_death_impact_01
    Play_gravekeeper_land
    Play_gravekeeper_idle_loop
    Stop_gravekeeper_attack1_fly_loop
    Stop_gravekeeper_idle_loop
    Stop_gravekeeper_attack2_fly_loop
    Play_gravekeeper_attack2_shoot	
    Play_gravekeeper_attack1_explode
    Play_gravekeeper_death_01
    Play_gravekeeper_jump	
    Play_gravekeeper_attack1_open	
    Play_gravekeeper_idle_VO
    Play_gravekeeper_attack2_impact
    Play_gravekeeper_impact_canister
    Play_gravekeeper_step
    Play_gravekeeper_attack2_fly_loop

    Play_greater_wisp_idle
    Play_greater_wisp_active_loop
    Play_wisp_attack_chargeup	
    Play_greater_wisp_attack
    Play_greater_wisp_death
    Play_item_proc_wisp_explo
    Play_wisp_idle
    Play_greater_wisp_impact
    Stop_greater_wisp_active_loop
    Stop_wisp_active_loop
    Play_wisp_impact
    Play_wisp_death	
    Play_wisp_active_loop
    Play_wisp_attack_fire
    Play_wisp_spawn	

    Play_magmaWorm_idle_burn_loop
    Stop_magmaWorm_idle_burn_loop


    Play_item_proc_dagger_fly

    Play_AMB_zone_dark_rain

    Play_merc_shift_slice

    */
