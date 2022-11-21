using System.Collections;
using System.Collections.Generic;
using Attributes;
using UnityEngine;
using ScriptableObjects;
using System;

public class Astronaut : MonoBehaviour
{
    [SerializeField]
    [ReadonlyInspector]
    private int _id;

    [SerializeField]
    [ReadonlyInspector]
    private int _parentId;

    private void Awake()
    {
        SetId();
        AddAstronautToList();
    }

    private void AddAstronautToList()
    {
        Astronauts.Add(this);
    }

    private void SetId()
    {
        if (_id == 0)
            _id = IdManager.NextId;
    }

    public int Id => _id;
    public int ParentId => _parentId;
    public static List<Astronaut> Astronauts = new();

    public void SetParentId(int id)
    {
        _parentId = id;
    }

    public void SetId(int id)
    {
        _id = id;
    }
}