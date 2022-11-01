using System;
using System.Collections.Generic;
using System.Text;

namespace WMGame3D
{
    public sealed class Background
    {
        private const int BgCount = 2;

        private Mesh backQuad;
        private Material backMat;

        public Background()
        {
            backQuad = Mesh.FromFile("skybox.md2");
            backMat.Texture = Texture2D.FromFile("bg" + new Random().Next(0, BgCount) + ".png");
            backMat.ZWrite = true;
        }

        public void Draw()
        {
            Engine.Current.Graphics.DrawMesh(backQuad, new Transform(Engine.Current.Graphics.Camera.Position
                , new Vector3(90, 0, Game.Current.TotalTime), Vector3.One), backMat);
        }
    }
}
