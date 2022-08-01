using UnityEngine;
using System.Collections;

public abstract class BaseTile : BasePrefab
{
    public bool destroyable;
    public bool alone = true;

    // HIDE IN INSPECTOR TODOS.
    public SpriteRenderer emissionUp;
    public SpriteRenderer emissionRight;
    public SpriteRenderer emissionLeft;
    public SpriteRenderer emissionDown;

    public abstract void AddEmissiveEdges(bool isRecursive = true);
    public abstract void RemoveEmissiveEdges(bool isRecursive = true);
    protected abstract void EnableAllEmissiveEdges();

    public override void Awake()
    {
        canContinuousPlacing = true;
    }

    public IEnumerator PickUpProcess()
    {
        AddEmissiveEdges();
        GetComponent<BoxCollider2D>().enabled = false;
        yield return new WaitForEndOfFrame();
        GetComponent<BoxCollider2D>().enabled = true;
        EnableAllEmissiveEdges();
    }

    protected void SaveBaseTileData<T>(ref T baseTileData) where T : BaseTileData
    {
        baseTileData.SetBaseTileData(destroyable, alone);
        baseTileData.SetEmissionData(emissionUp.gameObject.activeSelf, emissionRight.gameObject.activeSelf, emissionLeft.gameObject.activeSelf, emissionDown.gameObject.activeSelf);
    }

    protected void LoadBaseTileData<T>(ref T baseTileData) where T : BaseTileData
    {
        destroyable = baseTileData.destroyable;
        alone = baseTileData.alone;
        emissionUp.gameObject.SetActive(baseTileData.emissionUpEnabled);
        emissionRight.gameObject.SetActive(baseTileData.emissionRightEnabled);
        emissionLeft.gameObject.SetActive(baseTileData.emissionLeftEnabled);
        emissionDown.gameObject.SetActive(baseTileData.emissionDownEnabled);
        if (GameManager.instance.IsPlayingLevel())
        {
            if (!alone && !destroyable)
                Destroy(GetComponent<BoxCollider2D>());
        }
    }
}