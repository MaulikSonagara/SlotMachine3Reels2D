using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class LogManager : MonoBehaviour
{
    [Header("UI References")]
    public Transform logContainer;       // parent object for logs (VerticalLayoutGroup)
    public GameObject logEntryPrefab;    // prefab for a single log entry

    private List<SpinLog> logs = new List<SpinLog>();
    private const int MaxLogs = 20;
    private const string PlayerPrefsKey = "SpinLogs";

    public static LogManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Instance = this; } return;

        DontDestroyOnLoad(Instance);
    }

    void Start()
    {
        LoadLogs();
        RefreshUI();
    }

    public void AddLog(int betAmount, int[] results, int winAmount)
    {
        SpinLog newLog = new SpinLog
        {
            betAmount = betAmount,
            betTime = DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy"),
            reelResults = results,
            winAmount = winAmount
        };

        logs.Insert(0, newLog); // latest at top

        if (logs.Count > MaxLogs)
            logs.RemoveAt(logs.Count - 1);

        SaveLogs();
        RefreshUI();
    }

    private void RefreshUI()
    {
        foreach (Transform child in logContainer)
            Destroy(child.gameObject);

        foreach (var log in logs)
        {
            GameObject entry = Instantiate(logEntryPrefab, logContainer);

            entry.transform.Find("BetText").GetComponent<Text>().text = $"Bet: {log.betAmount}";
            entry.transform.Find("TimeText").GetComponent<Text>().text = log.betTime;
            entry.transform.Find("ResultText").GetComponent<Text>().text = $"Win: {log.winAmount}";

            // Result Icons
            Transform iconsParent = entry.transform.Find("Icons");
            foreach (Transform child in iconsParent) Destroy(child.gameObject);

            foreach (int idx in log.reelResults)
            {
                GameObject icon = new GameObject("Icon", typeof(Image));
                icon.transform.SetParent(iconsParent, false);
                icon.GetComponent<Image>().sprite = PaytableUIManager.Instance.GetSymbolSprite(idx);
            }
        }
    }

    private void SaveLogs()
    {
        string json = JsonUtility.ToJson(new SpinLogListWrapper { logs = logs });
        PlayerPrefs.SetString(PlayerPrefsKey, json);
        PlayerPrefs.Save();
    }

    private void LoadLogs()
    {
        string json = PlayerPrefs.GetString(PlayerPrefsKey, "");
        if (!string.IsNullOrEmpty(json))
        {
            SpinLogListWrapper wrapper = JsonUtility.FromJson<SpinLogListWrapper>(json);
            if (wrapper != null && wrapper.logs != null)
                logs = wrapper.logs;
        }
    }

    [System.Serializable]
    private class SpinLogListWrapper
    {
        public List<SpinLog> logs;
    }
}
