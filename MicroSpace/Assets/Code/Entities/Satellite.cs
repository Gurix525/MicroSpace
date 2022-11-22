using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using Attributes;
using ScriptableObjects;
using System.Collections;

namespace Entities
{
    public class Satellite : MonoBehaviour
    {
        #region Fields

        [SerializeField]
        [ReadonlyInspector]
        private int _id;

        [SerializeField]
        [ReadonlyInspector]
        private List<Block> _blocks = new();

        [SerializeField]
        [ReadonlyInspector]
        private Vector2 _position;

        [SerializeField]
        [ReadonlyInspector]
        private float _rotation;

        [SerializeField]
        [ReadonlyInspector]
        private Vector2 _velocity;

        private bool _isSatelliteLoaded = true;

        private static readonly float _satelliteUnloadDistance = 200F;

        #endregion Fields

        #region Properties

        public List<Block> Blocks { get => _blocks; set => _blocks = value; }

        public List<Wall> Walls => _blocks
            .Where(block => block is Wall)
            .Select(block => block as Wall)
            .ToList();

        public List<Floor> Floors => _blocks
            .Where(block => block is Floor)
            .Select(block => block as Floor)
            .ToList();

        public int ElementsCount { get => Blocks.Count; }

        public int Id { get => _id; set => _id = value; }

        public Vector2 Position { get => _position; set => _position = value; }

        public float Rotation { get => _rotation; set => _rotation = value; }

        public Vector2 Velocity { get => _velocity; set => _velocity = value; }

        public Rigidbody2D Rigidbody2D { get; private set; }

        #endregion Properties

        #region Public

        public void StartUpdateSatellite()
        {
            StartCoroutine(UpdateSatelliteRepeatedly());
        }

        public void ActivateOrDeactivateChildren(Transform focusedSatellite)
        {
            if (IsSatelliteDistantFromGivenSatellite(focusedSatellite) && _isSatelliteLoaded)
                SetSatelliteChildrenActive(false);
            else if (!IsSatelliteDistantFromGivenSatellite(focusedSatellite) && !_isSatelliteLoaded)
                SetSatelliteChildrenActive(true);
        }

        #endregion Public

        #region Private

        private void UpdateSatellite()
        {
            UpdateBlocks();
            UpdateProperties();
            if (IsSatelliteEmpty())
                DestroySatellite();
        }

        private IEnumerator UpdateSatelliteRepeatedly()
        {
            UpdateSatellite();
            yield return null;
            UpdateSatellite();
        }

        private void DestroySatellite()
        {
            if (Camera.main.transform.parent == transform)
                Camera.main.transform.parent = null;
            Destroy(gameObject);
        }

        private bool IsSatelliteEmpty()
        {
            return Blocks.Count <= 0;
        }

        private void UpdateProperties()
        {
            Position = transform.position;
            Rotation = transform.eulerAngles.z;
            Velocity = GetComponent<Rigidbody2D>().velocity;
        }

        private void SetId()
        {
            if (Id == 0)
                Id = IdManager.NextId;
        }

        private void SetSatelliteChildrenActive(bool state)
        {
            foreach (Transform child in transform)
                child.gameObject.SetActive(state);
            _isSatelliteLoaded = state;
        }

        private bool IsSatelliteDistantFromGivenSatellite(Transform satellite)
        {
            return Vector2.Distance(transform.position, satellite.position) >
                _satelliteUnloadDistance;
        }

        private void UpdateBlocks()
        {
            Blocks.Clear();
            foreach (Transform child in transform)
            {
                var block = child.gameObject.GetComponent<Block>();
                if (block != null && !IsTemporalDesignation(block))
                {
                    block.UpdateBlock();
                    Blocks.Add(block);
                }
            }
        }

        private bool IsTemporalDesignation(Block block)
        {
            return block is TemporalDesignation;
        }

        //private void UpdateRooms()
        //{
        //    List<Block> blocks = new();
        //    foreach (var item in Blocks)
        //        blocks.Add(item);

