//Created by: Liam Gilmore
using Devhouse.Tools.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IconManager : Singleton<IconManager>
{
    [SerializeField]
    private GameObject[] iconsArray = new GameObject[6];

    [SerializeField]
    private Sprite summitIcon;
    [SerializeField]
    private Sprite houseIcon;
    [SerializeField]
    private Sprite paintBrushIcon;
    [SerializeField]
    private Sprite treeIcon;
    [SerializeField]
    private Sprite weatherIcon;
    [SerializeField]
    private Sprite wildLifeIcon;
    [SerializeField]
    private Sprite exitSubmenuIcon;

    private readonly string summit1Name = "Summit α";
    private readonly string summit2Name = "Summit β";
    private readonly string summit3Name = "Summit γ";
    private readonly string summit4Name = "Summit δ";

    private void Start()
    {
        iconsArray[0].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = summit1Name;
        iconsArray[1].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = summit2Name;
        iconsArray[3].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = summit3Name;
        iconsArray[4].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = summit4Name;
    }

    public void ToggleIconsVisibility(bool toggle)
    {
        foreach (GameObject icon in iconsArray)
        {
            icon.SetActive(toggle);
        }
    }

    public void ExitSubmenu()
    {
        for (int i = 0; i < iconsArray.Length; i++)
        {
            switch (i)
            {
                case 0:
                    iconsArray[i].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = summit1Name;
                    break;
                case 1:
                    iconsArray[i].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = summit2Name;
                    break;
                case 2:
                    break;
                case 3:
                    iconsArray[i].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = summit3Name;
                    break;
                case 4:
                    iconsArray[i].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = summit4Name;
                    break;
                case 5:
                    break;
            }
            if (i != 2 && i != 5)
            {
                iconsArray[i].SetActive(true);
                iconsArray[i].GetComponent<Image>().sprite = summitIcon;
            }
        }
    }

    public void EnterSubmenu() 
    {
        iconsArray[0].GetComponent<Image>().sprite = exitSubmenuIcon;
        iconsArray[0].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "Back";

        iconsArray[1].GetComponent<Image>().sprite = paintBrushIcon;
        iconsArray[1].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "Hue";

        iconsArray[2].GetComponent<Image>().sprite = wildLifeIcon;
        iconsArray[2].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "Deer";

        iconsArray[3].GetComponent<Image>().sprite = houseIcon;
        iconsArray[3].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "Lodges";

        iconsArray[4].GetComponent<Image>().sprite = treeIcon;
        iconsArray[4].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "Trees";

        iconsArray[5].GetComponent<Image>().sprite = weatherIcon;
        iconsArray[5].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "Weather";

        ToggleIconsVisibility(true);
    }
}
