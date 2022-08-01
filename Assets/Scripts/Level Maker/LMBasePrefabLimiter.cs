using UnityEngine;
using System.Collections.Generic;

public class LMBasePrefabLimiter : MonoBehaviour
{
    private Dictionary<int, List<BasePrefab>> _dictionaryBySortingLayerOrder;

    void Awake()
    {
        _dictionaryBySortingLayerOrder = new Dictionary<int, List<BasePrefab>>();
    }

    public void CheckLimitedBasePrefab(BasePrefab basePrefab, bool movingPrefab = false)
    {
        if (!_dictionaryBySortingLayerOrder.ContainsKey(basePrefab.sortingLayerOrder))
            _dictionaryBySortingLayerOrder.Add(basePrefab.sortingLayerOrder, new List<BasePrefab>());
        if (!movingPrefab)
            _dictionaryBySortingLayerOrder[basePrefab.sortingLayerOrder].Add(basePrefab);

        if (_dictionaryBySortingLayerOrder[basePrefab.sortingLayerOrder].Count > basePrefab.spawnAmountLimit)
        {
            var earliestSpawnedBasePrefab = _dictionaryBySortingLayerOrder[basePrefab.sortingLayerOrder][0];
            _dictionaryBySortingLayerOrder[basePrefab.sortingLayerOrder].RemoveAt(0);
            for (int i = 0; i < earliestSpawnedBasePrefab.gridCells.Count; i++)
                earliestSpawnedBasePrefab.gridCells[i].RemoveBasePrefabFromGridCell(earliestSpawnedBasePrefab.spawnedSubLevel);
            if (earliestSpawnedBasePrefab != null)
                Destroy(earliestSpawnedBasePrefab.gameObject);
        }
    }

    public void RemoveFromDictionary(BasePrefab basePrefab)
    {
        _dictionaryBySortingLayerOrder[basePrefab.sortingLayerOrder].Remove(basePrefab);
    }
}