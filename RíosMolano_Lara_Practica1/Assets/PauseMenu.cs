using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {
    
    public void Replay()
    {
        SceneManager.LoadSceneAsync("ShootingGame");
    }

    public void Quit()
    {
        SceneManager.LoadSceneAsync("SampleScene");
    }
}
