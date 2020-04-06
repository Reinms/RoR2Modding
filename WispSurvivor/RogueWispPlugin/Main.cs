﻿using BepInEx;
#if R2API
using R2API.Utils;
#endif
using RoR2;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Reflection;
using UnityEngine;
#if NETWORKING
using MonoMod.RuntimeDetour;
using MonoMod.Cil;
using ReinCore;
#endif
namespace Rein.RogueWispPlugin
{
#pragma warning disable CA2243 // Attribute string literals should parse correctly
#if NETWORKING

    [BepInDependency( NetLib.NetLib.guid, BepInDependency.DependencyFlags.SoftDependency )]
#endif
    [BepInDependency( Rein.AssemblyLoad.guid, BepInDependency.DependencyFlags.HardDependency )]
    [BepInPlugin( pluginGUID, pluginName, pluginVersion )]
#pragma warning restore CA2243 // Attribute string literals should parse correctly
    internal partial class Main : BaseUnityPlugin
    {
        #region random vars
        private const String pluginGUID = "com.Rein.RogueWisp";
        private const String pluginName = "Rogue Wisp";
        private const String pluginVersion = Consts.ver;
        public String thing1;
        public String thing2;
        public String thing3;
        private readonly Boolean working;
        #endregion
        #region execution tracking stuff
        private static ExecutionState _state = ExecutionState.PreLoad;
        internal static ExecutionState state
        {
            get
            {
                return _state;
            }
            set
            {
                if( _state == ExecutionState.Broken ) return;
                _state = value;
                //Main.LogI( "State inc: " + _state.ToString() );

            }
        }
        private static void IncState()
        {
            state = state + 1;
            UpdateLibraries();
        }
        internal enum ExecutionState
        {
            Broken = -1,
            PreLoad = 0,
            Constructor = 1,
            Awake = 2,
            Enable = 3,
            Catalogs = 4,
            Start = 5,
            Execution = 6
        }
        #endregion
#if TIMER
        private readonly Stopwatch watch;
#endif
        private List<PluginInfo> _plugins;
        private List<PluginInfo> plugins
        {
            get
            {
                if( this._plugins == null )
                {
                    this._plugins = new List<PluginInfo>();
                    foreach( PluginInfo p in BepInEx.Bootstrap.Chainloader.PluginInfos.Values )
                    {
                        if( p == null ) continue;
                        this._plugins.Add( p );
                    }
                }
                return this._plugins;
            }
        }
        internal static Main instance;
        #region Events
        internal event Action Load;
        internal event Action FirstFrame;
        internal event Action Enable;
        internal event Action Disable;
        internal event Action Frame;
        internal event Action PostFrame;
        internal event Action Tick;
        internal event Action GUI;
        #endregion
#if LOADCHECKS
        //partial Boolean LoadChecks();
#endif
#if COMPATCHECKS
        partial void CompatChecks();
#endif
#if CROSSMODFUNCTIONALITY
        partial void CrossModFunctionality();
#endif
        partial void CreateAccessors();
        static partial void UpdateLibraries();
        partial void CreateCustomAccessors();
#if ROGUEWISP
        partial void CreateRogueWisp();
#endif
#if BOSSHPBAR
        partial void EditBossHPBar();
#endif
#if NETWORKING
        partial void RegisterNetworkMessages();
#else
        partial void SetupNetworkingFramework();
        partial void SetupNetworkingFunctions();
#endif
#if ANCIENTWISP
        partial void CreateAncientWisp();
#endif
#if ARCHAICWISP
        partial void CreateArchaicWisp();
#endif
#if FIRSTWISP
        partial void CreateFirstWisp();
#endif
#if WISPITEM
        partial void CreateWispFriend();
#endif
#if STAGE
        partial void CreateStage();
#endif
#if SOUNDPLAYER
        partial void SetupSoundPlayer();
#endif
#if MATEDITOR
        partial void SetupMatEditor();
#endif
        partial void SetupSkinSelector();
        partial void SetupEffectSkinning();       
        static Main()
        {
        }
        public Main()
        {
            instance = this;
            var r2apiExists = false;
            HashSet<String> submodules = new HashSet<String>();
            //ReflectionOnlyAssemblyResolve
            for( Int32 i = 0; i < this.plugins.Count; ++i )
            {
                var p = this.plugins[i];
                if( p.Metadata.GUID == "com.bepis.r2api" )
                {
                    r2apiExists = true;
                }
                
            }

            this.thing1 = "Note that everything in this codebase that can be used safely is already part of R2API.";
            this.thing2 = "If you're here to copy paste my code, please don't complain to me when it doesn't work like magic or you cause a major issue for a user.";
            this.thing3 = "Look fish no more handler scanning! pls no delet meh!";

            this.CreateAccessors();
            this.CreateCustomAccessors();

            IncState();

            this.working = true;

            this.Load += IncState;
            this.Enable += IncState;
            this.FirstFrame += IncState;

            this.CompatChecks();
            if( this.working )
            {
#if TIMER
                this.watch = new Stopwatch();
                this.Load += this.AwakeTimeStart;
                this.Enable += this.EnableTimeStart;
                this.FirstFrame += this.StartTimeStart;
#endif
                this.Tick += () => RoR2Application.isModded = true;

#if CROSSMODFUNCTIONALITY
                this.CrossModFunctionality();
#endif
#if ROGUEWISP
                this.CreateRogueWisp();
#endif
#if BOSSHPBAR
                this.EditBossHPBar();
#endif
#if ANCIENTWISP
                this.CreateAncientWisp();
#endif
#if ARCHAICWISP
                this.CreateArchaicWisp();
#endif
#if FIRSTWISP
                this.CreateFirstWisp();
#endif
#if WISPITEM
                this.CreateWispFriend();
#endif
#if STAGE
                this.CreateStage();
#endif
#if NETWORKING
                this.RegisterNetworkMessages();
#else
                this.SetupNetworkingFramework();
                this.SetupNetworkingFunctions();
#endif
#if SOUNDPLAYER
                this.SetupSoundPlayer();
#endif
#if MATEDITOR
                this.SetupMatEditor();
#endif
                this.SetupSkinSelector();
                this.SetupEffectSkinning();


#if LV1DPSTEST
                SpawnsCore.monsterEdits += this.SpawnsCore_monsterEdits1;
                RoR2.GlobalEventManager.onCharacterDeathGlobal += this.GlobalEventManager_onCharacterDeathGlobal;
#endif



                this.FirstFrame += this.Main_FirstFrame;
                //this.FirstFrame += this.RandomCrap;
                //this.GUI += this.DebugTexture;
#if TIMER
                this.Load += this.AwakeTimeStop;
                this.Enable += this.EnableTimeStop;
                this.FirstFrame += this.StartTimeStop;
#endif
            } else
            {
                Main.LogF( "Rogue Wisp has failed to load properly and will not be enabled. See preceding errors." );
            }

            this.FirstFrame += this.Main_FirstFrame2;

            this.Enable += IncState;
            this.FirstFrame += IncState;
            

            //this.FirstFrame += this.TestingShit;
        }

