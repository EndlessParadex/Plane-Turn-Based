using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public Text healthText;
    public Text apText;
    public Transform handPanel; // Panel tempat tombol kartu
    public GameObject cardButtonPrefab; // buat prefab sederhana: Button dengan Text

    private List<GameObject> currentCardButtons = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        UpdateHealthUI(PlayerController.Instance.currentHealth, PlayerController.Instance.maxHealth);
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

    public void UpdateHandUI(List<CardData> hand)
    {
        // Hapus tombol lama
        foreach (var btn in currentCardButtons)
            Destroy(btn);
        currentCardButtons.Clear();

        // Buat tombol baru untuk setiap kartu
        foreach (CardData card in hand)
        {
            GameObject btnObj = Instantiate(cardButtonPrefab, handPanel);
            Text btnText = btnObj.GetComponentInChildren<Text>();
            if (btnText) btnText.text = $"{card.cardName}\n{card.apCost} AP";
            Button btn = btnObj.GetComponent<Button>();
            CardData capturedCard = card;
            btn.onClick.AddListener(() => PlayerHand.Instance.PlayCard(capturedCard));
            currentCardButtons.Add(btnObj);
        }
    }
}