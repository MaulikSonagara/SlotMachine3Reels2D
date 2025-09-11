using UnityEngine;

[CreateAssetMenu(fileName = "NewPaytable", menuName = "SlotMachine/Paytable")]
public class Paytable : ScriptableObject
{
    [Header("All slot machine symbols")]
    public SlotSymbol[] symbols;
}
