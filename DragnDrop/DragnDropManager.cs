using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArchCore.DragnDrop
{
    public class DragnDropManager : MonoBehaviour
    {
        private IDraggable dragObject;

        private Collider2D[] collidersBuffer = new Collider2D[10];

        [SerializeField] private Camera targetCamera;

        private DragData lastData = new DragData();
        
        private void Update()
        {
            Vector2 mousePos = targetCamera.ScreenToWorldPoint(Input.mousePosition);
            
            DragData dragData = new DragData();
            dragData.position = mousePos;
            dragData.lastPosition = lastData.position;
            dragData.delta = dragData.position - dragData.lastPosition;
            
            if (dragObject != null)
            {
                if (Input.GetMouseButtonUp(0))
                {
                    dragObject.EndDrag(dragData);

                    var size = Physics2D.OverlapPointNonAlloc(mousePos, collidersBuffer);

                    for (var index = 0; index < size; index++)
                    {
                        var coll = collidersBuffer[index];
                        IDropTarget dropTarget = coll.GetComponent<IDropTarget>();
                        if (dropTarget != null)
                        {
                            if (dropTarget.CanDrop(dragObject, dragData) && dragObject.CanDrop(dropTarget, dragData))
                            {
                                dragObject.EndDrag(dragData);
                                dragObject.Drop(dropTarget, dragData);
                                dropTarget.Drop(dragObject, dragData);
                                dragObject = null;
                                break;
                            }
                        }
                    }

                    if (dragObject != null)
                    {
                        dragObject.EndDrag(dragData);
                        dragObject.Drop(null, dragData);
                        dragObject = null;
                    }

                }
                else
                {
                    if (!dragObject.Drag(dragData))
                    {
                        dragObject.EndDrag(dragData);
                        dragObject = null;
                    }
                }

            }
            else if (Input.GetMouseButtonDown(0))
            {
                var size = Physics2D.OverlapPointNonAlloc(mousePos, collidersBuffer);

                for (var index = 0; index < size; index++)
                {
                    var coll = collidersBuffer[index];
                    dragObject = coll.GetComponent<IDraggable>();
                    if (dragObject != null)
                    {
                        if (!dragObject.StartDrag(dragData))
                        {
                            dragObject = null;
                        }
                        else
                        {
                            break;
                        }
                        
                    }
                }
            }
            
            lastData = dragData;
        }
    }
}