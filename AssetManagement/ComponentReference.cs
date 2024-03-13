using System;
using ArchCore.Utils;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ArchCore.AssetManagement
{
    public class ComponentReference<TComponent> : AssetReference where TComponent : Component
    {
        public ComponentReference(string guid) : base(guid)
        {
        }
   

        public new virtual AsyncOperationHandle<TComponent> InstantiateAsync(Vector3 position, Quaternion rotation, Transform parent = null)
        {
            return Addressables.ResourceManager.CreateChainOperation<TComponent, GameObject>(base.InstantiateAsync(position, Quaternion.identity, parent), GameObjectReady);
        }
   
        public new virtual AsyncOperationHandle<TComponent> InstantiateAsync(Transform parent = null, bool instantiateInWorldSpace = false)
        {
            return Addressables.ResourceManager.CreateChainOperation<TComponent, GameObject>(base.InstantiateAsync(parent, instantiateInWorldSpace), GameObjectReady);
        }      
        
        public new virtual AsyncOperationHandle<TComponent> LoadAssetAsync()
        {
            return Addressables.ResourceManager.CreateChainOperation<TComponent, GameObject>(base.LoadAssetAsync<GameObject>(), GameObjectReady);
        }
        
//        public override AsyncOperationHandle<TObject> LoadAssetAsync<TObject>()
//        {
//            return Addressables.ResourceManager.CreateChainOperation<TObject, TComponent>(LoadAssetAsync(), F<TObject>);
//            return default(AsyncOperationHandle<TObject>);
//        }
//
//        private AsyncOperationHandle<TObject> F<TObject>(AsyncOperationHandle<TComponent> arg)// where TObject : Object
//        {
//            if(arg.Result is TObject x)
//            return Addressables.ResourceManager.CreateCompletedOperation(x, string.Empty); 
//        }

        protected AsyncOperationHandle<TComponent> GameObjectReady(AsyncOperationHandle<GameObject> arg)
        {
            component = arg.Result.GetComponent<TComponent>();
            return Addressables.ResourceManager.CreateCompletedOperation(component, string.Empty);
        }

        public override bool ValidateAsset(Object obj)
        {
            var go = obj as GameObject;
            return go != null && go.GetComponent<TComponent>() != null;
        }
    
        public override bool ValidateAsset(string path)
        {
#if UNITY_EDITOR
            //this load can be expensive...
            var go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            return go != null && go.GetComponent<TComponent>() != null;
#else
            return false;
#endif
        }

        protected TComponent component;
        
        public new virtual TComponent Asset {
            get
            {
                if (component) return component;

                if (!base.Asset)
                    return null;

                component = ((GameObject)base.Asset).GetComponent<TComponent>();
                
                return component;
            }
        }
    }



}
