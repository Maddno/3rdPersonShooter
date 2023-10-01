using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] float sceneLoadDelay = 2f;
    public void LoadGameLvl()
    {
        SceneManager.LoadScene("GameLvl");
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadGameOver()
    {
       StartCoroutine(WaitAndLoad("GameOver", sceneLoadDelay));
    }

    public void LoadGameWin()
    {
       StartCoroutine(WaitAndLoad("GameWin", sceneLoadDelay));
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void SavedData()
    {
        SaveLoadSystem.SavePlayer(playerController);
    }

    public void LoadData()
    {
        SavedData data = SaveLoadSystem.LoadPlayer();

        playerController.playerHealth = data.health;

        Vector3 position;
        position.x = data.position[0];
        position.y = data.position[1];
        position.z = data.position[2];

        playerController.transform.position = position;
    }

    IEnumerator WaitAndLoad(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }
}
