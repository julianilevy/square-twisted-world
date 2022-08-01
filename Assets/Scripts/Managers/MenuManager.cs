using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    public FadeTransition menuCamera;
    public Material fadeMaterial;
    public GameObject menuLogo;
    public GameObject mainMenu;
    public GameObject playSubMenu;
    public GameObject playSubMenuStoryMode;
    public GameObject playSubMenuCustomLevels;
    public GameObject levelMakerSubMenu;
    public GameObject controlsSubMenu;

    [HideInInspector]
    public static MenuManager instance;

    void Awake()
    {
        if (instance == null)
            instance = this;

        menuCamera.Initialize(fadeMaterial, 0f);
        menuCamera.DisableFade(GameManager.fadeTime);
    }

    public void OpenPlaySubMenu() // Llamado desde el botón...
    {
        mainMenu.SetActive(false);
        playSubMenu.SetActive(true);
    }

    public void OpenLevelMakerSubMenu() // Llamado desde el botón...
    {
        mainMenu.SetActive(false);
        levelMakerSubMenu.SetActive(true);
        LevelListManager.instance.CreateLevelList("Custom", LevelListManager.instance.lmCustomLevelListReference, LevelListManager.instance.lmCustomLevelList, LevelListManager.instance.EditLevel);
    }

    public void StartNewStoryModeLevel() // Llamado desde el botón...
    {
        menuCamera.Initialize(fadeMaterial, 0f);
        menuCamera.EnableFade(GameManager.fadeTime);
        StartCoroutine(ExecuteStartNewStoryModeLevel());
    }

    IEnumerator ExecuteStartNewStoryModeLevel()
    {
        yield return new WaitForSeconds(GameManager.fadeTime);

        SceneManager.LoadScene("Play");
        GameManager.instance.UsingLevelMaker(true);
        // Aclarar que es story mode...
    }

    public void StartNewCustomLevel() // Llamado desde el botón...
    {
        menuCamera.Initialize(fadeMaterial, 0f);
        menuCamera.EnableFade(GameManager.fadeTime);
        StartCoroutine(ExecuteStartNewCustomLevel());
    }

    IEnumerator ExecuteStartNewCustomLevel()
    {
        yield return new WaitForSeconds(GameManager.fadeTime);

        SceneManager.LoadScene("Play");
        GameManager.instance.UsingLevelMaker(true);
        // Aclarar que es custom...
    }

    public void OpenControlsSubMenu() // Llamado desde el botón...
    {
        menuLogo.SetActive(false);
        mainMenu.SetActive(false);
        controlsSubMenu.SetActive(true);
    }

    public void Exit() // Llamado desde el botón...
    {
        Application.Quit();
    }

    public void BackFromControlsSubMenu() // Llamado desde el botón...
    {
        menuLogo.SetActive(true);
        mainMenu.SetActive(true);
        controlsSubMenu.SetActive(false);
    }

    public void BackFromLevelMakerSubMenu() // Llamado desde el botón...
    {
        mainMenu.SetActive(true);
        levelMakerSubMenu.SetActive(false);
        LevelListManager.instance.DeleteLevelList(LevelListManager.instance.lmStoryModeLevelList);
        LevelListManager.instance.DeleteLevelList(LevelListManager.instance.lmCustomLevelList);
    }

    public void BackFromPlaySubMenu() // Llamado desde el botón...
    {
        mainMenu.SetActive(true);
        playSubMenu.SetActive(false);
    }

    public void OpenPlaySubMenuStoryMode() // Llamado desde el botón...
    {
        playSubMenu.SetActive(false);
        playSubMenuStoryMode.SetActive(true);
        GameManager.instance.lastLevelWasStory = true;
        LevelListManager.instance.CreateLevelList("Story Mode", LevelListManager.instance.storyModeLevelListReference, LevelListManager.instance.storyModeLevelList, LevelListManager.instance.PlayLevel);
    }

    public void BackFromPlaySubMenuStoryMode() // Llamado desde el botón...
    {
        playSubMenu.SetActive(true);
        playSubMenuStoryMode.SetActive(false);
        LevelListManager.instance.DeleteLevelList(LevelListManager.instance.storyModeLevelList);
    }

    public void OpenPlaySubMenuCustomLevels() // Llamado desde el botón...
    {
        playSubMenu.SetActive(false);
        playSubMenuCustomLevels.SetActive(true);
        GameManager.instance.lastLevelWasStory = false;
        LevelListManager.instance.CreateLevelList("Custom", LevelListManager.instance.customLevelListReference, LevelListManager.instance.customLevelList, LevelListManager.instance.PlayLevel);
    }

    public void BackFromPlaySubMenuCustomLevels() // Llamado desde el botón...
    {
        playSubMenu.SetActive(true);
        playSubMenuCustomLevels.SetActive(false);
        LevelListManager.instance.DeleteLevelList(LevelListManager.instance.customLevelList);
    }
}