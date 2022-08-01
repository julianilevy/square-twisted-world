using UnityEngine;

public class Key : FullEntityMotor, ISavable, ILoadable<KeyData>
{
    [HideInInspector]
    public SpriteRenderer keyEmission;

    private KeyData _savedData;

    public override void Update()
    {
        base.Update();
        UpdateColor();
    }

    void UpdateColor()
    {
        if (currentGravity == Gravity.UP)
            keyEmission.material.color = Gravity.COLOR_UP;
        else if (currentGravity == Gravity.RIGHT)
            keyEmission.material.color = Gravity.COLOR_RIGHT;
        else if (currentGravity == Gravity.LEFT)
            keyEmission.material.color = Gravity.COLOR_LEFT;
        else if (currentGravity == Gravity.DOWN)
            keyEmission.material.color = Gravity.COLOR_DOWN;
    }

    public void SaveData()
    {
        if (!beingDragged)
        {
            _savedData = new KeyData();
            SaveTransformData(ref _savedData);
            SaveGravityEntityData(ref _savedData);
        }
    }

    public void LoadData(KeyData keyData)
    {
        _savedData = keyData;

        if (_savedData != null)
        {
            if (!beingDragged)
            {
                LoadTransformData(ref _savedData);
                LoadGravityEntityData(ref _savedData);
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