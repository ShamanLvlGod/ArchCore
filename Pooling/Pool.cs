using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ArchCore.AssetManagement;
using ArchCore.Utils;
using ArchCore.Utils.Observables;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Assertions;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using Object = UnityEngine.Object;

namespace ArchCore.Pooling
{
	public class Pool
	{
		public SwitchObservableEvent OnReady { get; }
		
		private readonly Transform storeLocation;
		private readonly Dictionary<object, Func<object>> createActions;
		private MethodInfo registerGameObjectPool;
		private SwitchObservableEventToken token;

		public Pool(Transform storeLocation, params PoolData[] poolDatas)
		{
			
			
			OnReady = new SwitchObservableEvent(out token);
			this.storeLocation = storeLocation;
			createActions = new Dictionary<object, Func<object>>();
			Fill(poolDatas);

			
		}

		public void Append(params PoolData[] poolDatas)
		{
			Fill(poolDatas);
		}

		private int waitersCount = 0;
		private int doneCount = 0;

		void CompleteWaiter()
		{
			doneCount++;
			if(doneCount >= waitersCount)
				token.Call(true);
		}

		void Fill(PoolData[] poolDataList)
		{
			token.Call(false);
			registerGameObjectPool = GetType().GetMethod("RegisterGameObjectPool", BindingFlags.Instance | BindingFlags.NonPublic);

			waitersCount += poolDataList.Length;
			foreach (PoolData data in poolDataList)
			{
				if(data.asset.Asset)
					Register(data.asset, data.asset.Asset as GameObject, data.minSize, data.maxSize);
				else
					data.asset.LoadAssetAsync<GameObject>().Completed += r => Register(data.asset, r.Result, data.minSize, data.maxSize);
			}
		}

		private void Register(AssetReference assetReference, GameObject asset, int minSize, int maxSize)
		{
			if (!asset) throw new Exception($"No asset found!");

			var poolObject = asset.GetComponent<IPoolObject>();
			
			Type rtype = poolObject.GetType();
			Type infs = rtype.GetInterfaces().First(x =>
				x.IsGenericType &&
				x.GetGenericTypeDefinition() == typeof(IPoolObject<>));
	
			if(infs == null) throw new Exception($"Incapable type {rtype} should implement from IPoolObject<>!");

			while (infs.GenericTypeArguments[0] != rtype)
			{
				rtype = rtype.BaseType;
			}

			string addressableName = "";
			IList<IResourceLocation> locations = ResourceKeyUtility.GetResourceLocations(assetReference.RuntimeKey);
			foreach (var loc in locations)
			{
				addressableName = loc.PrimaryKey;
			}
			
			registerGameObjectPool.MakeGenericMethod(rtype).Invoke(this, new object[]{minSize, maxSize, poolObject, new object[] { assetReference.RuntimeKey, addressableName }});
			CompleteWaiter();
		}

		private void RegisterGameObjectPool<T>(int minSize, int maxSize, T asset, object[] keys) where T : MonoBehaviour, IPoolObject<T>
		{
			var pool = new GameObjectPool<T>(minSize, maxSize, asset, storeLocation);
			Func<object> instantiation = () => pool.Instantiate();
			foreach (var key in keys)
			{
				if (createActions.ContainsKey(key))
				{
					Debug.LogError($"Pool already contains object with key {key}");
					continue;
				}
				createActions[key] = instantiation;
			}
		}
	
	
		public T Instantiate<T>(object key) where T : IPoolObject
		{
			if (key is IKeyEvaluator e)
				key = e.RuntimeKey;
			
			if (!createActions.ContainsKey(key))
			{
				Debug.LogError($"No pool for asset reference {key}.");
				return default;
			}
			
			return (T)createActions[key]();
		}
		
	}

	
}