using System.Collections.Generic;
using System.Linq;
using Maths;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.Tilemaps;
using ExtensionMethods;
using Miscellaneous;

namespace Entities
{
    public class SatelliteGenerator : MonoBehaviour
    {
        #region Fields

        [SerializeField, Range(1F, 50F)]
        private float _additivePerlinScale = 25F;

        [SerializeField, Range(0F, 1F)]
        private float _treshold = 0.67F;

        [SerializeField]
        private int _maxOpenCells = 500;

        private Satellite _satellite;

        #endregion Fields

        #region Public

        public void Generate()
        {
            if (!TryGetRandomShifts(
                out float originalValue,
                out Vector2[] randomShifts))
                return;
            CreateWalls(originalValue, randomShifts);
            CreateFloors(originalValue, randomShifts);
        }

        #endregion Public

        #region Unity

        private void Awake()
        {
            _satellite = GetComponent<Satellite>();
        }

        #endregion Unity

        #region Private

        private void CreateWalls(float originalValue, Vector2[] randomShifts)
        {
            foreach (var cell in CreateCells(originalValue, randomShifts))
            {
                GameObject wall = Instantiate(
                    Prefabs.Wall,
                    _satellite.transform);
                wall.transform.SetLocalPositionAndRotation(
                    new(cell.Key.x, cell.Key.y),
                    Quaternion.identity);
            }
        }

        private void CreateFloors(float originalValue, Vector2[] randomShifts)
        {
            foreach (var cell in CreateCells(originalValue, randomShifts))
            {
                GameObject floor = Instantiate(
                    Prefabs.Floor,
                    _satellite.transform);
                floor.transform.SetLocalPositionAndRotation(
                    new(cell.Key.x, cell.Key.y),
                    Quaternion.identity);
            }
        }

        private Dictionary<Vector2Int, float> CreateCells(
            float originalValue,
            Vector2[] randomShifts)
        {
            Dictionary<Vector2Int, float> cells = new();
            Dictionary<Vector2Int, float> openCells = new();
            openCells.Add(new(0, 0), originalValue);
            while (openCells.Count > 0)
            {
                OpenNewCells(randomShifts, cells, openCells);
            }
            return cells;
        }

        private void OpenNewCells(
            Vector2[] randomShifts,
            Dictionary<Vector2Int, float> cells,
            Dictionary<Vector2Int, float> openCells)
        {
            var cell = openCells.Last();
            foreach (var sideCell in GetSideCells(ref cell))
            {
                OpenSideCell(randomShifts, cells, openCells, sideCell);
            }
            cells.Add(cell.Key, cell.Value);
            openCells.Remove(cell.Key);
        }

        private void OpenSideCell(
            Vector2[] randomShifts,
            Dictionary<Vector2Int, float> cells,
            Dictionary<Vector2Int, float> openCells,
            Vector2Int sideCell)
        {
            if (DoesCellExist(cells, openCells, ref sideCell))
            {
                if (IsCellValueInRange(
                    sideCell,
                    randomShifts,
                    GetRange(),
                    out float value))
                {
                    if (cells.Count < _maxOpenCells)
                        openCells.Add(sideCell, value);
                    else
                        cells.Add(sideCell, value);
                }
            }
        }

        private static bool DoesCellExist(Dictionary<Vector2Int, float> cells, Dictionary<Vector2Int, float> openCells, ref Vector2Int sideCell)
        {
            return !cells.ContainsKey(sideCell)
                            && !openCells.ContainsKey(sideCell);
        }

        private Range GetRange()
        {
            float modifier = Random.Range(-1F, 1F);
            return new(
                _treshold + modifier * (_treshold / 10F),
                1 - modifier * (_treshold / 10F));
        }

        private static Vector2Int[] GetSideCells(
            ref KeyValuePair<Vector2Int, float> cell)
        {
            return new Vector2Int[]{
                new(cell.Key.x + 1, cell.Key.y),
                new(cell.Key.x - 1, cell.Key.y),
                new(cell.Key.x, cell.Key.y + 1),
                new(cell.Key.x, cell.Key.y - 1)
            };
        }

        private bool TryGetRandomShifts(
            out float originalValue,
            out Vector2[] randomShifts)
        {
            originalValue = 0F;
            randomShifts = new Vector2[5];
            for (int j = 0; j < 10000; j++)
            {
                for (int i = 0; i < randomShifts.Length; i++)
                {
                    randomShifts[i].x = Random.value * 1000000;
                    randomShifts[i].y = Random.value * 1000000;
                }
                originalValue = CalculateValue(new(0, 0), randomShifts);
                if (originalValue > _treshold)
                    break;
                if (j == 999)
                {
                    return false;
                }
            }
            return true;
        }

        private bool IsCellValueInRange(
            Vector2Int cell,
            Vector2[] randomShifts,
            Range range,
            out float value)
        {
            value = CalculateValue(cell, randomShifts);
            return range.IsIncluding(value);
        }

        private float CalculateValue(
            Vector2Int cell,
            Vector2[] randomShifts)
        {
            float sum = 0F;
            for (int i = 0; i < randomShifts.Length; i++)
            {
                sum += Mathf.PerlinNoise(
                    (cell.x + randomShifts[i].x) / _additivePerlinScale,
                    (cell.y + randomShifts[i].y) / _additivePerlinScale);
            }

            sum /= randomShifts.Length;

            return sum.Map(0.16F, 0.77F, 0F, 1F);
        }

        #endregion Private
    }
}