using UnityEngine;
using TMPro; // jika pakai TextMeshPro

public class TurnUI : MonoBehaviour
{
    private TextMeshProUGUI turnText;

    private void Start()
    {
        turnText = GetComponent<TextMeshProUGUI>();
        if (turnText == null)
        {
            Debug.LogError("TurnUI: Tidak ditemukan komponen TextMeshProUGUI di GameObject ini.");
            return;
        }

        if (TurnManager.Instance != null)
            TurnManager.Instance.TurnChanged += UpdateTurnText;
    }

    private void UpdateTurnText(TurnState turn)
    {
        if (turnText != null)
        {
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

    private void OnDestroy()
    {
        if (TurnManager.Instance != null)
            TurnManager.Instance.TurnChanged -= UpdateTurnText;
    }
}