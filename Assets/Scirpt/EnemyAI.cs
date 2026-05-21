using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public static EnemyAI Instance { get; private set; }
    public int health = 20;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void StartEnemyTurn()
    {
        Debug.Log("Musuh melakukan aksi...");
        // Contoh serangan ke player
        PlayerController.Instance.TakeDamage(5);
        
        // Selesai aksi, akhiri giliran musuh
        Invoke(nameof(EndEnemyTurn), 1f);
    }

    private void EndEnemyTurn()
    {
        TurnManager.Instance.EndTurn(); // panggil EndTurn
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log($"Musuh kena {damage} damage. Sisa HP: {health}");
        if (health <= 0)
        {
            Debug.Log("Musuh mati!");
        }
    }
}