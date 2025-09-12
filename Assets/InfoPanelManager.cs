using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class InfoPanelManager : MonoBehaviour
{
    [Header("References")]
    public Paytable paytable;             
    public Transform rowContainer;        
    public GameObject rowPrefab;          

    void Start()
    {
        PopulateInfoPanel();
    }

    public void PopulateInfoPanel()
    {
        if (paytable == null || rowContainer == null || rowPrefab == null)
        {
            Debug.LogWarning("[InfoPanelManager] Missing references!");
            return;
        }

        // clear
        foreach (Transform child in rowContainer)
            Destroy(child.gameObject);

        // sort in basevalue asc order
        var sortedSymbols = paytable.symbols.OrderBy(s => s.baseValue).ToArray();

        
        foreach (var symbol in sortedSymbols)
        {
            GameObject rowObj = Instantiate(rowPrefab, rowContainer);
            
            Image icon = rowObj.transform.Find("Icon").GetComponent<Image>();
            Text nameText = rowObj.transform.Find("NameText").GetComponent<Text>();
            Text baseText = rowObj.transform.Find("BaseValueText").GetComponent<Text>();
            Text twoXText = rowObj.transform.Find("TwoXText").GetComponent<Text>();
            Text threeXText = rowObj.transform.Find("ThreeXText").GetComponent<Text>();

            if (icon != null) icon.sprite = symbol.icon;
            if (nameText != null) nameText.text = symbol.name;
            if (baseText != null) baseText.text = symbol.baseValue.ToString();
            if (twoXText != null) twoXText.text = $"x{symbol.twoMultiplier}";
            if (threeXText != null) threeXText.text = $"x{symbol.threeMultiplier}";
        }
    }
}
