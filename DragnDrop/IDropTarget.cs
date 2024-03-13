using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArchCore.DragnDrop
{
    public interface IDropTarget 
    {
        T GetTarget<T>() where T : class;
        void Hover(IDraggable target, DragData dragData);
        bool CanDrop(IDraggable target, DragData dragData);
        void Drop(IDraggable target, DragData dragData);
    }
}