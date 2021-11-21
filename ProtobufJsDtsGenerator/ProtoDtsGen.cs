using Google.Protobuf.Reflection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoDescriptorHelper;

public class ProtoDtsGen
{
    public StringBuilder builder = new StringBuilder();

    public string curNamespace => NamespaceStack[NamespaceStack.Count - 1];
    public string curIndent => IndentStrs[Indent];

    public List<string> NamespaceStack = new List<string>() { "" };


    public int Indent = 0;

    public static string[] IndentStrs;

    static ProtoDtsGen()
    {
        IndentStrs = new string[16];
        for (int i = 0; i < 16; i++)
        {
            IndentStrs[i] = new string(' ', i * 4);
        }
    }

    public static class XlsxDef
    {
    }

    public int Gen(ProtoDescriptorHelper.Resolver pdr, string dts)
    {
        GenHeader();

        foreach (var fd in pdr.FileDescriptors)
        {
            if (fd.EnumTypes.Count > 0)
            {
                foreach (var nestedEnum in fd.EnumTypes)
                {
                    SetNameSpace(fd.Package);
                    GenEnum(nestedEnum);
                }
            }

            if (fd.MessageTypes.Count > 0)
            {
                foreach (var nestedMd in fd.MessageTypes)
                {
                    SetNameSpace(fd.Package);
                    GenMessage(nestedMd);
                }
            }
        }
        while (NamespaceStack.Count > 1) PopNamespace();
        File.WriteAllText(dts, builder.ToString());
        return 0;
    }

    void SetNameSpace(string nameSpace)
    {
        if (nameSpace == curNamespace) return;
        List<string> arrNamespace = new List<string>();
        for (string tmpNamespace = nameSpace; !string.IsNullOrEmpty(tmpNamespace);)
        {
            arrNamespace.Add(tmpNamespace);
            int lastIndex = tmpNamespace.LastIndexOf('.');
            if (lastIndex >= 0)
            {
                tmpNamespace = tmpNamespace.Substring(0, lastIndex);
            }
            else
            {
                break;
            }
        }
        // match the index of name space
        int matchedNamesapceIndex = 1;
        for (; matchedNamesapceIndex < NamespaceStack.Count; matchedNamesapceIndex++)
        {
            if (arrNamespace.Count - matchedNamesapceIndex < 0 ||
                NamespaceStack[matchedNamesapceIndex] != arrNamespace[arrNamespace.Count - matchedNamesapceIndex])
            {
                break;
            }
        }
        // remove un-matched
        for (int i = NamespaceStack.Count; i > matchedNamesapceIndex; i--)
        {
            PopNamespace();
        }

        // set namespace
        for (int i = arrNamespace.Count - matchedNamesapceIndex; i >= 0; i--)
        {
            PushNamespace(arrNamespace[i]);
        }
    }

    StringBuilder Export(string itemPrefix = null, string itemName = null)
    {
        //     export somethings
        builder.Append(curIndent)
            .Append(string.IsNullOrEmpty(curNamespace) ? "export " : "");
        if (itemPrefix != null)
        {
            // namespace name { 
            return builder.Append(itemPrefix)
                .Append(' ')
                .Append(itemName).Append(" {\n");
        }

        return builder;
    }

    void PushNamespace(string ns)
    {
        if (string.IsNullOrEmpty(ns))
        {
            Debug.Warn("Empty Namespace Skiped.");
        }
        Export().Append("namespace ");
        var dotIndex = ns.LastIndexOf('.');
        if (dotIndex > 0)
            builder.Append(ns, dotIndex + 1, ns.Length - dotIndex - 1);
        else
            builder.Append(ns);

        builder.Append(" {\n");
        NamespaceStack.Add(ns);
        Indent++;
    }
    void PopNamespace()
    {
        if (NamespaceStack.Count == 1) throw new InvalidOperationException("try pop the root namespace");
        NamespaceStack.RemoveAt(NamespaceStack.Count - 1);

        Indent--;
        builder.Append(curIndent).AppendLine("}");
    }

    void GenHeader()
    {
        builder.AppendLine(@"import * as $protobuf from ""protobufjs/minimal"";");

    }

