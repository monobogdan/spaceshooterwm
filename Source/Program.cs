using System;
using System.Collections.Generic;
using System.Text;

namespace WMGame3D
{
    class Program
    {
        static void Main(string[] args)
        {
            Engine.Init(800, 600);
            Engine.Current.Run();
        }
    }
}
