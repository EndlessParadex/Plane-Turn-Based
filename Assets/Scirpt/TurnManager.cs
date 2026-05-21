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

    // Action Points
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
        
        // Tarik kartu baru
        if (PlayerHand.Instance != null)
            PlayerHand.Instance.DrawCards(3);
        
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
                Debug.Log("AP habis, mengakhiri giliran...");
                EndTurn(); // panggil EndTurn
            }
        }
        else
        {
            Debug.Log("AP tidak cukup!");
        }
    }

    // Method EndTurn untuk kedua giliran
    public void EndTurn()
    {
        if (CurrentTurn == TurnState.PlayerTurn)
        {
            // Buang semua kartu di tangan
            if (PlayerHand.Instance != null)
                PlayerHand.Instance.DiscardAllCards();
            
            // Ganti ke giliran musuh
            CurrentTurn = TurnState.EnemyTurn;
            TurnChanged?.Invoke(CurrentTurn);
            Debug.Log("Giliran Musuh dimulai.");
            
            // Jalankan AI musuh
            if (EnemyAI.Instance != null)
                EnemyAI.Instance.StartEnemyTurn();
        }
        else // giliran musuh
        {
            // Kembali ke giliran player
            StartPlayerTurn();
        }
    }
}