        private void Main_FirstFrame2()
        {
            rogueWispBodyIndex = BodyCatalog.FindBodyIndex( this.RW_body );
        }

        #region Boss dps timer stuff
#if LV1DPSTEST
        private void GlobalEventManager_onCharacterDeathGlobal( DamageReport obj )
        {
            var v = obj?.victimBody?.GetComponent<TimeThing>();
            if( v != null ) StopTimer();
        }

        private void SpawnsCore_monsterEdits1( ClassicStageInfo stageInfo, Run runInstance, DirectorCardCategorySelection monsterSelection )
        {
            foreach( var m in monsterSelection.categories )
            {
                if( m.name.Contains( "ions" ) )
                {
                    foreach( var n in m.cards )
                    {
                        //Main.LogW( n.spawnCard.name );
                        if( n.spawnCard.name != "cscBeetleQueen" )
                        {
                            n.selectionWeight = 0;
                        } else
                        {
                            var master = (n.spawnCard as CharacterSpawnCard).prefab;
                            foreach( var v in master.GetComponents<RoR2.CharacterAI.AISkillDriver>() )
                            {
                                if( v.skillSlot == SkillSlot.Secondary )
                                {
                                    v.maxUserHealthFraction = 0f;
                                    v.minUserHealthFraction = 0f;
                                }
                            }
                            master.GetComponent<CharacterMaster>().bodyPrefab.AddComponent<TimeThing>();
                        }
                    }
                } else
                {
                    foreach( var n in m.cards )
                    {
                        var body = n.spawnCard.prefab.GetComponent<CharacterMaster>().bodyPrefab.GetComponent<CharacterBody>();
                        body.baseDamage = 0f;
                        body.levelDamage = 0f;
                    }
                }
            }
        }


