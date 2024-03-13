using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArchCore.Pooling
{
	public abstract class BaseObjectPool<T> where T : IPoolObject
	{
		public readonly int minSize, maxSize;

		protected readonly Stack<T> pool;

		protected abstract void Fill();
		//protected abstract void Clear();
		public abstract T Instantiate();
		protected abstract void Destroy(T poolObject);

		protected BaseObjectPool(int minSize, int maxSize)
		{
			this.minSize = minSize;
			this.maxSize = maxSize;
			pool = new Stack<T>();
		}
	}

}
