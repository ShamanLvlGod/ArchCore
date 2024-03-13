using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArchCore.Utils
{
	public class CoroutineRunner : MonoBehaviour
	{
		static CoroutineRunner instance;

		private void Awake()
		{
			instance = this;
			StartCoroutine(Synchroniser());
		}

		public static Coroutine RunCoroutine(IEnumerator task)
		{
			return instance.StartCoroutine(task);
		}

		public static void Stop(Coroutine coroutine)
		{
			if(instance)
				instance.StopCoroutine(coroutine);
		}

		private Queue<Action> syncCalls = new Queue<Action>();

		public static void SyncCall(Action call)
		{
			instance.syncCalls.Enqueue(call);
		}

		private IEnumerator Synchroniser()
		{
			while (true)
			{
				while (syncCalls.Count > 0)
				{
					syncCalls.Dequeue()();
				}

				yield return null;
			}
		}
	}

	public class ScheduleTask : YieldTask
	{
		public ScheduleTask(Action task, float timer) : base(DelayedTask(task, timer))
		{
		}

		private static IEnumerator DelayedTask(Action task, float timer)
		{
			yield return new WaitForSeconds(timer);

			task?.Invoke();
		}
	}

	public class YieldTask : CustomYieldInstruction
	{
		private readonly IEnumerator task;
		private Action onComplete;

		public event Action OnComplete
		{
			add
			{
				onComplete += value;
				if (completed)
				{
					value?.Invoke();
				}
			}
			remove { onComplete -= value; }
		}

		public override bool keepWaiting => !completed;


		private bool completed = false;
		private Coroutine coroutine, sub_coroutine;

		public YieldTask(IEnumerator task)
		{
			this.task = task;
		}

		public YieldTask Start()
		{
			coroutine = CoroutineRunner.RunCoroutine(CompleteCoroutine(task));
			return this;
		}

		private IEnumerator CompleteCoroutine(IEnumerator cor)
		{
			sub_coroutine = CoroutineRunner.RunCoroutine(cor);
			yield return sub_coroutine;
			Complete();
		}

		private void Complete()
		{
			onComplete?.Invoke();
			completed = true;
		}

		public void Stop()
		{
			if (coroutine != null)
				CoroutineRunner.Stop(coroutine);
			if (sub_coroutine != null)
				CoroutineRunner.Stop(sub_coroutine);
		}
	}
}