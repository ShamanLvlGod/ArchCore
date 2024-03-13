using System;
using System.Collections;
using ArchCore.Utils.Executions;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace ArchCore.AssetManagement.Executions
{
    public abstract class ReturnAssetExe<T> : BaseExecution where T : Object
    {
        public event Action<T> OnComplete; 
        public abstract T Asset { get; }

        public ReturnAssetExe()
        {
            OnExecutionEnd += () => OnComplete?.Invoke(Asset);
        }
    }

    public class AssetReadyExe<T> : ReturnAssetExe<T> where T : Object
    {
        private T asset;
        public override T Asset => asset;

        public void SetAsset(T asset)
        {
            this.asset = asset;
            Complete();
        }
        
    }
    
    public class AssetLoadExe : ReturnAssetExe<Object>
    {
        public override Object Asset => asset.Asset;
        
        private readonly AssetReference asset;

        public AssetLoadExe(AssetReference asset)
        {
            this.asset = asset;
        }

        protected override IEnumerator ExecutionProcess()
        {
            if (asset.Asset)
            {
                Complete();
                yield break;
            }
            asset.LoadAssetAsync<Object>().Completed += OpOnCompleted;
            
        }

        private void OpOnCompleted(AsyncOperationHandle<Object> obj)
        {
            Complete();
        }
    }
    
    public class AssetLoadExe<T> : ReturnAssetExe<T> where T : Component
    {
        public override T Asset => asset.Asset;
        
        private readonly ComponentReference<T> asset;

        public AssetLoadExe(ComponentReference<T> asset)
        {
            this.asset = asset;
        }

        protected override IEnumerator ExecutionProcess()
        {
            if (asset.Asset)
            {
                Complete();
                yield break;
            }
            asset.LoadAssetAsync().Completed += OpOnCompleted;
        }

        private void OpOnCompleted(AsyncOperationHandle<T> obj)
        {
            Complete();
        }
    }
    
    public class PathAssetLoadExe<T> : ReturnAssetExe<T> where T : Object
    {
        public override T Asset => asset;
        private T asset;
        private readonly string path;

        public PathAssetLoadExe(string path)
        {
            this.path = path;
        }

        protected override IEnumerator ExecutionProcess()
        {
            Addressables.LoadAssetAsync<T>(path).Completed += OpOnCompleted;
            yield break;
        }

        private void OpOnCompleted(AsyncOperationHandle<T> obj)
        {
            asset = obj.Result;
            Complete();
        }
        
    }
    
    public class ComponentPathAssetLoadExe<T> : ReturnAssetExe<T> where T : Component
    {
        public override T Asset => asset;

        private readonly string path;
        private T asset;

        public ComponentPathAssetLoadExe(string path)
        {
            this.path = path;
        }

        protected override IEnumerator ExecutionProcess()
        {
            Addressables.LoadAssetAsync<GameObject>(path).Completed += OpOnCompleted;
            yield break;
        }

        private void OpOnCompleted(AsyncOperationHandle<GameObject> obj)
        {
            asset = obj.Result.GetComponent<T>();
            Complete();
        }
        
    }
    
    public class YieldAsyncOperationExe<T> : BaseExecution
    {
        public T Result { get; private set; }
        
        public YieldAsyncOperationExe(AsyncOperationHandle<T> operationHandle)
        {
            operationHandle.Completed += OpOnCompleted;
        }
        
        private void OpOnCompleted(AsyncOperationHandle<T> obj)
        {
            Result = obj.Result;
            Complete();
        }
        
    }
}