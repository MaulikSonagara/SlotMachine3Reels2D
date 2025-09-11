using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandleButton : MonoBehaviour
{
    [Header("References")]
    public GameObject HandleBtn;
    public RectTransform row1;
    public RectTransform row2;
    public RectTransform row3;
    public Animator animator;
    public Button BetBtn;

    [Header("AudioManager")]
    public AudioManager audioManager;

    private bool isSpinning = false;

    private int lastSlot1, lastSlot2, lastSlot3;

    public static HandleButton Instance { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
    }

    void Start()
    {
        if (BetBtn != null)
            BetBtn.onClick.AddListener(SpinSlots);
               
    }

    public void SpinSlots()
    {
        if (isSpinning) return;

        if (BetManager.Instance == null)
        {
            Debug.LogError("BetManager instance not found in scene.");
            return;
        }

        if (!BetManager.Instance.TrySpendBet())
            return; // no funds

        isSpinning = true;

        animator?.SetTrigger("pull");

        int slot1, slot2, slot3;
        int prob = Random.Range(0, 100);

        // Triple match (7%)
        if (prob < 7)
        {
            slot1 = slot2 = slot3 = GetWeightedRandomSymbol();
        }
        // Double match (28%)
        else if (prob < 35) // 7 + 28 = 35
        {
            slot1 = GetWeightedRandomSymbol();
            int pairType = Random.Range(0, 3);
            if (pairType == 0)
            {
                slot2 = slot1;
                do { slot3 = GetWeightedRandomSymbol(); } while (slot3 == slot1);
            }
            else if (pairType == 1)
            {
                slot3 = slot1;
                do { slot2 = GetWeightedRandomSymbol(); } while (slot2 == slot1);
            }
            else
            {
                slot2 = slot3 = GetWeightedRandomSymbol();
                do { slot1 = GetWeightedRandomSymbol(); } while (slot1 == slot2);
            }
        }
        // All different (65%)
        else
        {
            HashSet<int> chosen = new HashSet<int>();
            while (chosen.Count < 3)
            {
                chosen.Add(GetWeightedRandomSymbol());
            }
            int[] arr = new int[3];
            chosen.CopyTo(arr);
            slot1 = arr[0];
            slot2 = arr[1];
            slot3 = arr[2];
        }

        lastSlot1 = slot1;
        lastSlot2 = slot2;
        lastSlot3 = slot3;

        StartCoroutine(SpinRow(row1, slot1, 0.5f, 0));
        StartCoroutine(SpinRow(row2, slot2, 1.0f, 1));
        StartCoroutine(SpinRow(row3, slot3, 1.5f, 2, true));
    }

    int GetWeightedRandomSymbol()
    {
        var symbols = BetManager.Instance.paytable.symbols;
        int totalWeight = 0;

        foreach (var s in symbols)
            totalWeight += s.weight;

        int rand = Random.Range(0, totalWeight);
        int cumulative = 0;

        for (int i = 0; i < symbols.Length; i++)
        {
            cumulative += symbols[i].weight;
            if (rand < cumulative)
                return i;
        }
        return symbols.Length - 1; // fallback
    }

    IEnumerator SpinRow(RectTransform row, int targetIndex, float duration, int reelIndex, bool isLastRow = false)
    {
        // start reel spin sound
        audioManager.PlayReelSpin(reelIndex);

        float elapsed = 0f;
        float spinLoops = 2.5f;
        float totalTime = duration + spinLoops;
        float scrollSpeed = 300f;

        float topY = BetManager.Instance.paytable.symbols[0].yPosition;
        float bottomY = BetManager.Instance.paytable.symbols[BetManager.Instance.paytable.symbols.Length - 1].yPosition;

        while (elapsed < totalTime)
        {
            elapsed += Time.deltaTime;
            Vector3 pos = row.localPosition;
            pos.y -= Time.deltaTime * scrollSpeed;

            if (pos.y < bottomY)
            {
                pos.y = topY;
            }

            row.localPosition = pos;
            yield return null;
        }

        // snap to symbol
        Vector3 finalPos = row.localPosition;
        finalPos.y = BetManager.Instance.paytable.symbols[targetIndex].yPosition;
        row.localPosition = finalPos;

        // Stop/spin/play icon select sound
        audioManager.StopReelSpin(reelIndex);

        if (isLastRow)
        {
            isSpinning = false;

            // resolve win/lose
            BetManager.Instance.ResolveSpin(lastSlot1, lastSlot2, lastSlot3);

            if (LogManager.Instance != null)
            {
                int[] results = new int[] { lastSlot1, lastSlot2, lastSlot3 };
                int lastWinAmount = BetManager.Instance.CalculatePayoutPublic(lastSlot1, lastSlot2, lastSlot3);
                LogManager.Instance.AddLog(BetManager.Instance.betAmount, results, lastWinAmount);
            }



            if (lastSlot1 == lastSlot2 && lastSlot2 == lastSlot3)
            {
                // all same sound
                audioManager.PlaySFX(audioManager.winAudio3X);
            }
            else if (lastSlot1 == lastSlot2 || lastSlot1 == lastSlot3 || lastSlot2 == lastSlot3)
            {
                // 2 same sound
                audioManager.PlaySFX(audioManager.winAudio2X);
            }
            else
            {
                // all different sound
                audioManager.PlaySFX(audioManager.looseAudio);
            }
        }

    }


    private void OnMouseDown()
    {
        if (isSpinning) return;
        if (HandleBtn != null && gameObject == HandleBtn)
        {
            SpinSlots();
        }
    }
}
