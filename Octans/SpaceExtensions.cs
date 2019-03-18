﻿using System.Diagnostics.Contracts;

namespace Octans
{
    public static class SpaceExtensions
    {
        [Pure]
        public static Ray ToLocal(in this Ray worldRay, in IShape shape) =>
            worldRay.Transform(shape.TransformInverse());

        [Pure]
        public static IIntersections Intersects(this IShape shape, in Ray worldRay)
        {
            var localRay = worldRay.ToLocal(in shape);
            return shape.LocalIntersects(in localRay);
        }

        [Pure]
        public static Point ToLocal(this IShape shape, in Point worldPoint)
        {
            var world = worldPoint;
            if (shape.Parent != null)
            {
                world = shape.Parent.ToLocal(in worldPoint);
            }

            return shape.TransformInverse() * world;
        }

        [Pure]
        public static Point ToLocal(in this Point worldPoint, IShape shape, IPattern pattern)
        {
            var world = worldPoint;
            if (shape.Parent != null)
            {
                world = shape.Parent.ToLocal(in worldPoint);
            }

            var localPoint = shape.ToLocal(in world);
            return pattern.TransformInverse() * localPoint;
        }

        [Pure]
        public static Vector NormalAt(this IShape shape, in Point worldPoint, in Intersection intersection)
        {
            var localPoint = shape.ToLocal(in worldPoint);
            var localNormal = shape.LocalNormalAt(localPoint, in intersection);
            return shape.NormalToWorld(in localNormal);
        }

        [Pure]
        public static Vector NormalToWorld(this IShape shape, in Vector localNormal)
        {
            var normal = shape.TransformInverse().Transpose() * localNormal;
            normal = normal.ZeroW().Normalize();
            if (shape.Parent != null)
            {
                normal = NormalToWorld(shape.Parent, normal);
            }

            return normal;
        }
    }
}