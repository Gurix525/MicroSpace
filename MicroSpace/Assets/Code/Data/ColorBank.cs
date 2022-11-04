using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Data
{
    public static class ColorBank
    {
        #region General

        public static Color Invisible => GetColor("#0000");

        #endregion General

        #region Designations

        public static Color TemporalDesignationNormal => GetColor("#99CC9999");
        public static Color TemporalDesignationObstructed => GetColor("#CF7E7E99");
        public static Color WallDesignationNormal => GetColor("#CCCCCC88");
        public static Color WallDesignationObstructed => GetColor("#E0595988");
        public static Color FloorDesignationNormal => GetColor("#66666688");
        public static Color FloorDesignationObstructed => GetColor("#7D242488");

        public static Color CancelDesignationActive => GetColor("#C11B1299");
        public static Color CancelDesignationInactive => GetColor("#FF9C9799");

        public static Color MiningDesignationActive => GetColor("#C11B1299");
        public static Color MiningDesignationInactive => GetColor("#FFCCC966");

        #endregion Designations

        private static Color GetColor(string hex)
        {
            ColorUtility.TryParseHtmlString(hex, out Color color);
            return color;
        }
    }
}