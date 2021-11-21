# 如何使用

1. 使用Google官方Protoc的"-o"将所有.proto文件序列化成一个描述文件。
1. "ProtobufJsDtsGenerator.exe --pbdesc descriptors.pb --dts_out src/gen/pb.d.ts"

# 改动内容
* 64位整数改为了使用BigInt，见： https://github.com/mjgp2/protobuf.js
* Enum参考了C#的生成规则，见：https://github.com/bluedoom/protobuf.js/commit/24336b7b305864a867b2084b692e4021c7cd23d6
*

# 为啥要单独做Dts生成器

* protobufjs的dts生成速度太慢了，而且目前还不兼容新版本的nodejs
