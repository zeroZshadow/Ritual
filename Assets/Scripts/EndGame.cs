using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour {

    void Update()
    {
        if (Input.GetKeyDown("return"))
        {
            SceneManager.LoadScene("Scenes/StartGame");
        }

    }
}
