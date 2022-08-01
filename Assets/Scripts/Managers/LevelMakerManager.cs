using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(LMContainers))]
[RequireComponent(typeof(LMBasePrefabLayerSorter))]
[RequireComponent(typeof(LMWaypointPicker))]
public class LevelMakerManager : MonoBehaviour
{
    public LMSettings levelMakerSettings;
    public Camera levelMakerCamera;
    public GameObject grid;
    public Canvas canvas;
    public Text currentSubLevelText;

    [HideInInspector]
    public static LevelMakerManager instance;
    [HideInInspector]
    public LMContainers lmContainers;
    [HideInInspector]
    public LMBasePrefabLayerSorter lmBasePrefabLayerSorter;
    [HideInInspector]
    public LMWaypointPicker lmWaypointPicker;
    [HideInInspector]
    public int currentSubLevel = 0;
    [HideInInspector]
    public int subLevelsAmount = 1;

    void Awake()
    {
        if (instance == null)
            instance = this;

        lmContainers = GetComponent<LMContainers>();
        lmBasePrefabLayerSorter = GetComponent<LMBasePrefabLayerSorter>();
        lmWaypointPicker = GetComponent<LMWaypointPicker>();

        if (GameManager.instance.IsUsingLevelMaker())
            EnableLevelMaker();
        else
            DisableLevelMaker();
    }

    void EnableLevelMaker()
    {
        levelMakerSettings.gameObject.SetActive(true);
        levelMakerCamera.gameObject.SetActive(true);
        grid.SetActive(true);
        canvas.gameObject.SetActive(true);
    }

    void DisableLevelMaker()
    {
        levelMakerSettings.gameObject.SetActive(false);
        levelMakerCamera.gameObject.SetActive(false);
        grid.SetActive(false);
        canvas.gameObject.SetActive(false);
    }

    public void SetActiveSubLevel(int index)
    {
        currentSubLevel = index;

        for (int i = 0; i < lmContainers.subLevelContainers.Length; i++)
        {
            if (i != index)
                lmContainers.subLevelContainers[i].SetActive(false);
            else
                lmContainers.subLevelContainers[i].SetActive(true);
        }
    }

    public void ChangeSubLevel() // Llamado desde el botón...
    {
        lmContainers.subLevelContainers[currentSubLevel].SetActive(false);
        if (currentSubLevel < subLevelsAmount - 1)
            currentSubLevel++;
        else
            currentSubLevel = 0;
        lmContainers.subLevelContainers[currentSubLevel].SetActive(true);
        currentSubLevelText.text = currentSubLevel.ToString();

        levelMakerSettings.DeselectPlacedPrefab();
        if (levelMakerSettings.lmDragAndDropOnGrid.currentDraggedPrefab != null)
            levelMakerSettings.lmDragAndDropOnGrid.currentDraggedPrefab.transform.SetParent(null);
    }
}