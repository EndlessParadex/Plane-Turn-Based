using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }
    public int maxHealth = 30;
    public int currentHealth;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        currentHealth = maxHealth;
        UIManager.Instance?.UpdateHealthUI(currentHealth, maxHealth);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0) currentHealth = 0;
        Debug.Log($"Player menerima {damage} damage. Sisa HP: {currentHealth}");
        UIManager.Instance?.UpdateHealthUI(currentHealth, maxHealth);
        if (currentHealth <= 0) Debug.Log("Game Over!");
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        Debug.Log($"Player heal {amount} HP. HP sekarang: {currentHealth}");
        UIManager.Instance?.UpdateHealthUI(currentHealth, maxHealth);
    }
}