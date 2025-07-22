using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void OnStoryButtonClicked(int storyIndex)
    {
        StoryLoader.selectedStoryIndex = storyIndex;
        if(storyIndex == 0)
        {
            SceneManager.LoadScene("Story1");
        }
        else if(storyIndex == 1)
        {
            SceneManager.LoadScene("Story2");
        }
        else
        {
            SceneManager.LoadScene("Story3");
        }
        
    }

    public void OnRandomStoryClicked()
    {
        int randomIndex = Random.Range(0, 3); // 0=Story1, 1=Story2, 2=Story3
        StoryLoader.selectedStoryIndex = randomIndex;
        SceneManager.LoadScene("Ingame");
    }
}

