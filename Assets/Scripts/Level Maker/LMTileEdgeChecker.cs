using UnityEngine;

public class LMTileEdgeChecker : MonoBehaviour
{
    private LMSettings _lmSettings;

    void Awake()
    {
        _lmSettings = GetComponent<LMSettings>();
    }

    public void CheckAllTiles() // Llamado desde Saver...
    {
        for (int i = 0; i < LevelMakerManager.instance.subLevelsAmount; i++)
        {
            LevelMakerManager.instance.SetActiveSubLevel(i);

            TileChecker(i);
            PassableTileChecker(i);
        }

        _lmSettings.lmSaver.ContinuePreSavingProcess();
    }

    void TileChecker(int index)
    {
        var tileList = LevelMakerManager.instance.lmContainers.containers[index][typeof(Tile)];

        for (int i = 0; i < tileList.transform.childCount; i++)
            tileList.transform.GetChild(i).GetComponent<Tile>().AddEmissiveEdges();
    }

    void PassableTileChecker(int index)
    {
        var passableTileList = LevelMakerManager.instance.lmContainers.containers[index][typeof(PassableTile)];

        for (int i = 0; i < passableTileList.transform.childCount; i++)
            passableTileList.transform.GetChild(i).GetComponent<PassableTile>().AddEmissiveEdges();
    }
}