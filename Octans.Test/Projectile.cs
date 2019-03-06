using System;

namespace Octans.Test
{
    public class Projectile
    {
        public Projectile(Point position, Vector velocity)
        {
            Position = position;
            Velocity = velocity;
        }

        public Point Position { get; }
        public Vector Velocity { get; }

        public Projectile Tick(World world)
        {
            var pos = Position + Velocity;
            var vel = Velocity + world.Gravity + world.Wind;
            return new Projectile(pos, vel);
        }

        // ReSharper disable once InconsistentNaming
        public (int x, int y) ToXY() => (x: (int) MathF.Round(Position.X), y: (int) MathF.Round(Position.Y));
    }
}