        private static Stopwatch bossTimer = new Stopwatch();
        internal static void StartTimer()
        {
            bossTimer.Restart();
        }
        internal static void StopTimer()
        {
            if( bossTimer.IsRunning )
            {
                bossTimer.Stop();
                Chat.AddMessage( String.Format( "Time: {0}ms, Ticks: {1}", bossTimer.ElapsedMilliseconds, bossTimer.ElapsedTicks ) );
            }
        }
        internal class TimeThing : MonoBehaviour
        {
            private void Start()
            {
                Main.StartTimer();
            }

            private void OnDisable()
            {
                Main.StopTimer();
            }
        }
#endif
        #endregion


        internal static Texture2D debugTexture;
        private void DebugTexture()
        {
            if( debugTexture != null )
            {
                UnityEngine.GUI.Label( new Rect( 0f, 0f, 1000f, 1000f ), debugTexture );
            }
        }

        private static Texture2D uvTex;
        
        internal static Texture2D GetUVTexture()
        {
            if( uvTex == null )
            {
                uvTex = new Texture2D( 256, 256, TextureFormat.RGBAFloat, false );
                
                for( Int32 x = 0; x < uvTex.width; ++x )
                {
                    var red = 0.85f * (Single)x / (Single)(uvTex.width-1);
                    red += 0.15f;
                    for( Int32 y = 0; y < uvTex.height; ++y )
                    {
                        var green = 0.85f * (Single)y / (Single)(uvTex.height-1);
                        green += 0.15f;

                        var col = Color.black;

                        if( x % 16 == 0 || y % 16 == 0 )
                        {
                            col = Color.white;
                        }

                        if( x == 0 || y == 0 || x == uvTex.width - 1 || y == uvTex.height - 1 )
                        {
                            col = Color.red;
                        }

                        if( x == uvTex.width / 2 || y == uvTex.height / 2 )
                        {
                            col = Color.cyan;
                        }



                        uvTex.SetPixel( x, y, col );
                    }
                }
                uvTex.Apply();
            }

            return uvTex;
        }


        private delegate void InvokeEffectsReloadDelegate( ConCommandArgs args );
        private InvokeEffectsReloadDelegate CCEffectsReload = (InvokeEffectsReloadDelegate)Delegate.CreateDelegate(typeof(InvokeEffectsReloadDelegate),typeof(EffectCatalog),"CCEffectsReload" );
        private void Main_FirstFrame() => this.CCEffectsReload(default);

#pragma warning disable IDE0051 // Remove unused private members
        public void Awake() => this.Load?.Invoke();
        public void Start() => this.FirstFrame?.Invoke();
        public void OnEnable() => this.Enable?.Invoke();
        public void OnDisable() => this.Disable?.Invoke();
        public void Update() => this.Frame?.Invoke();
        public void LateUpdate() => this.PostFrame?.Invoke();
        public void FixedUpdate() => this.Tick?.Invoke();
        public void OnGUI() => this.GUI?.Invoke();
#pragma warning restore IDE0051 // Remove unused private members

#if TIMER
        private void AwakeTimeStart() => this.watch.Restart();
        private void AwakeTimeStop()
        {
            this.watch.Stop();
            Main.LogM( "Awake Time: " + this.watch.ElapsedMilliseconds );
        }
        private void EnableTimeStart() => this.watch.Restart();
        private void EnableTimeStop()
        {
            this.watch.Stop();
            Main.LogM( "Enable Time: " + this.watch.ElapsedMilliseconds );
        }
        private void StartTimeStart() => this.watch.Restart();
        private void StartTimeStop()
        {
            this.watch.Stop();
            Main.LogM( "Start Time: " + this.watch.ElapsedMilliseconds );
        }
#endif
        internal static void Log( BepInEx.Logging.LogLevel level, System.Object data, String member, Int32 line )
        {
            if( data == null )
            {
                Main.instance.Logger.LogError( "Null data sent by: " + member + " line: " + line );
                return;
            }

            if( level == BepInEx.Logging.LogLevel.Fatal || level == BepInEx.Logging.LogLevel.Error || level == BepInEx.Logging.LogLevel.Warning | level == BepInEx.Logging.LogLevel.Message )
            {
                Main.instance.Logger.Log( level, data );
            } else
            {
#if LOGGING
                Main.instance.Logger.Log( level, data );
#endif
            }
#if FINDLOGS
            Main.instance.Logger.LogWarning( "Log: " + level.ToString() + " called by: " + member + " : " + line );
#endif
        }

