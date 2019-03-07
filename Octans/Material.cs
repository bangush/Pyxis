﻿namespace Octans
{
    public class Material
    {
        public Material()
        {
            Pattern = new SolidColor(Colors.White);
            Ambient = 0.1f;
            Diffuse = 0.9f;
            Specular = 0.9f;
            Shininess = 200f;
        }

        public float Shininess { get; set; }

        public float Specular { get; set; }

        public float Diffuse { get; set; }

        public float Ambient { get; set; }

        public IPattern Pattern { get; set; }
    }
}