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

    private bool isSpinning = false;

    private int lastSlot1, lastSlot2, lastSlot3;

    void Start()
    {
        if (BetBtn != null)
            BetBtn.onClick.AddListener(SpinSlots);
    }

    void SpinSlots()
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

        var length = BetManager.Instance.paytable.symbols.Length;

        // icon probablity
        int slot1, slot2, slot3;
        int prob = Random.Range(0, 100);

        if (prob < 10)
        {
            slot1 = slot2 = slot3 = Random.Range(0, length);
        }
        else if (prob < 40)
        {
            slot1 = Random.Range(0, length);
            int pairType = Random.Range(0, 3);
            if (pairType == 0)
            {
                slot2 = slot1;
                do { slot3 = Random.Range(0, length); } while (slot3 == slot1);
            }
            else if (pairType == 1)
            {
                slot3 = slot1;
                do { slot2 = Random.Range(0, length); } while (slot2 == slot1);
            }
            else
            {
                slot2 = slot3 = Random.Range(0, length);
                do { slot1 = Random.Range(0, length); } while (slot1 == slot2);
            }
        }
        else
        {
            List<int> slots = new List<int>();
            for (int i = 0; i < length; i++) slots.Add(i);

            slot1 = slots[Random.Range(0, slots.Count)]; slots.Remove(slot1);
            slot2 = slots[Random.Range(0, slots.Count)]; slots.Remove(slot2);
            slot3 = slots[Random.Range(0, slots.Count)];
        }

        lastSlot1 = slot1;
        lastSlot2 = slot2;
        lastSlot3 = slot3;

        StartCoroutine(SpinRow(row1, slot1, 0.5f));
        StartCoroutine(SpinRow(row2, slot2, 1.0f));
        StartCoroutine(SpinRow(row3, slot3, 1.5f, true));
    }

    IEnumerator SpinRow(RectTransform row, int targetIndex, float duration, bool isLastRow = false)
    {
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

        Vector3 finalPos = row.localPosition;
        finalPos.y = BetManager.Instance.paytable.symbols[targetIndex].yPosition;
        row.localPosition = finalPos;

        if (isLastRow)
        {
            isSpinning = false;
            BetManager.Instance.ResolveSpin(lastSlot1, lastSlot2, lastSlot3);
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
