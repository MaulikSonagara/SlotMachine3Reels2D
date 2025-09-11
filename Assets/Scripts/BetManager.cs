using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BetManager : MonoBehaviour
{
    public static BetManager Instance { get; private set; }

    [Header("Data")]
    public Paytable paytable;
    public int startingBalance = 1000;
    public int betAmount = 10;

    [Header("Runtime State")]
    public int balance;
    private bool autoBetActive = false;
    private Coroutine autoBetRoutine;

    [Header("UI")]
    public Text balanceText;
    public Text betText;
    public Text claimedText;
    public Text messageText;


    [Header("AudioManager")]
    public AudioManager audioManager;

    [Header("Bet Buttons")]
    public Button plus1Button;
    public Button plus10Button;
    public Button plus100Button;
    public Button plus1000Button;
    public Button minus1Button;
    public Button minus10Button;
    public Button minus100Button;
    public Button minus1000Button;
    public Button betButton;       // single spin
    public Button autoBetButton;   // toggle auto-spin

    const string KEY_BALANCE = "SM_Balance";
    const string KEY_BET = "SM_Bet";

    private Coroutine clearClaimCoroutine;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();


        Load();
    }

    void Start()
    {
        UpdateUI();
        HookupButtons();
    }

    void HookupButtons()
    {
        if (plus1Button != null) plus1Button.onClick.AddListener(() => {
            audioManager.PlaySFX(audioManager.betAmountPlusSFX);
            AdjustBet(1);
        });
        if (plus10Button != null) plus10Button.onClick.AddListener(() => {
            audioManager.PlaySFX(audioManager.betAmountPlusSFX);
            AdjustBet(10);
        });
        if (plus100Button != null) plus100Button.onClick.AddListener(() => {
            audioManager.PlaySFX(audioManager.betAmountPlusSFX);
            AdjustBet(100);
        });
        if (plus1000Button != null) plus1000Button.onClick.AddListener(() => {
            audioManager.PlaySFX(audioManager.betAmountPlusSFX);
            AdjustBet(1000);
        });

        if (minus1Button != null) minus1Button.onClick.AddListener(() => {
            audioManager.PlaySFX(audioManager.betAmountMinusSFX);
            AdjustBet(-1);
        });
        if (minus10Button != null) minus10Button.onClick.AddListener(() => {
            audioManager.PlaySFX(audioManager.betAmountMinusSFX);
            AdjustBet(-10);
        });
        if (minus100Button != null) minus100Button.onClick.AddListener(() => {
            audioManager.PlaySFX(audioManager.betAmountMinusSFX);
            AdjustBet(-100);
        });
        if (minus1000Button != null) minus1000Button.onClick.AddListener(() => {
            audioManager.PlaySFX(audioManager.betAmountMinusSFX);
            AdjustBet(-1000);
        });

        if (betButton != null) betButton.onClick.AddListener(SpinOnce);
        if (autoBetButton != null) autoBetButton.onClick.AddListener(ToggleAutoBet);
    }

    void AdjustBet(int delta)
    {
        if (balance == 0)
        {
            betAmount = 0;
            ShowMessage("No funds");
            UpdateUI();
            Save();
            return;
        }

        int newBet = betAmount + delta;
        betAmount = Mathf.Clamp(newBet, 1, balance);
        UpdateUI();
        Save();
    }

    #region Spin / AutoBet
    public void SpinOnce()
    {
        if (HandleButton.Instance != null)
        {
            HandleButton.Instance.SpinSlots();
        }
        else
        {
            ShowMessage("No slot machine found");
        }
    }

    void ToggleAutoBet()
    {
        if (autoBetActive)
        {
            StopAutoBet();
        }
        else
        {
            StartAutoBet();
        }
    }

    void StartAutoBet()
    {
        
        if (balance < betAmount || betAmount <= 0)
        {
            ShowMessage("Not enough funds");
            audioManager.PlaySFX(audioManager.betAmountMinusSFX);
            return;
        }

        autoBetActive = true;
        autoBetRoutine = StartCoroutine(AutoSpinRoutine());
        ShowMessage("AutoBet ON");
        autoBetButton.image.color = new Color32(96, 136, 255, 255);
        audioManager.PlaySFX(audioManager.betAmountPlusSFX);
    }

    void StopAutoBet()
    {
        autoBetActive = false;
        if (autoBetRoutine != null) StopCoroutine(autoBetRoutine);
        autoBetRoutine = null;
        ShowMessage("AutoBet OFF");
        autoBetButton.image.color = new Color32(255, 246, 95, 255);
        audioManager.PlaySFX(audioManager.betAmountMinusSFX);
    }

    IEnumerator AutoSpinRoutine()
    {
        while (autoBetActive)
        {
            if (balance < betAmount)
            {
                ShowMessage("Not enough funds for AutoBet");
                StopAutoBet();   
                yield break;
            }

            SpinOnce();
            yield return new WaitForSeconds(2.5f); // adjust duration to reel spin
        }
    }

    #endregion

    #region TrySpend / ResolveSpin
    public bool TrySpendBet()
    {
        if (betAmount <= 0)
        {
            ShowMessage("Set a bet");
            return false;
        }

        if (balance < betAmount)
        {
            ShowMessage("Not enough funds");
            return false;
        }

        balance -= betAmount;
        UpdateUI();
        Save();
        return true;
    }

    public void ResolveSpin(int slotIndex1, int slotIndex2, int slotIndex3)
    {
        int payout = CalculatePayout(slotIndex1, slotIndex2, slotIndex3);

        if (payout > 0)
        {
            balance += payout;
            ShowClaimed("+" + payout);
            ShowMessage("You win!");
        }
        else
        {
            ShowClaimed("0");
            ShowMessage("Lose");
        }

        UpdateUI();
        Save();
    }
    #endregion

    #region Payout logic
    int CalculatePayout(int s1, int s2, int s3)
    {
        var symbols = paytable.symbols;
        if (s1 < 0 || s1 >= symbols.Length || s2 < 0 || s2 >= symbols.Length || s3 < 0 || s3 >= symbols.Length)
            return 0;

        var A = symbols[s1];
        var B = symbols[s2];
        var C = symbols[s3];

        // 3-of-a-kind
        if (s1 == s2 && s2 == s3)
            return Mathf.RoundToInt(betAmount * A.baseValue * A.threeMultiplier);

        // 2-of-a-kind
        if (s1 == s2)
            return Mathf.RoundToInt(betAmount * A.baseValue * A.twoMultiplier);
        if (s2 == s3)
            return Mathf.RoundToInt(betAmount * B.baseValue * B.twoMultiplier);
        if (s1 == s3)
            return Mathf.RoundToInt(betAmount * A.baseValue * A.twoMultiplier);

        return 0;
    }
    #endregion

    #region UI + Persistence
    void UpdateUI()
    {
        if (balanceText != null) balanceText.text = $"{balance}";
        if (betText != null) betText.text = $"{betAmount}";
    }

    void ShowClaimed(string text)
    {
        if (claimedText == null) return;
        if (clearClaimCoroutine != null) StopCoroutine(clearClaimCoroutine);
        claimedText.text = text;
        clearClaimCoroutine = StartCoroutine(ClearClaimAfter(2.0f));
    }

    IEnumerator ClearClaimAfter(float sec)
    {
        yield return new WaitForSeconds(sec);
        if (claimedText != null) claimedText.text = "";
    }

    void ShowMessage(string msg, float duration = 1.5f)
    {
        if (messageText == null) return;
        messageText.text = msg;
        StopCoroutine(nameof(ClearMessageAfter));
        StartCoroutine(ClearMessageAfter(duration));
    }

    IEnumerator ClearMessageAfter(float sec)
    {
        yield return new WaitForSeconds(sec);
        if (messageText != null) messageText.text = "";
    }

    void Save()
    {
        PlayerPrefs.SetInt(KEY_BALANCE, balance);
        PlayerPrefs.SetInt(KEY_BET, betAmount);
        PlayerPrefs.Save();
    }

    void Load()
    {
        balance = PlayerPrefs.GetInt(KEY_BALANCE, startingBalance);
        betAmount = PlayerPrefs.GetInt(KEY_BET, betAmount);

        if (balance == 0) betAmount = 0;
        else betAmount = Mathf.Clamp(betAmount, 1, Mathf.Max(1, balance));
    }
    #endregion
}
