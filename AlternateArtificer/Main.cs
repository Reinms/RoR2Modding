﻿
namespace AlternativeArtificer
{
    using BepInEx;
    using RoR2;
    using System;
    using UnityEngine;
    using R2API;
    using R2API.Utils;
    using R2API.AssetPlus;
    using AlternateArtificer.SelectablePassive;
    using AlternativeArtificer.States.Main;

    [R2APISubmoduleDependency(nameof(ItemAPI),nameof(AssetPlus),nameof(EffectAPI),nameof(PrefabAPI),nameof(LoadoutAPI))]
    [BepInDependency( "com.bepis.r2api" )]
    [BepInIncompatibility( "com.examplemodder.ArtificerRangeTeleport" )]
    [BepInIncompatibility( "com.examplemodder.ArtificerToggleHover" )]
    [BepInIncompatibility( "com.bongopd.ArtGlassCannon"  )]
    [BepInIncompatibility( "com.ThunderDownUnder.SolidIcewall" )]
    [BepInIncompatibility( "com.Raus.IonUtility")]
    [BepInIncompatibility( "com.PallesenProductions.ExpandedSkills" )]
    [BepInPlugin( "com.ReinThings.AltArti", "Rein-AlternativeArtificer", "1.0.5" )]
    public partial class Main : BaseUnityPlugin
    {
        private GameObject artiBody;
        private SkillLocator artiSkillLocator;
        private void Awake()
        {
            RegisterSkillTypes();
            try
            {
                var serType = new EntityStates.SerializableEntityStateType( typeof( States.Main.AltArtiPassive ) );
                var str = serType.stateType.ToString();
            } catch
            {
                base.Logger.LogError( "AlternativeArtificer has been disabled due to a missing SubModule. Please check R2API installation" );
                return;
            }

            base.Logger.LogInfo( "EntityAPI found, loading" );

#if REIN
            var wildPrint = Resources.Load<GameObject>("Prefabs/NetworkedObjects/DuplicatorWild").GetComponent<ShopTerminalBehavior>();
            wildPrint.bannedItemTag = ItemTag.Any;
#endif



            artiBody = Resources.Load<GameObject>( "Prefabs/CharacterBodies/MageBody" );
            artiSkillLocator = artiBody.GetComponent<SkillLocator>();

            DoBuffs();

            EditModel();
            EditSkills();
            EditComponents();
            EditProjectiles();

            CreateProjectiles();
            DoEffects();
            DoText();

            base.Logger.LogInfo( "Loaded successfully" );
        }

        private void OnEnable()
        {
            AddHooks();
            //base.Logger.LogInfo( "Enabled successfully" );
        }
        private void OnDisable()
        {
            RemoveHooks();
            //base.Logger.LogInfo( "Disabled successfully" );
        }
        private void FixedUpdate()
        {
            RoR2Application.isModded = true;
        }

        private void Start()
        {
            var nova = EntityStates.EntityState.Instantiate( new EntityStates.SerializableEntityStateType( typeof( EntityStates.Mage.Weapon.ChargeNovabomb ) ) );
            var ice = EntityStates.EntityState.Instantiate( new EntityStates.SerializableEntityStateType( typeof( EntityStates.Mage.Weapon.ChargeIcebomb ) ) );

            UnityEngine.Object.Destroy( ((EntityStates.Mage.Weapon.ChargeNovabomb)nova).chargeEffectPrefab.GetComponent<EffectComponent>() );
            UnityEngine.Object.Destroy( ((EntityStates.Mage.Weapon.ChargeNovabomb)ice).chargeEffectPrefab.GetComponent<EffectComponent>() );
        }
    }
}

//BUGS
// TODO: Lose the current passive state when revived with Dio
// TODO: Ion surge space input