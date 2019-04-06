﻿namespace Octans.Shading
{
    public interface INormalDistribution
    {
        float Factor(in ShadingInfo info);
        (Vector wi, Color reflectance) Sample(in IntersectionInfo info, in LocalFrame localFrame, float e0, float e1);

        Color Transmission(in ShadingInfo si, in IntersectionInfo info, in Vector wo, in Vector wm, in Vector wi);
    }
}