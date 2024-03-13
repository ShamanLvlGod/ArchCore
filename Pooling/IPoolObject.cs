using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArchCore.Pooling
{
	public interface IPoolObject
	{
		bool InPool { get; set; }

		event Action PoolDestroy;
		event Func<IPoolObject> PoolInstantiate;

		void Destroy();
		
		void OnCreated();
		void OnDestroyed();
		void OnSpawned();
		void OnDespawned();
		
	}
	
	public interface IPoolObject<out T> : IPoolObject where T : IPoolObject<T>
	{
		T GetPooledCopy();
	}

}
