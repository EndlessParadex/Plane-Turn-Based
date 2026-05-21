using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHand : MonoBehaviour
{
    public static PlayerHand Instance { get; private set; }

    [Header("Deck & UI")]
    public List<CardData> deck;             // Masukkan data kartu dari Project
    public GameObject cardPrefab;           // Prefab kartu (dengan CardDragHandler)
    public Transform handPanel;             // Panel di Canvas (Horizontal Layout Group)
    public int handSize = 3;                // Jumlah kartu ditarik tiap giliran

    [Header("Internal")]
    private List<GameObject> handObjects = new List<GameObject>();
    private Queue<CardData> drawPile = new Queue<CardData>();
    private List<CardData> discardPile = new List<CardData>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        InitializeDeck();
        DrawCards(handSize);
    }

    void InitializeDeck()
    {
        drawPile.Clear();
        foreach (CardData card in deck)
            drawPile.Enqueue(card);
        Shuffle();
    }

    void Shuffle()
    {
        List<CardData> temp = new List<CardData>(drawPile);
        for (int i = 0; i < temp.Count; i++)
        {
            int rand = Random.Range(i, temp.Count);
            CardData t = temp[i];
            temp[i] = temp[rand];
            temp[rand] = t;
        }
        drawPile = new Queue<CardData>(temp);
    }

    // Tarik kartu baru ke tangan
    public void DrawCards(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            if (drawPile.Count == 0)
            {
                // Jika habis, shuffle discard pile kembali ke draw pile
                if (discardPile.Count > 0)
                {
                    drawPile = new Queue<CardData>(discardPile);
                    discardPile.Clear();
                    Shuffle();
                }
                else break;
            }

            if (handObjects.Count < 5) // max hand 5
            {
                CardData newCard = drawPile.Dequeue();
                GameObject cardObj = Instantiate(cardPrefab, handPanel);
                CardDragHandler dragHandler = cardObj.GetComponent<CardDragHandler>();
                if (dragHandler != null)
                    dragHandler.cardData = newCard;

                // Update tampilan teks (nama kartu & biaya AP)
                Text[] texts = cardObj.GetComponentsInChildren<Text>();
                if (texts.Length >= 2)
                {
                    texts[0].text = newCard.cardName;
                    texts[1].text = $"AP {newCard.apCost}";
                }
                handObjects.Add(cardObj);
            }
        }
    }

    // Buang semua kartu di tangan ke discard pile
    public void DiscardAllCards()
    {
        foreach (GameObject cardObj in handObjects)
        {
            CardDragHandler dragHandler = cardObj.GetComponent<CardDragHandler>();
            if (dragHandler != null)
                discardPile.Add(dragHandler.cardData);
            Destroy(cardObj);
        }
        handObjects.Clear();
    }

    // Fungsi ini dipanggil oleh DropZone saat kartu di-drop
    public void UseCard(CardData card, GameObject cardObject)
    {
        // Cek giliran player
        if (TurnManager.Instance.CurrentTurn != TurnState.PlayerTurn)
        {
            Debug.Log("Bukan giliran player!");
            return;
        }

        // Cek AP cukup
        if (TurnManager.Instance.CurrentAP >= card.apCost)
        {
            // Kurangi AP
            TurnManager.Instance.SpendAP(card.apCost);

            // Efek kartu
            if (card.damage > 0)
            {
                EnemyAI enemy = FindObjectOfType<EnemyAI>();
                if (enemy != null) enemy.TakeDamage(card.damage);
            }
            if (card.heal > 0)
            {
                PlayerController.Instance.Heal(card.heal);
            }

            // Hapus kartu dari tangan
            handObjects.Remove(cardObject);
            Destroy(cardObject);
            discardPile.Add(card);
        }
        else
        {
            Debug.Log("AP tidak cukup!");
        }
    }
}