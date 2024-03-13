using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.Assertions;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace ArchCore.AssetManagement
{
	public static class ResourceKeyUtility
	{
		private static MethodInfo getResourceLocatorsMethod;
		private static object addressablesImpl;

		public static void Query()
		{
			
			try
			{
				addressablesImpl =
					typeof(Addressables).GetProperty("Instance", BindingFlags.NonPublic | BindingFlags.Static)
						.GetValue(null);
				
				getResourceLocatorsMethod = addressablesImpl.GetType()
					.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).First(m =>
						m.Name == "GetResourceLocations" && m.GetParameters().Length == 3);

				Assert.IsNotNull(getResourceLocatorsMethod);

			}
			catch (Exception e)
			{
				throw new Exception($"{e} exception was thrown while trying to access AddressablesImpl.GetResourceLocations(...). Maybe implementaton was changed please update the reflection getter code or rollback to previous working version of addressables.");
			}
		}

		public static IList<IResourceLocation> GetResourceLocations(object key)
		{
			object[] args = {key, null, null};
			getResourceLocatorsMethod.Invoke(addressablesImpl, args);
			return (IList<IResourceLocation>) args[2];
		}

	}

}
