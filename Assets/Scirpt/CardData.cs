using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "Card Game/Card")]
public class CardData : ScriptableObject
{
    public string cardName;
    public int apCost;
    public int damage;
    public int heal;
    public string description;
}
