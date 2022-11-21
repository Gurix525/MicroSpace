using System.Collections;
using System.Collections.Generic;
using Attributes;
using UnityEngine;
using ScriptableObjects;
using System;

public class Astronaut : MonoBehaviour
{
    #region Fields

    [SerializeField]
    [ReadonlyInspector]
    private int _id;

    [SerializeField]
    [ReadonlyInspector]
    private int _parentId;

    #endregion Fields

    #region Properties

    public int Id => _id;
    public int ParentId => _parentId;
    public static List<Astronaut> Astronauts = new();

    #endregion Properties

    #region Public

    public void SetParentId(int id)
    {
        _parentId = id;
    }

    public void SetId(int id)
    {
        _id = id;
    }

    #endregion Public

    #region Private

    private void AddAstronautToList()
    {
        Astronauts.Add(this);
    }

    private void RemoveAstronautFromList()
    {
        Astronauts.Remove(this);
    }

    private void SetId()
    {
        if (_id == 0)
            _id = IdManager.NextId;
    }

    #endregion Private

    #region Unity

    private void Awake()
    {
        SetId();
        AddAstronautToList();
    }

    private void OnDestroy()
    {
        RemoveAstronautFromList();
    }

    #endregion Unity
}