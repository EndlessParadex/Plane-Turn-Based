using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Area tempat kartu dijatuhkan untuk dimainkan.
/// Pasang script ini di panel "Play Zone" di Canvas.
/// Panel ini harus punya komponen Image dan GraphicRaycaster aktif di Canvas.
/// </summary>
public class DropZone : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        // Ambil kartu yang sedang di-drag
        GameObject draggedObject = eventData.pointerDrag;
        if (draggedObject == null) return;

        // Pastikan objek yang di-drop adalah kartu
        CardDragHandler dragHandler = draggedObject.GetComponent<CardDragHandler>();
        if (dragHandler == null || dragHandler.cardData == null) return;

        // Gunakan kartu lewat PlayerHand
        PlayerHand.Instance?.UseCard(dragHandler.cardData, draggedObject);
    }
}