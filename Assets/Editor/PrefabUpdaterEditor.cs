using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[ExecuteInEditMode]
public class PrefabUpdaterEditor : Editor
{
    private static List<BasePrefab> _allPrefabs;

    [MenuItem("Custom/Prefab Updater")]
    static void UpdatePrefabs()
    {
        GetAllPrefabs();
        UpdateSortingLayers();
        UpdateSortingLayerOffsetIndex();
    }

    static void GetAllPrefabs()
    {
        _allPrefabs = new List<BasePrefab>();
        var folderToSearch = new string[1] { "Assets/Prefabs" };
        var allPaths = AssetDatabase.FindAssets("", folderToSearch);

        for (int i = 0; i < allPaths.Length; i++)
        {
            allPaths[i] = AssetDatabase.GUIDToAssetPath(allPaths[i]);
            _allPrefabs.Add((BasePrefab)AssetDatabase.LoadAssetAtPath(allPaths[i], typeof(BasePrefab)));
        }
    }

    static void UpdateSortingLayers()
    {
        for (int i = 0; i < _allPrefabs.Count; i++)
        {
            if (_allPrefabs[i] != null)
                _allPrefabs[i].SetSortingLayer();
        }
    }

    static void UpdateSortingLayerOffsetIndex()
    {
        for (int i = 0; i < _allPrefabs.Count; i++)
        {
            if (_allPrefabs[i] != null)
                _allPrefabs[i].SetSortingLayerIndexOffset(_allPrefabs[i].transform);
        }
    }
}