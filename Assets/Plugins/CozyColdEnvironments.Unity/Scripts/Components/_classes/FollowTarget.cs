using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Components
{
    public class FollowTarget : CCBehaviour
    {
        [SerializeField]
        private Transform? target;

        [SerializeField]
        private Axes positionAxes = Axes.All;

        public Transform? Target {
            get => target;
            set => SetTarget(value);
        }

        private Axes PositionAxes {
            get => positionAxes;
            set => SetPositionAxes(value);
        }

        public FollowTarget SetTarget(Transform? value)
        {
            target = value;
            return this;
        }

        public FollowTarget SetPositionAxes(Axes value)
        {
            positionAxes = value;
            return this;
        }

        private void Update()
        {
            if (target == null
                ||
                positionAxes == Axes.None)
            {
                return;
            }

            var targetPos = target.position;
            var nextPos = cTransform.position;

            if (positionAxes.HasFlagT(Axes.X))
                nextPos.x = targetPos.x;

            if (positionAxes.HasFlagT(Axes.Y))
                nextPos.y = targetPos.y;

            if (positionAxes.HasFlagT(Axes.Z))
                nextPos.z = targetPos.z;

            cTransform.position = nextPos;
        }
    }
}
