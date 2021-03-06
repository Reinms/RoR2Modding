﻿namespace Rein.Sniper.UI.Components
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;

    using Mono.Cecil.Cil;

    using MonoMod.Utils;

    using RoR2;
    using RoR2.UI;

    using Rein.Sniper.Components;
    using Rein.Sniper.Enums;
    using Rein.Sniper.Modules;

    using UnityEngine;

    public class SniperCrosshairController : RoR2.UI.CrosshairController
    {
        static SniperCrosshairController()
        {
            //var dmd1 = new DynamicMethodDefinition( "base_Awake<<<SniperCrosshairController", null, new[] { typeof(CrosshairController) } );
            //var dmd2 = new DynamicMethodDefinition( "get_hudElement<<<SniperCrosshairController", typeof(HudElement), new[] { typeof(CrosshairController) } );
            //var allflags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
            //var ilp1 = dmd1.GetILProcessor();
            //ilp1.Emit( OpCodes.Jmp, typeof( CrosshairController ).GetMethod( "Awake", allflags ) );
            //ilp1.Emit( OpCodes.Ret );
            //var ilp2 = dmd2.GetILProcessor();
            //ilp2.Emit( OpCodes.Ldarg_0 );
            //ilp2.Emit( OpCodes.Ldfld, typeof( CrosshairController ).GetField( "hudElement", allflags ) );
            //ilp2.Emit( OpCodes.Ret );

            //base_Awake = (Action<CrosshairController>)dmd1.Generate().CreateDelegate<Action<CrosshairController>>();
            //base_hudElement = (Func<CrosshairController, HudElement>)dmd2.Generate().CreateDelegate<Func<CrosshairController, HudElement>>();
        }
        //private static Action<CrosshairController> base_Awake;
        //private static Func<CrosshairController,HudElement> base_hudElement;



        [HideInInspector]
        [SerializeField]
        private PartialScopeController partialScope;
        [HideInInspector]
        [SerializeField]
        private FullScopeController fullScope;
        [HideInInspector]
        [SerializeField]
        private ReloadIndicatorController reloadIndicator;


        [HideInInspector]
        [SerializeField]
        private NibController topNib;
        [HideInInspector]
        [SerializeField]
        private NibController leftNib;
        [HideInInspector]
        [SerializeField]
        private NibController rightNib;
        [HideInInspector]
        [SerializeField]
        private GameObject dot;
        [HideInInspector]
        [SerializeField]
        private RectTransform main;


        private SniperCharacterBody body;
        //private HudElement hudElem;
        private GenericSkill primarySlot;
        private GenericSkill secondarySlot;


        internal Boolean partialScopeActive
        {
            set
            {
                this.partialScope.active = value;
            }
        }

        internal Boolean fullScopeActive
        {
            set
            {
                //Log.WarningT( $"fullon {value}" );
                this.fullScope.active = value;
            }
        }

        internal Single range
        {
            set
            {
                //Log.WarningT( $"range {value}" );
                this.fullScope.distance = value;
            }
        }

        internal Single charge
        {
            set
            {
                this.partialScope.charge = value;
            }
        }

        internal Single scopeReadyFrac
        {
            set
            {
                this.partialScope.readyFrac = value;
            }
        }

        internal Boolean scopeReady
        {
            set
            {
                this.partialScope.ready = value;
            }
        }

        internal Int32 maxSecondaryStock
        {
            set
            {
                //Log.WarningT( $"M2Max {value}" );
                this.partialScope.maxStock = (Byte)value;
            }
        }

        internal Int32 secondaryStock
        {
            set
            {
                //Log.WarningT( $"M2Stock {value}" );
                this.partialScope.currentStock = (Byte)value;
            }
        }

        internal Int32 reloadMax
        {
            set => this.reloadIndicator.maxStock = (Byte)value;
        }

        internal Int32 reloadCurrent
        {
            set => this.reloadIndicator.currentStock = (Byte)value;
        }

        internal ReloadTier reloadTier
        {
            set => this.reloadIndicator.reloadTier = value;
        }



        protected new void Awake()
        {
            base.Awake();
            //this.hudElem = base_hudElement( this );
            if( base.hudElement == null )
            {
                Log.ErrorT( "Null hud" );
                return;
            }

            this.topNib.type = ConfigModule.topNib;
            this.leftNib.type = ConfigModule.leftNib;
            this.rightNib.type = ConfigModule.rightNib;
            this.dot.SetActive(ConfigModule.showCenterDot);
            this.main.localEulerAngles = new Vector3(0f, 0f, ConfigModule.rotate180 ? 180f : 0f);
            this.main.localScale *= new Vector2(ConfigModule.scale, ConfigModule.scale);
        }

        protected void Start()
        {
            // CLEANUP: In prefab set these to enabled
            this.partialScope.gameObject.SetActive( true );
            this.fullScope.gameObject.SetActive( true );
            this.body = base.hudElement.targetCharacterBody as SniperCharacterBody;
            if( !this.body || this.body is null || !this.body.master.hasEffectiveAuthority )
            {
                this.body = null;
                base.StartCoroutine( this.Hookup() );
            } else
            {
                this.body.sniperCrosshair = this;
            }
        }

        private IEnumerator Hookup()
        {
            while( ( this.body = base.hudElement.targetCharacterBody as SniperCharacterBody ) is null || !( this.body?.master?.hasEffectiveAuthority ?? false ) )
                yield return new WaitForEndOfFrame();

            this.body.sniperCrosshair = this;
        }

        //private void OnValidate()
        //{
        //    this.partialScope = base.transform.Find( "PartialScope" ).GetComponent<PartialScopeController>();
        //    this.reloadIndicator = base.transform.Find( "ReloadIndHolder" ).GetComponent<ReloadIndicatorController>();
        //    this.fullScope = base.transform.Find( "FullScope" ).GetComponent<FullScopeController>();
        //}
    }
}