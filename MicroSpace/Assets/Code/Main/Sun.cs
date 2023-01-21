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
            foreach (var entity in GetEntities())
                SetSurfaceBasedOnType(entity);
            IlluminateBlocks();
            Profiler.EndSample();
            //foreach (var range in _ranges)
            //{
            //    Debug.DrawRay(Vector3.zero, Quaternion.Euler(0, 0, range.Start) * Vector3.up * 20000, Color.magenta);
            //    Debug.DrawRay(Vector3.zero, Quaternion.Euler(0, 0, range.End) * Vector3.up * 20000, Color.magenta);
            //}
        }

        private static Dictionary<Entity, int> GetEntities()
        {
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
            return entities;
        }

        private void IlluminateBlocks()
        {
            Profiler.BeginSample("IlluminateWalls");
            Dictionary<Vector2Int, SolidBlock> surfaceBlocks = Wall.EnabledWalls
                .Where(wall => wall.Value.IsSurface)
                .ToDictionary(wall => wall.Key, wall => (SolidBlock)wall.Value);
            var surfaceFloors = Floor.EnabledFloors
                .Where(floor => floor.Value.IsSurface)
                .ToDictionary(floor => floor.Key, floor => floor.Value);
            foreach (var floor in surfaceFloors)
            {
                surfaceBlocks.TryAdd(floor.Key, floor.Value);
            }
            var blocksSatelliteGrouping = surfaceBlocks
                .GroupBy(block => block.Value.Satellite)
                .ToArray();
            List<Dictionary<Vector2Int, SolidBlock>> blockChunks = new();
            foreach (var satelliteBlocks in blocksSatelliteGrouping)
            {
                var originalBlocks = satelliteBlocks.ToDictionary(x => x.Key, x => x.Value);
                while (originalBlocks.Count > 0)
                {
                    Dictionary<Vector2Int, SolidBlock> closedBlocks = new();
                    Dictionary<Vector2Int, SolidBlock> openedBlocks = new();
                    openedBlocks.Add(originalBlocks.Last().Key, originalBlocks.Last().Value);
                    originalBlocks.Remove(originalBlocks.Last().Key);
                    while (openedBlocks.Count > 0)
                    {
                        var lastWall = openedBlocks.Last();
                        KeyValuePair<Vector2Int, SolidBlock> currentBlock =
                            new(lastWall.Key, lastWall.Value);
                        foreach (var wall in GetSideBlocks(currentBlock))
                        {
                            if (originalBlocks.ContainsKey(wall) && !closedBlocks.ContainsKey(wall))
                            {
                                openedBlocks.TryAdd(wall, originalBlocks[wall]);
                                originalBlocks.Remove(wall);
                            }
                        }
                        closedBlocks.Add(currentBlock.Key, currentBlock.Value);
                        openedBlocks.Remove(currentBlock.Key);
                    }
                    blockChunks.Add(closedBlocks);
                }
            }
            foreach (var chunk in blockChunks)
            {
                var blocksPolygonPath = GetBlocksPolygonPath(chunk, out Satellite satellite);
                Vector2[] polygonPath = GetPolygonPath(blocksPolygonPath, satellite.transform);
                for (int i = 0; i < polygonPath.Length - 1; i++)
                {
                    Debug.DrawLine(polygonPath[i], polygonPath[i + 1], Color.yellow);
                }
                Debug.DrawLine(polygonPath[^1], polygonPath[0], Color.yellow);
            }
            Profiler.EndSample();
        }

        private static Vector2[] GetPolygonPath(
            Vector2Int[] blocksPolygonPath,
            Transform satellite)
        {
            List<Vector2Int> path = new();
            int direction = 0;
            path.Add(blocksPolygonPath[0]);
            int index = 0;
            while (true)
            {
                if (path.Count > 1 && path[0] == path[^1])
                    break;
                var sideNodes = GetSideNodes(path[^1]);
                while (true)
                {
                    var sideNode = sideNodes[direction];
                    if (index < blocksPolygonPath.Length - 1)
                    {
                        if (GetBlockVertices(blocksPolygonPath[index + 1]).ContainsKey(sideNode))
                        {
                            path.Add(sideNode);
                            index++;
                            direction--;
                            direction = direction < 0 ? 3 : direction;
                            break;
                        }
                    }
                    if (GetBlockVertices(blocksPolygonPath[index]).ContainsKey(sideNode))
                    {
                        path.Add(sideNode);
                        direction--;
                        direction = direction < 0 ? 3 : direction;
                        break;
                    }
                    direction++;
                    direction = direction > 3 ? 0 : direction;
                }
            }
            path.RemoveAt(path.Count - 1);
            return path
                .Select(node => (Vector2)satellite.TransformPoint(
                    ((Vector2)node) - (Vector2.one / 2F)))
                .ToArray();
        }

        private static Vector2Int[] GetBlocksPolygonPath(
            Dictionary<Vector2Int, SolidBlock> chunk, out Satellite satellite)
        {
            Profiler.BeginSample("GetBlocksPolygonPath");
            var sortedBlocks = chunk
                .OrderBy(block => block.Key.y)
                .ThenBy(block => block.Key.x)
                .ToDictionary(block => block.Key, block => block.Value);
            List<Vector2Int> blockPath = new();
            blockPath.Add(sortedBlocks.First().Key);

            int direction = 0;
            while (true)
            {
                if (blockPath.Count > 1
                    && blockPath[0] == blockPath[^1]
                    && !sortedBlocks.Any(
                        sorted => !blockPath
                        .Distinct()
                        .ToDictionary(block => block, block => false).ContainsKey(sorted.Key)))
                    break;
                int attempts = 0;
                var sideBlocks = GetSideBlocks(blockPath[^1]);
                while (true)
                {
                    attempts++;
                    if (sortedBlocks.ContainsKey(sideBlocks[direction]))
                    {
                        blockPath.Add(sideBlocks[direction]);
                        direction--;
                        direction = direction < 0 ? 3 : direction;
                        break;
                    }
                    direction++;
                    direction = direction > 3 ? 0 : direction;
                    if (attempts > 4)
                        break;
                }
                if (attempts > 4)
                    break;
            }
            Profiler.EndSample();
            satellite = chunk.Values.First().Satellite;
            return blockPath.ToArray();
        }

        private static Vector2Int[] GetSideBlocks(
            Vector2Int block)
        {
            return new Vector2Int[]{
                block + Vector2Int.right,
                block + Vector2Int.up,
                block + Vector2Int.left,
                block + Vector2Int.down
            };
        }

        private static Vector2Int[] GetSideBlocks(
            KeyValuePair<Vector2Int, SolidBlock> block)
        {
            return new Vector2Int[]{
                block.Key + Vector2Int.right,
                block.Key + Vector2Int.up,
                block.Key + Vector2Int.left,
                block.Key + Vector2Int.down
            };
        }

        private static Dictionary<Vector2Int, bool> GetBlockVertices(Vector2Int position)
        {
            return new Dictionary<Vector2Int, bool>()
            {
                { position, false },
                { position + Vector2Int.right,false },
                { position + Vector2Int.up,false },
                { position + Vector2Int.one,false }
            };
        }

        private static Vector2Int[] GetSideNodes(Vector2Int node)
        {
            return new Vector2Int[]{
                node + Vector2Int.right,
                node + Vector2Int.up,
                node + Vector2Int.left,
                node + Vector2Int.down
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