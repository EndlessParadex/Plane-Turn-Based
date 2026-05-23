using UnityEngine;

/// <summary>
/// Mengontrol stats player: HP, perisai, menerima damage, dan heal.
/// </summary>
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    [Header("Stats")]
    public int maxHealth = 30;
    public int currentHealth;
    public int currentShield;  // Perisai menyerap damage sebelum HP berkurang

    // -------------------------------------------------------
    // Unity Messages
    // -------------------------------------------------------

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        currentHealth = maxHealth;
        currentShield = 0;
    }

    private void Start()
    {
        UIManager.Instance?.UpdatePlayerUI(currentHealth, maxHealth, currentShield);
    }

    // -------------------------------------------------------
    // Combat
    // -------------------------------------------------------

    /// <summary>
    /// Player menerima damage. Perisai menyerap damage lebih dulu.
    /// </summary>
    public void TakeDamage(int damage)
    {
        // Perisai menyerap damage terlebih dahulu
        if (currentShield > 0)
        {
            int absorbed  = Mathf.Min(currentShield, damage);
            currentShield -= absorbed;
            damage        -= absorbed;
            Debug.Log($"Perisai menyerap {absorbed} damage. Sisa perisai: {currentShield}");
        }

        // Sisa damage mengurangi HP
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;

        Debug.Log($"Player menerima {damage} damage. HP: {currentHealth}/{maxHealth}");
        UIManager.Instance?.UpdatePlayerUI(currentHealth, maxHealth, currentShield);

        if (currentHealth <= 0)
        {
            Debug.Log("Game Over! Player kalah.");
            OnPlayerDeath();
        }
    }

    /// <summary>
    /// Pulihkan HP player sebesar jumlah tertentu (tidak melebihi maxHealth).
    /// </summary>
    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        Debug.Log($"Player heal {amount} HP. HP sekarang: {currentHealth}/{maxHealth}");
        UIManager.Instance?.UpdatePlayerUI(currentHealth, maxHealth, currentShield);
    }

    /// <summary>
    /// Tambahkan perisai ke player. Perisai bertahan hingga awal giliran berikutnya.
    /// </summary>
    public void AddShield(int amount)
    {
        currentShield += amount;
        Debug.Log($"Player mendapat perisai +{amount}. Total perisai: {currentShield}");
        UIManager.Instance?.UpdatePlayerUI(currentHealth, maxHealth, currentShield);
    }

    /// <summary>
    /// Reset perisai ke nol. Dipanggil oleh TurnManager setiap awal giliran player.
    /// </summary>
    public void ResetShield()
    {
        currentShield = 0;
        UIManager.Instance?.UpdatePlayerUI(currentHealth, maxHealth, currentShield);
    }

    // -------------------------------------------------------
    // Death
    // -------------------------------------------------------

    private void OnPlayerDeath()
    {
        // === TODO: Tambahkan logika Game Over di sini ===
        // Contoh: SceneManager.LoadScene("GameOverScene");
        // Atau: UIManager.Instance.ShowGameOverScreen();
    }
}