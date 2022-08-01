using DarkTonic.MasterAudio;
using UnityEngine;

public class Collectable : BasePrefab, ISavable, ILoadable<CollectableData>
{
    [HideInInspector]
    public SpriteRenderer[] sprites;
    [HideInInspector]
    public ParticleSystem pickUpParticles;

    private CollectableData _savedData;
    private bool _pickedUp;

    public override void Awake()
    {
        base.Awake();
        canBeRotated = true;
    }

    void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (GameManager.instance.IsPlayingLevel())
        {
            if (collider2D.gameObject.layer == K.LAYER_PLAYER)
            {
                if (!_pickedUp)
                {
                    ProgressManager.instance.AddCollectable();
                    for (int i = 0; i < sprites.Length; i++)
                        sprites[i].gameObject.SetActive(false);
                    pickUpParticles.Play();
                    _pickedUp = true;
                    MasterAudio.PlaySound3DFollowTransformAndForget("CollectableGrab", collider2D.gameObject.transform);
                    Destroy(gameObject, 3f);
                }
            }
        }
    }

    public void SaveData()
    {
        if (!beingDragged)
        {
            _savedData = new CollectableData();
            SaveTransformData(ref _savedData);
        }
    }

    public void LoadData(CollectableData collectableData)
    {
        _savedData = collectableData;

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
