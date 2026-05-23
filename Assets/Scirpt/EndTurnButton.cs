using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Tombol "Akhiri Giliran".
/// Otomatis nonaktif saat bukan giliran player (mencegah klik saat musuh bermain).
/// Pasang script ini di GameObject tombol yang punya komponen Button.
/// </summary>
public class EndTurnButton : MonoBehaviour
{
    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();

        if (button == null)
        {
            Debug.LogError("EndTurnButton: Tidak ditemukan komponen Button di GameObject ini!");
            return;
        }

        // Daftarkan fungsi yang dipanggil saat tombol diklik
        button.onClick.AddListener(OnEndTurnClicked);

        // Dengarkan perubahan giliran agar tombol bisa dinonaktifkan saat giliran musuh
        if (TurnManager.Instance != null)
            TurnManager.Instance.TurnChanged += OnTurnChanged;
    }

    private void OnDestroy()
    {
        // Lepas event saat object dihancurkan
        if (TurnManager.Instance != null)
            TurnManager.Instance.TurnChanged -= OnTurnChanged;
    }

    private void OnEndTurnClicked()
    {
        TurnManager.Instance?.EndTurn();
    }

    private void OnTurnChanged(TurnState newTurn)
    {
        // Tombol hanya aktif saat giliran player
        if (button != null)
            button.interactable = (newTurn == TurnState.PlayerTurn);
    }
}