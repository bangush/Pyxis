﻿using System;
using FluentAssertions;
using Pyxis.Camera;
using Pyxis.Sampling;
using Xunit;

namespace Pyxis.Test.Camera
{
    public class PinholeCameraTests
    {
        [Fact]
        public void ConstructingCamera()
        {
            var c = new PinholeCamera(in Transform.Identity, MathF.PI / 2f, 160f/120);
            c.AspectRatio.Should().Be(160f / 120);
            c.FieldOfView.Should().Be(MathF.PI / 2f);
            c.CameraToWorld.Matrix.Should().Be(Matrix.Identity);
            c.CameraToWorld.Inverse.Should().Be(Matrix.Identity);
        }

        [Fact]
        public void RayThroughCenterOfCanvas()
        {
            var width = 201;
            var height = 101;
            var c = new PinholeCamera(in Transform.Identity, MathF.PI / 2f, (float) width / height);
            var coordinate = new PixelCoordinate(100, 50);
            var (r, _) =
                c.CameraRay(
                    new PixelSample(new PixelInformation(coordinate, width, height), new UVPoint(0.5f, 0.5f)),
                    new TestSampler(0f, 0f, 0.5f));
            r.Origin.Should().Be(Point.Zero);
            r.Direction.Should().Be(new Vector(0, 0, -1));
        }

        [Fact]
        public void RayThroughCornerOfCanvas()
        {
            var width = 201;
            var height = 101;
            var c = new PinholeCamera(in Transform.Identity, MathF.PI / 2f, (float)width / height);
            var coordinate = new PixelCoordinate(0, 0);
            var (r, _) =
                c.CameraRay(
                    new PixelSample(new PixelInformation(coordinate, width, height), new UVPoint(0.5f, 0.5f)),
                    new TestSampler(0f, 0f, 0.5f));
            r.Origin.Should().Be(Point.Zero);
            r.Direction.Should().Be(new Vector(0.66519f, 0.33259f, -0.66851f));
        }

        [Fact]
        public void RayAfterCameraTransform()
        {
            var width = 201;
            var height = 101;
            var transform = Transform.RotateY(MathF.PI / 4f) * Transform.Translate(0, -2, 5);
            var c = new PinholeCamera(transform, MathF.PI / 2f, (float)width / height);
            var coordinate = new PixelCoordinate(100, 50);
            var (r, _) =
                c.CameraRay(
                    new PixelSample(new PixelInformation(coordinate, width, height), new UVPoint(0.5f, 0.5f)),
                    new TestSampler(0f, 0f, 0.5f));
            r.Origin.Should().Be(new Point(0, 2, -5));
            r.Direction.Should().Be(new Vector(MathF.Sqrt(2f) / 2f, 0.0f, -MathF.Sqrt(2f) / 2f));
        }
    }

    public class TestSampler : ISampler
    {
        private readonly float _u;
        private readonly float _v;
        private readonly float _rand;

        public TestSampler(float u, float v, float rand)
        {
            _u = u;
            _v = v;
            _rand = rand;
        }

        public UVPoint NextUV()
        {
            return new UVPoint(_u, _v);
        }

        public float Random()
        {
            return _rand;
        }

        public ISampler Create(ulong i)
        {
            return this;
        }
    }
}