﻿using System;
using Octans.Light;

namespace Octans.Shading
{
    public class ShadingContext2
    {
        //private static int seed = Environment.TickCount;

        //private static readonly ThreadLocal<Random> random =
        //    new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref seed)));

        private readonly IFresnelFunction _ff;
        private readonly IGeometricShadow _gsf;

        //private readonly ThreadLocal<long> _index =
        //    new ThreadLocal<long>(() => Thread.CurrentThread.ManagedThreadId * 10);

        private readonly INormalDistribution _ndf;


        public ShadingContext2(int minDepth,
                               int maxDepth,
                               INormalDistribution ndf,
                               IGeometricShadow gsf,
                               in IFresnelFunction ff)
        {
            MinDepth = minDepth;
            MaxDepth = maxDepth;
            _ndf = ndf;
            _gsf = gsf;
            _ff = ff;
        }

        public int MinDepth { get; }

        public int MaxDepth { get; }

        //public static float Rand() => (float) random.Value.NextDouble();

        public static bool IsShadowed(World w, in Point p, in Point lightPoint)
        {
            var v = lightPoint - p;
            //   var distance = v.Magnitude();
            var direction = v.Normalize();
            var r = new Ray(p, direction);
            var xs = w.Intersect(in r);
            var h = xs.Hit(true);
            xs.Return();
            return h.HasValue && h.Value.T < v.Magnitude();
        }

        //private Color HitColor(World world, in IntersectionInfo info, int depth)
        //{
        //    // TODO: Forward propagate the quasi-random sampling instead of thread local tracking.
        //    var ambient = info.Geometry.Material.Ambient;
        //    var surfaceColor = info.Geometry.Material.Texture.ShapeColor(info.Geometry, info.OverPoint);

        //    // Illumination Equation
        //    // I = k_a*I_a + I_i(k_d(L%N) + k_s(V%R)^n) + k_t*I_t + k_r*I_r;
        //    // Ambient + Direct Diffuse + Direct Specular + Indirect (Specular & Diffuse)

        //    var surface = ambient * surfaceColor;
        //    // TODO: This is a hack/assumption to jump out for quick shading of environment map.
        //    if (ambient >= 1f)
        //    {
        //        return surface;
        //    }

        //    // Direct lighting
        //    foreach (var light in world.Lights)
        //    {
        //        // ReSharper disable InconsistentNaming
        //        var intensity = IntensityAt(world, info.OverPoint, light);
        //        var specularIntensity = 0f;
        //        var k_d = Colors.Black;
        //        var k_s = Colors.Black;
        //        var si = new ShadingInfo(intensity, in light, in info);
        //        if (intensity > 0f)
        //        {
        //            var denominator = 4f * si.NdotL * si.NdotV;
        //            if (denominator > 0f)
        //            {
        //                var D = 1f;
        //                D *= _ndf.Factor(in si);

        //                var G = 1f;
        //                G *= _gsf.Factor(in si);

        //                var F = 1f;
        //                F *= _ff.Factor(in si);

        //                specularIntensity = D * F * G / denominator;
        //            }

        //            // TODO: Unify. Diffuse color already includes diffuse intensity/attenuation at the moment.
        //            k_d = si.DiffuseColor;
        //            k_s = si.SpecularColor * specularIntensity;
        //        }
        //        // ReSharper restore InconsistentNaming

        //        var c = k_d + k_s;
        //        // Lambert's cosine law.
        //        surface += si.LightIntensity * c * si.NdotL;
        //    }

        //    // Russian roulette
        //    var rrFactor = 1f;
        //    if (depth >= 2)
        //    {
        //        var continueProbability = MathF.Min(1f - 0.0625f * depth, MathF.Max(surface.Red, MathF.Max(surface.Green, surface.Blue))  );
        //        if (Rand() > continueProbability)
        //        {
        //            return surface;
        //        }
        //        rrFactor = 1f / continueProbability;

        //        //float stopProbability = MathF.Min(1f, 0.0625f * depth);
        //        //if (Rand() <= stopProbability)
        //        //{
        //        //    return surface;
        //        //}
        //        //rrFactor = 1f / (1f - stopProbability);
        //    }


        //    // Indirect lighting
        //    var localFrame = new LocalFrame(info.Normal);
        //    var indirect = Colors.Black;

        //    // TODO: Make parameter for ray count.
        //    var rayCount = 1;
        //    var captured = 0;

