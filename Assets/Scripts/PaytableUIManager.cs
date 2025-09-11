using UnityEngine;
using UnityEngine.UI;

public class PaytableUIManager : MonoBehaviour
{
    [Header("References")]
    public Paytable paytable;        // paytable reference
    public BetManager betManager;    

    [System.Serializable]
    public class RowUI
    {
        public SlotSymbol symbol;        // Icon Sriptable Object
        public Text twoOfKindText;       
        public Text threeOfKindText;     
    }

    [Header("UI Rows (visual order)")]
    public RowUI[] rows;  // paytable UI

    int lastKnownBet = -1;

    public static PaytableUIManager Instance {  get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (betManager == null && BetManager.Instance != null)
            betManager = BetManager.Instance;

        UpdatePaytableUI();
    }

    void Update()
    {
        // auto refresh on bet change
        int currentBet = GetCurrentBet();
        if (currentBet != lastKnownBet)
        {
            UpdatePaytableUI();
            lastKnownBet = currentBet;
        }
    }

    int GetCurrentBet()
    {
        if (betManager != null) return betManager.betAmount;
        if (BetManager.Instance != null) return BetManager.Instance.betAmount;
        return 0;
    }

    public void UpdatePaytableUI()
    {
        if (paytable == null)
        {
            Debug.LogWarning("[PaytableUIManager] Paytable asset not assigned.");
            return;
        }

        if (rows == null || rows.Length == 0)
        {
            Debug.LogWarning("[PaytableUIManager] No rows assigned in inspector.");
            return;
        }

        int bet = GetCurrentBet();

        for (int i = 0; i < rows.Length; i++)
        {
            var row = rows[i];
            SlotSymbol symbol = row.symbol;

            if (symbol == null)
            {
                if (i < paytable.symbols.Length)
                    symbol = paytable.symbols[i];
                else
                {
                    if (row.twoOfKindText != null) row.twoOfKindText.text = "-";
                    if (row.threeOfKindText != null) row.threeOfKindText.text = "-";
                    Debug.LogWarning($"[PaytableUIManager] Row {i} has no symbol and paytable has no symbol at index {i}.");
                    continue;
                }
            }

            if (symbol == null)
            {
                if (row.twoOfKindText != null) row.twoOfKindText.text = "-";
                if (row.threeOfKindText != null) row.threeOfKindText.text = "-";
                continue;
            }

            
            int twoAmount = Mathf.RoundToInt(bet * symbol.baseValue * symbol.twoMultiplier);
            int threeAmount = Mathf.RoundToInt(bet * symbol.baseValue * symbol.threeMultiplier);

            if (row.twoOfKindText != null) row.twoOfKindText.text = twoAmount.ToString();
            if (row.threeOfKindText != null) row.threeOfKindText.text = threeAmount.ToString();
        }
    }

    public Sprite GetSymbolSprite(int index)
    {
        if (paytable == null || paytable.symbols == null) return null;

        if (index < 0 || index >= paytable.symbols.Length)
            return null;

        return paytable.symbols[index].icon; 
    }

}
