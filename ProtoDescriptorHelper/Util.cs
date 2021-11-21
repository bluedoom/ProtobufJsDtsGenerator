using Google.Protobuf;
using Google.Protobuf.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PBType = Google.Protobuf.Reflection.FieldDescriptorProto.Types.Type;

namespace ProtoDescriptorHelper
{
    public static class Util
    {
        private static readonly Dictionary<System.Type, WireFormat.WireType> Codecs = new Dictionary<System.Type, WireFormat.WireType>
            {
                { typeof(bool), WireFormat.WireType.Varint },
                { typeof(int), WireFormat.WireType.Varint },
                { typeof(long),  WireFormat.WireType.Varint },
                { typeof(uint), WireFormat.WireType.Varint },
                { typeof(ulong),  WireFormat.WireType.Varint },
                { typeof(float), WireFormat.WireType.Fixed32},
                { typeof(double), WireFormat.WireType.Fixed64},
                { typeof(string),  WireFormat.WireType.LengthDelimited},
                { typeof(ByteString), WireFormat.WireType.LengthDelimited}
            };
        public static WireFormat.WireType GetWireType(PBType type)
        {
            return type switch
            {
                PBType.Int64 => WireFormat.WireType.Varint,
                PBType.Uint64 => WireFormat.WireType.Varint,
                PBType.Int32 => WireFormat.WireType.Varint,
                PBType.Uint32 => WireFormat.WireType.Varint,
                PBType.Bool => WireFormat.WireType.Varint,
                PBType.Sint32 => WireFormat.WireType.Varint,
                PBType.Sint64 => WireFormat.WireType.Varint,
                PBType.Double => WireFormat.WireType.Fixed64,
                PBType.Float => WireFormat.WireType.Fixed32,
                PBType.Fixed64 => WireFormat.WireType.Fixed64,
                PBType.Fixed32 => WireFormat.WireType.Fixed32,
                PBType.Sfixed32 => WireFormat.WireType.Fixed32,
                PBType.Sfixed64 => WireFormat.WireType.Fixed64,
                PBType.String => WireFormat.WireType.LengthDelimited,
                //PBType.Group => ,
                //PBType.Message => ,
                PBType.Bytes => WireFormat.WireType.LengthDelimited,
                //PBType.Enum => ,
                _ => throw new NotImplementedException(type.ToString()),
            };
        }

        public static object CreateFieldCodec(PBType type, int fieldNumber)
        {
            return type switch
            {
                PBType.Double => FieldCodec.ForDouble(WireFormat.MakeTag(fieldNumber, GetWireType(type))),
                PBType.Float => FieldCodec.ForFloat(WireFormat.MakeTag(fieldNumber, GetWireType(type))),
                PBType.Int64 => FieldCodec.ForInt64(WireFormat.MakeTag(fieldNumber, GetWireType(type))),
                PBType.Uint64 => FieldCodec.ForUInt64(WireFormat.MakeTag(fieldNumber, GetWireType(type))),
                PBType.Int32 => FieldCodec.ForInt32(WireFormat.MakeTag(fieldNumber, GetWireType(type))),
                PBType.Fixed64 => FieldCodec.ForFixed64(WireFormat.MakeTag(fieldNumber, GetWireType(type))),
                PBType.Fixed32 => FieldCodec.ForFixed32(WireFormat.MakeTag(fieldNumber, GetWireType(type))),
                PBType.Bool => FieldCodec.ForBool(WireFormat.MakeTag(fieldNumber, GetWireType(type))),
                PBType.String => FieldCodec.ForString(WireFormat.MakeTag(fieldNumber, GetWireType(type))),
                //PBType.Group => FieldCodec.ForGroup(WireFormat.MakeTag(fieldNumber, GetWireType(type))),
                //PBType.Message => FieldCodec.ForMessage(WireFormat.MakeTag(fieldNumber, GetWireType(type))),
                PBType.Bytes => FieldCodec.ForBytes(WireFormat.MakeTag(fieldNumber, GetWireType(type))),
                PBType.Uint32 => FieldCodec.ForUInt32(WireFormat.MakeTag(fieldNumber, GetWireType(type))),
                //PBType.Enum => FieldCodec.ForEnum(WireFormat.MakeTag(fieldNumber, GetWireType(type))),
                PBType.Sfixed32 => FieldCodec.ForSFixed32(WireFormat.MakeTag(fieldNumber, GetWireType(type))),
                PBType.Sfixed64 => FieldCodec.ForSFixed64(WireFormat.MakeTag(fieldNumber, GetWireType(type))),
                PBType.Sint32 => FieldCodec.ForSInt32(WireFormat.MakeTag(fieldNumber, GetWireType(type))),
                PBType.Sint64 => FieldCodec.ForSInt64(WireFormat.MakeTag(fieldNumber, GetWireType(type))),
                _ => throw new NotImplementedException(type.ToString()),
            };
        }

