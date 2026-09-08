using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX
{
    public static class RayHelper
    {
        public static Vector3 GetPointByY(this Ray ray, float y)
        {
            if (Mathf.Abs(ray.direction.y) > 0.0001f)
            {
                float t = (-ray.origin.y + y) / ray.direction.y;

                if (t >= 0f)
                    return ray.origin + t * ray.direction;
            }

            return default;
        }
    }
}
