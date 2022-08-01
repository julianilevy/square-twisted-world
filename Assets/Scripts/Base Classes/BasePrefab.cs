using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BasePrefab : TransformVector2
{
    [HideInInspector]
    public Vector3 originalPosition;
    [HideInInspector]
    public Quaternion originalRotation;
    [HideInInspector]
    public List<GridCell> gridCells;
    [HideInInspector]
    public bool needsToBeCreated = true;
    [HideInInspector]
    public bool canBePlacedAfterPickUp;
    [HideInInspector]
    public bool canContinuousPlacing;
    [HideInInspector]
    public bool beingDragged;
    [HideInInspector]
    public bool canBeRotated;
    [HideInInspector]
    public bool snapToTile;
    [HideInInspector]
    public float snapValueX;
    [HideInInspector]
    public float snapValueY;
    [HideInInspector]
    public bool canTileOverlap;
    [HideInInspector]
    public bool hasWaypoints;
    [HideInInspector]
    public List<LMWaypoint> usableWaypointList;
    [HideInInspector]
    public List<Waypoint> visualWaypointList;
    [HideInInspector]
    public int spawnAmountLimit;
    [HideInInspector]
    public int spawnedSubLevel;
    [HideInInspector]
    public int sortingLayerOrder;
    [HideInInspector]
    public int sortingLayerIndexOffset;

    private int _sortingLayerMaxIndex;

    public override void Awake()
    {
        base.Awake();
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    protected void SetTileOverlap()
    {
        var prefabChildren = GetComponentsInChildren<BasePrefab>();
        foreach (var prefabChild in prefabChildren)
            prefabChild.canTileOverlap = true;
    }

    public Collider2D[] GetColliderHits(LayerMask collisionMask)
    {
        if (GetComponent<BoxCollider2D>())
        {
            var bounds = GetComponent<BoxCollider2D>().bounds;
            var topLeft = new Vector2(bounds.min.x, bounds.max.y);
            var bottomRight = new Vector2(bounds.max.x, bounds.min.y);
            var hits = Physics2D.OverlapAreaAll(topLeft, bottomRight, collisionMask);

            return hits;
        }
        else if (GetComponent<CircleCollider2D>())
        {
            var radius = GetComponent<CircleCollider2D>().radius;
            var hits = Physics2D.OverlapCircleAll(transform.position, radius, collisionMask);

            return hits;
        }

        return null;
    }

    public void StartMovingPrefab()
    {
        var mandatoryTime = 0.1f;
        canBePlacedAfterPickUp = false;
        StartCoroutine(MovingPrefabMandatoryTime(mandatoryTime));
    }

    IEnumerator MovingPrefabMandatoryTime(float time)
    {
        yield return new WaitForSeconds(time);
        canBePlacedAfterPickUp = true;
    }

    public Vector3 GetSnapValueByRotation(Quaternion rotation)
    {
        if (rotation.eulerAngles == Rotation.EULER_UP)
            return new Vector3(snapValueX, snapValueY, 0f);
        else if (rotation.eulerAngles == Rotation.EULER_RIGHT)
            return new Vector3(snapValueY, snapValueX, 0f);
        else if (rotation.eulerAngles == Rotation.EULER_LEFT)
            return new Vector3(-snapValueY, -snapValueX, 0f);
        else if (rotation.eulerAngles == Rotation.EULER_DOWN)
            return new Vector3(-snapValueX, -snapValueY, 0f);

        return Vector3.zero;
    }

    public virtual BasePrefab EqualBoolToBasePrefab(Ref<bool> boolToEqual)
    {
        return null;
    }

    public void SetSortingLayer()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<SpriteRenderer>())
            {
                var child = transform.GetChild(i);
                sortingLayerOrder = SortingLayer.GetLayerValueFromID((child.GetComponent<SpriteRenderer>().sortingLayerID));

                if (child.GetComponent<BasePrefab>())
                {
                    var childBasePrefab = child.GetComponent<BasePrefab>();
                    childBasePrefab.sortingLayerOrder = SortingLayer.GetLayerValueFromID((child.GetComponent<SpriteRenderer>().sortingLayerID));

                    childBasePrefab.SetSortingLayer();
                }
            }
        }
    }

    public void SetSortingLayerIndexOffset(Transform transformData)
    {
        var maxIndex = 0;

        for (int i = 0; i < transformData.childCount; i++)
        {
            if (transformData.GetComponent<SpriteRenderer>())
            {
                var spriteRenderer = transformData.GetComponent<SpriteRenderer>();

                if (spriteRenderer.sortingOrder > maxIndex)
                    maxIndex = spriteRenderer.sortingOrder;
            }
            if (transformData.GetChild(i).GetComponent<SpriteRenderer>())
            {
                var childSpriteRenderer = transformData.GetChild(i).GetComponent<SpriteRenderer>();

                if (childSpriteRenderer.sortingOrder > maxIndex)
                    maxIndex = childSpriteRenderer.sortingOrder;
            }

            if (_sortingLayerMaxIndex < maxIndex)
                _sortingLayerMaxIndex = maxIndex;

            SetSortingLayerIndexOffset(transformData.GetChild(i));
        }

        sortingLayerIndexOffset = _sortingLayerMaxIndex + 1;
    }

    public void SetGridCells()
    {
        gridCells = LevelMakerManager.instance.levelMakerSettings.GetGridCellsHits(this);
    }

    public void SetGridCells(GridCell gridCell)
    {
        if (gridCells == null)
            gridCells = new List<GridCell>();
        gridCells.Add(gridCell);
    }
}