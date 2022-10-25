using System;
using System.Collections.Generic;
using System.Text;

namespace WMGame3D
{
    class Program
    {
        static void Main(string[] args)
        {
            Engine.Init();
            Engine.Current.Run();
        }
    }
}
