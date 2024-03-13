using System;
using System.Collections;
using System.Collections.Generic;
using ArchCore.Utils.Executions;
using UnityEngine;
using UnityEngine.Events;

namespace ArchCore.DragnDrop
{
    public class DelegatedDraggable : MonoBehaviour, IDraggable
    {
        private IDraggable delegator;
        
        public void Delegate(IDraggable delegator)
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

        public bool CanDrop(IDropTarget target, DragData dragData)
        {
            return delegator.CanDrop(target, dragData);
        }

        void IDraggable.Drop(IDropTarget target, DragData dragData)
        {
            delegator.Drop(target, dragData);
        }

        bool IDraggable.Drag(DragData dragData)
        {
            return delegator.Drag(dragData);
        }

        bool IDraggable.StartDrag(DragData dragData)
        {
            return delegator.StartDrag(dragData);
        }

        void IDraggable.EndDrag(DragData dragData)
        {
            delegator.EndDrag(dragData);
        }

    }
}