using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Mengelola semua tampilan UI di layar:
/// HP player, perisai, AP, dan HP musuh.
///
/// Cara pakai di Inspector:
///   Drag Text object HP Player ke "Player Health Text"
///   Drag Text object Perisai ke "Player Shield Text"
///   Drag Text object AP ke "Ap Text"
///   Drag Text object HP Musuh ke "Enemy Health Text"
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Player UI")]
    public Text playerHealthText;  // Tampilkan "HP: 25/30"
    public Text playerShieldText;  // Tampilkan "Perisai: 10" atau kosong
    public Text apText;            // Tampilkan "AP: 2"

    [Header("Enemy UI")]
    public Text enemyHealthText;   // Tampilkan "HP Musuh: 15/20"

    // -------------------------------------------------------
    // Unity Messages
    // -------------------------------------------------------

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        // Langganan event di Awake (bukan Start) agar tidak ketinggalan
        // event yang mungkin sudah ditembak di Awake script lain.
        // Unity menjamin semua Awake selesai sebelum Start manapun berjalan.
        if (TurnManager.Instance != null)
            TurnManager.Instance.APChanged += UpdateAPUI;

        if (EnemyAI.Instance != null)
            EnemyAI.Instance.HealthChanged += UpdateEnemyHealthUI;
    }

    private void Start()
    {
        // Tampilkan nilai awal setelah semua script Awake selesai
        if (TurnManager.Instance != null)
            UpdateAPUI(TurnManager.Instance.CurrentAP);

        if (EnemyAI.Instance != null)
            UpdateEnemyHealthUI(EnemyAI.Instance.currentHealth, EnemyAI.Instance.maxHealth);

        if (PlayerController.Instance != null)
            UpdatePlayerUI(
                PlayerController.Instance.currentHealth,
                PlayerController.Instance.maxHealth,
                PlayerController.Instance.currentShield
            );
    }

    private void OnDestroy()
    {
        // Penting: lepas event agar tidak terjadi memory leak
        if (TurnManager.Instance != null)
            TurnManager.Instance.APChanged -= UpdateAPUI;

        if (EnemyAI.Instance != null)
            EnemyAI.Instance.HealthChanged -= UpdateEnemyHealthUI;
    }

    // -------------------------------------------------------
    // Update Methods (dipanggil oleh script lain via event)
    // -------------------------------------------------------

    /// <summary>
    /// Update tampilan HP dan perisai player.
    /// </summary>
    public void UpdatePlayerUI(int current, int max, int shield)
    {
        if (playerHealthText)
            playerHealthText.text = $"HP: {current}/{max}";

        if (playerShieldText)
            playerShieldText.text = shield > 0 ? $"Perisai: {shield}" : "";
    }

    /// <summary>
    /// Update tampilan AP player.
    /// </summary>
    public void UpdateAPUI(int ap)
    {
        if (apText)
            apText.text = $"AP: {ap}";
    }

    /// <summary>
    /// Update tampilan HP musuh.
    /// </summary>
    public void UpdateEnemyHealthUI(int current, int max)
    {
        if (enemyHealthText)
            enemyHealthText.text = $"HP Musuh: {current}/{max}";
    }
}