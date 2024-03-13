using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace ArchCore.AssetManagement
{
    
    public class SingularAssetsProvider
    {
        private static Dictionary<object, Object> database = new Dictionary<object, Object>();
        
        public static AsyncOperationHandle<T> LoadAssetAsync<T>(IKeyEvaluator assetReference)// where T : Object
        {
            return Addressables.ResourceManager.CreateChainOperation(Addressables.LoadAssetAsync<T>(assetReference.RuntimeKey), 
                r => Register(assetReference.RuntimeKey, r));
        }

        private static AsyncOperationHandle<T> Register<T>(object key, AsyncOperationHandle<T> obj)// where T : Object
        {
            
            database[key] = obj.Result as Object;
            return obj;
        }

        public static T GetAsset<T>(IKeyEvaluator assetReference) where T : Object
        {
            if (!database.ContainsKey(assetReference.RuntimeKey)) return null;
            
            return database[assetReference.RuntimeKey] as T;
        }
    }
    
    

}