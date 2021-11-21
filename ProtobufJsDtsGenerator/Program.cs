using System;
using System.Collections.Generic;

namespace ProtobufJsDtsGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> pbdescriptors = new List<string>();
            string dtsOut = "";

            for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                if (arg == "--pbdesc")
                {
                    for (i++; i < args.Length; i++)
                    {
                        arg = args[i];
                        if (!arg.StartsWith("--"))
                        {
                            pbdescriptors.Add(arg);
                        }
                        else
                        {
                            i--;
                            break;
                        }
                    }
                }
                else if (arg == "--dts_out")
                {
                    dtsOut = args[++i];
                }
            }
            var pdr = new ProtoDescriptorHelper.Resolver(pbdescriptors.ToArray());
            new ProtoDtsGen().Gen(pdr, dtsOut);
        }

    }
}
