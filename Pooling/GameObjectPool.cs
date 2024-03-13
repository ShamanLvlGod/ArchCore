using UnityEngine;

namespace ArchCore.Pooling
{
	public class GameObjectPool<T> : BaseObjectPool<T> where T : MonoBehaviour, IPoolObject
	{
		private readonly T sample;
		private readonly Transform storeLocation;

		public GameObjectPool(int minSize, int maxSize, T sample, Transform storeLocation) : base(minSize, maxSize)
		{
			this.sample = sample;
			this.storeLocation = storeLocation;

			Fill();
		}

		protected sealed override void Fill()
		{
			sample.PoolInstantiate += Instantiate;

			for (int j = 0; j < minSize; j++)
			{
				T p = Object.Instantiate(sample, storeLocation, true);
				p.gameObject.SetActive(false);
				p.PoolInstantiate += Instantiate;
				p.PoolDestroy += () => Destroy(p);
				pool.Push(p);
				p.InPool = true;
				p.OnCreated();
			}
		}

		public override T Instantiate()
		{
			return Instantiate(Vector3.zero);
		}

		public T Instantiate(Transform parent)
		{
			return Instantiate(Vector3.zero, parent);
		}

		public T Instantiate(Vector3 position, Transform parent = null)
		{
			T retObj;

			if (pool.Count > 0)
			{
				retObj = pool.Pop();
			}
			else
			{
				retObj = Object.Instantiate(sample);
				retObj.PoolInstantiate += Instantiate;
				retObj.PoolDestroy += () => Destroy(retObj);
				retObj.OnCreated();
			}

			retObj.gameObject.SetActive(true);
			retObj.transform.SetParent(parent);
			retObj.transform.position = position;
			retObj.InPool = false;
			retObj.OnSpawned();

			return retObj;
		}

		protected override void Destroy(T poolObject)
		{
			if (poolObject.InPool) return;

			if (pool.Count < maxSize)
			{
				poolObject.gameObject.SetActive(false);
				poolObject.transform.SetParent(storeLocation);
				poolObject.transform.position = new Vector3(-100, -100, 0);

				poolObject.InPool = true;
				pool.Push(poolObject);
				poolObject.OnDespawned();
			}
			else
			{
				if (poolObject != null && poolObject.gameObject != null)
				{
					poolObject.OnDespawned();
					poolObject.OnDestroyed();
					Object.Destroy(poolObject.gameObject);
				}
			}
		}
	}
}