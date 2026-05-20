using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    public static PlayerHand Instance { get; private set; }

    public List<CardData> deck;
    public int handSize = 3;
    private List<CardData> hand = new List<CardData>();
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
            if (hand.Count < 5) // max hand 5
                hand.Add(drawPile.Dequeue());
        }
        UIManager.Instance?.UpdateHandUI(hand);
    }

    public void PlayCard(CardData card)
    {
        if (!hand.Contains(card)) return;
        if (TurnManager.Instance.CurrentTurn != TurnState.PlayerTurn) return;

        if (TurnManager.Instance.CurrentAP >= card.apCost)
        {
            TurnManager.Instance.SpendAP(card.apCost);

            if (card.damage > 0)
            {
                EnemyAI enemy = FindObjectOfType<EnemyAI>();
                if (enemy != null) enemy.TakeDamage(card.damage);
            }
            if (card.heal > 0)
                PlayerController.Instance.Heal(card.heal);

            hand.Remove(card);
            discardPile.Add(card);
            UIManager.Instance?.UpdateHandUI(hand);
        }
        else
        {
            Debug.Log("AP tidak cukup untuk kartu ini!");
        }
    }

    public void DiscardAllCards()
    {
        discardPile.AddRange(hand);
        hand.Clear();
        UIManager.Instance?.UpdateHandUI(hand);
    }
}