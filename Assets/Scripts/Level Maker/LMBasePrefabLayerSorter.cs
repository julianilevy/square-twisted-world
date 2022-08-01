using UnityEngine;
using System.Collections.Generic;

public class LMBasePrefabLayerSorter : MonoBehaviour
{
    private Dictionary<int, int> _prefabDictionary;
    private int _currentSortingLayer;
    private int _maxSortingOrder = 32700;

    void Awake()
    {
        _prefabDictionary = new Dictionary<int, int>();
    }

    public void SetSortingLayer(BasePrefab basePrefab)
    {
        if (basePrefab.GetComponent<BaseTile>())
            return;

        var firstOfTypeCreated = false;
        if (!_prefabDictionary.ContainsKey(basePrefab.sortingLayerOrder))
        {
            _prefabDictionary.Add(basePrefab.sortingLayerOrder, basePrefab.sortingLayerIndexOffset);
            firstOfTypeCreated = true;
        }
        _currentSortingLayer = basePrefab.sortingLayerOrder;

        foreach (Transform basePrefabChild in basePrefab.transform)
        {
            if (basePrefabChild.GetComponent<SpriteRenderer>())
            {
                var spriteRenderer = basePrefabChild.GetComponent<SpriteRenderer>();
                if (firstOfTypeCreated)
                    spriteRenderer.sortingOrder -= _maxSortingOrder;
                else
                    spriteRenderer.sortingOrder += _prefabDictionary[_currentSortingLayer] - _maxSortingOrder;
            }
            SetSortingLayerToAllChildren(basePrefabChild, firstOfTypeCreated);
        }

        if (!firstOfTypeCreated)
            _prefabDictionary[_currentSortingLayer] += basePrefab.sortingLayerIndexOffset;
    }

    void SetSortingLayerToAllChildren(Transform basePrefabChild, bool firstOfTypeCreated)
    {
        foreach (Transform child in basePrefabChild)
        {
            if (child.GetComponent<SpriteRenderer>())
            {
                var spriteRenderer = child.GetComponent<SpriteRenderer>();
                if (firstOfTypeCreated)
                    spriteRenderer.sortingOrder -= _maxSortingOrder;
                else
                    spriteRenderer.sortingOrder += _prefabDictionary[_currentSortingLayer] - _maxSortingOrder;
            }
            SetSortingLayerToAllChildren(child, firstOfTypeCreated);
        }
    }

    public void SetMaxSortingOrder(BasePrefab basePrefab)
    {
        if (basePrefab.GetComponent<BaseTile>())
            return;

        foreach (Transform basePrefabChild in basePrefab.transform)
        {
            if (basePrefabChild.GetComponent<SpriteRenderer>())
            {
                var spriteRenderer = basePrefabChild.GetComponent<SpriteRenderer>();
                spriteRenderer.sortingOrder += _maxSortingOrder;
            }
            SetMaxSortingOrderToAllChildren(basePrefabChild, _maxSortingOrder);
        }
    }

    void SetMaxSortingOrderToAllChildren(Transform basePrefabChild, int maxSortingOrder)
    {
        foreach (Transform child in basePrefabChild)
        {
            if (child.GetComponent<SpriteRenderer>())
            {
                var spriteRenderer = child.GetComponent<SpriteRenderer>();
                spriteRenderer.sortingOrder += maxSortingOrder;
            }
            SetMaxSortingOrderToAllChildren(child, maxSortingOrder);
        }
    }
}