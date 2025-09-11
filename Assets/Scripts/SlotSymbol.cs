using UnityEngine;

[CreateAssetMenu(fileName = "NewSlotSymbol", menuName = "SlotMachine/SlotSymbol")]
public class SlotSymbol : ScriptableObject
{
    [Header("Identity")]
    public string displayName;   // cherry

    [Header("Visual")]
    public Sprite icon;          // image

    [Header("Value / Payout (tweak as needed)")]
    [Tooltip("Base value for this symbol. You can combine this with multipliers to compute final payout.")]
    public int baseValue = 1;

    [Tooltip("Multiplier applied for 2-of-a-kind (optional). Final payout = bet * baseValue * twoMultiplier")]
    public int twoMultiplier = 1;

    [Tooltip("Multiplier applied for 3-of-a-kind (optional). Final payout = bet * baseValue * threeMultiplier")]
    public int threeMultiplier = 5;

    [Header("Spin probability")]
    [Tooltip("Higher weight → symbol appears more often when randomly chosen")]
    public int weight = 1;

    [Header("Icon Position")]
    public float yPosition;
}