        //    foreach (Block block in blocks)
        //    {
        //        if (block is not Floor)
        //            continue;

        //        var upBlock = blocks.Find(
        //            x => x.transform.localPosition.x == block.transform.localPosition.x &&
        //            x.transform.localPosition.y == block.transform.localPosition.y + 1);
        //        var downBlock = blocks.Find(
        //            x => x.transform.localPosition.x == block.transform.localPosition.x &&
        //            x.transform.localPosition.y == block.transform.localPosition.y - 1);
        //        var leftBlock = blocks.Find(
        //            x => x.transform.localPosition.x == block.transform.localPosition.x - 1 &&
        //            x.transform.localPosition.y == block.transform.localPosition.y);
        //        var rightBlock = blocks.Find(
        //            x => x.transform.localPosition.x == block.transform.localPosition.x + 1 &&
        //            x.transform.localPosition.y == block.transform.localPosition.y);

        //        if (upBlock == null || downBlock == null ||
        //            leftBlock == null || rightBlock == null)
        //            ((Floor)block).IsExposed = true;
        //        else
        //            ((Floor)block).IsExposed = false;

        //        if (upBlock is Floor)
        //            ((Floor)block).UpFloor = (Floor)upBlock;
        //        if (downBlock is Floor)
        //            ((Floor)block).DownFloor = (Floor)downBlock;
        //        if (leftBlock is Floor)
        //            ((Floor)block).LeftFloor = (Floor)leftBlock;
        //        if (rightBlock is Floor)
        //            ((Floor)block).RightFloor = (Floor)rightBlock;
        //    }

        //    var exposedFloors = blocks
        //        .Where(x => x is Floor)
        //        .Where(x => ((Floor)x).IsExposed);

        //    foreach (var floor in exposedFloors)
        //        exposeFloor((Floor)floor);

        //    foreach (var floor in Floors)
        //        setRoom(floor);

        //    void exposeFloor(Floor floor)
        //    {
        //        floor.IsExposed = true;

        //        if (floor.UpFloor != null)
        //            if (!floor.UpFloor.IsExposed)
        //                exposeFloor(floor.UpFloor);

        //        if (floor.DownFloor != null)
        //            if (!floor.DownFloor.IsExposed)
        //                exposeFloor(floor.DownFloor);

        //        if (floor.LeftFloor != null)
        //            if (!floor.LeftFloor.IsExposed)
        //                exposeFloor(floor.LeftFloor);

        //        if (floor.RightFloor != null)
        //            if (!floor.RightFloor.IsExposed)
        //                exposeFloor(floor.RightFloor);
        //    }

        //    void setRoom(Floor floor, Room room = null)
        //    {
        //        if (floor.Room.Id > 0)
        //            room = floor.Room;
        //        else
        //        {
        //            if (room == null)
        //            {
        //                Rooms.Add(new(Rooms.Count + 1));
        //                room = Rooms[^1];
        //            }
        //            floor.Room = room;
        //        }

        //        if (floor.UpFloor != null)
        //            if (floor.UpFloor.Room.Id == 0)
        //                setRoom(floor.UpFloor, room);
        //        if (floor.DownFloor != null)
        //            if (floor.DownFloor.Room.Id == 0)
        //                setRoom(floor.DownFloor, room);
        //        if (floor.LeftFloor != null)
        //            if (floor.LeftFloor.Room.Id == 0)
        //                setRoom(floor.LeftFloor, room);
        //        if (floor.RightFloor != null)
        //            if (floor.RightFloor.Room.Id == 0)
        //                setRoom(floor.RightFloor, room);
        //    } // Jeszcze sie za duzo roomow tworzy bo

        //    // walle sie kopiują z pustymi roomami na WallData
        //}

        private void GetRigidbody2D()
        {
            Rigidbody2D = GetComponent<Rigidbody2D>();
        }

        #endregion Private

        #region Unity

        private void Awake()
        {
            SetId();
            GetRigidbody2D();
        }

        #endregion Unity
    }
}