using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Menangani drag-and-drop sebuah kartu di UI.
/// Pasang script ini di prefab kartu.
/// </summary>
public class CardDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private Vector2 startPosition;
    private Transform startParent;

    // rootCanvas dicari setiap OnBeginDrag karena kartu bisa berpindah parent
    private Canvas rootCanvas;

    public CardData cardData;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        // Tambah CanvasGroup secara otomatis jika belum ada
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPosition = rectTransform.anchoredPosition;
        startParent   = transform.parent;

        // Cari root Canvas saat drag dimulai (bukan di Awake) agar selalu akurat
        rootCanvas = GetComponentInParent<Canvas>();
        if (rootCanvas != null && rootCanvas.isRootCanvas == false)
            rootCanvas = rootCanvas.rootCanvas;

        // Pindahkan kartu ke root Canvas agar tampil di atas segalanya
        if (rootCanvas != null)
            transform.SetParent(rootCanvas.transform);

        canvasGroup.alpha           = 0.6f;  // Semi-transparan saat di-drag
        canvasGroup.blocksRaycasts  = false; // Biarkan event menembus ke DropZone
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (rootCanvas != null)
            rectTransform.anchoredPosition += eventData.delta / rootCanvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha          = 1f;
        canvasGroup.blocksRaycasts = true;

        // Kembalikan kartu ke tangan jika tidak di-drop ke DropZone yang valid
        transform.SetParent(startParent);
        rectTransform.anchoredPosition = startPosition;
    }
}