using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameResetController : MonoBehaviour
{
     
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if(Input.GetKeyUp(KeyCode.R)) {
            SceneManager.LoadScene(0);
        }
    }
}
