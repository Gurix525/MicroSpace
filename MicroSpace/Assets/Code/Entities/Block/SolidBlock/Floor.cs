using System;
using UnityEngine;
using Attributes;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Entities
{
    public class Floor : SolidBlock
    {
        public SolidBlock[] NeighbouringBlocks = new SolidBlock[4];
    }
}