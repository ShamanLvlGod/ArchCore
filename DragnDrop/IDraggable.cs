using System.Collections;
using System.Collections.Generic;
using ArchCore.Utils.Executions;
using UnityEngine;
using UnityEngine.Events;

namespace ArchCore.DragnDrop
{

    public interface IDraggable
    {

	    T GetTarget<T>() where T : class;
		bool CanDrop(IDropTarget target, DragData dragData);
        void Drop(IDropTarget target, DragData dragData);
        bool Drag(DragData dragData);
        bool StartDrag(DragData dragData);
        void EndDrag(DragData dragData);
        
        
        
    }
}