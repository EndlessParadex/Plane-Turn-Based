using System.Collections;
using System.Collections.Generic;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHandController : MonoBehaviour
{
    public static PlayerHandController Instance;

    public GameObject cardPrefab; // drag prefab card dari Project
    public Transform handPanel;   // Panel di Canvas tempat kartu ditampilkan (misal Horizontal Layout Group)
    public List<CardData> deck;   // isi dengan kartu-kartu via Inspector

    private List<GameObject> handObjects = new List<GameObject>();
    private Queue<CardData> drawPile = new Queue<CardData>();
    private List<CardData> discardPile = new List<CardData>();

    void Awake() { Instance = this; }

    void Start()
    {
        InitializeDeck();
        DrawCards(3); // mulai dengan 3 kartu
    }

    void InitializeDeck()
    {
        drawPile.Clear();
        foreach (CardData cd in deck) drawPile.Enqueue(cd);
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

    public void DrawCards(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            if (drawPile.Count == 0)
            {
                if (discardPile.Count > 0)
                {
                    drawPile = new Queue<CardData>(discardPile);
                    discardPile.Clear();
                    Shuffle();
                }
                else break;
            }
            CardData newCard = drawPile.Dequeue();
            GameObject cardObj = Instantiate(cardPrefab, handPanel);
            cardObj.GetComponent<CardDragHandler>().cardData = newCard;
            // Update tampilan teks
            Text[] texts = cardObj.GetComponentsInChildren<Text>();
            if (texts.Length >= 2)
            {
                texts[0].text = newCard.cardName;
                texts[1].text = $"AP {newCard.apCost}";
            }
            handObjects.Add(cardObj);
        }
    }

    public void UseCard(CardData card, GameObject cardObject)
    {
        // Cek apakah giliran player
        if (TurnManager.Instance.CurrentTurn != TurnState.PlayerTurn)
        {
            Debug.Log("Bukan giliran player!");
            ReturnCard(cardObject);
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
            ReturnCard(cardObject);
        }
    }

    void ReturnCard(GameObject cardObj)
    {
        // kartu kembali ke posisi awal di handPanel (tidak perlu aksi khusus karena drag handler sudah handle)
    }

    public void DiscardAllCards()
    {
        foreach (GameObject cardObj in handObjects)
        {
            discardPile.Add(cardObj.GetComponent<CardDragHandler>().cardData);
            Destroy(cardObj);
        }
        handObjects.Clear();
    }

    public void EndTurnCleanup()
    {
        DiscardAllCards();
        DrawCards(3); // di awal giliran berikutnya akan di-draw lagi
    }
}