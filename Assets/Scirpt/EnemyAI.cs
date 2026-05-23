using UnityEngine;

/// <summary>
/// Mengontrol musuh: HP, menerima damage, dan menjalankan aksi per giliran.
/// </summary>
public class EnemyAI : MonoBehaviour
{
    public static EnemyAI Instance { get; private set; }

    [Header("Stats")]
    public int maxHealth = 20;
    public int currentHealth;

    // Apakah musuh sudah mati? Mencegah pemrosesan ganda.
    private bool isDead = false;

    // Event — UIManager mendengarkan ini untuk update tampilan HP
    public delegate void OnHealthChanged(int current, int max);
    public event OnHealthChanged HealthChanged;

    // -------------------------------------------------------
    // Unity Messages
    // -------------------------------------------------------

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        currentHealth = maxHealth;
        isDead        = false;
    }

    private void Start()
    {
        // Beri tahu UI saat pertama kali game mulai
        HealthChanged?.Invoke(currentHealth, maxHealth);
    }

    // -------------------------------------------------------
    // Turn Logic
    // -------------------------------------------------------

    /// <summary>
    /// Dipanggil oleh TurnManager saat giliran musuh dimulai.
    /// </summary>
    public void StartEnemyTurn()
    {
        if (isDead) return; // Musuh sudah mati, tidak perlu beraksi

        Debug.Log("Musuh sedang berpikir...");

        // Pilih aksi secara acak (bisa dikembangkan menjadi sistem kartu musuh)
        int action = Random.Range(0, 3);

        switch (action)
        {
            case 0:
                int normalDamage = Random.Range(3, 7);
                Debug.Log($"Musuh menyerang! {normalDamage} damage.");
                PlayerController.Instance?.TakeDamage(normalDamage);
                break;

            case 1:
                int heavyDamage = Random.Range(6, 12);
                Debug.Log($"Musuh melancarkan serangan kuat! {heavyDamage} damage!");
                PlayerController.Instance?.TakeDamage(heavyDamage);
                break;

            case 2:
                Debug.Log("Musuh mempertahankan diri — tidak ada serangan giliran ini.");
                break;
        }

        // Akhiri giliran musuh setelah 1.5 detik
        Invoke(nameof(EndEnemyTurn), 1.5f);
    }

    private void EndEnemyTurn()
    {
        TurnManager.Instance?.EndTurn();
    }

    // -------------------------------------------------------
    // Damage & Death
    // -------------------------------------------------------

    /// <summary>
    /// Musuh menerima sejumlah damage dari kartu player.
    /// </summary>
    public void TakeDamage(int damage)
    {
        // Abaikan jika musuh sudah mati
        if (isDead) return;

        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;

        Debug.Log($"Musuh kena {damage} damage! Sisa HP: {currentHealth}/{maxHealth}");
        HealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            isDead = true;
            Debug.Log("Musuh mati! Kamu menang!");
            OnEnemyDeath();
        }
    }

    private void OnEnemyDeath()
    {
        CancelInvoke(); // Batalkan EndEnemyTurn yang mungkin masih tertunda
        gameObject.SetActive(false);

        // === TODO: Tambahkan logika menang di sini ===
        // Contoh: SceneManager.LoadScene("WinScene");
    }
}