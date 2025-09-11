using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ReelBuilder : MonoBehaviour
{
    [Header("Data")]
    public Paytable paytable;          // Your ScriptableObject with all symbols

    [Header("UI")]
    public RectTransform reelContainer; // Parent with VerticalLayoutGroup
    public GameObject iconPrefab;       // Prefab with Image component
    public float iconHeight = 150f;     // Height of each icon (set in Inspector or auto-detect)

    private List<RectTransform> iconSlots = new List<RectTransform>();
    private float totalStripHeight;

    void Start()
    {
        BuildReel();
    }

    public void BuildReel()
    {
        // Clear old icons
        foreach (Transform child in reelContainer)
            Destroy(child.gameObject);
        iconSlots.Clear();

        // Create one icon for each symbol
        foreach (var symbol in paytable.symbols)
        {
            GameObject iconGO = Instantiate(iconPrefab, reelContainer);
            Image img = iconGO.GetComponent<Image>();
            img.sprite = symbol.icon;
            img.preserveAspect = true;
            iconSlots.Add(iconGO.GetComponent<RectTransform>());
        }

        // Calculate total strip height (for wrapping)
        totalStripHeight = paytable.symbols.Length * iconHeight;
    }

    // Helper to get total strip height for spin calculations
    public float GetTotalStripHeight()
    {
        return totalStripHeight;
    }

    // Helper to get icon height
    public float GetIconHeight()
    {
        return iconHeight;
    }
}