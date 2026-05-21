using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public Text healthText;
    public Text apText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (PlayerController.Instance != null)
            UpdateHealthUI(PlayerController.Instance.currentHealth, PlayerController.Instance.maxHealth);
        if (TurnManager.Instance != null)
            UpdateAPUI(TurnManager.Instance.CurrentAP);
        TurnManager.Instance.APChanged += UpdateAPUI;
    }

    public void UpdateHealthUI(int current, int max)
    {
        if (healthText) healthText.text = $"HP: {current}/{max}";
    }

    public void UpdateAPUI(int ap)
    {
        if (apText) apText.text = $"AP: {ap}";
    }
}