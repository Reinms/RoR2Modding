﻿using System;
using BepInEx;
using RoR2;

namespace ReinCore
{
    public static class SpawnsCore
    {
        public static Boolean loaded { get; internal set; } = false;
        public static event MonsterEditDelegate monsterEdits;
        public static event InteractableEditDelegate interactableEdits;
        public static event FamilyEditDelegate familyEdits;
        public static event MiscEditDelegate miscEdits;

        public delegate void MonsterEditDelegate( ClassicStageInfo stageInfo, Run runInstance, DirectorCardCategorySelection monsterSelection );
        public delegate void InteractableEditDelegate( ClassicStageInfo stageInfo, Run runInstance, DirectorCardCategorySelection interactableSelection );
        public delegate void FamilyEditDelegate( ClassicStageInfo stageInfo, Run runInstance, ClassicStageInfo.MonsterFamily[] possibleFamilies );
        public delegate void MiscEditDelegate( ClassicStageInfo stageInfo, Run runInstance );



        static SpawnsCore()
        {
            HooksCore.on_RoR2_ClassicStageInfo_Awake += HooksCore_on_RoR2_ClassicStageInfo_Awake;
            loaded = true;
        }

        private static Accessor<ClassicStageInfo,DirectorCardCategorySelection> monsterCategories = new Accessor<ClassicStageInfo, DirectorCardCategorySelection>( "monsterCategories" );
        private static Accessor<ClassicStageInfo,DirectorCardCategorySelection> interactableCategories = new Accessor<ClassicStageInfo, DirectorCardCategorySelection>( "interactableCategories" );

        private static void HooksCore_on_RoR2_ClassicStageInfo_Awake( HooksCore.orig_RoR2_ClassicStageInfo_Awake orig, ClassicStageInfo self )
        {
            if( loaded )
            {
                monsterEdits?.Invoke( self, Run.instance, monsterCategories.Get( self ) );
                interactableEdits?.Invoke( self, Run.instance, interactableCategories.Get( self ) );
                familyEdits?.Invoke( self, Run.instance, self.possibleMonsterFamilies );
                miscEdits?.Invoke( self, Run.instance );
            }
            orig( self );
        }
    }
}