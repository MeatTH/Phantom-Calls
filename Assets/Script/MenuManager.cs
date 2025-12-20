using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void OnStoryButtonClicked(int storyIndex)
    {
    
        StoryLoader.selectedStoryIndex = storyIndex;

        if (storyIndex == 0)
        {
            SceneManager.LoadScene("Story1");
        }
        else if (storyIndex == 1)
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
        // หยุด BGM ก่อนเปลี่ยนซีน
        //if (SoundManager.instance != null)
        //{
        //    SoundManager.instance.StopBGM();
        //}

        // สุ่มค่า 0,1,2
        int randomIndex = Random.Range(0, 3);

        // บันทึกค่า story ที่สุ่มไว้ (ถ้าระบบอื่นใช้ selectedStoryIndex อยู่)
        StoryLoader.selectedStoryIndex = randomIndex;

        // โหลดฉากตาม index ที่สุ่มได้
        if (randomIndex == 0)
        {
            SceneManager.LoadScene("Story1");
        }
        else if (randomIndex == 1)
        {
            SceneManager.LoadScene("Story2");
        }
        else if (randomIndex == 2)
        {
            SceneManager.LoadScene("Story3");
        }

        Debug.Log("Random story index = " + randomIndex);
    }
}
