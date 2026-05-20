using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public static EnemyAI Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void StartEnemyTurn()
    {
        Debug.Log("Musuh melakukan aksi...");
        PlayerController.Instance.TakeDamage(5);
        Invoke(nameof(EndEnemyTurn), 1f);
    }

    public void TakeDamage(int damage)
    {
        Debug.Log($"Musuh menerima {damage} damage!");
    }
    
    private void EndEnemyTurn()
    {
        TurnManager.Instance.EndTurn();
    }


}