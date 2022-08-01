using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LMModifiableList : MonoBehaviour
{
    [HideInInspector]
    public LMBasePrefabModifable currentSelectedPrefab;
    [HideInInspector]
    public GameObject modifableList;
    [HideInInspector]
    public GameObject noneBase;
    [HideInInspector]
    public Text boolBase;
    [HideInInspector]
    public Text floatBase;
    [HideInInspector]
    public Text enumBase;

    private LMSettings _lmSettings;
    private List<Tuple<Toggle, Ref<bool>, bool, List<Tuple<GameObject, Ref<float>>>, List<Tuple<GameObject, Ref<bool>>>>> _boolList;
    private List<Tuple<InputField, Ref<float>, List<Tuple<GameObject, Ref<float>>>, List<Tuple<GameObject, Ref<bool>>>>> _floatList;
    private List<Tuple<Dropdown, Tuple<Ref<string>, int>, List<Tuple<GameObject, Ref<float>>>, List<Tuple<GameObject, Ref<bool>>>>> _enumList;
    private bool _ableToChangeGridCellFill;

    void Awake()
    {
        _lmSettings = GetComponent<LMSettings>();
        _boolList = new List<Tuple<Toggle, Ref<bool>, bool, List<Tuple<GameObject, Ref<float>>>, List<Tuple<GameObject, Ref<bool>>>>>();
        _floatList = new List<Tuple<InputField, Ref<float>, List<Tuple<GameObject, Ref<float>>>, List<Tuple<GameObject, Ref<bool>>>>>();
        _enumList = new List<Tuple<Dropdown, Tuple<Ref<string>, int>, List<Tuple<GameObject, Ref<float>>>, List<Tuple<GameObject, Ref<bool>>>>>();
    }

    public void CreateModifableList()
    {
        if (currentSelectedPrefab != null)
        {
            currentSelectedPrefab.DeleteModifiableList();
            DeleteAllModifableList();
            currentSelectedPrefab.CreateModifiableList();
            CreateEmptySpace();
            CreateModifableBoolLists();
            CreateModifableFloatLists();
            CreateModifableEnumLists();
            CreateEmptySpace();
        }
    }

    void Update()
    {
        if (currentSelectedPrefab != null)
        {
            CheckModifableBoolListsChanges();
            CheckModifableFloatListsChanges();
            CheckModifableEnumListsChanges();
        }
    }

    void CreateModifableBoolLists()
    {
        for (int i = 0; i < currentSelectedPrefab.boolList.Count; i++)
        {
            var newBoolElement = NewBaseElement(boolBase, currentSelectedPrefab.boolList[i].first);
            var toggle = newBoolElement.GetComponentInChildren<Toggle>();
            toggle.isOn = currentSelectedPrefab.boolList[i].second.value;

            var dependantFloatList = new List<Tuple<GameObject, Ref<float>>>();
            if (currentSelectedPrefab.boolList[i].fourth.Count > 0)
            {
                for (int j = 0; j < currentSelectedPrefab.boolList[i].fourth.Count; j++)
                    CreateDependantFloatList(dependantFloatList, currentSelectedPrefab.boolList[i].fourth[j].first, currentSelectedPrefab.boolList[i].fourth[j].second);
            }

            var dependantBoolList = new List<Tuple<GameObject, Ref<bool>>>();
            if (currentSelectedPrefab.boolList[i].fifth.Count > 0)
            {
                for (int j = 0; j < currentSelectedPrefab.boolList[i].fifth.Count; j++)
                    CreateDependantBoolList(dependantBoolList, currentSelectedPrefab.boolList[i].fifth[j].first, currentSelectedPrefab.boolList[i].fifth[j].second);
            }

            _boolList.Add(Tuple.Create(toggle, currentSelectedPrefab.boolList[i].second, currentSelectedPrefab.boolList[i].third, dependantFloatList, dependantBoolList));
        }
    }

    void CreateModifableFloatLists()
    {
        for (int i = 0; i < currentSelectedPrefab.floatList.Count; i++)
        {
            var newFloatElement = NewBaseElement(floatBase, currentSelectedPrefab.floatList[i].first);
            var inputField = newFloatElement.GetComponentInChildren<InputField>();
            inputField.text = currentSelectedPrefab.floatList[i].second.value.ToString();

            var dependantFloatList = new List<Tuple<GameObject, Ref<float>>>();
            if (currentSelectedPrefab.floatList[i].third.Count > 0)
            {
                for (int j = 0; j < currentSelectedPrefab.floatList[i].third.Count; j++)
                    CreateDependantFloatList(dependantFloatList, currentSelectedPrefab.floatList[i].third[j].first, currentSelectedPrefab.floatList[i].third[j].second);
            }

            var dependantBoolList = new List<Tuple<GameObject, Ref<bool>>>();
            if (currentSelectedPrefab.floatList[i].fourth.Count > 0)
            {
                for (int j = 0; j < currentSelectedPrefab.floatList[i].fourth.Count; j++)
                    CreateDependantBoolList(dependantBoolList, currentSelectedPrefab.floatList[i].fourth[j].first, currentSelectedPrefab.floatList[i].fourth[j].second);
            }

            _floatList.Add(Tuple.Create(inputField, currentSelectedPrefab.floatList[i].second, dependantFloatList, dependantBoolList));
        }
    }

    void CreateModifableEnumLists()
    {
        for (int i = 0; i < currentSelectedPrefab.enumList.Count; i++)
        {
            var newEnumElement = NewBaseElement(enumBase, currentSelectedPrefab.enumList[i].first);
            var dropdown = newEnumElement.GetComponentInChildren<Dropdown>();
            for (int j = 0; j < currentSelectedPrefab.enumList[i].third.Count; j++)
            {
                var newDropDownOptionData = new Dropdown.OptionData(currentSelectedPrefab.enumList[i].third[j].ToString());
                dropdown.options.Add(newDropDownOptionData);
            }
            dropdown.value = currentSelectedPrefab.enumList[i].second.second;
            dropdown.transform.GetChild(0).GetComponent<Text>().text = dropdown.options[currentSelectedPrefab.enumList[i].second.second].text;

            var dependantFloatList = new List<Tuple<GameObject, Ref<float>>>();
            if (currentSelectedPrefab.enumList[i].fourth.Count > 0)
            {
                for (int j = 0; j < currentSelectedPrefab.enumList[i].fourth.Count; j++)
                    CreateDependantFloatList(dependantFloatList, currentSelectedPrefab.enumList[i].fourth[j].first, currentSelectedPrefab.enumList[i].fourth[j].second);
            }

            var dependantBoolList = new List<Tuple<GameObject, Ref<bool>>>();
            if (currentSelectedPrefab.enumList[i].fifth.Count > 0)
            {
                for (int j = 0; j < currentSelectedPrefab.enumList[i].fifth.Count; j++)
                    CreateDependantBoolList(dependantBoolList, currentSelectedPrefab.enumList[i].fifth[j].first, currentSelectedPrefab.enumList[i].fifth[j].second);
            }

            _enumList.Add(Tuple.Create(dropdown, currentSelectedPrefab.enumList[i].second, dependantFloatList, dependantBoolList));
        }
    }

    void CheckModifableBoolListsChanges()
    {
        var changeBoolValueAtEndOfIteration = false;

        if (_boolList.Count > 0)
        {
            for (int i = 0; i < _boolList.Count; i++)
            {
                if (_boolList[i].third)
                {
                    if (_ableToChangeGridCellFill)
                    {
                        _ableToChangeGridCellFill = false;
                        ChangeCollidingGridCellsFill("Fill");
                    }
                    if (_boolList[i].first.isOn != _boolList[i].second.value)
                    {
                        if (IsAbleToToggle(i))
                        {
                            changeBoolValueAtEndOfIteration = true;
                            ChangeCollidingGridCellsFill("Clear");
                            _boolList[i].second.value = _boolList[i].first.isOn;
                        }
                        else
                            _boolList[i].first.isOn = _boolList[i].second.value;
                    }
                }
                else
                    _boolList[i].second.value = _boolList[i].first.isOn;

                if (_boolList[i].fourth.Count > 0)
                {
                    for (int j = 0; j < _boolList[i].fourth.Count; j++)
                    {
                        if (!_boolList[i].second.value)
                            _boolList[i].fourth[j].first.SetActive(false);
                        else
                            _boolList[i].fourth[j].first.SetActive(true);

                        CheckFloatDependantList(_boolList[i].fourth[j].first, _boolList[i].fourth[j].second);
                    }
                }

                if (_boolList[i].fifth.Count > 0)
                {
                    for (int j = 0; j < _boolList[i].fifth.Count; j++)
                    {
                        if (!_boolList[i].second.value)
                            _boolList[i].fifth[j].first.SetActive(false);
                        else
                            _boolList[i].fifth[j].first.SetActive(true);

                        CheckBoolDependantList(_boolList[i].fifth[j].first, _boolList[i].fifth[j].second);
                    }
                }
            }
            if (changeBoolValueAtEndOfIteration)
                _ableToChangeGridCellFill = true;
        }
    }

    void CheckModifableFloatListsChanges()
    {
        if (_floatList.Count > 0)
        {
            for (int i = 0; i < _floatList.Count; i++)
            {
                float parseResult01;
                if (float.TryParse(_floatList[i].first.text, out parseResult01))
                {
                    if (parseResult01 >= 999f)
                        _floatList[i].first.text = "999";
                }
                float parseResult02;
                if (float.TryParse(_floatList[i].first.text, out parseResult02))
                    _floatList[i].second.value = parseResult02;

                if (_floatList[i].third.Count > 0)
                {
                    for (int j = 0; j < _floatList[i].third.Count; j++)
                        CheckFloatDependantList(_floatList[i].third[j].first, _floatList[i].third[j].second);
                }

                if (_floatList[i].fourth.Count > 0)
                {
                    for (int j = 0; j < _floatList[i].fourth.Count; j++)
                        CheckBoolDependantList(_floatList[i].fourth[j].first, _floatList[i].fourth[j].second);
                }
            }
        }
    }

    void CheckModifableEnumListsChanges()
    {
        if (_enumList.Count > 0)
        {
            for (int i = 0; i < _enumList.Count; i++)
            {
                _enumList[i].second.first.value = _enumList[i].first.transform.GetChild(0).GetComponent<Text>().text;

                if (_enumList[i].third.Count > 0)
                {
                    for (int j = 0; j < _enumList[i].third.Count; j++)
                        CheckFloatDependantList(_enumList[i].third[j].first, _enumList[i].third[j].second);
                }

                if (_enumList[i].fourth.Count > 0)
                {
                    for (int j = 0; j < _enumList[i].fourth.Count; j++)
                        CheckBoolDependantList(_enumList[i].fourth[j].first, _enumList[i].fourth[j].second);
                }
            }
        }
    }

    public void DeleteAllModifableList()
    {
        _boolList.Clear();
        _floatList.Clear();
        _enumList.Clear();

        for (int i = 4; i < modifableList.transform.childCount; i++)
            Destroy(modifableList.transform.GetChild(i).gameObject);
    }

    void CreateEmptySpace()
    {
        var newEmptySpace = Instantiate(noneBase.gameObject);
        newEmptySpace.SetActive(true);
        newEmptySpace.transform.SetParent(modifableList.transform, false);
        newEmptySpace.name = "Empty Space";
    }

    GameObject NewBaseElement(Text text, string name)
    {
        var newBaseElement = Instantiate(text.gameObject);
        newBaseElement.SetActive(true);
        newBaseElement.transform.SetParent(modifableList.transform, false);
        newBaseElement.name = name;
        newBaseElement.GetComponent<Text>().text = name;
        return newBaseElement;
    }

    void CreateDependantFloatList(List<Tuple<GameObject, Ref<float>>> dependantFloatList, string name, Ref<float> refValue)
    {
        var newDependantFloat = Instantiate(floatBase.gameObject);
        newDependantFloat.SetActive(true);
        newDependantFloat.transform.SetParent(modifableList.transform, false);
        newDependantFloat.name = name;
        newDependantFloat.GetComponent<Text>().fontSize = newDependantFloat.GetComponent<Text>().fontSize - 5;
        newDependantFloat.GetComponent<Text>().text = " -  " + name;

        var inputFieldDependant = newDependantFloat.GetComponentInChildren<InputField>();
        inputFieldDependant.text = refValue.value.ToString();
        dependantFloatList.Add(Tuple.Create(newDependantFloat, refValue));
    }

    void CreateDependantBoolList(List<Tuple<GameObject, Ref<bool>>> dependantBoolList, string name, Ref<bool> refValue)
    {
        var newDependantBool = Instantiate(boolBase.gameObject);
        newDependantBool.SetActive(true);
        newDependantBool.transform.SetParent(modifableList.transform, false);
        newDependantBool.name = name;
        newDependantBool.GetComponent<Text>().fontSize = newDependantBool.GetComponent<Text>().fontSize - 5;
        newDependantBool.GetComponent<Text>().text = " -  " + name;

        var toggleDependant = newDependantBool.GetComponentInChildren<Toggle>();
        toggleDependant.isOn = refValue.value;
        dependantBoolList.Add(Tuple.Create(newDependantBool, refValue));
    }

    void CheckFloatDependantList(GameObject dependantContainer, Ref<float> refValue)
    {
        var inputField = dependantContainer.GetComponentInChildren<InputField>();

        float parseResult01;
        if (float.TryParse(inputField.text, out parseResult01))
        {
            if (parseResult01 >= 999f)
                inputField.text = "999";
        }
        float parseResult02;
        if (float.TryParse(inputField.text, out parseResult02))
            refValue.value = parseResult02;
    }

    void CheckBoolDependantList(GameObject dependantContainer, Ref<bool> refValue)
    {
        var toggle = dependantContainer.GetComponentInChildren<Toggle>();
        refValue.value = toggle.isOn;
    }

    bool IsAbleToToggle(int index)
    {
        var basePrefabToCheck = currentSelectedPrefab.GetComponent<BasePrefab>().EqualBoolToBasePrefab(_boolList[index].second);

        if (basePrefabToCheck != null)
        {
            if (!basePrefabToCheck.gameObject.activeSelf)
            {
                var gridCellHits = new List<GridCell>();

                basePrefabToCheck.gameObject.SetActive(true);

                foreach (var collider in basePrefabToCheck.GetColliderHits(_lmSettings.gridCollisionMask))
                    gridCellHits.Add(collider.GetComponent<GridCell>());

                basePrefabToCheck.gameObject.SetActive(false);

                foreach (var gridCellHit in gridCellHits)
                {
                    if (!basePrefabToCheck.canTileOverlap)
                    {
                        if (!gridCellHit.tilesFilled[LevelMakerManager.instance.currentSubLevel] && !gridCellHit.prefabsFilled[LevelMakerManager.instance.currentSubLevel])
                            return true;
                    }
                    else
                    {
                        if (!gridCellHit.prefabsFilled[LevelMakerManager.instance.currentSubLevel])
                            return true;
                    }
                }
            }
            else
                return true;
        }
        else
            return true;

        return false;
    }

    void ChangeCollidingGridCellsFill(string action)
    {
        var gridCellHits = new List<GridCell>();
        var basePrefabChildren = currentSelectedPrefab.GetComponentsInChildren<BasePrefab>();
        var canTileOverlap = false;

        foreach (var prefabChild in basePrefabChildren)
        {
            if (prefabChild.canTileOverlap)
                canTileOverlap = true;

            foreach (var collider in prefabChild.GetColliderHits(_lmSettings.gridCollisionMask))
                gridCellHits.Add(collider.GetComponent<GridCell>());
        }
        if (gridCellHits.Count > 0)
        {
            if (action == "Clear")
            {
                foreach (var currentGridCell in gridCellHits)
                {
                    if (!canTileOverlap)
                    {
                        currentGridCell.tilesFilled[LevelMakerManager.instance.currentSubLevel] = false;
                        currentGridCell.prefabsFilled[LevelMakerManager.instance.currentSubLevel] = false;
                    }
                    else
                        currentGridCell.prefabsFilled[LevelMakerManager.instance.currentSubLevel] = false;
                }
            }
            else if (action == "Fill")
            {
                foreach (var currentGridCell in gridCellHits)
                {
                    if (!canTileOverlap)
                    {
                        currentGridCell.tilesFilled[LevelMakerManager.instance.currentSubLevel] = true;
                        currentGridCell.prefabsFilled[LevelMakerManager.instance.currentSubLevel] = true;
                    }
                    else
                        currentGridCell.prefabsFilled[LevelMakerManager.instance.currentSubLevel] = true;
                }
            }
        }
    }
}