        //    var di = _index.Value;
        //    while (captured < rayCount)
        //    {
        //        //var e0 = (float)Randoms.WellBalanced.NextDouble();
        //        //var e1 = (float)Randoms.WellBalanced.NextDouble();
        //        var (e0, e1) = QuasiRandom.Next(di++);
        //        var (wi, f) = _ndf.Sample(in info, in localFrame, e0, e1);
        //        if (!(wi.Z > 0f))
        //        {
        //            continue;
        //        }

        //        var direction = localFrame.ToWorld(in wi);
        //        var reflectedRay = new Ray(info.OverPoint, direction);
        //        var color = ColorAt(world, in reflectedRay, depth + 1, 0);
        //        indirect += color * f;
        //        captured++;
        //    }

        //    _index.Value = di;
        //    indirect /= captured;
        //    surface += indirect;

        //    return surface * rrFactor;
        //}

        public Color ColorAt(World world, in Ray ray, ISampler sampler)
        {
            var currentRay = ray;
            var throughPut = Colors.White;
            var color = Colors.Black;
            //   var maxDepth = 16;
            //   var depthFactor = 1 / (maxDepth);

            for (var depth = 0; depth < MaxDepth; depth++)
            {
                var xs = world.Intersect(in currentRay);
                var hit = xs.Hit();
                xs.Return();
                if (!hit.HasValue)
                {
                    break;
                }

                var info = new IntersectionInfo(hit.Value, currentRay);
                var material = info.Geometry.Material;
                var ambient = material.Ambient;
                var surfaceColor = material.Texture.ShapeColor(info.Geometry, info.OverPoint);

                // Illumination Equation
                // I = k_a*I_a + I_i(k_d(L%N) + k_s(V%R)^n) + k_t*I_t + k_r*I_r;
                // Ambient + Direct Diffuse + Direct Specular + Indirect (Specular & Diffuse)

                color += ambient * surfaceColor * throughPut;
                // TODO: This is a hack/assumption to jump out for quick shading of environment map.
                if (ambient >= 1f)
                {
                    break;
                }

                var alpha = material.Roughness * material.Roughness;
                var directProbability = alpha == 0 ? 0 : MathFunction.MixF(0.60f, 0.10f, 1f - alpha);
                if (sampler.Random() <= directProbability)
                {
                    var direct = DirectLighting(world, info, sampler);
                    color += direct * throughPut * (1 / directProbability);
                    break;
                }

                throughPut *= 1f / (1f - directProbability);

                // Indirect lighting
                (currentRay, throughPut) = IndirectLighting(sampler, info, in throughPut);


                if (depth < MinDepth)
                {
                    continue;
                }

                // Russian roulette
                var maxChannel = MathF.Max(throughPut.Red, MathF.Max(throughPut.Green, throughPut.Blue));
                var continueProbability = MathFunction.ClampF(0f, 1f, maxChannel);
                if (sampler.Random() > continueProbability)
                {
                    break;
                }

                throughPut *= 1f / continueProbability;

                //var stopProbability =  MathF.Min(1f, depthFactor * (depth));
                //if (sampler.Random() <= stopProbability)
                //{
                //    break;
                //}

                //throughPut *= 1f / (1f - stopProbability);
            }

            // _index.Value = index;
            return color;
        }

        private (Ray bounce, Color throughPut) IndirectLighting(ISampler sampler,
                                                                IntersectionInfo info,
                                                                in Color throughPut)
        {
            var localFrame = new LocalFrame(info.Normal);
            var (e0, e1) = sampler.NextUV();
            while (true)
            {
                var (wi, f) = _ndf.Sample(in info, in localFrame, e0, e1);
                if (!(wi.Z > 0f))
                {
                    (e0, e1) = sampler.NextUV();
                    continue;
                }

                var direction = localFrame.ToWorld(in wi);
                var currentRay = new Ray(info.OverPoint, direction);
                return (currentRay, throughPut * f);
            }
        }

