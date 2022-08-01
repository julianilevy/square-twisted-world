public class LevelData
{
    public int levelID;
    public string levelName;
    public bool wasCompleted;
    public int collectablesObtained;
    public int collectablesTotal;
    public SubLevelData[] subLevelData;

    public LevelData()
    {
        subLevelData = new SubLevelData[0];
    }
}