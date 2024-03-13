using System;
using UnityEngine;

namespace ArchCore.Pooling
{
	public abstract class PoolGameObject : MonoBehaviour, IPoolObject<PoolGameObject>
	{
		bool IPoolObject.InPool { get; set; }

		public event Action PoolDestroy;
		public event Func<IPoolObject> PoolInstantiate;

		void IPoolObject.OnCreated()
		{
			OnCreated();
		}

		void IPoolObject.OnDestroyed()
		{
			OnDestroyed();
		}

		void IPoolObject.OnSpawned()
		{
			OnSpawned();
		}

		void IPoolObject.OnDespawned()
		{
			OnDespawned();
		}

		protected virtual void OnCreated() {}

		protected virtual void OnDestroyed() {}
		
		protected virtual void OnSpawned() {}
		
		protected virtual void OnDespawned() {}


		public void Destroy()
		{
			PoolDestroy?.Invoke();
		}

		public PoolGameObject GetPooledCopy()
		{
			return (PoolGameObject)PoolInstantiate?.Invoke();
		}	
	}

}