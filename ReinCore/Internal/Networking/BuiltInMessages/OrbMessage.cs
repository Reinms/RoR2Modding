﻿namespace ReinCore
{
    using System;
    using RoR2;
    using UnityEngine;
    using UnityEngine.Networking;

    internal struct OrbMessage : INetMessage
    {
        public void Serialize( NetworkWriter writer )
        {
        }

        public void Deserialize( NetworkReader reader )
        {
        }

        public void OnRecieved() => throw new NotImplementedException();
    }
}
