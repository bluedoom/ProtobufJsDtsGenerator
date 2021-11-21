using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Google.Protobuf.Reflection;
using System.Linq;
using Google.Protobuf;
using System.Reflection;
using PBType = Google.Protobuf.Reflection.FieldDescriptorProto.Types.Type;
using ProtoDescriptorHelper.Options;

namespace ProtoDescriptorHelper
{
    public class Resolver
    {
        /// <summary>
        /// All FileDescriptors
        /// </summary>
        public IReadOnlyList<FileDescriptor> FileDescriptors { get; private set; }

        public ExtensionRegistry ExtensionReg = new ExtensionRegistry();

        public Dictionary<string, Extension> Extensions = new Dictionary<string, Extension>();
        

        public void Init(IEnumerable<byte[]> files,out List<ByteString> fileRaws)
        {
            fileRaws = new List<ByteString>();
            var parser = FileDescriptorSet.Parser;
            Debug.Info($"####Start Extension Create####");
            foreach (var file in files)
            {
                foreach (var d in parser.ParseFrom(file).File)
                {
                    string packageName = string.IsNullOrEmpty(d.Package) ? string.Empty : d.Package + '.';
                    fileRaws.Add(d.ToByteString());
                    foreach (var e in d.Extension)
                    {
                        var extension = CreateExtension(e);
                        var fullName = packageName + e.Name;
                        ExtensionReg.Add(extension);
                        Extensions.Add(fullName, extension);
                        Debug.Info("\t"+fullName);
                    }
                }
            }
            Debug.Info("####End Extension Create####");
        }
        Extension CreateExtension(FieldDescriptorProto fd)
        {
            var extendeeType = OptionsUtil.GetType(fd.Extendee);
            var valueType = Util.ToCsType(fd.Type);

            var eType = typeof(Extension<,>).MakeGenericType(extendeeType, valueType);

            return Activator.CreateInstance(eType, fd.Number, Util.CreateFieldCodec(fd.Type,fd.Number)) as Extension;

        }

        /// <summary>
        /// Get CustomOptionType by FullName
        /// </summary>
        /// <typeparam name="TTarget">extension target, like MessageOptions, FieldOptions...</typeparam>
        /// <typeparam name="ValueType">extension type, like bool, string...</typeparam>
        /// <param name="fullName">full name of extension</param>
        /// <returns>optionsType</returns>
        public Extension<TTarget, ValueType> GetExtensionDescriptor<TTarget, ValueType>(string fullName) where TTarget : IExtendableMessage<TTarget>
        {
            Extension<TTarget, ValueType> ret = null;
            if (Extensions.TryGetValue(fullName, out var ex))
            {
                ret = ex as Extension<TTarget, ValueType>;
            }
            if (ret == null)
            {
                throw new NotSupportedException($"Can't Find Extension {fullName}");
            }
            return ret;
        }

        public Resolver(params string[] paths)
        {
            List<byte[]> files = paths.Select(s => File.ReadAllBytes(s)).ToList();
            Init(files,out var raws);

            var ppt = DescriptorReflection.Descriptor.GetType().GetProperty("Proto", BindingFlags.NonPublic | BindingFlags.Instance);
            var bs = (ppt.GetValue(DescriptorReflection.Descriptor) as FileDescriptorProto).ToByteString();
            
            raws.Insert(0,bs);
            FileDescriptors = FileDescriptor.BuildFromByteStrings(raws, ExtensionReg);
        }
    }

}