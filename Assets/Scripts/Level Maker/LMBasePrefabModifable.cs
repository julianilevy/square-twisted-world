using UnityEngine;
using System.Collections.Generic;

public abstract class LMBasePrefabModifable : MonoBehaviour
{
    public List<Tuple<string, Ref<bool>, bool, List<Tuple<string, Ref<float>>>, List<Tuple<string, Ref<bool>>>>> boolList;
    public List<Tuple<string, Ref<float>, List<Tuple<string, Ref<float>>>, List<Tuple<string, Ref<bool>>>>> floatList;
    public List<Tuple<string, Tuple<Ref<string>, int>, List<string>, List<Tuple<string, Ref<float>>>, List<Tuple<string, Ref<bool>>>>> enumList;

    protected virtual void Awake()
    {
        boolList = new List<Tuple<string, Ref<bool>, bool, List<Tuple<string, Ref<float>>>, List<Tuple<string, Ref<bool>>>>>();
        floatList = new List<Tuple<string, Ref<float>, List<Tuple<string, Ref<float>>>, List<Tuple<string, Ref<bool>>>>>();
        enumList = new List<Tuple<string, Tuple<Ref<string>, int>, List<string>, List<Tuple<string, Ref<float>>>, List<Tuple<string, Ref<bool>>>>>();
    }

    public void CreateModifiableList()
    {
        AddBoolLists();
        AddFloatLists();
        AddEnumLists();
    }

    public void DeleteModifiableList()
    {
        boolList.Clear();
        floatList.Clear();
        enumList.Clear();
    }

    public virtual void AddBoolLists()
    {
    }
    public virtual void AddFloatLists()
    {
    }
    public virtual void AddEnumLists()
    {
    }

    protected void AddToBoolList(string publicName, Ref<bool> variableName, bool canBeDisabled)
    {
        var emptyFloatDependantList = new List<Tuple<string, Ref<float>>>();
        var emptyBoolDependantList = new List<Tuple<string, Ref<bool>>>();
        boolList.Add(Tuple.Create(publicName, variableName, canBeDisabled, emptyFloatDependantList, emptyBoolDependantList));
    }

    protected void AddToBoolList(string publicName, Ref<bool> variableName, bool canBeDisabled, List<Tuple<string, Ref<float>>> floatDependantList)
    {
        var emptyBoolDependantList = new List<Tuple<string, Ref<bool>>>();
        boolList.Add(Tuple.Create(publicName, variableName, canBeDisabled, floatDependantList, emptyBoolDependantList));
    }

    protected void AddToBoolList(string publicName, Ref<bool> variableName, bool canBeDisabled, List<Tuple<string, Ref<bool>>> boolDependantList)
    {
        var emptyFloatDependantList = new List<Tuple<string, Ref<float>>>();
        boolList.Add(Tuple.Create(publicName, variableName, canBeDisabled, emptyFloatDependantList, boolDependantList));
    }

    protected void AddToBoolList(string publicName, Ref<bool> variableName, bool canBeDisabled, List<Tuple<string, Ref<float>>> floatDependantList, List<Tuple<string, Ref<bool>>> boolDependantList)
    {
        boolList.Add(Tuple.Create(publicName, variableName, canBeDisabled, floatDependantList, boolDependantList));
    }

    protected void AddToBoolList(string publicName, Ref<bool> variableName, bool canBeDisabled, List<Tuple<string, Ref<bool>>> boolDependantList, List<Tuple<string, Ref<float>>> floatDependantList)
    {
        boolList.Add(Tuple.Create(publicName, variableName, canBeDisabled, floatDependantList, boolDependantList));
    }

    protected void AddToFloatList(string publicName, Ref<float> variableName)
    {
        var emptyFloatDependantList = new List<Tuple<string, Ref<float>>>();
        var emptyBoolDependantList = new List<Tuple<string, Ref<bool>>>();
        floatList.Add(Tuple.Create(publicName, variableName, emptyFloatDependantList, emptyBoolDependantList));
    }

    protected void AddToFloatList(string publicName, Ref<float> variableName, List<Tuple<string, Ref<float>>> floatDependantList)
    {
        var emptyBoolDependantList = new List<Tuple<string, Ref<bool>>>();
        floatList.Add(Tuple.Create(publicName, variableName, floatDependantList, emptyBoolDependantList));
    }

    protected void AddToFloatList(string publicName, Ref<float> variableName, List<Tuple<string, Ref<bool>>> boolDependantList)
    {
        var emptyFloatDependantList = new List<Tuple<string, Ref<float>>>();
        floatList.Add(Tuple.Create(publicName, variableName, emptyFloatDependantList, boolDependantList));
    }

    protected void AddToFloatList(string publicName, Ref<float> variableName, List<Tuple<string, Ref<float>>> floatDependantList, List<Tuple<string, Ref<bool>>> boolDependantList)
    {
        floatList.Add(Tuple.Create(publicName, variableName, floatDependantList, boolDependantList));
    }

    protected void AddToFloatList(string publicName, Ref<float> variableName, List<Tuple<string, Ref<bool>>> boolDependantList, List<Tuple<string, Ref<float>>> floatDependantList)
    {
        floatList.Add(Tuple.Create(publicName, variableName, floatDependantList, boolDependantList));
    }

    protected void AddToEnumList(string publicName, Tuple<Ref<string>, int> variableNameAndIndex, List<string> dropdownOptions)
    {
        var emptyFloatDependantList = new List<Tuple<string, Ref<float>>>();
        var emptyBoolDependantList = new List<Tuple<string, Ref<bool>>>();
        enumList.Add(Tuple.Create(publicName, variableNameAndIndex, dropdownOptions, emptyFloatDependantList, emptyBoolDependantList));
    }

    protected void AddToEnumList(string publicName, Tuple<Ref<string>, int> variableNameAndIndex, List<string> dropdownOptions, List<Tuple<string, Ref<float>>> floatDependantList)
    {
        var emptyBoolDependantList = new List<Tuple<string, Ref<bool>>>();
        enumList.Add(Tuple.Create(publicName, variableNameAndIndex, dropdownOptions, floatDependantList, emptyBoolDependantList));
    }

    protected void AddToEnumList(string publicName, Tuple<Ref<string>, int> variableNameAndIndex, List<string> dropdownOptions, List<Tuple<string, Ref<bool>>> boolDependantList)
    {
        var emptyFloatDependantList = new List<Tuple<string, Ref<float>>>();
        enumList.Add(Tuple.Create(publicName, variableNameAndIndex, dropdownOptions, emptyFloatDependantList, boolDependantList));
    }

    protected void AddToEnumList(string publicName, Tuple<Ref<string>, int> variableNameAndIndex, List<string> dropdownOptions, List<Tuple<string, Ref<float>>> floatDependantList, List<Tuple<string, Ref<bool>>> boolDependantList)
    {
        enumList.Add(Tuple.Create(publicName, variableNameAndIndex, dropdownOptions, floatDependantList, boolDependantList));
    }

    protected void AddToEnumList(string publicName, Tuple<Ref<string>, int> variableNameAndIndex, List<string> dropdownOptions, List<Tuple<string, Ref<bool>>> boolDependantList, List<Tuple<string, Ref<float>>> floatDependantList)
    {
        enumList.Add(Tuple.Create(publicName, variableNameAndIndex, dropdownOptions, floatDependantList, boolDependantList));
    }
}