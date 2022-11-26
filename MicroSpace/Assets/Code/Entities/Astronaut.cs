using System.Collections;
using System.Collections.Generic;
using Attributes;
using UnityEngine;
using ScriptableObjects;
using System;
using UnityEngine.Events;

namespace Entities
{
    public class Astronaut : MonoBehaviour
    {
        #region Fields

        [SerializeField]
        [ReadonlyInspector]
        private int _id;

        private int _parentId;

        #endregion Fields

        #region Properties

        public int Id => _id;
        public static List<Astronaut> Astronauts { get; } = new();
        public UnityEvent GettingParentId = new();

        #endregion Properties

        #region Public

        public int GetParentId()
        {
            GettingParentId.Invoke();
            return _parentId;
        }

        public void SetParentId(int id)
        {
            _parentId = id;
        }

        public void SetId(int id)
        {
            _id = id;
        }

        #endregion Public

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
    }
}