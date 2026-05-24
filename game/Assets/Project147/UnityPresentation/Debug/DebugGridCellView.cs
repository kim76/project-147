using Project147.GameCore.Grid;
using UnityEngine;

namespace Project147.UnityPresentation.Debug
{
    public sealed class DebugGridCellView : MonoBehaviour
    {
        private DebugGridSceneController controller;

        public GridCoordinate Coordinate { get; private set; }

        public void Initialise(DebugGridSceneController owner, GridCoordinate coordinate)
        {
            controller = owner;
            Coordinate = coordinate;
        }

        private void OnMouseDown()
        {
            if (controller != null)
            {
                controller.TryPlaceTower(Coordinate);
            }
        }
    }
}

