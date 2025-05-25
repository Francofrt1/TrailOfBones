using Multiplayer;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    void Start()
    {
        if (GameManager.Instance != null)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
    
    public void Play()
    {
        ScenesManager.ChangeScene("MainLevel");
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Multiplayer()
    {
        ScenesManager.ChangeScene("MultiplayerSelector");
    }
}
