using Google.Protobuf.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtoDescriptorHelper.Options
{
    class OptionsUtil
    {
        internal static Type GetType(string extendee)=> extendee switch
        {
            ".google.protobuf.EnumOptions" => typeof(EnumOptions),
            ".google.protobuf.MessageOptions" => typeof(MessageOptions),
            ".google.protobuf.EnumValueOptions" => typeof(EnumValueOptions),
            ".google.protobuf.FieldOptions" => typeof(FieldOptions),
            ".google.protobuf.FileOptions" => typeof(Google.Protobuf.Reflection.FileOptions),
            ".google.protobuf.OneofOptions" => typeof(OneofOptions),
            ".google.protobuf.ServiceOptions" =>typeof(ServiceOptions),
            ".google.protobuf.MethodOptions" => typeof(MethodOptions),
            _ => throw new NotImplementedException(extendee),
        };

    }
}
