using System.Collections.Generic;
using Project147.GameCore.Grid;
using UnityEngine;

namespace Project147.UnityPresentation.Debug
{
    public sealed class DebugAlienPathFollower : MonoBehaviour
    {
        private IReadOnlyList<GridCoordinate> path = new List<GridCoordinate>();
        private float cellSize = 1;
        private float speedCellsPerSecond = 2;
        private int pathIndex;
        private Vector3 targetPosition;

        public void Initialise(
            IReadOnlyList<GridCoordinate> pathToFollow,
            float gridCellSize,
            float movementSpeedCellsPerSecond)
        {
            path = pathToFollow ?? new List<GridCoordinate>();
            cellSize = gridCellSize;
            speedCellsPerSecond = movementSpeedCellsPerSecond;
            pathIndex = 0;

            if (path.Count == 0)
            {
                gameObject.SetActive(false);
                return;
            }

            gameObject.SetActive(true);
            transform.localPosition = ToWorldPosition(path[0]);
            targetPosition = transform.localPosition;
        }

        private void Update()
        {
            if (path.Count == 0)
            {
                return;
            }

            if (Vector3.Distance(transform.localPosition, targetPosition) <= 0.001f)
            {
                AdvanceTarget();
            }

            var speed = speedCellsPerSecond * cellSize;
            transform.localPosition = Vector3.MoveTowards(
                transform.localPosition,
                targetPosition,
                speed * Time.deltaTime);
        }

        private void AdvanceTarget()
        {
            pathIndex++;

            if (pathIndex >= path.Count)
            {
                pathIndex = 0;
            }

            targetPosition = ToWorldPosition(path[pathIndex]);
        }

        private Vector3 ToWorldPosition(GridCoordinate coordinate)
        {
            return new Vector3(coordinate.Column * cellSize, 0.35f, coordinate.Row * cellSize);
        }
    }
}

