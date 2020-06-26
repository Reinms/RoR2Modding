﻿namespace Sniper.Components
{
    using System;

    using RoR2;
    using RoR2.UI;

    using Sniper.Data;
    using Sniper.States.Bases;

    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent( typeof( HudElement ), typeof( CrosshairController ) )]
#pragma warning disable CA1812 // Avoid uninstantiated internal classes
    internal class ScopeUIController : MonoBehaviour
#pragma warning restore CA1812 // Avoid uninstantiated internal classes
    {
        internal void HookUpComponents()
        {
            this.hudElement = base.GetComponent<HudElement>();
            this.crosshair = base.GetComponent<CrosshairController>();
        }

        [SerializeField]
        internal HudElement hudElement;
        [SerializeField]
        internal CrosshairController crosshair;
        [SerializeField]
        internal GameObject stockHolder = null;
        [SerializeField]
        internal UIStockController stockController = null;
        [SerializeField]
        internal GameObject stockChildPrefab = null;
        [SerializeField]
        internal HGTextMeshProUGUI rangeIndicator;
        [SerializeField]
        internal HGTextMeshProUGUI zoomIndicator;
        [SerializeField]
        internal Image chargeIndicator;
        [SerializeField]
        internal GameObject scopeActive = null;
        [SerializeField]
        internal GameObject scopeInactive = null;

        private Transform camTargetTransform;
        private Transform origParent;
        private Vector3 origPos;
        private Quaternion origRot;
        private readonly Transform scopeParent;



        private void Start()
        {
            this.body = this.hudElement.targetCharacterBody as SniperCharacterBody;
            this.body.scopeInstanceData.CrosshairCheckIn( this );
            this.camTargetTransform = this.body.aimOriginTransform;
            this.origParent = this.camTargetTransform.parent;
            this.origPos = this.camTargetTransform.localPosition;
            this.origRot = this.camTargetTransform.localRotation;
        }


        internal void StartZoomSession( ScopeBaseState state, ZoomParams zoomParams )
        {
            this.stateInstance = state;
            this.zoomParams = zoomParams;
            this.camTarget = this.stateInstance.cameraTarget;

            this.chargeIndicator.enabled = this.stateInstance.usesCharge;
            this.chargeIndicator.fillAmount = 0f;
            this.stockHolder?.SetActive( this.stateInstance.usesStock );
            this.scoped = false;
            this.OnScopedChange( false );

            if( this.stateInstance.usesStock )
            {
                // TODO: Implement stock based scoping
            }
        }

        internal void UpdateUI( Single zoom )
        {
            this.scoped = this.zoomParams.IsInScope( zoom );
            Single fov = this.zoomParams.GetFoV( zoom );
            if( this.camTarget )
            {
                this.camTarget.fovOverride = fov;
            }
            // TODO: Verify camera position for scope

            if( this.stateInstance != null && this.stateInstance.usesCharge && this.chargeIndicator != null && this.chargeIndicator.IsActive() )
            {
                this.chargeIndicator.fillAmount = this.stateInstance.currentCharge;
            }
        }

        internal void EndZoomSession()
        {
            this.ResetCamera();
            this.stateInstance = null;
        }

        private void ResetCamera()
        {
            this.camTarget.fovOverride = -1f;
            this.camTarget.aimMode = CameraTargetParams.AimType.Standard;
        }


        private ScopeBaseState stateInstance;
        private ZoomParams zoomParams;
        private SniperCharacterBody body;
        private CameraTargetParams camTarget;
        private Boolean scoped
        {
            get => this._scoped == true;
            set
            {
                if( value != this._scoped )
                {
                    this._scoped = value;
                    this.OnScopedChange( value );
                }
            }
        }
        private Boolean _scoped;
        private void OnScopedChange( Boolean enabled )
        {
            this.camTarget.aimMode = enabled ? CameraTargetParams.AimType.FirstPerson : CameraTargetParams.AimType.AimThrow;
            if( this.scopeActive != null )
            {
                this.scopeActive.SetActive( enabled );
            }

            if( this.scopeInactive != null )
            {
                this.scopeInactive.SetActive( !enabled );
            }
        }

    }
}
