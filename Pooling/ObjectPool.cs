﻿using System;
using UnityEngine;

namespace ArchCore.Pooling
{
	[Obsolete]
	public class ObjectPool<T> : BaseObjectPool<T> where T : class, IPoolObject, new()
	{
		public ObjectPool(int minSize, int maxSize) : base(minSize, maxSize)
		{
			Fill();
		}
		

		private T CreateNew()
		{
			return new T();
		}

		protected sealed override void Fill()
		{
			for (int j = 0; j < minSize; j++)
			{
				T p = CreateNew();
				p.InPool = true;
				pool.Push(p);
				p.PoolDestroy += () => Destroy(p);
				p.PoolInstantiate += () => (IPoolObject) Instantiate();
			}
		}
		
		public override T Instantiate()
		{
			T retObj;

			if (pool.Count > 0)
			{
				retObj = pool.Pop();
			}
			else
			{
				retObj = CreateNew();
				retObj.PoolDestroy += () => Destroy(retObj);
				retObj.PoolInstantiate += () => (IPoolObject) Instantiate();
			}

			retObj.InPool = false;
			return retObj;
		}

		protected override void Destroy(T poolObject)
		{
			if(poolObject.InPool) return;
			
			if (pool.Count < maxSize)
			{
				poolObject.InPool = true;
				pool.Push(poolObject);
			}
			else
			{

			}
		}
	}
}


