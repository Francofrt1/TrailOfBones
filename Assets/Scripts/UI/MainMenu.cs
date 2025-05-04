using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    void Start()
    {
        if (GameManager.Instance != null){GameManager.Instance.SetCursorState(true);}
    }

    //Load Scene    
    public void Play(){
        /* 
     * Teniendo en cuenta el orden de las escenas en el Build Settings, carga la siguiente escena guiandose por el index actual.
     * Se obtiene el �ndice de la escena activa y se le suma 1 para cargar la siguiente.
        */
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartGame();
        }
      /* SceneManager.LoadScene("Scene Name");  */ // Otra forma de hacerlo sin trabajar con los indexs.
    }
}
