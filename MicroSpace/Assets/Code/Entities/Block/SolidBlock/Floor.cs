using System;
using UnityEngine;
using Attributes;
using Maths;
using System.Linq;
using Miscellaneous;
using TMPro;
using ExtensionMethods;

namespace Entities
{
    public class Floor : SolidBlock
    {
        [SerializeField]
        [ReadonlyInspector]
        private SolidBlock[] _neighbouringBlocks = new SolidBlock[8];

        [SerializeField]
        private int _gas = 1000;

        private TextMeshPro _text;

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

        public Line[] CheckingLines => _relativeCheckingLines
            .Select(line => line.TransformLine(transform))
            .ToArray();

        // 2 na każdą stronę (góra, prawo, dół, lewo)
        public SolidBlock[] NeighbouringBlocks
        { get => _neighbouringBlocks; set => _neighbouringBlocks = value; }

        public int Gas { get => _gas; set => _gas = value; }

        private Floor[] NeighbouringFloors => NeighbouringBlocks
            .Where(block => block is Floor)
            .Select(block => block as Floor)
            .MakeRandomPermutation()
            .OrderBy(floor => floor.Gas)
            .ToArray();

        private int NeighbouringVoids => NeighbouringBlocks
            .Where(block => block == null)
            .Count();

        private TextMeshPro Text =>
            _text ??= GetComponentInChildren<TextMeshPro>();

        private void ExchangeGasses()
        {
            for (int i = 0; i < NeighbouringVoids; i++)
                _gas -= (int)Math.Ceiling(_gas / (double)8);
            foreach (Floor floor in NeighbouringFloors)
                if (_gas > floor.Gas)
                {
                    int difference = _gas - floor.Gas;
                    int flow = (int)Math.Ceiling(difference / (double)8);
                    _gas -= flow;
                    floor.Gas += flow;
                }
        }

        private void FixedUpdate()
        {
            Text.text = _gas.ToString();
        }

        private void OnEnable()
        {
            GasExchangeTimer.GasExchangeTicked.AddListener(ExchangeGasses);
        }

        private void OnDisable()
        {
            GasExchangeTimer.GasExchangeTicked.RemoveListener(ExchangeGasses);
        }
    }
}