using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace ArchCore.AssetManagement
{
    public class SingularComponentAssetReference<TComponent> : 
        ComponentReference<TComponent> where TComponent : Component
    {
        public SingularComponentAssetReference(string guid) : base(guid)
        {
        }
        
        
        public override AsyncOperationHandle<TComponent> LoadAssetAsync()
        {
            return Addressables.ResourceManager.CreateChainOperation(SingularAssetsProvider.LoadAssetAsync<GameObject>(this), GameObjectReady);
        }
        
        public override TComponent Asset {
            get
            {
                if (component) return component;

                component = SingularAssetsProvider.GetAsset<GameObject>(this)?.GetComponent<TComponent>();
                
                if (component) return component;
                
                if (!base.Asset)
                    return null;

                component = ((GameObject)((AssetReference)this).Asset).GetComponent<TComponent>();
                
                return component;
            }
        }
    }
    
    public class SingularAssetReference : 
        AssetReference
    {
        public SingularAssetReference(string guid) : base(guid)
        {
        }
        
        
        public override AsyncOperationHandle<TObject> LoadAssetAsync<TObject>()
        {
            return SingularAssetsProvider.LoadAssetAsync<TObject>(this);
        }

        private Object asset;
        
        public override Object Asset {
            get
            {
                if (base.Asset) return base.Asset;
                if (asset) return asset;

                asset = SingularAssetsProvider.GetAsset<Object>(this);

                return asset;
            }
        }

        public override AsyncOperationHandle<GameObject> InstantiateAsync(Transform parent = null, bool instantiateInWorldSpace = false)
        {
            throw new NotImplementedException();
        }

        public override AsyncOperationHandle<GameObject> InstantiateAsync(Vector3 position, Quaternion rotation, Transform parent = null)
        {
            throw new NotImplementedException();
        }
    }
    
    public class SingularAssetReference<TObject> : AssetReferenceT<TObject> where TObject : Object
    {
        public SingularAssetReference(string guid) : base(guid)
        {
        }
        
        
        public override AsyncOperationHandle<TObject> LoadAssetAsync()
        {
            return SingularAssetsProvider.LoadAssetAsync<TObject>(this);
        }

        private Object asset;
        
        public override Object Asset {
            get
            {
                if (base.Asset) return base.Asset;
                if (asset) return asset;

                asset = SingularAssetsProvider.GetAsset<Object>(this);

                return asset;
            }
        }
    }
}