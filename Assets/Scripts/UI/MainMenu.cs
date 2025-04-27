using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{    //Load Scene    
    public void Play(){
        /* 
     * Teniendo en cuenta el orden de las escenas en el Build Settings, carga la siguiente escena guiandose por el index actual.
     * Se obtiene el índice de la escena activa y se le suma 1 para cargar la siguiente.
        */
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); 
      /* SceneManager.LoadScene("Scene Name");  */ // Otra forma de hacerlo sin trabajar con los indexs.
    }
}
