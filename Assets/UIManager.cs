using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Main Buttons (PanelBtns)")]
    public Button settingsBtn;
    public Button logBtn;
    public Button infoBtn;

    [Header("Panels")]
    public GameObject settingsPanel;
    public GameObject logPanel;
    public GameObject infoPanel;

    [Header("Close Buttons (inside each panel)")]
    public Button settingsCloseBtn;
    public Button logCloseBtn;
    public Button infoCloseBtn;

    private void Start()
    {
        // UI buttons
        if (settingsBtn != null) settingsBtn.onClick.AddListener(OpenSettings);
        if (logBtn != null) logBtn.onClick.AddListener(OpenLog);
        if (infoBtn != null) infoBtn.onClick.AddListener(OpenInfo);

        // panel close buttons
        if (settingsCloseBtn != null) settingsCloseBtn.onClick.AddListener(CloseSettings);
        if (logCloseBtn != null) logCloseBtn.onClick.AddListener(CloseLog);
        if (infoCloseBtn != null) infoCloseBtn.onClick.AddListener(CloseInfo);

        HideAllPanels();
    }

    private void HideAllPanels()
    {
        settingsPanel.SetActive(false);
        logPanel.SetActive(false);
        infoPanel.SetActive(false);
    }

    private void OpenSettings()
    {
        HideAllPanels();
        settingsPanel.SetActive(true);
    }

    private void OpenLog()
    {
        HideAllPanels();
        logPanel.SetActive(true);
    }

    private void OpenInfo()
    {
        HideAllPanels();
        infoPanel.SetActive(true);
    }

    private void CloseSettings() => settingsPanel.SetActive(false);
    private void CloseLog() => logPanel.SetActive(false);
    private void CloseInfo() => infoPanel.SetActive(false);
}
