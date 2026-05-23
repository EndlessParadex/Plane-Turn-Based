using UnityEngine;

/// <summary>
/// Nilai yang mewakili siapa yang sedang bermain.
/// </summary>
public enum TurnState
{
    PlayerTurn,
    EnemyTurn
}

/// <summary>
/// Mengatur alur giliran: Player → Musuh → Player → ...
/// Juga mengelola Action Points (AP) player.
/// </summary>
public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }

    public TurnState CurrentTurn { get; private set; } = TurnState.PlayerTurn;

    // Action Points
    public int CurrentAP { get; private set; }
    public int maxAP = 3;

    // === Events ===
    // Script lain bisa "mendengar" perubahan giliran dan AP lewat events ini
    public delegate void OnTurnChanged(TurnState newTurn);
    public event OnTurnChanged TurnChanged;

    public delegate void OnAPChanged(int currentAP);
    public event OnAPChanged APChanged;

    // Mencegah EndTurn dipanggil dua kali bersamaan
    private bool isEndingTurn = false;

    // -------------------------------------------------------
    // Unity Messages
    // -------------------------------------------------------

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Mulai permainan dengan giliran player
        // (PlayerHand sudah inisialisasi deck di Awake-nya, jadi ini aman)
        StartPlayerTurn();
    }

    // -------------------------------------------------------
    // Turn Flow
    // -------------------------------------------------------

    private void StartPlayerTurn()
    {
        CurrentTurn = TurnState.PlayerTurn;
        CurrentAP   = maxAP;

        // Reset perisai player setiap awal giliran
        PlayerController.Instance?.ResetShield();

        // Beri tahu semua script yang mendengarkan
        APChanged?.Invoke(CurrentAP);
        TurnChanged?.Invoke(CurrentTurn);

        // Tarik kartu baru untuk player
        PlayerHand.Instance?.DrawCards(3);

        Debug.Log($"=== Giliran Player dimulai. AP: {CurrentAP} ===");
    }

    /// <summary>
    /// Kurangi AP sebesar jumlah tertentu.
    /// Jika AP habis, giliran player berakhir otomatis.
    /// </summary>
    public void SpendAP(int amount)
    {
        if (CurrentTurn != TurnState.PlayerTurn) return;

        CurrentAP -= amount;
        if (CurrentAP < 0) CurrentAP = 0;

        APChanged?.Invoke(CurrentAP);
        Debug.Log($"Menggunakan {amount} AP. Sisa AP: {CurrentAP}");

        // Giliran berakhir otomatis jika AP habis
        if (CurrentAP <= 0)
        {
            Debug.Log("AP habis — giliran player berakhir otomatis.");
            EndTurn();
        }
    }

    /// <summary>
    /// Akhiri giliran saat ini dan pindah ke giliran berikutnya.
    /// Bisa dipanggil oleh tombol End Turn atau otomatis saat AP habis.
    /// </summary>
    public void EndTurn()
    {
        // Jaga agar EndTurn tidak berjalan dua kali secara bersamaan
        if (isEndingTurn) return;
        isEndingTurn = true;

        if (CurrentTurn == TurnState.PlayerTurn)
        {
            // Buang semua kartu player yang tersisa
            PlayerHand.Instance?.DiscardAllCards();

            // Pindah ke giliran musuh
            CurrentTurn = TurnState.EnemyTurn;
            TurnChanged?.Invoke(CurrentTurn);
            Debug.Log("=== Giliran Musuh dimulai. ===");

            isEndingTurn = false;
            EnemyAI.Instance?.StartEnemyTurn();
        }
        else
        {
            // Giliran musuh selesai → kembali ke player
            isEndingTurn = false;
            StartPlayerTurn();
        }
    }
}