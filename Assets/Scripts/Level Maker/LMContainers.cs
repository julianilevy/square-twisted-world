using UnityEngine;
using System;
using System.Collections.Generic;

public class LMContainers : MonoBehaviour
{
    [HideInInspector]
    public Dictionary<Type, GameObject>[] containers;
    [HideInInspector]
    public GameObject[] subLevelContainers;

    void Awake()
    {
        containers = new Dictionary<Type, GameObject>[LevelMakerManager.instance.subLevelsAmount];
        subLevelContainers = new GameObject[LevelMakerManager.instance.subLevelsAmount];

        for (int i = 0; i < containers.Length; i++)
        {
            containers[i] = new Dictionary<Type, GameObject>();

            CreateSubLevelContainer(i);
            CreateContainer(typeof(Player), i);
            CreateContainer(typeof(PassableTile), i);
            CreateContainer(typeof(UnifiedCollider), i);
            CreateContainer(typeof(MovingPlatform), i);
            CreateContainer(typeof(MovingPlatformVerticalSpikes), i);
            CreateContainer(typeof(MovingPlatformHorizontalSpikes), i);
            CreateContainer(typeof(Tile), i);
            CreateContainer(typeof(Saw), i);
            CreateContainer(typeof(RayShooter), i);
            CreateContainer(typeof(GravityChangerPlatform), i);
            CreateContainer(typeof(Key), i);
            CreateContainer(typeof(LockedDoor), i);
            CreateContainer(typeof(LevelEnd), i);
            CreateContainer(typeof(Spike), i);
            CreateContainer(typeof(Collectable), i);
            CreateContainer(typeof(Checkpoint), i);
        }
    }

    public void CreateSubLevelContainer(int index)
    {
        var container = new GameObject("Sub-Level " + index + " [Container]");
        container.transform.position = Vector3.zero;
        subLevelContainers[index] = container;
        if (index != LevelMakerManager.instance.currentSubLevel)
            subLevelContainers[index].SetActive(false);
    }

    public void CreateContainer(Type type, int index)
    {
        var container = new GameObject(type.ToString() + " [Container]");
        container.transform.position = Vector3.zero;
        container.transform.SetParent(subLevelContainers[index].transform);
        containers[index].Add(type, container);
    }

    public void AddToContainer(Type type, GameObject prefab)
    {
        prefab.transform.SetParent(containers[LevelMakerManager.instance.currentSubLevel][type].transform);
    }

    public void AddToContainer(Type type, GameObject prefab, int index)
    {
        prefab.transform.SetParent(containers[index][type].transform);
    }
}