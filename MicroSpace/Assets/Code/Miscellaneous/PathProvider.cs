using UnityEngine;
using System;
using UnityEngine.AI;
using System.Linq;
using System.Collections.Generic;

namespace Miscellaneous
{
    public static class PathProvider
    {
        #region Public

        public static bool TryGetPath(
            Vector3 currentPosition,
            Vector3 targetPosition,
            out List<Vector3> path,
            Transform target = null)
        {
            Vector2 targetPositionCorrection = CreateCorrection(
                currentPosition,
                targetPosition);
            Vector2 correctedTargetPosition = CorrectTargetPosition(
                ref targetPositionCorrection,
                target);
            if (IsPathPossibleToCreate(
                currentPosition,
                correctedTargetPosition,
                out NavMeshHit currentClosestHit,
                out NavMeshHit targetClosestHit))
            {
                CreatePath(currentClosestHit, targetClosestHit, out path);
                return !IsPathObstructed(path, targetClosestHit);
            }
            path = new();
            return false;
        }

        private static bool IsPathObstructed(
            List<Vector3> path,
            NavMeshHit targetClosestHit)
        {
            if (path.Count > 0)
                if (Vector2.Distance(path[^1], targetClosestHit.position) > 1)
                    return true;
            return false;
        }

        #endregion Public

        #region Private

        private static bool CreatePath(
            NavMeshHit currentClosestHit,
            NavMeshHit targetClosestHit,
            out List<Vector3> pathPoints)
        {
            NavMeshPath path = new();
            bool isPathFound = NavMesh.CalculatePath(
                currentClosestHit.position,
                targetClosestHit.position,
                NavMesh.AllAreas,
                path);
            pathPoints = path.corners.ToList();
            return isPathFound;
        }

        private static bool CheckIfPositionIsValid(
            Vector2 position,
            out NavMeshHit closestPosition)
        {
            return NavMesh.SamplePosition(
                            position, out closestPosition, 2F, NavMesh.AllAreas);
        }

        private static bool IsPathPossibleToCreate(
            Vector3 currentPosition,
            Vector2 correctedTargetPosition,
            out NavMeshHit currentClosestHit,
            out NavMeshHit targetClosestHit)
        {
            bool isCurrentClosestPositionValid = CheckIfPositionIsValid(
                currentPosition,
                out currentClosestHit);
            bool isTargetClosestPositionValid = CheckIfPositionIsValid(
                correctedTargetPosition,
                out targetClosestHit);
            return isCurrentClosestPositionValid && isTargetClosestPositionValid;
        }

        private static Vector2 CorrectTargetPosition(
            ref Vector2 correction,
            Transform target = null)
        {
            return target.position
                + target.InverseTransformDirection(correction);
        }

        private static Vector2 CreateCorrection(
            Vector3 currentPosition,
            Vector3 targetPosition)
        {
            Vector2 correction = (currentPosition - targetPosition).normalized * 0.01F;
            if (Math.Abs(correction.x) > Math.Abs(correction.y))
                correction = new(0.01F * Math.Sign(correction.x), 0F);
            else
                correction = new(0F, 0.01F * Math.Sign(correction.y));
            return correction;
        }

        #endregion Private
    }
}