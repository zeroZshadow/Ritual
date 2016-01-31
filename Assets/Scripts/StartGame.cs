using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour {
    public GameObject creditsPanel;

    void Update()
    {
        if (Input.GetKeyDown("return"))
        {
            SceneManager.LoadScene("Scenes/Levels/Level1");
        }
        if(Input.GetKeyDown("c"))
        {
            var newState = !creditsPanel.gameObject.activeSelf;

            creditsPanel.gameObject.SetActive(newState);
        }

    }
}
