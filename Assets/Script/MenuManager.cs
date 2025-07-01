using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void OnStoryButtonClicked(int storyIndex)
    {
        StoryLoader.selectedStoryIndex = storyIndex;
        SceneManager.LoadScene("Ingame");
    }

    public void OnRandomStoryClicked()
    {
        int randomIndex = Random.Range(0, 3); // 0=Story1, 1=Story2, 2=Story3
        StoryLoader.selectedStoryIndex = randomIndex;
        SceneManager.LoadScene("Ingame");
    }
}

