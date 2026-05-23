using UnityEngine;
using TMPro;

/// <summary>
/// Menampilkan teks giliran saat ini ("Giliran Pemain" / "Giliran Musuh").
/// Pasang script ini di GameObject yang punya komponen TextMeshProUGUI.
/// </summary>
public class TurnUI : MonoBehaviour
{
    private TextMeshProUGUI turnText;

    private void Start()
    {
        turnText = GetComponent<TextMeshProUGUI>();

        if (turnText == null)
        {
            Debug.LogError("TurnUI: Tidak ditemukan komponen TextMeshProUGUI di GameObject ini!");
            return;
        }

        // Langganan ke event perubahan giliran
        if (TurnManager.Instance != null)
        {
            TurnManager.Instance.TurnChanged += UpdateTurnText;
            // Tampilkan giliran awal langsung
            UpdateTurnText(TurnManager.Instance.CurrentTurn);
        }
    }

    private void OnDestroy()
    {
        // Lepas event agar tidak memory leak
        if (TurnManager.Instance != null)
            TurnManager.Instance.TurnChanged -= UpdateTurnText;
    }

    private void UpdateTurnText(TurnState turn)
    {
        if (turnText == null) return;

        switch (turn)
        {
            case TurnState.PlayerTurn:
                turnText.text = "Giliran Pemain";
                break;
            case TurnState.EnemyTurn:
                turnText.text = "Giliran Musuh";
                break;
        }
    }
}