        internal static void LogI( System.Object data, [CallerMemberName] String member = "", [CallerLineNumber] Int32 line = 0 ) => Main.Log( BepInEx.Logging.LogLevel.Info, data, member, line );
        internal static void LogM( System.Object data, [CallerMemberName] String member = "", [CallerLineNumber] Int32 line = 0 ) => Main.Log( BepInEx.Logging.LogLevel.Message, data, member, line );
        internal static void LogD( System.Object data, [CallerMemberName] String member = "", [CallerLineNumber] Int32 line = 0 ) => Main.Log( BepInEx.Logging.LogLevel.Debug, data, member, line );
        internal static void LogW( System.Object data, [CallerMemberName] String member = "", [CallerLineNumber] Int32 line = 0 ) => Main.Log( BepInEx.Logging.LogLevel.Warning, data, member, line );
        internal static void LogE( System.Object data, [CallerMemberName] String member = "", [CallerLineNumber] Int32 line = 0 ) => Main.Log( BepInEx.Logging.LogLevel.Error, data, member, line );
        internal static void LogF( System.Object data, [CallerMemberName] String member = "", [CallerLineNumber] Int32 line = 0 ) => Main.Log( BepInEx.Logging.LogLevel.Fatal, data, member, line );
        internal static Int32 logCounter = 0;
        internal static void LogC( [CallerMemberName] String member = "", [CallerLineNumber] Int32 line = 0 ) => Main.Log( BepInEx.Logging.LogLevel.Info, member + ": " + line + ":: " + logCounter++, member, line );
    }
}
// Thought organization:::

// SKIN STUFF
//  Need to hook in to CharacterModel.UpdateRendererMaterials and check for 
//
//
//
//
//
//
//
//
//
//379.63%
//
//
//
//
//
//
//
//
//


//Particle builder
//Builders for other material types
//Remake all wisp vfx with improved materials
//Create custom materials




//For next release:



// TOD: Transparency on barrier portion of boss hp bar
// TOD: Archaic wisp add to wisp family event


/*
[Warning:Rogue Wisp]
MinMaxCurve
[Warning:Rogue Wisp] MinMaxGradient
[Warning:Rogue Wisp] ParticleSystemSimulationSpace
[Warning:Rogue Wisp] Transform
[Warning:Rogue Wisp] ParticleSystemScalingMode
[Warning:Rogue Wisp] ParticleSystemEmitterVelocityMode
[Warning:Rogue Wisp] ParticleSystemStopAction
[Warning:Rogue Wisp] ParticleSystemCullingMode
[Warning:Rogue Wisp] ParticleSystemRingBufferMode
[Warning:Rogue Wisp] Vector2
[Warning:Rogue Wisp] ParticleSystemEmissionType
[Warning:Rogue Wisp] Vector3
[Warning:Rogue Wisp] ParticleSystemShapeType
[Warning:Rogue Wisp] ParticleSystemShapeMultiModeValue
[Warning:Rogue Wisp] ParticleSystemMeshShapeType
[Warning:Rogue Wisp] Mesh
[Warning:Rogue Wisp] MeshRenderer
[Warning:Rogue Wisp] SkinnedMeshRenderer
[Warning:Rogue Wisp] Sprite
[Warning:Rogue Wisp] SpriteRenderer
[Warning:Rogue Wisp] Texture2D
[Warning:Rogue Wisp] ParticleSystemShapeTextureChannel
[Warning:Rogue Wisp] ParticleSystemInheritVelocityMode
[Warning:Rogue Wisp] ParticleSystemGameObjectFilter
[Warning:Rogue Wisp] ParticleSystemNoiseQuality
[Warning:Rogue Wisp] ParticleSystemCollisionType
[Warning:Rogue Wisp] ParticleSystemCollisionMode
[Warning:Rogue Wisp] LayerMask
[Warning:Rogue Wisp] ParticleSystemCollisionQuality
[Warning:Rogue Wisp] ParticleSystemOverlapAction
[Warning:Rogue Wisp] ParticleSystem
[Warning:Rogue Wisp] ParticleSystemAnimationMode
[Warning:Rogue Wisp] ParticleSystemAnimationTimeMode
[Warning:Rogue Wisp] ParticleSystemAnimationType
[Warning:Rogue Wisp] UVChannelFlags
[Warning:Rogue Wisp] Light
[Warning:Rogue Wisp] ParticleSystemTrailMode
[Warning:Rogue Wisp] ParticleSystemTrailTextureMode
*/


