using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArchCore.Pooling
{
    public abstract class ProtoPoolObject : IPoolObject<ProtoPoolObject>, IPrototype
    {
        protected ProtoPoolObject prototype;
	
        public object Clone()
        {
            IPrototype copy = MemberwiseClone() as IPrototype;
            // ReSharper disable once PossibleNullReferenceException
            copy.SetProto(this);
            return copy;
        }

        public virtual void ResetToProto()
        {
			
        }

        void IPrototype.SetProto(IPrototype prototype)
        {
            this.prototype = prototype as ProtoPoolObject;
        }


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

        public ProtoPoolObject GetPooledCopy()
        {
            return (ProtoPoolObject)PoolInstantiate?.Invoke();
        }	
    }
}