using UnityEngine;
using UnityEngine.UI;
using UTIRLib.Diagnostics;
using UTIRLib.Injector;
using UTIRLib.TwoD;

#nullable enable

namespace UTIRLib.UI
{
    [DisallowMultipleComponent]
    public class GridLayoutGroupElement : CanvasElement
    {
        private Vector3? resolvedRelativePositionPosition;
        private Relative2DPosition relativePosition;

        [SerializeField]
        [GetComponentInParentIfNull]
        protected UserInterface userInterface;

        public Relative2DPosition RelativePosition {
            get {
                if (!resolvedRelativePositionPosition.HasValue
                    || transform.position != resolvedRelativePositionPosition)
                {
                    resolvedRelativePositionPosition = transform.position;
                    ResolvePosition();
                }

                return relativePosition;
            }
        }

        [GetComponentInParent]
        public GridLayoutGroup Grid { get; private set; } = null!;

        public GridLayoutGroupElement? GetNeighbourElement(Direction2D direction)
        {
            TirLibDebug.Log($"Define neghbour by direction: {direction}",
                            this,
                            true);

            Vector2 pos;
            if (InScreenSpace) pos = ScreenPosition;
            else pos = rectTransform.position;

            Vector2 cellSize = Grid.cellSize;
            Vector2 spacing = Grid.spacing;

            switch (direction)
            {
                case Direction2D.Down:
                    pos.y -= cellSize.y + spacing.y;
                    break;

                case Direction2D.Up:
                    pos.y += cellSize.y + spacing.y;
                    break;

                case Direction2D.Left:
                    pos.x -= cellSize.x + spacing.x;
                    break;

                case Direction2D.Right:
                    pos.x += cellSize.x + spacing.x;
                    break;

                default:
                    return null;
            }

            return userInterface.Raycaster.RaycastFirst<GridLayoutGroupElement>(pos, this);
        }

        private void ResolvePosition()
        {
            Direction2D neighbourElementsPositions = CheckNeighbourElements();

            relativePosition = neighbourElementsPositions.GetRelativePosition();

            TirLibDebug.Assert(relativePosition == Relative2DPosition.None,
                               $"Cannot define relative direction.",
                               this);
        }

        private Direction2D CheckNeighbourElements()
        {
            Direction2D[] toCheckDirectionsArray = new Direction2D[]{
                Direction2D.Left,
                Direction2D.Right,
                Direction2D.Up,
                Direction2D.Down
            };

            Direction2D directions = Direction2D.None;
            foreach (var dir in toCheckDirectionsArray)
            {
                if (GetNeighbourElement(dir) != null)
                {
                    directions |= dir;
                }
            }

            return directions;
        }
    }
}