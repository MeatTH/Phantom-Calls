using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void OnPlayButtonClicked()
    {
        SceneManager.LoadScene("Selectstory"); // name Scene 
    }

    public void OnExitButtonClicked()
    {
        Application.Quit();
    }
    public void OnBackToMainButtonClicked()
    {
        SceneManager.LoadScene("Mainmenu"); // ชื่อ scene ต้องตรงกับใน Build Settings
    }
}