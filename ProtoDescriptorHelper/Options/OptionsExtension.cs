using Google.Protobuf;
using Google.Protobuf.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtoDescriptorHelper
{
    static class OptionsExtension
    {
        static readonly MessageOptions defaultMessageOptions = new MessageOptions();
        static readonly FileOptions defaultFileOptions = new FileOptions();
        static readonly FieldOptions defaultFieldOptions = new FieldOptions();
        static readonly EnumOptions defaultEnumOptions = new EnumOptions();

        public static TValue GetExtOrDefault<TValue>(this MessageOptions options, Extension<MessageOptions, TValue> extension)
        {
            return (options ?? defaultMessageOptions).GetExtension(extension);
        }

        public static bool TryGetExtension<TValue>(this MessageOptions options, Extension<MessageOptions, TValue> extension,out TValue value)
        {
            value = options.GetExtOrDefault(extension);
            return options != null;
        }

        public static bool TryGetExtension<TValue>(this FileOptions options, Extension<FileOptions, TValue> extension,out TValue value)
        {
            value = (options ?? defaultFileOptions).GetExtension(extension);
            return options != null;
        }

        public static bool TryGetExtension<TValue>(this EnumOptions options, Extension<EnumOptions, TValue> extension, out TValue value)
        {
            value = (options ?? defaultEnumOptions).GetExtension(extension);
            return options != null;
        }
        public static TValue GetExtOrDefault<TValue>(this FieldOptions options, Extension<FieldOptions, TValue> extension)
        {
            return (options ?? defaultFieldOptions).GetExtension(extension);
        }

        public static bool TryGetExtension<TValue>(this FieldOptions options, Extension<FieldOptions, TValue> extension, out TValue value)
        {
            value = options.GetExtOrDefault(extension);
            return options != null;
        }
    }
}
