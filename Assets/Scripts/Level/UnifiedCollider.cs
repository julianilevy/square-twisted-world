using UnityEngine;

public class UnifiedCollider : BasePrefab, ISavable, ILoadable<UnifiedColliderData>
{
    private UnifiedColliderData _savedData;

    public override void Awake()
    {
        base.Awake();
    }

    public void SaveData()
    {
        if (!beingDragged)
        {
            _savedData = new UnifiedColliderData();
            SaveTransformData(ref _savedData);
            _savedData.SetOffsetValues(GetComponent<BoxCollider2D>().offset.x, GetComponent<BoxCollider2D>().offset.y);
            _savedData.SetSizeValues(GetComponent<BoxCollider2D>().size.x, GetComponent<BoxCollider2D>().size.y);
            _savedData.SetTagAndLayer(gameObject.tag, gameObject.layer);
        }
    }

    public void LoadData(UnifiedColliderData unifiedColliderData)
    {
        _savedData = unifiedColliderData;

        if (_savedData != null)
        {
            if (!beingDragged)
            {
                LoadTransformData(ref _savedData);
                GetComponent<BoxCollider2D>().offset = new Vector2((float)_savedData.offset[0], (float)_savedData.offset[1]);
                GetComponent<BoxCollider2D>().size = new Vector2((float)_savedData.size[0], (float)_savedData.size[1]);
                gameObject.tag = _savedData.tag;
                gameObject.layer = _savedData.layer;
            }
        }
    }

    public TransformData GetSavedData()
    {
        if (_savedData != null)
            return _savedData;

        return null;
    }
}