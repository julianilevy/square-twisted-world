using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridCell : MonoBehaviour
{
    public bool[] tilesFilled;
    public bool[] prefabsFilled;
    public BaseTile[] currentTiles;
    public BasePrefab[] currentPrefabs;
    public SpriteRenderer inside;

    private int _maxCoroutineIterations = 10;
    private int _tileFillmentIteration;
    private int _prefabFillmentIteration;

    public Collider2D GetGridCellHit(LayerMask collisionMask)
    {
        var bounds = GetComponent<BoxCollider2D>().bounds;
        var topLeft = new Vector2(bounds.min.x, bounds.max.y);
        var bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        var hit = Physics2D.OverlapArea(topLeft, bottomRight, collisionMask);

        return hit;
    }

    public Collider2D[] GetAllGridCellHits(LayerMask collisionMask)
    {
        var bounds = GetComponent<BoxCollider2D>().bounds;
        var topLeft = new Vector2(bounds.min.x, bounds.max.y);
        var bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        var hits = Physics2D.OverlapAreaAll(topLeft, bottomRight, collisionMask);

        return hits;
    }

    public void SetGridTileFillment(BaseTile tile, bool doExtraCheck = true)
    {
        if (tile != null)
        {
            if (!tilesFilled[LevelMakerManager.instance.currentSubLevel])
            {
                currentTiles[LevelMakerManager.instance.currentSubLevel] = tile;
                tilesFilled[LevelMakerManager.instance.currentSubLevel] = true;
                _tileFillmentIteration = 0;

                if (doExtraCheck)
                    StartCoroutine(RemoveExtraTiles());
            }
        }
    }

    public void SetGridPrefabFillment(BasePrefab prefab, bool doExtraCheck = true)
    {
        if (prefab != null)
        {
            currentPrefabs[LevelMakerManager.instance.currentSubLevel] = prefab;
            prefabsFilled[LevelMakerManager.instance.currentSubLevel] = true;
            if (!prefab.canTileOverlap)
                tilesFilled[LevelMakerManager.instance.currentSubLevel] = true;
            _prefabFillmentIteration = 0;

            if (doExtraCheck)
                StartCoroutine(RemoveExtraPrefabs());
        }
    }

    public void CheckGridTileFillment()
    {
        _tileFillmentIteration = 0;
        StartCoroutine(RemoveExtraTiles());
    }

    public void CheckGridPrefabFillment()
    {
        _prefabFillmentIteration = 0;
        StartCoroutine(RemoveExtraPrefabs());
    }

    IEnumerator RemoveExtraTiles()
    {
        yield return new WaitForEndOfFrame();

        if (tilesFilled[LevelMakerManager.instance.currentSubLevel])
        {
            var bounds = GetComponent<BoxCollider2D>().bounds;
            var topLeft = new Vector2(bounds.min.x, bounds.max.y);
            var bottomRight = new Vector2(bounds.max.x, bounds.min.y);

            var hits = Physics2D.OverlapAreaAll(topLeft, bottomRight);
            var tileHits = new List<Collider2D>();

            foreach (var hit in hits)
            {
                if (hit.GetComponent<BaseTile>())
                {
                    if (!hit.GetComponent<BaseTile>().beingDragged)
                        tileHits.Add(hit);
                }
            }

            if (tileHits.Count == 0)
            {
                currentTiles[LevelMakerManager.instance.currentSubLevel] = null;
                if (currentPrefabs[LevelMakerManager.instance.currentSubLevel] == null)
                    tilesFilled[LevelMakerManager.instance.currentSubLevel] = false;
                else
                {
                    if (currentPrefabs[LevelMakerManager.instance.currentSubLevel].canTileOverlap)
                        tilesFilled[LevelMakerManager.instance.currentSubLevel] = false;
                }
            }

            if (currentPrefabs[LevelMakerManager.instance.currentSubLevel] != null)
            {
                if (!currentPrefabs[LevelMakerManager.instance.currentSubLevel].canTileOverlap)
                {
                    foreach (var tile in tileHits)
                    {
                        tile.GetComponent<BaseTile>().AddEmissiveEdges();
                        Destroy(tile.gameObject);
                    }
                }
            }

            if (currentTiles[LevelMakerManager.instance.currentSubLevel] != null)
            {
                foreach (var tile in tileHits)
                {
                    if (tile.gameObject != currentTiles[LevelMakerManager.instance.currentSubLevel].gameObject)
                    {
                        tile.GetComponent<BaseTile>().AddEmissiveEdges();
                        Destroy(tile.gameObject);
                    }
                }
            }
        }

        if (!tilesFilled[LevelMakerManager.instance.currentSubLevel] || currentTiles[LevelMakerManager.instance.currentSubLevel] == null)
            yield break;

        _tileFillmentIteration++;
        if (_tileFillmentIteration >= _maxCoroutineIterations)
            yield break;

        StartCoroutine(RemoveExtraTiles());
    }

    IEnumerator RemoveExtraPrefabs()
    {
        yield return new WaitForEndOfFrame();

        if (prefabsFilled[LevelMakerManager.instance.currentSubLevel])
        {
            var bounds = GetComponent<BoxCollider2D>().bounds;
            var topLeft = new Vector2(bounds.min.x, bounds.max.y);
            var bottomRight = new Vector2(bounds.max.x, bounds.min.y);

            var hits = Physics2D.OverlapAreaAll(topLeft, bottomRight);
            var prefabHits = new List<Collider2D>();

            foreach (var hit in hits)
            {
                if (hit.GetComponent<BasePrefab>() && !hit.GetComponent<BaseTile>())
                {
                    if (!hit.GetComponent<BasePrefab>().beingDragged)
                        prefabHits.Add(hit);
                }
            }

            if (prefabHits.Count == 0)
            {
                currentPrefabs[LevelMakerManager.instance.currentSubLevel] = null;
                prefabsFilled[LevelMakerManager.instance.currentSubLevel] = false;
                if (currentTiles[LevelMakerManager.instance.currentSubLevel] == null)
                    tilesFilled[LevelMakerManager.instance.currentSubLevel] = false;
            }

            if (currentPrefabs[LevelMakerManager.instance.currentSubLevel] != null)
            {
                foreach (var prefab in prefabHits)
                {
                    if (prefab.gameObject != currentPrefabs[LevelMakerManager.instance.currentSubLevel].gameObject)
                    {
                        if (!prefab.transform.IsChildOf(currentPrefabs[LevelMakerManager.instance.currentSubLevel].transform))
                            Destroy(prefab.gameObject);
                    }
                }
            }
        }

        if (!prefabsFilled[LevelMakerManager.instance.currentSubLevel] || currentPrefabs[LevelMakerManager.instance.currentSubLevel] == null)
            yield break;

        _prefabFillmentIteration++;
        if (_prefabFillmentIteration >= _maxCoroutineIterations)
            yield break;

        StartCoroutine(RemoveExtraPrefabs());
    }

    public void RemoveBasePrefabFromGridCell(int specificSubLevel)
    {
        tilesFilled[specificSubLevel] = false;
        currentTiles[specificSubLevel] = null;
        prefabsFilled[specificSubLevel] = false;
        currentPrefabs[specificSubLevel] = null;
    }
}