        private Color DirectLighting(World world, IntersectionInfo info, ISampler sampler)
        {
            var surface = Colors.Black;
            // Direct lighting
            for (var i = 0; i < world.Lights.Count; i++)
            {
                var light = world.Lights[i];
                // ReSharper disable InconsistentNaming
                var (intensity, sampled) = IntensityAt(world, info.OverPoint, light, sampler);


                if (!(intensity > 0f))
                {
                    continue;
                }

                var si = new ShadingInfo(intensity, in light, in info, in sampled);
                var specularIntensity = 0f;
                var denominator = 4f * si.NdotL * si.NdotV;
                if (denominator > 0.001f)
                {
                    var D = 1f;
                    D *= _ndf.Factor(in si);

                    var G = 1f;
                    G *= _gsf.Factor(in si);

                    var F = 1f;
                    F *= _ff.Factor(in si);

                    specularIntensity = D * F * G / denominator;
                }

                // TODO: Unify. Diffuse color already includes diffuse intensity/attenuation at the moment.
                var k_d = si.DiffuseColor;
                var k_s = si.SpecularColor * specularIntensity;

                var c = k_d + k_s;
                // Lambert's cosine law.
                surface += si.LightIntensity * c * si.NdotL;
                // ReSharper restore InconsistentNaming
            }

            return surface;
        }

        //public Color ReflectedColor(World world, in IntersectionInfo info, int remaining = 5)
        //{
        //    if (remaining < 1)
        //    {
        //        return Colors.Black;
        //    }

        //    var roughness = info.Geometry.Material.Roughness;
        //    var reflective = 1f - MathF.Pow(roughness, 0.14f);

        //    //var reflective = info.Geometry.Material.Reflective;
        //    if (reflective <= 0f)
        //    {
        //        return Colors.Black;
        //    }

        //    var reflectedRay = new Ray(info.OverPoint, info.Reflect);
        //    var color = ColorAt(world, in reflectedRay, --remaining);
        //    return color * reflective;
        //}

        //public Color RefractedColor(World world, in IntersectionInfo info, int remaining)
        //{
        //    if (remaining < 1)
        //    {
        //        return Colors.Black;
        //    }

        //    if (info.Geometry.Material.Transparency <= 0f)
        //    {
        //        return Colors.Black;
        //    }

        //    var nRatio = info.N1 / info.N2;
        //    var cosI = info.Eye % info.Normal;
        //    var sin2T = nRatio * nRatio * (1f - cosI * cosI);
        //    if (sin2T > 1f)
        //    {
        //        // Total internal reflection.
        //        return Colors.Black;
        //    }

        //    var cosT = MathF.Sqrt(1f - sin2T);
        //    var direction = info.Normal * (nRatio * cosI - cosT) - info.Eye * nRatio;
        //    var refractedRay = new Ray(info.UnderPoint, direction);
        //    return ColorAt(world, in refractedRay, --remaining) * info.Geometry.Material.Transparency;
        //}

        //public static float Schlick(in IntersectionInfo info)
        //{
        //    // Cosine of angle between eye and normal vectors.
        //    var cos = info.Eye % info.Normal;

        //    // Total internal reflections can only occur when N1 > N2.
        //    if (info.N1 > info.N2)
        //    {
        //        var n = info.N1 / info.N2;
        //        var sin2T = n * n * (1f - cos * cos);
        //        if (sin2T > 1f)
        //        {
        //            return 1.0f;
        //        }

        //        // Cosine of theta_t
        //        var cosT = MathF.Sqrt(1f - sin2T);
        //        cos = cosT;
        //    }

        //    // r0 == F0 - equivalent refractive index
        //    var r0 = (info.N1 - info.N2) / (info.N1 + info.N2);
        //    r0 *= r0;
        //    return r0 + (1f - r0) * MathF.Pow(1 - cos, 5);
        //}

        public static (float intensity, Point sampledPoint) IntensityAt(
            World world,
            in Point point,
            ILight light,
            ISampler sampler)
        {
            switch (light)
            {
                case PointLight _:
                    return (IsShadowed(world, in point, light.Position) ? 0.0f : 1.0f, light.Position);
                case AreaLight area:
                {
                    var (lu, lv) = sampler.NextUV();
                    var lPoint = area.GetPoint(lu, lv);
                    return (!IsShadowed(world, in point, lPoint) ? 1f : 0f, lPoint);


                    //    var total = 0.0f;
                    //for (var v = 0; v < area.VSteps; v++)
                    //{
                    //    for (var u = 0; u < area.USteps; u++)
                    //    {
                    //        if (!IsShadowed(world, in point, area.UVPoint(u, v)))
                    //        {
                    //            total += 1.0f;
                    //        }
                    //    }
                    //}

                    //return total / area.Samples;
                }
                default:
                    return (0f, new Point(0, 0, 0));
            }
        }
    }
}