using DelaunatorSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.ExtensionMethods
{
    public static class PointExtensions
    {
        public static Vector2 ToVector2(this Point point)
            => new((float)point.X, (float)point.Y);
    }
}