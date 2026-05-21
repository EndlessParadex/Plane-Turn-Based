using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        GameObject draggedObject = eventData.pointerDrag;
        if (draggedObject == null) return;

        CardDragHandler dragHandler = draggedObject.GetComponent<CardDragHandler>();
        if (dragHandler != null && dragHandler.cardData != null)
        {
            PlayerHand.Instance.UseCard(dragHandler.cardData, draggedObject);
        }
    }
}