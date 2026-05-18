using UnityEngine;

public class sceneman : MonoBehaviour
{
    public void changeScene(int sceneIndex)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
    }
}
