using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject CardCreator;

    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void StartCardCreator()
    {
        CardCreator.SetActive(true);
    }
    
    public void CloseCardCreator()
    {
        CardCreator.SetActive(false);
        CardCreator.GetComponent<CreateCardMenu>().output.text = "";
    }

    public void CreateCard()
    {
        if (CardCreator.GetComponent<CreateCardMenu>().canBeCompiled)
        {
            CardCreator.GetComponent<CreateCardMenu>().SaveInput();
            CardCreator.GetComponent<CreateCardMenu>().canBeCompiled = false;
            CardCreator.SetActive(false);
        }
    }
}