    void GenFields(MessageDescriptor md, bool isInterface)
    {
        foreach (var field in md.Fields.InDeclarationOrder())
        {
            var realField = field;
            var optinal = isInterface || (!field.IsRepeated && field.FieldType != FieldType.Enum);
            // decorateType?: ;
            builder.Append(curIndent)
                .Append(field.Name)
                .Append(optinal ? "?: " : ": ");

            bool isMap = field.IsMap;
            if (isMap)
            {
                builder.Append($"{{ [k: string]:");
                // value
                realField = field.MessageType.Fields.InDeclarationOrder()[1];
            }
            var tsType = realField.ToTsType();

            if (realField.FieldType == FieldType.Message)
            {
                builder.Append(tsType, 0, tsType.Length - realField.MessageType.Name.Length)
                    .Append("I");
                tsType = realField.MessageType.Name;
            }
            builder.Append(tsType)
                .Append(field.IsRepeated && !isMap ? "[]" : "")
                .Append(isMap ? "}" : "")
                .Append(";\n");
        }

        // Oneof
        if (!isInterface)
        {
            foreach (var oneof in md.Oneofs)
            {
                builder.Append(curIndent)
                    .Append(oneof.Name)
                    .Append("?: (");
                foreach (var oneofField in oneof.Fields)
                {
                    builder.Append('"')
                        .Append(oneofField.Name)
                        .Append("\"|");
                }
                builder.Remove(builder.Length - 1, 1);
                builder.AppendLine(");");
            }
        }
    }

    void GenInterface(MessageDescriptor md)
    {
        Export("interface", $"I{md.Name}");
        Indent++;
        GenFields(md, true);
        AppendBraceEnd();
    }

    StringBuilder AppendNamespace(string fullName, string name)
    {
        var count = fullName.Length > name.Length ? fullName.Length - name.Length : 0;
        return builder.Append(fullName, 0, count);
    }

    void GenMessage(MessageDescriptor md)
    {
        GenInterface(md);
        Export("class", $"{md.Name} implements I{md.Name}");
        Indent++;
        GenFields(md, false);
        // gen Method
        builder
            .Append(curIndent).AppendLine($"constructor(p?: I{md.Name});")
            .Append(curIndent).AppendLine($"public static create(properties?: I{md.Name}): {md.Name};")
            .Append(curIndent).AppendLine($"public static encode(m: I{md.Name}, w?: $protobuf.Writer): $protobuf.Writer;")
            .Append(curIndent).AppendLine($"public static encodeDelimited(m: I{md.Name}, w?: $protobuf.Writer): $protobuf.Writer;")
            .Append(curIndent).AppendLine($"public static decode(r: ($protobuf.Reader|Uint8Array), l?: number): {md.Name};")
            .Append(curIndent).AppendLine($"public static verify(m: {{ [k: string]: any }}): (string|null);")
            .Append(curIndent).AppendLine($"public static fromObject(d: {{ [k: string]: any }}): PairStrStr;")
            .Append(curIndent).AppendLine($"public static toObject(m: PairStrStr, o?: $protobuf.IConversionOptions): {{ [k: string]: any }};")
            .Append(curIndent).AppendLine($"public toJSON(): {{ [k: string]: any }};");

        AppendBraceEnd();
        if (md.NestedTypes.Count > 0)
        {
            foreach (var nestedMd in md.NestedTypes)
            {
                if (!IsSpecialType(nestedMd))
                //&& string.Compare(curNamespace, 0, nestedMd.FullName, 0, nestedMd.FullName.Length - nestedMd.Name.Length - 1) == 0)
                {

                    SetNameSpace(md.FullName);
                    GenMessage(nestedMd);
                }
            }
        }

        if (md.EnumTypes.Count > 0)
        {
            foreach (var nestedEnum in md.EnumTypes)
            {
                SetNameSpace(md.FullName);
                GenEnum(nestedEnum);
            }
        }
    }

    void GenEnum(EnumDescriptor ed)
    {
        Export("enum", ed.Name);
        Indent++;
        foreach (var item in ed.Values)
        {
            builder.Append(curIndent)
                .Append(Util.GetEnumValueName(ed.Name, item.Name))
                .Append(" = ")
                .Append(item.Number)
                .Append(",\n");
        }
        AppendBraceEnd();
    }

    bool IsSpecialType(MessageDescriptor md)
    {
        return md.GetOptions()?.MapEntry ?? false;
    }
    void AppendBraceEnd()
    {
        Indent--;
        builder.Append(curIndent).Append("}\n");
    }
}
