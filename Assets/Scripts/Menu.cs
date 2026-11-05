using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void OnInterrogateButton()
    {
        SceneManager.LoadScene("Interrogation");
    }
    public void OnLieDetectButton()
    {
        SceneManager.LoadScene(1);
    }

    public void OnQuitButton()
    {
        Application.Quit();
    }

}
