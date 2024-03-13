using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ArchCore.Pooling
{
	public class PoolData
	{
//		public readonly Type type;
		public readonly int minSize, maxSize;
		public readonly AssetReference asset;
//		public readonly object sample;
//		public readonly string name;

//		public PoolData(int minSize, int maxSize, Type type, string name)
//		{
//			this.type = type;
//			this.name = name;
//			this.minSize = minSize;
//			this.maxSize = maxSize;
//		}
//		
//		public PoolData(int minSize, int maxSize, object sample, string name)
//		{
//			this.sample = sample;
//			this.name = name;
//			this.minSize = minSize;
//			this.maxSize = maxSize;
//		}
		
		public PoolData(int minSize, int maxSize, AssetReference asset)
		{
			this.asset = asset;
			this.minSize = minSize;
			this.maxSize = maxSize;
		}
	}
}
