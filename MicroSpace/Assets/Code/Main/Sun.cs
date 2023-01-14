using System.Collections.Generic;
using System.Linq;
using Entities;
using Maths;
using UnityEngine;

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
            Dictionary<Entity, int> entities = new();
            foreach (var wall in Wall.EnabledWalls)
                entities.Add(wall, 0);
            foreach (var floor in Floor.EnabledFloors)
                entities.Add(floor, 1);
            foreach (var rigidEntity in RigidEntity.EnabledRigidEntities)
                entities.Add(rigidEntity, 2);
            entities = entities
                .OrderBy(entity => entity.Key.transform.position.magnitude)
                .ToDictionary(pair => pair.Key, pair => pair.Value);
            foreach (var entity in entities)
            {
                IlluminateBasedOnType(entity);
            }
        }

        private void IlluminateBasedOnType(KeyValuePair<Entity, int> entity)
        {
            switch (entity.Value)
            {
                default:
                    IlluminateWall((Wall)entity.Key);
                    break;

                case 1:
                    IlluminateFloor((Floor)entity.Key);
                    break;

                case 2:
                    IlluminateRigidEntity((RigidEntity)entity.Key);
                    break;
            }
        }

        private void IlluminateWall(Wall wall)
        {
            Range wallRange = wall.ShadowRange;
            foreach (var range in _ranges)
            {
                if (wallRange.IsCovered(range))
                {
                    wall.SetLightActive(false);
                    return;
                }
            }
            wall.SetLightActive(true);
            _ranges.Add(wallRange);
            OrganiseRanges();
        }

        private void IlluminateFloor(Floor floor)
        {
            Range floorRange = floor.ShadowRange;
            foreach (var range in _ranges)
            {
                if (floorRange.IsCovered(range))
                {
                    floor.SetLightActive(false);
                    return;
                }
            }
            floor.SetLightActive(true);
        }

        private void IlluminateRigidEntity(RigidEntity rigidEntity)
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
            _ranges = _ranges.OrderBy(range => range.Start).ToList();
            for (int i = 0; i < _ranges.Count - 1; i++)
            {
                if (_ranges[i].IsOverlapping(_ranges[i + 1], out Range combinedRange))
                {
                    _ranges[i] = combinedRange;
                    _ranges.RemoveAt(i + 1);
                }
            }
        }

        #endregion Private
    }
}