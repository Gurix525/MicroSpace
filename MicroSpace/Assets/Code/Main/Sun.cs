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
                var blocksPolygonPath = GetBlocksPolygonPath(chunk);
                Vector2Int[] polygonPath = GetPolygonPath(blocksPolygonPath);
            }
            Profiler.EndSample();
        }

        private static Vector2Int[] GetPolygonPath(Vector2Int[] blocksPolygonPath)
        {
            Dictionary<Vector2Int, bool> path = new();
            int index = 0;
            int direction = 0;
            Vector2Int currentNode = blocksPolygonPath[0];
            path.Add(currentNode, false);
            while (true)
            {
                var sideNodes = GetSideNodes(currentNode);
                while (true)
                {
                    if (index < blocksPolygonPath.Length - 1)
                    {
                        var nextBlockVertices = GetBlockVertices(blocksPolygonPath[index + 1]);
                        if (nextBlockVertices.ContainsKey(sideNodes[direction]))
                        {
                            currentNode = sideNodes[direction];
                            if (path.ContainsKey(currentNode))
                                break;
                            path.Add(currentNode, false);
                            index++;
                            direction--;
                            direction = direction < 0 ? 3 : direction;
                            break;
                        }
                    }
                    var currentBlockVertices = GetBlockVertices(blocksPolygonPath[index]);
                    if (currentBlockVertices.ContainsKey(sideNodes[direction]))
                    {
                        currentNode = sideNodes[direction];
                        if (path.ContainsKey(currentNode))
                            break;
                        path.Add(currentNode, false);
                        direction--;
                        direction = direction < 0 ? 3 : direction;
                        break;
                    }
                    direction++;
                    direction = direction > 3 ? 0 : direction;
                    continue;
                }
                if (path.ContainsKey(currentNode))
                    break;
            }
            return path
                .Select(node => node.Key)
                .ToArray();
        }

        private static Vector2Int[] GetBlocksPolygonPath(Dictionary<Vector2Int, SolidBlock> chunk)
        {
            Profiler.BeginSample("GetBlocksPolygonPath");
            var sortedBlocks = chunk
                .OrderBy(block => block.Key.y)
                .ThenBy(block => block.Key.x)
                .ToDictionary(block => block.Key, block => block.Value);
            Dictionary<Vector2Int, int> blockPath = new();
            blockPath.Add(sortedBlocks.First().Key, 0);
            var currentBlock = blockPath.First();
            int currentDirection = 0;
            int currentBlockNumber = 1;
            int reversingBlock = 0;
            while (true)
            {
                int attempts = 0;
                var sideBlocks = GetSideBlocks(
                    new(currentBlock.Key, sortedBlocks[currentBlock.Key]));
                while (true)
                {
                    attempts++;
                    if (blockPath.ContainsKey(sideBlocks[currentDirection]))
                        break;
                    if (sortedBlocks.ContainsKey(sideBlocks[currentDirection]))
                    {
                        blockPath.Add(
                            sideBlocks[currentDirection],
                            currentBlockNumber);
                        currentBlockNumber++;
                        currentDirection--;
                        currentDirection = currentDirection < 0 ? 3 : currentDirection;
                        currentBlock = blockPath.First();
                        break;
                    }
                    currentDirection++;
                    currentDirection = currentDirection > 3 ? 0 : currentDirection;
                    if (attempts > 4)
                        break;
                }
                if (blockPath.ContainsKey(sideBlocks[currentDirection]))
                {
                    reversingBlock = blockPath[sideBlocks[currentDirection]];
                    break;
                }
                if (attempts > 4)
                    break;
            }
            var numberedBlockPath = blockPath.OrderBy(block => block.Value).ToList();
            for (int i = reversingBlock - 1; i >= 0; i--)
            {
                numberedBlockPath.Add(numberedBlockPath[i]);
            }
            Profiler.EndSample();
            return numberedBlockPath
                .Select(block => block.Key)
                .ToArray();
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