        internal static Type ToCsType(PBType type) => type switch
        {
            PBType.Float => typeof(float),
            PBType.Bool => typeof(bool),
            PBType.Fixed32 => typeof(UInt32),
            PBType.Uint32 => typeof(UInt32),
            PBType.Fixed64 => typeof(UInt64),
            PBType.Uint64 => typeof(UInt64),
            PBType.Double => typeof(Double),
            PBType.Int64 => typeof(Int64),
            PBType.Int32 => typeof(Int32),
            PBType.String => typeof(String),
            //PBType.Group => typeof(Group),
            //PBType.Message => typeof(Message),
            PBType.Bytes => typeof(byte[]),
            PBType.Enum => typeof(Enum),
            PBType.Sfixed32 => typeof(int),
            PBType.Sfixed64 => typeof(long),
            PBType.Sint64 => typeof(long),
            PBType.Sint32 => typeof(int),
            _ => throw new NotImplementedException(type.ToString()),
        };
        public static string ToTsType(this FieldDescriptor fd, out bool IsPrimitive) => ToTsType(fd.FieldType, fd, out IsPrimitive);
        public static string ToTsType(this FieldDescriptor fd) => ToTsType(fd.FieldType, fd, out _);


        internal static string ToTsType(FieldType type, FieldDescriptor fd, out bool IsPrimitive)
        {
            IsPrimitive = true;
            switch (type)
            {
                case FieldType.Double:
                case FieldType.Float:
                case FieldType.Int32:
                case FieldType.Fixed32:
                case FieldType.SFixed32:
                case FieldType.SInt32:
                case FieldType.UInt32: return "number";

                case FieldType.SFixed64:
                case FieldType.Fixed64:
                case FieldType.Int64:
                case FieldType.SInt64:
                case FieldType.UInt64: return "BigInt";

                case FieldType.Bool: return "boolean";
                case FieldType.String: return "string";

                case FieldType.Bytes: return "Uint8Array";
                default: break;
            };
            IsPrimitive = false;
            if (fd != null)
            {
                switch (fd.FieldType)
                {
                    case FieldType.Message: return fd.MessageType.FullName;
                    case FieldType.Enum: return fd.EnumType.FullName;
                    default: break;
                };
            }
            throw new NotImplementedException(type.ToString());
        }


        // Attempt to remove a prefix from a value, ignoring casing and skipping underscores.
        // (foo, foo_bar) => bar - underscore after prefix is skipped
        // (FOO, foo_bar) => bar - casing is ignored
        // (foo_bar, foobarbaz) => baz - underscore in prefix is ignored
        // (foobar, foo_barbaz) => baz - underscore in value is ignored
        // (foo, bar) => bar - prefix isn't matched; return original value
        public static unsafe string TryRemovePrefix(string prefix, string value)
        {
            var prefixLen = prefix.Length;
            var valueLen = value.Length;
            var prefix_to_match = stackalloc char[value.Length + prefix.Length];
            var prefixToMatchLen = 0;

            // First normalize to a lower-case no-underscores prefix to match against
            for (int i = 0; i < prefixLen; i++)
            {
                if (prefix[i] != '_')
                {
                    prefix_to_match[prefixToMatchLen++] = (char.ToLower(prefix[i]));
                }
            }

            // This keeps track of how much of value we've consumed
            int prefix_index, value_index;
            for (prefix_index = 0, value_index = 0;
                prefix_index < prefixToMatchLen && value_index < valueLen;
                value_index++)
            {
                // Skip over underscores in the value
                if (value[value_index] == '_')
                {
                    continue;
                }
                if (char.ToLower(value[value_index]) != prefix_to_match[prefix_index++])
                {
                    // Failed to match the prefix - bail out early.
                    return value;
                }
            }

            // If we didn't finish looking through the prefix, we can't strip it.
            if (prefix_index < prefixToMatchLen)
            {
                return value;
            }

            // Step over any underscores after the prefix
            while (value_index < valueLen && value[value_index] == '_')
            {
                value_index++;
            }

            // If there's nothing left (e.g. it was a prefix with only underscores afterwards), don't strip.
            if (value_index == valueLen)
            {
                return value;
            }
            return value.Substring(value_index);

        }
        public static string GetEnumValueName(string enum_name, string enum_value_name)
        {
            var stripped = TryRemovePrefix(enum_name, enum_value_name);
            var result = ShoutyToPascalCase(stripped);
            // Just in case we have an enum name of FOO and a value of FOO_2... make sure the returned
            // string is a valid identifier.
            if (char.IsDigit(result[0]))
            {
                result = "_" + result;
            }
            return result;
        }


        // Convert a string which is expected to be SHOUTY_CASE (but may not be *precisely* shouty)
        // into a PascalCase string. Precise rules implemented:

        // Previous input character      Current character         Case
        // Any                           Non-alphanumeric          Skipped
        // None - first char of input    Alphanumeric              Upper
        // Non-letter (e.g. _ or 1)      Alphanumeric              Upper
        // Numeric                       Alphanumeric              Upper
        // Lower letter                  Alphanumeric              Same as current
        // Upper letter                  Alphanumeric              Lower
        public static unsafe string ShoutyToPascalCase(string input)
        {
            var result = stackalloc char[input.Length];
            int pos = 0;
            // Simple way of implementing "always start with upper"
            char previous = '_';
            for (int i = 0; i < input.Length; i++)
            {
                char current = input[i];
                if (!char.IsLetterOrDigit(current))
                {
                    previous = current;
                    continue;
                }
                if (!char.IsLetterOrDigit(previous))
                {
                    result[pos++] = (char.ToUpper(current));
                }
                else if (char.IsDigit(previous))
                {
                    result[pos++] = (char.ToUpper(current));
                }
                else if (char.IsLower(previous))
                {
                    result[pos++] = (current);
                }
                else
                {
                    result[pos++] = (char.ToLower(current));
                }
                previous = current;
            }
            return new string(result, 0, pos);
        }

    }

}
