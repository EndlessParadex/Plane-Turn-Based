using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TurnState
{
    PlayerTurn,
    EnemyTurn
}

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }
    public TurnState CurrentTurn { get; private set; } = TurnState.PlayerTurn;
    public int CurrentAP { get; private set; }
    public int maxAP = 3;
    public delegate void OnTurnChanged(TurnState newTurn);
    public event OnTurnChanged TurnChanged;
    public delegate void OnAPChanged(int currentAP);
    public event OnAPChanged APChanged;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        StartPlayerTurn();
    }

    private void StartPlayerTurn()
    {
        CurrentTurn = TurnState.PlayerTurn;
        CurrentAP = maxAP;
        APChanged?.Invoke(CurrentAP);
        TurnChanged?.Invoke(CurrentTurn);
        Debug.Log($"Giliran Player dimulai. AP: {CurrentAP}");
    }

    public void SpendAP(int amount)
    {
        if (CurrentTurn != TurnState.PlayerTurn) return;
        if (CurrentAP >= amount)
        {
            CurrentAP -= amount;
            APChanged?.Invoke(CurrentAP);
            Debug.Log($"Menggunakan {amount} AP. Sisa AP: {CurrentAP}");
            if (CurrentAP <= 0)
            {
                Debug.Log("AP habis, akhiri giliran.");
                EndTurn();
            }
        }
        else
        {
            Debug.Log("AP tidak cukup!");
        }
    }

    public void EndTurn()
    {
        if (CurrentTurn == TurnState.PlayerTurn)
        {
            PlayerHand.Instance.DiscardAllCards(); // buang kartu
            CurrentTurn = TurnState.EnemyTurn;
            TurnChanged?.Invoke(CurrentTurn);
            EnemyAI.Instance.StartEnemyTurn();
        }
        else
        {
            // musuh selesai, mulai giliran player baru
            StartPlayerTurn();
            PlayerHand.Instance.DrawCards(PlayerHand.Instance.handSize);
        }
    }
}