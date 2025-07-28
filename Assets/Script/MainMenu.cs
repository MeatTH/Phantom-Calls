using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public AudioClip clickSound; // ลากไฟล์เสียงใส่จาก Inspector
    public AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayClickSound()
    {
        if (clickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }

    public void OnPlayButtonClicked()
    {
        PlayClickSound();
        SceneManager.LoadScene("Selectstory");
    }

    public void OnExitButtonClicked()
    {
        PlayClickSound();
        Application.Quit();
    }

    public void OnBackToMainButtonClicked()
    {
        PlayClickSound();
        SceneManager.LoadScene("Mainmenu");
    }
}
