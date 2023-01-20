using System;
using UnityEngine;
using Attributes;
using Maths;
using System.Linq;
using Miscellaneous;
using ExtensionMethods;
using System.Collections.Generic;
using ScriptableObjects;

namespace Entities
{
    public class Floor : SolidBlock, IGasContainer
    {
        #region Fields

        private static readonly Line[] _relativeCheckingLines =
        {
            new (new(-0.25F, 0.4F), new(-0.25F, 0.6F)),
            new (new(0.25F, 0.4F), new(0.25F, 0.6F)),
            new (new(0.4F, 0.25F), new(0.6F, 0.25F)),
            new (new(0.4F, -0.25F), new(0.6F, -0.25F)),
            new (new(0.25F, -0.4F), new(0.25F, -0.6F)),
            new (new(-0.25F, -0.4F), new(-0.25F, -0.6F)),
            new (new(-0.4F, -0.25F), new(-0.6F, -0.25F)),
            new (new(-0.4F, 0.25F), new(-0.6F, 0.25F))
        };

        #endregion Fields

        #region Properties

        public Line[] CheckingLines => _relativeCheckingLines
            .Select(line => line.TransformLine(transform))
            .ToArray();

        [field: SerializeField, ReadonlyInspector]
        public SolidBlock[] NeighbouringBlocks { get; set; } = new SolidBlock[8];

        public Dictionary<int, int> Gasses { get; set; } = new();

        public static Dictionary<Vector2Int, Floor> EnabledFloors { get; } = new();

        #endregion Properties

        #region Unity

        protected override void Start()
        {
            base.Start();
            AddSelfToSatelliteList();
            AddTile();
        }

        private void OnEnable()
        {
            AddSelfToEnabledList();
            GasExchangeTimer.GassesExchangeTicked.AddListener(ExchangeGasses);
            GasExchangeTimer.GassesExchangeFinished.AddListener(ClearEmptyGasses);
        }

        private void OnDisable()
        {
            RemoveSelfFromEnabledList();
            GasExchangeTimer.GassesExchangeTicked.RemoveListener(ExchangeGasses);
            GasExchangeTimer.GassesExchangeFinished.RemoveListener(ClearEmptyGasses);
        }

        private void OnDestroy()
        {
            RemoveSelfFromSatelliteList();
            RemoveTile();
        }

        #endregion Unity

        #region Private

        private Floor[] NeighbouringFloors => NeighbouringBlocks
            .Where(block => block != null)
            .Where(block => block.BlockType == BlockType.Floor)
            .Select(block => (Floor)block)
            .ToArray();

        private int NeighbouringVoids => NeighbouringBlocks
            .Where(block => block == null)
            .Count();

        private void ExchangeGas(int gasId)
        {
            for (int i = 0; i < NeighbouringVoids; i++)
                for (int j = 0; j < Gasses.Count; j++)
                    Gasses[j] -= (int)Math.Ceiling(Gasses[j] / (double)8);
            foreach (Floor floor in NeighbouringFloors)
            {
                if (!floor.Gasses.ContainsKey(gasId))
                    floor.Gasses.Add(gasId, 0);
            }
            var neighbouringFloors = NeighbouringFloors
                .MakeRandomPermutation();
            foreach (Floor floor in neighbouringFloors)
                if (Gasses[gasId] > floor.Gasses[gasId])
                {
                    int difference = Gasses[gasId] - floor.Gasses[gasId];
                    int flow = (int)Math.Ceiling(difference / (double)8);
                    Gasses[gasId] -= flow;
                    floor.Gasses[gasId] += flow;
                }
        }

        private void AddSelfToSatelliteList()
        {
            if (_satellite == null)
                return;
            _satellite.Blocks.Add(this);
            _satellite.SolidBlocks.Add(this);
            _satellite.Floors.Add(this);
        }

        private void AddTile()
        {
            _satellite.FloorsTilemap.SetTile(
                (Vector3Int)FixedLocalPosition,
                BlockModel.GetModel(ModelId).Tile);
            _satellite.FloorsTilemap.SetTileFlags(
                (Vector3Int)FixedLocalPosition,
                UnityEngine.Tilemaps.TileFlags.None);
        }

        private void RemoveSelfFromSatelliteList()
        {
            if (_satellite == null)
                return;
            _satellite.Blocks.Remove(this);
            _satellite.SolidBlocks.Remove(this);
            _satellite.Floors.Remove(this);
        }

        private void RemoveTile()
        {
            _satellite.FloorsTilemap.SetTile(
                (Vector3Int)FixedLocalPosition,
                null);
        }

        private void AddSelfToEnabledList()
        {
            if (!EnabledFloors.ContainsKey(FixedLocalPosition))
                EnabledFloors.Add(FixedLocalPosition, this);
        }

        private void RemoveSelfFromEnabledList()
        {
            if (EnabledFloors.ContainsKey(FixedLocalPosition))
                EnabledFloors.Remove(FixedLocalPosition);
        }

        #endregion Private

        #region Callbacks

        private void ExchangeGasses()
        {
            int[] gassesIds = Gasses.Keys.ToArray();
            foreach (int gasId in gassesIds)
            {
                ExchangeGas(gasId);
            }
        }

        private void ClearEmptyGasses()
        {
            int[] gassesIds = Gasses.Keys.ToArray();
            foreach (int gasId in gassesIds)
            {
                if (Gasses[gasId] == 0)
                    Gasses.Remove(gasId);
            }
        }

        #endregion Callbacks
    }
}