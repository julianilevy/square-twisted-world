using UnityEngine;
using System.Collections.Generic;

public class Checkpoint : BasePrefab, ISavable, ILoadable<CheckpointData>
{
    [HideInInspector]
    public SpriteRenderer checkpointEmission;
    [HideInInspector]
    public Collider2D checkpointCollider;

    public List<Animator> animators;
    [HideInInspector]
    public Material materialEmission;
    [HideInInspector]
    public Color materialColor;

    private string _currentGravity = "Down";
    private bool _currentlyEnabled = false;
    private bool _disabled = false;
    private CheckpointData _savedData;

    public override void Awake()
    {
        base.Awake();
        snapToTile = true;
        canBeRotated = true;
        if (GameManager.instance.IsPlayingLevel())
            checkpointCollider.enabled = true;
    }

    void OnTriggerStay2D(Collider2D collider2D)
    {
        if (GameManager.instance.IsPlayingLevel())
        {
            if (collider2D.gameObject.layer == K.LAYER_PLAYER)
                GameManager.instance.UpdateCheckpoint(this);
        }
    }

    public bool Enable()
    {
        if (!_disabled && !_currentlyEnabled)
        {
            foreach (var animator in animators)
                animator.Play("Enabling");
            _currentlyEnabled = true;

            return true;
        }

        return false;
    }

    public void Disable()
    {
        if (!_disabled && _currentlyEnabled)
        {
            foreach (var animator in animators)
                animator.Play("Disabling");
            _disabled = true;
            _currentlyEnabled = false;
        }
    }

    public void Rotate()
    {
        checkpointEmission.material = new Material(materialEmission);
        if (transform.rotation.eulerAngles == Rotation.EULER_UP)
            ChangeByGravity(Gravity.Forces.up, Gravity.COLOR_UP);
        else if (transform.rotation.eulerAngles == Rotation.EULER_RIGHT)
            ChangeByGravity(Gravity.Forces.right, Gravity.COLOR_RIGHT);
        else if (transform.rotation.eulerAngles == Rotation.EULER_LEFT)
            ChangeByGravity(Gravity.Forces.left, Gravity.COLOR_LEFT);
        else if (transform.rotation.eulerAngles == Rotation.EULER_DOWN)
            ChangeByGravity(Gravity.Forces.down, Gravity.COLOR_DOWN);
    }

    private void ChangeByGravity(string gravity, Color gravityColor)
    {
        _currentGravity = gravity;
        checkpointEmission.material.color = gravityColor;
        materialColor = gravityColor;
    }

    public void SaveData()
    {
        if (!beingDragged)
        {
            _savedData = new CheckpointData();
            SaveTransformData(ref _savedData);
        }
    }

    public void LoadData(CheckpointData checkpointData)
    {
        _savedData = checkpointData;

        if (_savedData != null)
        {
            if (!beingDragged)
            {
                LoadTransformData(ref _savedData);
                Rotate();
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