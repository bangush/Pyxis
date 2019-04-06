﻿using System.Linq;

namespace Octans.Geometry
{
    internal static class SolidExtensions
    {
        public static bool Includes(this IGeometry a, IGeometry b)
        {
            switch (a)
            {
                case Group g when g.Children.Any(c => ReferenceEquals(a, c) || Includes(c, b)):
                    return true;
                case ConstructiveSolid s:
                    return s.Left.Includes(b) || s.Right.Includes(b);
                default:
                    return ReferenceEquals(a, b);
            }
        }
    }
}