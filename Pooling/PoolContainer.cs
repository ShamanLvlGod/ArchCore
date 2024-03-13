using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.AddressableAssets;

namespace ArchCore.Pooling
{
    [System.Serializable]
    public class PoolContainerData
    {
        public int minSize, maxSize;
        public AssetReference asset;
        
        protected PoolContainerData(int minSize, int maxSize)
        {
            this.minSize = minSize;
            this.maxSize = maxSize;
        }
    }
   
    public class PoolContainer : MonoBehaviour
    {
        [SerializeField] private List<PoolContainerData> list = new List<PoolContainerData>();
        [SerializeField] private List<SubPoolContainer> subContainers = new List<SubPoolContainer>();
        
        public Pool Pool { get; private set; }

        public bool initOnAwake = true;

        private void Awake()
        {      
            if(initOnAwake) Init();
        }

        public void Init()
        {
            var tList = new List<PoolContainerData>(list);
            foreach (var sc in subContainers)
            {
                tList.AddRange(sc.GetData());
            }
            
            List<PoolData> poolDatas = new List<PoolData>();

            foreach (var poolData in tList)
            {
                if (poolData.asset != null)
                {  
                    poolDatas.Add(new PoolData(poolData.minSize, poolData.maxSize, poolData.asset));
                }
            }

            Pool = new Pool(transform, poolDatas.ToArray());
        }
    }
}


