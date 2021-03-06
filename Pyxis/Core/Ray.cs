﻿namespace Pyxis
{
    public readonly struct Ray
    {
        public readonly Point Origin;
        public readonly Vector Direction;
        public readonly Vector InverseDirection;

        public Ray(Point origin, Vector direction)
        {
            Origin = origin;
            Direction = direction;
            InverseDirection = 1f / direction;
        }

        public Point Position(float t) => Origin + Direction * t;
    }
}