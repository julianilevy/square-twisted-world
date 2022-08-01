using UnityEngine;
using System.Collections.Generic;

public class LockedDoor : BasePrefab, ISavable, ILoadable<LockedDoorData>
{
    public GameObject spritesContainer;
    public List<LockedDoorCollider> colliders;
    public Animator animator;

    private LockedDoorData _savedData;

    public override void Awake()
    {
        base.Awake();

        canBeRotated = true;
        snapToTile = true;
        snapValueX = 1.6f;
        snapValueY = 1.6f;
    }

    public void OnKeyFitted(LockedDoorCollider.Side side)
    {
        foreach (var collider in colliders)
            collider.unlocking = true;

        animator.gameObject.SetActive(true);
        spritesContainer.gameObject.SetActive(false);

        switch (side)
        {
            case LockedDoorCollider.Side.Up:
                animator.Play("OpeningFromUp");
                break;
            case LockedDoorCollider.Side.Right:
                animator.Play("OpeningFromRight");
                break;
            case LockedDoorCollider.Side.Left:
                animator.Play("OpeningFromLeft");
                break;
            case LockedDoorCollider.Side.Down:
                animator.Play("OpeningFromDown");
                break;
        }
    }

    public void DestroyColliders()
    {
        foreach (var collider in colliders)
            Destroy(collider.gameObject);

        Destroy(GetComponent<Collider2D>());
    }

    public void SaveData()
    {
        if (!beingDragged)
        {
            _savedData = new LockedDoorData();
            SaveTransformData(ref _savedData);
        }
    }

    public void LoadData(LockedDoorData lockedDoorData)
    {
        _savedData = lockedDoorData;

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