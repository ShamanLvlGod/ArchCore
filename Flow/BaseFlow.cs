using UnityEngine;

namespace ArchCore.Flow
{
	public abstract class BaseFlow
	{
		private IFlowKey key;
		protected virtual FlowResult OnFinish()
		{
			return null;
		}

		public virtual void Start()
		{
			
		}

		public FlowResult Finish()
		{
			Debug.Log($"-[#flow] {GetType().Name} Finish");
			return OnFinish();
		}

		protected BaseFlow(IFlowKey key)
		{
			this.key = key;
			Debug.Log($"-[#flow] {GetType().Name} Created");
		}
		
		~BaseFlow()
		{
			Debug.Log($"-[#flow] {GetType().Name} Destroyed");
		}

		protected void ContinueWith<T>(FlowArgs args = null) where T : BaseFlow
		{
			key.ContinueWith<T>(args, Finish());
		}
		
		protected IFlowHandler RunSubFlow<T>(FlowArgs args = null) where T : BaseFlow
		{
			return key.RunSubFlow<T>(args);
		}

		protected void Return()
		{
			key.Finish(OnFinish());
		}
		
		protected void Return(FlowResult result)
		{
			OnFinish();
			key.Finish(result);
		}

		public override string ToString()
		{
			return $"[{GetType().Name}]";
		}
	}
}