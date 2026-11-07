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
        SceneManager.LoadScene("LieDetector");
    }

    public void OnQuitButton()
    {
        Application.Quit();
    }

}
