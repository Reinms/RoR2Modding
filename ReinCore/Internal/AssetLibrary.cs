﻿using System;
using System.Collections.Generic;
using BepInEx;

namespace ReinCore
{
    internal static class AssetLibrary<TAsset> where TAsset : UnityEngine.Object
    {
        internal static void SetIndexType( Type indexType )
        {
            if( indexType == null ) throw new ArgumentNullException( nameof( indexType ) );
            if( !indexType.IsEnum ) throw new ArgumentException( String.Format( "Type {0} must be an enum", indexType.Name ), nameof( indexType ) );

            AssetsCore.indexAssociations[typeof( TAsset )] = indexType;
            AssetsCore.assetAssociations[indexType] = typeof( TAsset );
        }

        internal static TAsset GetAsset( Enum index )
        {
            if( !AssetsCore.MatchAssetIndexType( typeof( TAsset ), index.GetType() ) ) throw new ArgumentException( "Incorrect index type.", nameof(index) );
            var ind = index.GetValue<UInt64>();
            if( assets.TryGetValue( ind, out var asset ) )
            {
                return asset.value;
            } else
            {
                throw new KeyNotFoundException( String.Format( "The Key:{0} was not found.", index.GetName() ) );
            }
            return assets[ind].value;            
        }

        internal static Boolean CanGetAsset( Enum index )
        {
            if( !AssetsCore.MatchAssetIndexType( typeof( TAsset ), index.GetType() ) ) throw new ArgumentException( "Incorrect index type", nameof(index) );
            var ind = index.GetValue<UInt64>();
            return assets[ind].CanLoad();
        }

        internal static void AddAsset( AssetAccessor<TAsset> accessor )
        {
            assets[accessor.index.GetValue<UInt64>()] = accessor;
        }



        private static Dictionary<UInt64, AssetAccessor<TAsset>> assets = new Dictionary<UInt64, AssetAccessor<TAsset>>();
    }

}