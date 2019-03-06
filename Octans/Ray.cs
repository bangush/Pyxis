﻿namespace Octans
{
    public readonly struct Ray
    {
        public readonly Tuple Origin;
        public readonly Tuple Direction;

        public Ray(Tuple origin, Tuple direction)
        {
            Origin = origin.AsPoint();
            Direction = direction.AsVector();
        }

        public Tuple Position(float t)
        {
            return Origin + Direction * t;
        }

        public Ray Transform(in Matrix matrix)
        {
            var origin = matrix * Origin;
            var direction = matrix * Direction;
            return new Ray(origin,direction);
        }
    }
}