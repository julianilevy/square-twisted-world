using UnityEngine;
using System.Collections;
using DarkTonic.MasterAudio;

public class LevelEnd : BasePrefab, ISavable, ILoadable<LevelEndData>
{
    private LevelEndData _savedData;

    public override void Awake()
    {
        base.Awake();

        spawnAmountLimit = 1;
        canBeRotated = true;
        snapToTile = true;
        snapValueY = 1.1f;
    }

    void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (GameManager.instance.IsPlayingLevel())
        {
            if (collider2D.gameObject.layer == K.LAYER_PLAYER)
            {
                var player = collider2D.GetComponent<Player>();
                player.sprite.PlayAnimatorState("WinLevel");
                player.locked = true;
                MasterAudio.PlaySound3DFollowTransformAndForget("EndLevel", collider2D.gameObject.transform);
                StartCoroutine(GoToNextLevel());
            }
        }
    }

    IEnumerator GoToNextLevel()
    {
        yield return new WaitForSeconds(1.2f);

        CanvasUIManager.instance.EnableFade(CanvasUIManager.instance.diagonalLinesFade);

        yield return new WaitForSeconds(GameManager.fadeTime);

        ProgressManager.instance.SaveProgress();
        StartCoroutine(GameManager.instance.PlayNextLevel(1f));
    }

    public void SaveData()
    {
        if (!beingDragged)
        {
            _savedData = new LevelEndData();
            SaveTransformData(ref _savedData);
        }
    }

    public void LoadData(LevelEndData levelEndData)
    {
        _savedData = levelEndData;

        if (_savedData != null)
        {
            if (!beingDragged)
                LoadTransformData(ref _savedData);
        }
    }

    public TransformData GetSavedData()
    {
        if (_savedData != null)
            return _savedData;

        return null;
    }
}
