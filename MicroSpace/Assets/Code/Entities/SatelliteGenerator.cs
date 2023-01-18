using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Entities
{
    public class SatelliteGenerator : MonoBehaviour
    {
        #region Fields

        [SerializeField]
        private Gradient _gradient;

        [SerializeField]
        private float _perlinScale = 100F;

        [SerializeField]
        private float _perlinModifier = 2F;

        [SerializeField, Range(0F, 1F)]
        private float _treshold = 0.5F;

        private Satellite _satellite;

        #endregion Fields

        #region Public

        public void Generate()
        {
            int size = 1000;
            float random = Random.value * 10000;
            Dictionary<(int, int), bool> cells = new();
            Dictionary<(int, int), bool> openCells = new();
            openCells.Add((0, 0), false);

            while (openCells.Count > 0 && cells.Count <= size)
            {
                openCells.ContainsKey
            }

            //float[,] values = new float[100, 100];
            //for (int i = 0; i < values.GetLength(0); i++)
            //{
            //    for (int j = 0; j < values.GetLength(1); j++)
            //    {
            //        values[i, j] =
            //            (Mathf.PerlinNoise((i + random) / _perlinScale, (j + random) / _perlinScale)
            //            + Mathf.PerlinNoise((i + random) / _perlinScale / _perlinModifier, (j + random) / _perlinScale / _perlinModifier)) / 2F;
            //        _satellite.WallsTilemap.SetColor(
            //            new(i, j, 0),
            //            values[i, j] >= _treshold ? new(1, 1, 1, 1) : new(1, 1, 1, 0));
            //    }
            //}

            //_satellite.WallsTilemap.SetTile(new(i, j, 0), BlockModel.GetModel(0).Tile);
            //_satellite.WallsTilemap.SetTileFlags(new(i, j, 0), TileFlags.None);
        }

        #endregion Public

        #region Unity

        private void Awake()
        {
            _satellite = GetComponent<Satellite>();
        }

        #endregion Unity

        #region Private

        public float CalculateValue((int x, int y) cell, float random)
        {
            return (Mathf.PerlinNoise(
                (cell.x + random) / _perlinScale,
                (cell.y + random) / _perlinScale)
                + Mathf.PerlinNoise(
                    (cell.x + random) / _perlinScale / _perlinModifier,
                    (cell.y + random) / _perlinScale / _perlinModifier))
                / 2F;
        }

        //private class Cell
        //{
        //    private Vector2Int _position;

        //    public int X => _position.x;
        //    public int Y => _position.y;
        //    public bool IsSet { get; set; } = false;

        //    public Cell(int x, int y)
        //    {
        //        _position = new Vector2Int(x, y);
        //    }
        //}

        #endregion Private
    }
}