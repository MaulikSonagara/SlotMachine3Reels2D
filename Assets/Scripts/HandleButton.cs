using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandleButton : MonoBehaviour
{
    public GameObject HandleBtn;
    public GameObject row1;
    public GameObject row2;
    public GameObject row3;
    public Animator animator;
    public Button BetBtn;

    private float[] slotPositions = { 3f, 1.85f, 0.7f, -0.35f, -1.35f, -2.30f, -3.25f };
    private bool isSpinning = false; 

    void Start()
    {
        if (BetBtn != null)
            BetBtn.onClick.AddListener(SpinSlots);
    }

    void SpinSlots()
    {
        if (isSpinning) return; 
        isSpinning = true;

        animator.SetTrigger("pull");

        int slot1, slot2, slot3;

        // selecting icons 

        int prob = Random.Range(0, 100);

        if (prob < 10) // all same
        {
            slot1 = slot2 = slot3 = Random.Range(0, 7);
        }
        else if (prob < 40) // two same
        {
            slot1 = Random.Range(0, 7);

            int pairType = Random.Range(0, 3);
            if (pairType == 0)
            {
                slot2 = slot1;
                do { slot3 = Random.Range(0, 7); } while (slot3 == slot1);
            }
            else if (pairType == 1)
            {
                slot3 = slot1;
                do { slot2 = Random.Range(0, 7); } while (slot2 == slot1);
            }
            else
            {
                slot2 = slot3 = Random.Range(0, 7);
                do { slot1 = Random.Range(0, 7); } while (slot1 == slot2);
            }
        }
        else // all different
        {
            List<int> slots = new List<int>() { 0, 1, 2, 3, 4, 5, 6 };
            slot1 = slots[Random.Range(0, slots.Count)];
            slots.Remove(slot1);

            slot2 = slots[Random.Range(0, slots.Count)];
            slots.Remove(slot2);

            slot3 = slots[Random.Range(0, slots.Count)];
        }

        // row spinning
        StartCoroutine(SpinRow(row1, slot1, 0.5f));
        StartCoroutine(SpinRow(row2, slot2, 1.0f));
        StartCoroutine(SpinRow(row3, slot3, 1.5f, true));
    }

    IEnumerator SpinRow(GameObject row, int targetSlot, float duration, bool isLastRow = false)
    {
        float elapsed = 0f;
        float startY = row.transform.localPosition.y;
        float targetY = slotPositions[targetSlot];

        float spinLoops = 5f;
        float totalTime = duration + spinLoops;

        while (elapsed < totalTime)
        {
            elapsed += Time.deltaTime * 5f;

            Vector3 pos = row.transform.localPosition;
            pos.y -= Time.deltaTime * 5f;

            if (pos.y < slotPositions[slotPositions.Length - 1])
            {
                pos.y = slotPositions[0];
            }

            row.transform.localPosition = pos;
            yield return null;
        }

        Vector3 finalPos = row.transform.localPosition;
        finalPos.y = targetY;
        row.transform.localPosition = finalPos;

        if (isLastRow)
        {
            isSpinning = false; 
        }
    }

    private void OnMouseDown()
    {
        if (isSpinning) return; 
        if (gameObject == HandleBtn)
        {
            SpinSlots();
        }
    }
}
