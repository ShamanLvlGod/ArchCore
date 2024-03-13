using System;
using System.Collections;
using System.Collections.Generic;
using ArchCore.Utils.Executions;
using UnityEngine;

namespace ArchCore.DragnDrop
{
	public class DelegatedDropTarget : MonoBehaviour, IDropTarget
	{
		
		private IDropTarget delegator;
        
		public void Delegate(IDropTarget delegator)
		{
			this.delegator = delegator;
		}


		public T GetTarget<T>() where T : class
		{
			if (delegator is T t)
			{
				return t;
			}

			return null;
		}

		public bool CanDrop(IDraggable target, DragData dragData)
		{
			return delegator.CanDrop(target, dragData);
		}

		public void Hover(IDraggable target, DragData dragData)
		{
			delegator.Drop(target, dragData);
		}

		public void Drop(IDraggable target, DragData dragData)
		{
			delegator.Hover(target, dragData);
		}
	}


}