// Future plans and shit

// TOD: IDRS not showing on server
// TOD: Utility sounds (unsure what to do here)
// TOD: Little Disciple and Will O Wisp color should change with skin
// TOD: Body Animation smoothing params
// TOD: Rewrite secondary
// TOD: Character lobby description
// TOD: Customize crosshair
// TOD: Readme lore sillyness
// TOD: Effects obscuring vision
// TOD: Effect brightness settings
// TOD: Animation cleanup and improvements
// TOD: Null ref on kill enemy with primary when client
// TOD: Pod UV mapping (need to duplicate mesh)
// TOD: Capacitor limb issue
// TOD: Custom CharacterMain
// TOD: Broach shield location is weird
// TOD: Sounds for Special
// TOD: Meteor ignores wisp
// TOD: GPU particlesystem
// TOD: Clipping through ground still...

// ERRORS:
/*
[Info   : Unity Log] <style=cDeath><sprite name="Skull" tint=1> Close! <sprite name="Skull" tint=1></color>
[Info   : Unity Log] Could not save RunReport P:/Program Files (x86)/Steam/steamapps/common/Risk of Rain 2/Risk of Rain 2_Data/RunReports/PreviousRun.xml: The ' ' character, hexadecimal value 0x20, cannot be included in a name.
[Warning: Unity Log] Parent of RectTransform is being set with parent property. Consider using the SetParent method instead, with the worldPositionStays argument set to false. This will retain local orientation and scale rather than world orientation and scale, which can prevent common UI scaling issues.
[Error  : Unity Log] NullReferenceException
Stack trace:
UnityEngine.Transform.get_position () (at <f2abf40b37c34cf19b7fd98865114d88>:0)
RoR2.UI.CombatHealthBarViewer.UpdateAllHealthbarPositions (UnityEngine.Camera sceneCam, UnityEngine.Camera uiCam) (at <eae781bd93824da1b7902a2b6526887c>:0)
RoR2.UI.CombatHealthBarViewer.LayoutForCamera (RoR2.UICamera uiCamera) (at <eae781bd93824da1b7902a2b6526887c>:0)
RoR2.UI.CombatHealthBarViewer.SetLayoutHorizontal () (at <eae781bd93824da1b7902a2b6526887c>:0)
UnityEngine.UI.LayoutRebuilder.<Rebuild>m__3 (UnityEngine.Component e) (at <adbbd84a6a874fb3bb8dd55fe88db73d>:0)
UnityEngine.UI.LayoutRebuilder.PerformLayoutControl (UnityEngine.RectTransform rect, UnityEngine.Events.UnityAction`1[T0] action) (at <adbbd84a6a874fb3bb8dd55fe88db73d>:0)
UnityEngine.UI.LayoutRebuilder.Rebuild (UnityEngine.UI.CanvasUpdate executing) (at <adbbd84a6a874fb3bb8dd55fe88db73d>:0)
UnityEngine.UI.CanvasUpdateRegistry.PerformUpdate () (at <adbbd84a6a874fb3bb8dd55fe88db73d>:0)
UnityEngine.Canvas:SendWillRenderCanvases()

    */

