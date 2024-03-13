using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArchCore.Utils
{
	public class AsyncTask : CustomYieldInstruction, IDisposable
	{

		public static AsyncTask All(params AsyncTask[] tasks)
		{
			return new CombinedAsyncTask(new List<AsyncTask>(tasks));
		}

		public delegate void TaskSuccessDelegate();

		public delegate void TaskFailDelegate(Exception error);

		public delegate void TaskCompleteDelegate(Exception error);

		public bool IsDone { get; private set; }
		public Exception Error { get; private set; }

		private TaskSuccessDelegate onSuccess;
		private TaskFailDelegate onFail;
		private TaskCompleteDelegate onComplete;
		private bool _keepWaiting = true;

		public AsyncTask OnSuccess(TaskSuccessDelegate onSuccess)
		{
			this.onSuccess += onSuccess;
			if (IsDone && Error == null && onSuccess != null)
			{
				onSuccess.Invoke();
			}

			return this;
		}

		public AsyncTask OnFail(TaskFailDelegate onFail)
		{
			this.onFail += onFail;
			if (IsDone && Error != null && onFail != null)
			{
				onFail.Invoke(Error);
			}

			return this;
		}

		public AsyncTask OnComplete(TaskCompleteDelegate onComplete)
		{
			this.onComplete += onComplete;
			if (IsDone && onComplete != null)
			{
				onComplete.Invoke(Error);
			}

			return this;
		}

		public virtual void Success()
		{
			IsDone = true;

			onSuccess?.Invoke();

			onComplete?.Invoke(null);

			_keepWaiting = false;
		}

		public void Fail(Exception error)
		{
			Error = error;
			IsDone = true;

			onFail?.Invoke(error);

			onComplete?.Invoke(error);

			_keepWaiting = false;
		}

		public override bool keepWaiting => _keepWaiting;

		public AsyncTask RegisterForDispose(IDisposablesContainer disposablesContainer)
		{
			disposablesContainer.RegisterForDispose(this);

			return this;
		}

		public virtual void Dispose()
		{
			onSuccess = null;
			onComplete = null;
			onFail = null;
		}
	}

	public class AsyncTask<T> : AsyncTask
	{
		public new delegate void TaskSuccessDelegate(T result);

		public new delegate void TaskCompleteDelegate(T result, Exception error);

		public T Result { get; private set; }

		private TaskSuccessDelegate onSuccess;
		private TaskCompleteDelegate onComplete;

		public AsyncTask<T> OnSuccess(TaskSuccessDelegate onSuccess)
		{
			this.onSuccess += onSuccess;
			if (IsDone && Error == null && onSuccess != null)
			{
				onSuccess.Invoke(Result);
			}

			return this;
		}

		public AsyncTask<T> OnComplete(TaskCompleteDelegate onComplete)
		{
			this.onComplete += onComplete;
			if (IsDone && onComplete != null)
			{
				onComplete.Invoke(Result, Error);
			}

			return this;
		}

		public void Success(T result)
		{
			Result = result;
			base.Success();

			onSuccess?.Invoke(result);

			onComplete?.Invoke(result, null);
		}

		public override void Success()
		{
			throw new Exception("Calling Success without result on AsyncTask that expects " + typeof(T));
		}
	}

	internal class CombinedAsyncTask : AsyncTask
	{
		private List<AsyncTask> tasks;
		private bool failed;
		private Exception error;

		public CombinedAsyncTask(List<AsyncTask> tasks)
		{
			this.tasks = tasks;
			ListenAllSuccess();
		}

		private void ListenAllSuccess()
		{
			tasks.RemoveAll(t => t.IsDone);

			foreach (var task in tasks)
			{
				AsyncTask tempTask = task;
				task
					.OnSuccess(delegate
					{
						tasks.Remove(tempTask);
						CheckComplete();
					})
					.OnFail(delegate(Exception error)
					{
						tasks.Remove(tempTask);
						failed = true;
						this.error = error;
						CheckComplete();
					});
			}

			CheckComplete();
		}

		private void CheckComplete()
		{
			if (tasks.Count > 0)
			{
				return;
			}

			if (failed)
			{
				Fail(error);
			}
			else
			{
				Success();
			}
		}
	}
}