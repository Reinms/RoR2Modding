﻿namespace Sniper.Modules
{
    using ReinCore;
    using RoR2;
    using Sniper.Components;
    using System;
    using UnityEngine.Networking;

    internal static class NetworkModule
    {
        internal static void SetupNetworking()
        {
            if( !NetworkCore.RegisterMessageType<ResetSkillsMessage>() )
            {
#if ASSERT
                Log.Error( "Failed to register network message for skill resets" );
#endif
            }
        }


        internal struct ResetSkillsMessage : INetMessage
        {
            internal ResetSkillsMessage( SniperCharacterBody body )
            {
                this.body = body;
            }

            void INetMessage.OnRecieved() => CatalogModule.ResetSkills( this.body, false );
            void ISerializableObject.Serialize( NetworkWriter writer ) => writer.Write( this.body.networkIdentity );
            void ISerializableObject.Deserialize( NetworkReader reader ) => this.body = reader.ReadNetworkIdentity()?.GetComponent<SniperCharacterBody>();

            private SniperCharacterBody body;
        }
    }
}