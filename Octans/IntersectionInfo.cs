﻿namespace Octans
{
    public readonly struct IntersectionInfo
    {
        public IntersectionInfo(Intersection intersection, Ray ray) : this(
            intersection, ray, Intersections.Create(intersection))
        {
        }

        public IntersectionInfo(
            Intersection intersection,
            Ray ray,
            IIntersections intersections)
        {
            T = intersection.T;
            Geometry = intersection.Geometry;
            Point = ray.Position(T);
            Eye = -ray.Direction;
            Normal = Geometry.NormalAt(in Point, in intersection);
            if (Normal % Eye < 0f)
            {
                IsInside = true;
                Normal = -Normal;
            }
            else
            {
                IsInside = false;
            }

            var offset = Normal * Epsilon;
            OverPoint = Point + offset;
            UnderPoint = Point - offset;
            Reflect = Vector.Reflect(in ray.Direction, in Normal);

            (N1, N2) = IntersectionCalculations.DetermineN1N2(in intersection, in intersections);
        }

        public readonly float T;
        public readonly IGeometry Geometry;
        public readonly Point Point;
        public readonly Point OverPoint;
        public readonly Point UnderPoint;
        public readonly Vector Eye;
        public readonly Vector Normal;
        public readonly bool IsInside;
        public readonly Vector Reflect;
        public readonly float N1;
        public readonly float N2;

        public const float Epsilon = 0.0001f;
    }
}