using System;
using UnityEngine;
using Attributes;
using Maths;
using System.Linq;
using Miscellaneous;
using ExtensionMethods;
using System.Collections.Generic;

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

        public SolidBlock[] NeighbouringBlocks { get; set; } = new SolidBlock[8];

        public Dictionary<int, int> Gasses { get; set; } = new();

        public static List<Floor> EnabledFloors { get; } = new();

        #endregion Properties

        #region Unity

        private void OnEnable()
        {
            AddSelfToList();
            GasExchangeTimer.GassesExchangeTicked.AddListener(ExchangeGasses);
            GasExchangeTimer.GassesExchangeFinished.AddListener(ClearEmptyGasses);
        }

        private void OnDisable()
        {
            RemoveSelfFromList();
            GasExchangeTimer.GassesExchangeTicked.RemoveListener(ExchangeGasses);
            GasExchangeTimer.GassesExchangeFinished.RemoveListener(ClearEmptyGasses);
        }

        #endregion Unity

        #region Private

        private Floor[] NeighbouringFloors => NeighbouringBlocks
            .Where(block => block is Floor)
            .Select(block => block as Floor)
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

        private void AddSelfToList()
        {
            if (!EnabledFloors.Contains(this))
                EnabledFloors.Add(this);
        }

        private void RemoveSelfFromList()
        {
            if (EnabledFloors.Contains(this))
                EnabledFloors.Remove(this);
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