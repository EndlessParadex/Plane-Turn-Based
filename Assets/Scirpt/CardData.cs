using UnityEngine;

/// <summary>
/// Tipe kartu yang menentukan efek utamanya.
/// </summary>
public enum CardType
{
    Attack, // Kartu serangan — memberikan damage ke musuh
    Heal,   // Kartu pemulihan — menyembuhkan HP player
    Shield  // Kartu pertahanan — memberikan perisai ke player
}

/// <summary>
/// Data sebuah kartu. Buat kartu baru via:
/// klik kanan di Project → Card Game → Card
/// </summary>
[CreateAssetMenu(fileName = "NewCard", menuName = "Card Game/Card")]
public class CardData : ScriptableObject
{
    [Header("Info")]
    public string cardName;
    public CardType cardType;

    [Header("Cost")]
    public int apCost;

    [Header("Effects")]
    public int damage;   // Damage ke musuh
    public int heal;     // HP yang dipulihkan ke player
    public int shield;   // Perisai yang diberikan ke player

    [Header("Flavor")]
    [TextArea] public string description;
}
