using DelaunatorSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.ExtensionMethods
{
    public static class Vector2Extensions
    {
        public static Point ToPoint(this Vector2 v)
        {
            return new Point(v.x, v.y);
        }
    }
}