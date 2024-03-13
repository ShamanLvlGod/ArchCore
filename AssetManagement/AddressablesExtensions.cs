using System.Collections.Generic;
using System.Linq;
using ArchCore.Utils;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace ArchCore.AssetManagement
{
	public static class AddressablesExtensions
	{
		public static AsyncTask LoadAssetsAsync<T>(ICollection<AssetReference> references)
		{
			AsyncTask task = new AsyncTask();
			int counter = 0;
			foreach (var assetReference in references)
			{
				if (assetReference.Asset)
					Ready(default);
				else
					assetReference.LoadAssetAsync<T>().Completed += Ready;
			}

			
			void Ready(AsyncOperationHandle<T> h)
			{
				if (++counter >= references.Count)
				{
					task.Success();
				}
				
			}

			return task;
		}
	}
}