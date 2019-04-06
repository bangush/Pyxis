﻿using System;

namespace Octans
{
    public static class Transforms
    {
        public static Matrix Translate(float x, float y, float z) =>
            new Matrix(new[] {1.0f, 0, 0, x},
                       new[] {0.0f, 1, 0, y},
                       new[] {0.0f, 0, 1, z},
                       new[] {0.0f, 0, 0, 1});

        public static Matrix TranslateX(float x) => Translate(x, 0f, 0f);
        public static Matrix TranslateY(float y) => Translate(0f, y, 0f);
        public static Matrix TranslateZ(float z) => Translate(0f, 0f, z);

        public static Matrix Scale(float x, float y, float z) =>
            new Matrix(new[] {x, 0, 0, 0},
                       new[] {0.0f, y, 0, 0},
                       new[] {0.0f, 0, z, 0},
                       new[] {0.0f, 0, 0, 1});

        public static Matrix Scale(float factor) => Scale(factor, factor, factor);

        public static Matrix RotateX(float rad) =>
            new Matrix(new[] {1.0f, 0, 0, 0},
                       new[] {0.0f, MathF.Cos(rad), -MathF.Sin(rad), 0},
                       new[] {0.0f, MathF.Sin(rad), MathF.Cos(rad), 0},
                       new[] {0.0f, 0, 0, 1});

        public static Matrix RotateY(float rad) =>
            new Matrix(new[] {MathF.Cos(rad), 0, MathF.Sin(rad), 0},
                       new[] {0.0f, 1, 0, 0},
                       new[] {-MathF.Sin(rad), 0, MathF.Cos(rad), 0},
                       new[] {0.0f, 0, 0, 1});

        public static Matrix RotateZ(float rad) =>
            new Matrix(new[] {MathF.Cos(rad), -MathF.Sin(rad), 0, 0},
                       new[] {MathF.Sin(rad), MathF.Cos(rad), 0, 0},
                       new[] {0.0f, 0, 1, 0},
                       new[] {0.0f, 0, 0, 1});

        public static Matrix Shear(float xy, float xz, float yx, float yz, float zx, float zy) =>
            new Matrix(new[] {1.0f, xy, xz, 0},
                       new[] {yx, 1, yz, 0},
                       new[] {zx, zy, 1, 0},
                       new[] {0.0f, 0, 0, 1});

        public static Matrix View(Point from, Point to, Vector up)
        {
            var forward = (to - from).Normalize();
            var left = Vector.Cross(forward, up.Normalize());
            var trueUp = Vector.Cross(left, forward);
            var orientation = new Matrix(new[] {left.X, left.Y, left.Z, 0},
                                         new[] {trueUp.X, trueUp.Y, trueUp.Z, 0},
                                         new[] {-forward.X, -forward.Y, -forward.Z, 0},
                                         new[] {0.0f, 0, 0, 1});
            return orientation * Translate(-from.X, -from.Y, -from.Z);
        }

        public static Matrix Translate(this Matrix m, float x, float y, float z) => Translate(x, y, z) * m;

        public static Matrix Apply(this Matrix m, in Matrix n) => n * m;

        public static Matrix TranslateX(this Matrix m, float x) => TranslateX(x) * m;

        public static Matrix TranslateY(this Matrix m, float y) => TranslateY(y) * m;

        public static Matrix TranslateZ(this Matrix m, float z) => TranslateX(z) * m;

        public static Matrix Scale(this Matrix m, float x, float y, float z) => Scale(x, y, z) * m;

        public static Matrix Scale(this Matrix m, float factor) => Scale(factor) * m;

        public static Matrix RotateX(this Matrix m, float rad) => RotateX(rad) * m;

        public static Matrix RotateY(this Matrix m, float rad) => RotateY(rad) * m;

        public static Matrix RotateZ(this Matrix m, float rad) => RotateZ(rad) * m;

        public static Matrix Shear(this Matrix m, float xy, float xz, float yx, float yz, float zx, float zy) =>
            Shear(xy, xz, yx, yz, zx, zy) * m;
    }
}