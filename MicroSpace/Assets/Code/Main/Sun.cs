using System.Collections.Generic;
using System.Linq;
using Entities;
using Maths;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering.Universal;

namespace Main
{
    public class Sun : MonoBehaviour
    {
        #region Fields

        private int _timer;
        private List<Range> _ranges = new();

        #endregion Fields

        #region Unity

        private void FixedUpdate()
        {
            _ranges.Clear();
            Illuminate();
        }

        #endregion Unity

        #region Private

        private void Illuminate()
        {
            Profiler.BeginSample("Illuminate");
            Dictionary<Entity, int> entities = new();
            foreach (var wall in Wall.EnabledWalls)
                entities.Add(wall.Value, 0);
            foreach (var floor in Floor.EnabledFloors)
                entities.Add(floor.Value, 1);
            foreach (var rigidEntity in RigidEntity.EnabledRigidEntities)
                entities.Add(rigidEntity, 2);
            entities = entities
                .OrderBy(entity => entity.Key.transform.position.magnitude)
                .ToDictionary(pair => pair.Key, pair => pair.Value);
            foreach (var entity in entities)
            {
                SetSurfaceBasedOnType(entity);
            }
            IlluminateWalls();
            Profiler.EndSample();
            foreach (var range in _ranges)
            {
                Debug.DrawRay(Vector3.zero, Quaternion.Euler(0, 0, range.Start) * Vector3.up * 20000, Color.magenta);
                Debug.DrawRay(Vector3.zero, Quaternion.Euler(0, 0, range.End) * Vector3.up * 20000, Color.magenta);
            }
        }

        private void IlluminateWalls()
        {
            Profiler.BeginSample("IlluminateWalls");
            var surfaceWalls = Wall.EnabledWalls
                .Where(wall => wall.Value.IsSurface)
                .ToDictionary(wall => wall.Key, wall => wall.Value)
                .GroupBy(wall => wall.Value.Satellite)
                .ToArray();
            List<Dictionary<Vector2Int, Block>> stacks = new();
            foreach (var satelliteWalls in surfaceWalls)
            {
                var originalWalls = satelliteWalls.ToDictionary(x => x.Key, x => x.Value);
                while (originalWalls.Count > 0)
                {
                    Dictionary<Vector2Int, Block> closedWalls = new();
                    Dictionary<Vector2Int, Block> openedWalls = new();
                    openedWalls.Add(originalWalls.Last().Key, originalWalls.Last().Value);
                    originalWalls.Remove(originalWalls.Last().Key);
                    while (openedWalls.Count > 0)
                    {
                        var lastWall = openedWalls.Last();
                        KeyValuePair<Vector2Int, Block> currentWall =
                            new(lastWall.Key, lastWall.Value);
                        foreach (var wall in GetSideBlocks(ref currentWall))
                        {
                            if (originalWalls.ContainsKey(wall) && !closedWalls.ContainsKey(wall))
                            {
                                openedWalls.TryAdd(wall, originalWalls[wall]);
                                originalWalls.Remove(wall);
                            }
                        }
                        closedWalls.Add(currentWall.Key, currentWall.Value);
                        openedWalls.Remove(currentWall.Key);
                    }
                    stacks.Add(closedWalls);
                }
            }
            Profiler.EndSample();
        }

        private static Vector2Int[] GetSideBlocks(
            ref KeyValuePair<Vector2Int, Block> block)
        {
            return new Vector2Int[]{
                new(block.Key.x + 1, block.Key.y),
                new(block.Key.x - 1, block.Key.y),
                new(block.Key.x, block.Key.y + 1),
                new(block.Key.x, block.Key.y - 1)
            };
        }

        private void SetSurfaceBasedOnType(KeyValuePair<Entity, int> entity)
        {
            switch (entity.Value)
            {
                default:
                    SetWallSurface((Wall)entity.Key);
                    break;

                case 1:
                    SetFloorSurface((Floor)entity.Key);
                    break;

                case 2:
                    SetRigidEntitySurface((RigidEntity)entity.Key);
                    break;
            }
        }

        private void SetWallSurface(Wall wall)
        {
            Profiler.BeginSample("SetWallSurface");
            Range wallRange = wall.ShadowRange;
            foreach (var range in _ranges)
            {
                if (wallRange.IsCovered(range))
                {
                    wall.SetEnlighted(false);
                    Profiler.EndSample();
                    return;
                }
            }
            wall.SetEnlighted(true);
            _ranges.Add(wallRange);
            OrganiseRanges();
            Profiler.EndSample();
        }

        private void SetFloorSurface(Floor floor)
        {
            Profiler.BeginSample("SetFloorSurface");
            Range floorRange = floor.ShadowRange;
            foreach (var range in _ranges)
            {
                if (floorRange.IsCovered(range))
                {
                    floor.SetEnlighted(false);
                    Profiler.EndSample();
                    return;
                }
            }
            floor.SetEnlighted(true);
            Profiler.EndSample();
        }

        private void SetRigidEntitySurface(RigidEntity rigidEntity)
        {
            Range rigidEntityRange = rigidEntity.ShadowRange;
            foreach (var range in _ranges)
            {
                if (rigidEntityRange.IsCovered(range))
                {
                    rigidEntity.SetLightActive(false);
                    return;
                }
            }
            rigidEntity.SetLightActive(true);
        }

        private void OrganiseRanges()
        {
            Profiler.BeginSample("OrganiseRanges");
            _ranges = _ranges.OrderBy(range => range.Start).ToList();
            for (int i = 0; i < _ranges.Count - 1; i++)
            {
                if (_ranges[i].IsOverlapping(_ranges[i + 1], out Range combinedRange))
                {
                    _ranges[i] = combinedRange;
                    _ranges.RemoveAt(i + 1);
                    i--;
                }
            }
            Profiler.EndSample();
        }

        #endregion Private
    }
}