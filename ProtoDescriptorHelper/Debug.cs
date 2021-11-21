using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtoDescriptorHelper
{
    public static class Debug
    {
        public static void Info(string str)
        {
            Console.WriteLine($"Info: {str}");
        }

        public static void Warn(string str)
        {
            Console.WriteLine($"Warn: {str}");
        }

        public static void Error(string str)
        {
            Console.WriteLine($"Error: {str}");
        }
    }
}
