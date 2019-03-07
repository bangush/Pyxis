﻿using System;
using System.Collections.Generic;

namespace Octans
{
    public class Plane : ShapeBase
    {
        private const float Epsilon = 0.00001f;

        public override IReadOnlyList<Intersection> LocalIntersects(in Ray localRay)
        {
            // XZ plane
            if (MathF.Abs(localRay.Direction.Y) < Epsilon)
            {
                return Intersections.Empty;
            }

            var t = -localRay.Origin.Y / localRay.Direction.Y;
            return new Intersections(new Intersection(t, this));
        }

        public override Vector LocalNormalAt(in Point localPoint) => new Vector(0, 1, 0);
    }
}