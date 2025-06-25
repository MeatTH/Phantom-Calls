using UnityEngine;

public class SelectUI : MonoBehaviour
{
    public GameObject mainSelectPanel;
    public GameObject selectMenuPanel;

    public void OnBackButtonClicked()
    {
        mainSelectPanel.SetActive(false);
        selectMenuPanel.SetActive(true);
    }

    public void OnSelectStoryClicked()
    {
        selectMenuPanel.SetActive(false);
        mainSelectPanel.SetActive(true);
    }
}
