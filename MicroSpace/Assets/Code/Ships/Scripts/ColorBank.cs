using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Ships
{
    public static class ColorBank
    {
        public static Color TemporalDesignationNormal => GetColor("#99CC99");
        public static Color TemporalDesignationObstructed => GetColor("#CF7E7E");
        public static Color WallDesignationNormal => GetColor("#CCCCCC");
        public static Color WallDesignationObstructed => GetColor("#E05959");
        public static Color FloorDesignationNormal => GetColor("#666666");
        public static Color FloorDesignationObstructed => GetColor("#7D2424");

        private static Color GetColor(string hex)
        {
            ColorUtility.TryParseHtmlString(hex, out Color color);
            return color;
        }
    }
}