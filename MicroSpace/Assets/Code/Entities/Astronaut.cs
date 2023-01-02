using System.Collections.Generic;
using Attributes;
using UnityEngine;
using UnityEngine.Events;
using Miscellaneous;

namespace Entities
{
    public class Astronaut : Entity
    {
        #region Fields

        private int _parentId;

        #endregion Fields

        #region Properties

        public AstronautState AstronautState { get; set; }
        public UnityEvent GettingParentId { get; } = new();
        public static List<Astronaut> Astronauts { get; } = new();

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

        #endregion Public

        #region Unity

        private new void Awake()
        {
            base.Awake();
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

        #endregion Private
    }
}