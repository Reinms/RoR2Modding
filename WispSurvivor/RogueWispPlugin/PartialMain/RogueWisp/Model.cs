﻿#if ROGUEWISP
namespace Rein.RogueWispPlugin
{

    internal partial class Main
    {
        partial void RW_EditModel();
        partial void RW_CreateModelSkins();
        partial void RW_EditModelMesh();
        partial void RW_SetupChildLocator();
        partial void RW_SetupIDRS();
        partial void RW_SetupHurtBoxes();
        partial void RW_SetupChildren();

        partial void RW_Model()
        {
            this.RW_EditModel();
            this.RW_CreateModelSkins();
            this.RW_EditModelMesh();
            this.RW_SetupChildLocator();
            this.RW_SetupHurtBoxes();
            this.RW_SetupChildren();
            this.RW_SetupIDRS();
        }
    }

}
#endif