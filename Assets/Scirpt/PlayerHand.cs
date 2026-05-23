using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Mengelola tangan kartu player: draw, discard, dan penggunaan kartu.
/// Satu-satunya script yang mengurus kartu — PlayerHandController sudah dihapus.
/// </summary>
public class PlayerHand : MonoBehaviour
{
    public static PlayerHand Instance { get; private set; }

    [Header("Deck & UI")]
    public List<CardData> deck;           // Isi dengan kartu-kartu via Inspector
    public GameObject cardPrefab;         // Prefab kartu (harus punya CardDragHandler)
    public Transform handPanel;           // Panel tangan di Canvas

    [Header("Settings")]
    public int maxHandSize = 5;           // Maksimal kartu di tangan

    // === Data internal ===
    private List<GameObject> handObjects = new List<GameObject>();
    private Queue<CardData> drawPile     = new Queue<CardData>();
    private List<CardData> discardPile   = new List<CardData>();

    // -------------------------------------------------------
    // Unity Messages
    // -------------------------------------------------------

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        // Inisialisasi deck di Awake agar sudah siap sebelum Start manapun berjalan
        InitializeDeck();
    }

    // -------------------------------------------------------
    // Deck Management
    // -------------------------------------------------------

    private void InitializeDeck()
    {
        drawPile.Clear();
        discardPile.Clear();
        foreach (CardData card in deck)
            drawPile.Enqueue(card);
        Shuffle();
    }

    private void Shuffle()
    {
        List<CardData> temp = new List<CardData>(drawPile);
        for (int i = 0; i < temp.Count; i++)
        {
            int rand  = Random.Range(i, temp.Count);
            CardData t = temp[i];
            temp[i]    = temp[rand];
            temp[rand] = t;
        }
        drawPile = new Queue<CardData>(temp);
    }

    // -------------------------------------------------------
    // Public API
    // -------------------------------------------------------

    /// <summary>
    /// Ambil sejumlah kartu dari draw pile ke tangan player.
    /// Dipanggil oleh TurnManager setiap awal giliran player.
    /// </summary>
    public void DrawCards(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            // Batas maksimal tangan
            if (handObjects.Count >= maxHandSize)
            {
                Debug.Log($"Tangan penuh! Maksimal {maxHandSize} kartu.");
                break;
            }

            // Jika draw pile kosong, kocok ulang dari discard pile
            if (drawPile.Count == 0)
            {
                if (discardPile.Count > 0)
                {
                    drawPile = new Queue<CardData>(discardPile);
                    discardPile.Clear();
                    Shuffle();
                    Debug.Log("Deck habis — dikocok ulang dari discard pile.");
                }
                else
                {
                    Debug.Log("Tidak ada kartu tersisa di deck maupun discard!");
                    break;
                }
            }

            // Tarik kartu dan buat GameObject-nya
            CardData newCard = drawPile.Dequeue();
            GameObject cardObj = Instantiate(cardPrefab, handPanel);

            // Pasang data kartu ke drag handler
            CardDragHandler dragHandler = cardObj.GetComponent<CardDragHandler>();
            if (dragHandler != null)
                dragHandler.cardData = newCard;

            // Tampilkan teks pada kartu (nama & biaya AP)
            Text[] texts = cardObj.GetComponentsInChildren<Text>();
            if (texts.Length >= 2)
            {
                texts[0].text = newCard.cardName;
                texts[1].text = $"AP {newCard.apCost}";
            }

            handObjects.Add(cardObj);
        }
    }

    /// <summary>
    /// Buang semua kartu di tangan ke discard pile.
    /// Dipanggil oleh TurnManager saat player mengakhiri giliran.
    /// </summary>
    public void DiscardAllCards()
    {
        foreach (GameObject cardObj in handObjects)
        {
            if (cardObj == null) continue;

            CardDragHandler dragHandler = cardObj.GetComponent<CardDragHandler>();
            if (dragHandler != null && dragHandler.cardData != null)
                discardPile.Add(dragHandler.cardData);

            Destroy(cardObj);
        }
        handObjects.Clear();
    }

    /// <summary>
    /// Gunakan sebuah kartu. Dipanggil oleh DropZone saat kartu dijatuhkan.
    /// </summary>
    public void UseCard(CardData card, GameObject cardObject)
    {
        // Hanya bisa main kartu saat giliran player
        if (TurnManager.Instance == null ||
            TurnManager.Instance.CurrentTurn != TurnState.PlayerTurn)
        {
            Debug.Log("Bukan giliran player! Kartu dikembalikan.");
            return;
        }

        // Cek AP cukup
        if (TurnManager.Instance.CurrentAP < card.apCost)
        {
            Debug.Log($"AP tidak cukup! Butuh {card.apCost}, kamu punya {TurnManager.Instance.CurrentAP}.");
            return;
        }

        // ⚠️ Penting: hapus kartu dari tangan SEBELUM SpendAP dipanggil.
        // Jika AP habis, SpendAP → EndTurn → DiscardAllCards akan berjalan,
        // dan kita tidak mau kartu ini di-Destroy dua kali.
        handObjects.Remove(cardObject);
        discardPile.Add(card);
        Destroy(cardObject);

        // Kurangi AP (bisa memicu EndTurn otomatis jika AP = 0)
        TurnManager.Instance.SpendAP(card.apCost);

        // Terapkan efek kartu
        if (card.damage > 0)
            EnemyAI.Instance?.TakeDamage(card.damage);

        if (card.heal > 0)
            PlayerController.Instance?.Heal(card.heal);

        if (card.shield > 0)
            PlayerController.Instance?.AddShield(card.shield);

        Debug.Log($"Kartu '{card.cardName}' berhasil dimainkan!");
    }
}