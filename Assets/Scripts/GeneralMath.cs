using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class GeneralMath
    {
        /// <summary>
        /// Get the angle between two points
        /// </summary>
        public static float AngleTowardsPoint2D(Vector3 p1, Vector3 p2)
        {
            // calculate direction vector
            Vector2 dir2 = (new Vector2(p1.x, p1.z) -
                            new Vector2(p2.x, p2.z)).normalized;

            // calculate angle from direction vector
            float angle = (float)(Math.Atan2(dir2.y, -dir2.x) * (180 / Math.PI));

            return angle;
        }
    }
}