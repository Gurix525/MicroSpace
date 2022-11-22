using System;
using UnityEngine;
using Attributes;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Maths;
using System.Linq;

namespace Entities
{
    public class Floor : SolidBlock
    {
        [SerializeField]
        [ReadonlyInspector]
        private SolidBlock[] _neighbouringBlocks = new SolidBlock[8];

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
    }
}