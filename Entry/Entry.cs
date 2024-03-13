using System;
using System.Collections.Generic;
using ArchCore.Flow;
using Newtonsoft.Json;
using UnityEngine;
using Zenject;

namespace ArchCore.Entry
{
	public abstract class Entry : MonoBehaviour
	{
		private static bool initialized;
		[SerializeField] private GameObject[] nailings;
		
		private FlowMaster flowMaster;
		

		[Inject]
		void Construct(FlowMaster flowMaster)
		{
			this.flowMaster = flowMaster;
		}
		
		private void Start()
		{
			if (!initialized)
			{
				initialized = true;
				Nail();
				StartEntry(flowMaster);
			}
		}

		protected abstract void StartEntry(FlowMaster flowMaster);
		
		private void Nail()
		{
			foreach(GameObject toNail in nailings)
				DontDestroyOnLoad(toNail);
		}

		public virtual void ProjectInstall(DiContainer diContainer)
		{
			
		}
	}
}