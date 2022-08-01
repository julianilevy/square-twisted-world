using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrapper : MonoBehaviour
{
    public bool adminMode;

    void Awake()
    {
        CreateGameManager();
        SceneManager.LoadScene("Menu");
        Destroy(gameObject);
    }

    void CreateGameManager()
    {
        var gameManagerGO = new GameObject();
        gameManagerGO.name = "Game Manager";
        var gameManager = gameManagerGO.AddComponent<GameManager>();
        gameManager.adminMode = adminMode;
    }
}