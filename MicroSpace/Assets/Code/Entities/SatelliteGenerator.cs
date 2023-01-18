using System.Collections.Generic;
using System.Linq;
using Maths;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.Tilemaps;
using ExtensionMethods;

namespace Entities
{
    public class SatelliteGenerator : MonoBehaviour
    {
        #region Fields

        [SerializeField]
        private float _perlinScale = 100F;

        [SerializeField, Range(0F, 1F)]
        private float _treshold = 0.5F;

        private Satellite _satellite;

        #endregion Fields

        #region Public

        public void Generate()
        {
            System.Diagnostics.Stopwatch watch = new();
            watch.Start();

            _satellite.WallsTilemap.ClearAllTiles();

            int size = 500;
            (float a, float b)[] randoms = new (float, float)[5];
            float originalValue = 0F;
            for (int j = 0; j < 10000; j++)
            {
                for (int i = 0; i < randoms.Length; i++)
                {
                    randoms[i].a = Random.value * 1000000;
                    randoms[i].b = Random.value * 1000000;
                }
                originalValue = CalculateValue((0, 0), randoms);
                if (originalValue > _treshold)
                    break;
                if (j == 999)
                {
                    Debug.LogException(new System.InvalidOperationException("Brak wyniku"));
                    return;
                }
            }
            Range range = new(
                _treshold,
                1 - _treshold / 10F);
            Dictionary<(int x, int y), float> cells = new();
            Dictionary<(int x, int y), float> openCells = new();
            openCells.Add((0, 0), originalValue);

            while (openCells.Count > 0 && cells.Count < size)
            {
                var cell = openCells.Last();

                (int x, int y)[] sideCells =
                {
                    (x: cell.Key.x + 1, y: cell.Key.y),
                    (x: cell.Key.x - 1, y: cell.Key.y),
                    (x: cell.Key.x, y: cell.Key.y + 1),
                    (x: cell.Key.x, y: cell.Key.y - 1)
                };

                foreach (var sideCell in sideCells)
                {
                    if (!cells.ContainsKey(sideCell)
                        && !openCells.ContainsKey(sideCell))
                        if (IsValueInRange(sideCell, randoms, range, out float value))
                            openCells.Add(sideCell, value);
                }
                cells.Add(cell.Key, cell.Value);
                openCells.Remove(cell.Key);
            }

            while (openCells.Count > 0)
            {
                var cell = openCells.Last();

                (int x, int y)[] sideCells =
                {
                    (x: cell.Key.x + 1, y: cell.Key.y),
                    (x: cell.Key.x - 1, y: cell.Key.y),
                    (x: cell.Key.x, y: cell.Key.y + 1),
                    (x: cell.Key.x, y: cell.Key.y - 1)
                };

                foreach (var sideCell in sideCells)
                {
                    if (!cells.ContainsKey(sideCell)
                        && !openCells.ContainsKey(sideCell))
                        if (IsValueInRange(sideCell, randoms, range, out float value))
                            cells.Add(sideCell, value);
                }
                cells.Add(cell.Key, cell.Value);
                openCells.Remove(cell.Key);
            }

            foreach (var cell in cells)
            {
                _satellite.WallsTilemap.SetTile(
                    new(cell.Key.x, cell.Key.y, 0),
                    BlockModel.GetModel(0).Tile);
                _satellite.WallsTilemap.SetTileFlags(
                    new(cell.Key.x, cell.Key.y, 0),
                    TileFlags.None);
            }

            List<float> values = new();

            for (int i = 0; i <= 1000; i++)
            {
                for (int j = 0; j <= 1000; j++)
                {
                    values.Add(CalculateValue((i, j), randoms));
                }
            }

            float min = values.Min();
            float max = values.Max();

            watch.Stop();
            Debug.Log($"{min}:{max}");
        }

        #endregion Public

        #region Unity

        private void Awake()
        {
            _satellite = GetComponent<Satellite>();
        }

        #endregion Unity

        #region Private

        private bool IsValueInRange(
            (int x, int y) cell,
            (float, float)[] randoms,
            Range range,
            out float value)
        {
            value = CalculateValue(cell, randoms);
            return range.IsIncluding(value);
        }

        private float CalculateValue((int x, int y) cell, (float a, float b)[] randoms)
        {
            float result = 0F;
            for (int i = 0; i < randoms.Length; i++)
            {
                result += Mathf.PerlinNoise(
                (cell.x + randoms[i].a) / _perlinScale,
                (cell.y + randoms[i].b) / _perlinScale);
            }

            return result / randoms.Length;
        }

        #endregion Private
    }
}