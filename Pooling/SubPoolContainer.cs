using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArchCore.Pooling
{
    [CreateAssetMenu(fileName = "SubPoolContainer", menuName = "Utils/SubPoolContainer")]
    public class SubPoolContainer : ScriptableObject
    {
        [SerializeField] private List<PoolContainerData> list = new List<PoolContainerData>();

        public List<PoolContainerData> GetData() => list;

    }
}