﻿namespace AlternativeArtificer
{
    using AlternateArtificer.SelectablePassive;
    using R2API;
    using R2API.Utils;
    using Rein.Properties;
    using RoR2;
    using RoR2.Skills;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public partial class Main
    {
        private HashSet<SkillDef> addedSkills = new HashSet<SkillDef>();
        private HashSet<SkillFamily> addedSkillFamilies = new HashSet<SkillFamily>();

        private void EditSkills()
        {
            artiSkillLocator.passiveSkill.enabled = false;

            SelectablePassive();
            SetupStateMachines();
            TempModSpecial();
        }

        private void SelectablePassive()
        {
            var primary = artiSkillLocator.primary;
            var secondary = artiSkillLocator.secondary;
            var utility = artiSkillLocator.utility;
            var special = artiSkillLocator.special;

            var passiveFamily = ScriptableObject.CreateInstance<SkillFamily>();
            var primaryFamily = primary.skillFamily;
            var secondaryFamily = secondary.skillFamily;
            var utilityFamily = utility.skillFamily;
            var specialFamily = special.skillFamily;

            var passive = primary;
            primary = secondary;
            secondary = utility;
            utility = special;
            special = artiBody.AddComponent<GenericSkill>();

            artiSkillLocator.primary = primary;
            artiSkillLocator.secondary = secondary;
            artiSkillLocator.utility = utility;
            artiSkillLocator.special = special;

            var envSuit = ScriptableObject.CreateInstance<PassiveSkillDef>();
            var elementalIntensity = ScriptableObject.CreateInstance<PassiveSkillDef>();

            envSuit.skillNameToken = artiSkillLocator.passiveSkill.skillNameToken;
            envSuit.skillDescriptionToken = artiSkillLocator.passiveSkill.skillDescriptionToken;
            envSuit.icon = artiSkillLocator.passiveSkill.icon;

            envSuit.stateMachineDefaults = new PassiveSkillDef.StateMachineDefaults[1]
            {
                new PassiveSkillDef.StateMachineDefaults
                {
                    machineName = "Body",
                    initalState = new EntityStates.SerializableEntityStateType( typeof( EntityStates.Mage.MageCharacterMain ) ),
                    mainState = new EntityStates.SerializableEntityStateType( typeof( EntityStates.Mage.MageCharacterMain ) ),
                    defaultInitalState = new EntityStates.SerializableEntityStateType( typeof( EntityStates.GenericCharacterMain ) ),
                    defaultMainState = new EntityStates.SerializableEntityStateType( typeof( EntityStates.GenericCharacterMain ) )
                    
                }
            };

            elementalIntensity.skillNameToken = "REIN_ALTARTI_PASSIVE_NAME";
            elementalIntensity.skillDescriptionToken = "REIN_ALTARTI_PASSIVE_DESC";
            var tex = Tools.LoadTexture2D( Rein.Properties.Resources.passive_2__1_ );
            elementalIntensity.icon = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height ), envSuit.icon.pivot );

            elementalIntensity.stateMachineDefaults = new PassiveSkillDef.StateMachineDefaults[1]
            {
                new PassiveSkillDef.StateMachineDefaults
                {
                    machineName = "Jet",
                    initalState = new EntityStates.SerializableEntityStateType( typeof( States.Main.AltArtiPassive ) ),
                    mainState = new EntityStates.SerializableEntityStateType( typeof( States.Main.AltArtiPassive ) ),
                    defaultInitalState = new EntityStates.SerializableEntityStateType( typeof( EntityStates.Idle ) ),
                    defaultMainState = new EntityStates.SerializableEntityStateType( typeof( EntityStates.Idle ) )
                }
            };


            elementalIntensity.applyVisuals = ( model ) =>
            {
                var renderInfos = model.baseRendererInfos;
                if( renderInfos.Length == 10 )
                {
                    ((SkinnedMeshRenderer)model.baseRendererInfos[9].renderer).sharedMesh = this.artiChangedMesh;
                    model.baseRendererInfos[2].renderer.gameObject.SetActive( false );
                    model.baseRendererInfos[3].renderer.gameObject.SetActive( false );
                    model.baseRendererInfos[5].renderer.gameObject.SetActive( false );
                    model.baseRendererInfos[6].renderer.gameObject.SetActive( false );
                } else
                {
                    ((SkinnedMeshRenderer)model.baseRendererInfos[3].renderer).sharedMesh = this.artiChangedMesh;
                    model.baseRendererInfos[0].renderer.gameObject.SetActive( false );
                    model.baseRendererInfos[1].renderer.gameObject.SetActive( false );
                }

            };

            elementalIntensity.removeVisuals = ( model ) =>
            {
                var renderInfos = model.baseRendererInfos;
                if( renderInfos.Length == 10 )
                {
                    try
                    {
                        ((SkinnedMeshRenderer)model.baseRendererInfos[9].renderer).sharedMesh = this.artiDefaultMesh;
                    } catch { }
                    model.baseRendererInfos[2].renderer.gameObject.SetActive( true );
                    model.baseRendererInfos[3].renderer.gameObject.SetActive( true );
                    model.baseRendererInfos[5].renderer.gameObject.SetActive( true );
                    model.baseRendererInfos[6].renderer.gameObject.SetActive( true );
                } else
                {
                    ((SkinnedMeshRenderer)model.baseRendererInfos[3].renderer).sharedMesh = this.artiDefaultMesh;
                    model.baseRendererInfos[0].renderer.gameObject.SetActive( true );
                    model.baseRendererInfos[1].renderer.gameObject.SetActive( true );
                }
            };

            passiveFamily.variants = new SkillFamily.Variant[2]
            {
                new SkillFamily.Variant
                {
                    skillDef = envSuit,
                    unlockableName = "",
                    viewableNode = new ViewablesCatalog.Node( "envSuit" , false )
                },
                new SkillFamily.Variant
                {
                    skillDef = elementalIntensity,
                    unlockableName = "",
                    viewableNode = new ViewablesCatalog.Node( "elementalIntensity" , false )
                }
            };

            LoadoutAPI.AddSkillDef( envSuit );
            LoadoutAPI.AddSkillDef( elementalIntensity );
            LoadoutAPI.AddSkillFamily( passiveFamily );

            addedSkills.Add( envSuit );
            addedSkills.Add( elementalIntensity );
            addedSkillFamilies.Add( passiveFamily );

            passive.SetFieldValue<SkillFamily>( "_skillFamily", passiveFamily );
            primary.SetFieldValue<SkillFamily>( "_skillFamily", primaryFamily );
            secondary.SetFieldValue<SkillFamily>( "_skillFamily", secondaryFamily );
            utility.SetFieldValue<SkillFamily>( "_skillFamily", utilityFamily );
            special.SetFieldValue<SkillFamily>( "_skillFamily", specialFamily );
        }




        private void RegisterSkillTypes()
        {
            LoadoutAPI.AddSkill( typeof( States.Special.IonSurge ) );
            LoadoutAPI.AddSkill( typeof( States.Main.AltArtiPassive ) );
        }

        private void SetupStateMachines()
        {
            var stateOnHurt = artiBody.GetComponent<SetStateOnHurt>();
            var idles = stateOnHurt.idleStateMachine;
            Array.Resize<EntityStateMachine>( ref idles, 1 );
            stateOnHurt.idleStateMachine = idles;
        }






        private void TempModSpecial()
        {
            SkillFamily specialFamily = artiSkillLocator.special.skillFamily;

            var specialDef = specialFamily.variants[1].skillDef;
            specialDef.activationState = new EntityStates.SerializableEntityStateType( typeof( States.Special.IonSurge ) );

            specialDef.skillNameToken = "REIN_ALTARTI_LIGHTNING_SPECIAL_NAME";
            specialDef.skillDescriptionToken = "REIN_ALTARTI_LIGHTNING_SPECIAL_DESC";
        }
    }
}
