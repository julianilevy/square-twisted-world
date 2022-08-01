using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CanvasUIManager : MonoBehaviour
{
    [HideInInspector]
    public static CanvasUIManager instance;

    public FadeTransition uiCamera;
    public Camera fullMapCamera;
    public GameObject blackBackground;
    public GameObject background;
    public Material noiseFade;
    public Material diagonalLinesFade;
    public GameObject grabObjectAlert;
    public GameObject youWinScreen;
    public GameObject deaths;
    public GameObject collectables;
    public Text deathCounterText;
    public Text currentCollectablesText;
    public Text totalCollectablesText;

    void Start()
    {
        if (instance == null)
            instance = this;

        totalCollectablesText.text = GameManager.instance.collectablesTotal.ToString("00");

        UpdateUI();
        DisableFade();
    }

    public void EnableUI(bool value)
    {
        deaths.SetActive(value);
        collectables.SetActive(value);
    }

    public void EnableFade(Material material)
    {
        GameManager.instance.isAbleToPlay = false;
        GameManager.instance.lastFadedMaterial = material;

        uiCamera.Initialize(material, 0f);
        uiCamera.EnableFade(GameManager.fadeTime);
    }

    public void DisableFade()
    {
        uiCamera.Initialize(GameManager.instance.lastFadedMaterial, 1f);
        uiCamera.DisableFade(GameManager.fadeTime);
        StartCoroutine(ExecuteDisableFade());
    }

    IEnumerator ExecuteDisableFade()
    {
        yield return new WaitForSeconds(0.2f);

        GameManager.instance.isAbleToPlay = true;

        yield return new WaitForSeconds(GameManager.fadeTime - 0.1f);

        GameManager.fadeTime = 1f;
    }

    public void UpdateUI()
    {
        UpdateDeaths();
        UpdateCollectables();
    }

    void UpdateDeaths()
    {
        deathCounterText.text = GameManager.instance.deathCounter.ToString("0000");
    }

    void UpdateCollectables()
    {
        if (ProgressManager.instance != null)
            currentCollectablesText.text = ProgressManager.instance.collectablesObtained.ToString("00");
    }
}