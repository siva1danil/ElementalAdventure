using System.Diagnostics;
using System.Text.RegularExpressions;

using OpenTK.Graphics.OpenGL4;

namespace ElementalAdventure.Client.Core.Resources.Data;

public class DataLayout {
    private static readonly Regex VertexDataRegex = new(@"layout\s*?(?:.+?)?\s*?in\s+?(u?int|float|[ui]?vec[234]|mat[234])\s+?(aGlobal.+?);", RegexOptions.Compiled);
    private static readonly Regex InstanceDataRegex = new(@"layout\s*?(?:.+?)?\s*?in\s+?(u?int|float|[ui]?vec[234]|mat[234])\s+?(aInstance.+?);", RegexOptions.Compiled);
    private static readonly Regex UniformBlockRegex = new(@"layout\s*?\(\s*?std140\s*?\)\s*?uniform\s+?(?:.+?)\s*?{(.*?)}\s*?;", RegexOptions.Compiled | RegexOptions.Singleline);
    private static readonly Regex UniformFieldRegex = new(@"(u?int|float|[ui]?vec[234]|mat[234])\s+?(u.+?);", RegexOptions.Compiled);
    private static readonly Dictionary<string, (VertexAttribPointerType Type, int Size)> TypeMap = new() {
            { "int", (VertexAttribPointerType.Int, 1) }, { "uint", (VertexAttribPointerType.UnsignedInt, 1) },
            { "float", (VertexAttribPointerType.Float, 1) },
            { "vec2", (VertexAttribPointerType.Float, 2) }, { "ivec2", (VertexAttribPointerType.Float, 2) }, { "uvec2", (VertexAttribPointerType.Float, 2) },
            { "vec3", (VertexAttribPointerType.Float, 3) }, { "ivec3", (VertexAttribPointerType.Float, 3) }, { "uvec3", (VertexAttribPointerType.Float, 3) },
            { "vec4", (VertexAttribPointerType.Float, 4) }, { "ivec4", (VertexAttribPointerType.Float, 4) }, { "uvec4", (VertexAttribPointerType.Float, 4) },
            { "mat2", (VertexAttribPointerType.Float, 2 * 2) }, { "mat3", (VertexAttribPointerType.Float, 3 * 3) }, { "mat4", (VertexAttribPointerType.Float, 4 * 4) }
        };
    private static readonly Dictionary<string, (int Size, int Alignment)> Std140Map = new() {
            { "int", (4, 4) }, { "uint", (4, 4) },
            { "float", (4, 4) },
            { "vec2", (8, 8) }, { "ivec2", (8, 8) }, { "uvec2", (8, 8) },
            { "vec3", (12, 16) }, { "ivec3", (12, 16) }, { "uvec3", (12, 16) },
            { "vec4", (16, 16) }, { "ivec4", (16, 16) }, { "uvec4", (16, 16) },
            { "mat2", (32, 16) }, { "mat3", (48, 16) }, { "mat4", (64, 16) }
        };

    private readonly Entry[] _vertexData, _instanceData, _uniformData;
    private readonly int _vertexDataSize, _instanceDataSize, _uniformDataSize;

    public Entry[] VertexData => _vertexData;
    public Entry[] InstanceData => _instanceData;
    public Entry[] UniformData => _uniformData;
    public int VertexDataSize => _vertexDataSize;
    public int InstanceDataSize => _instanceDataSize;
    public int UniformDataSize => _uniformDataSize;

    public DataLayout(string vert) {
        List<Entry> vertexData = [], instanceData = [], uniformData = [];
        int vertexDataSize = 0, instanceDataSize = 0, uniformDataSize = 0;

        foreach (Match match in VertexDataRegex.Matches(vert)) {
            string type = match.Groups[1].Value, name = match.Groups[2].Value.Trim();
            if (!TypeMap.ContainsKey(type))
                throw new FormatException($"Unknown type '{type}' in vertex data layout.");
            vertexData.Add(new(name, TypeMap[type].Type, TypeMap[type].Size, vertexDataSize));
            vertexDataSize += TypeMap[type].Size * 4;
        }

        foreach (Match match in InstanceDataRegex.Matches(vert)) {
            string type = match.Groups[1].Value, name = match.Groups[2].Value.Trim();
            if (!TypeMap.ContainsKey(type))
                throw new FormatException($"Unknown type '{type}' in instance data layout.");
            instanceData.Add(new(name, TypeMap[type].Type, TypeMap[type].Size, instanceDataSize));
            instanceDataSize += TypeMap[type].Size * 4;
        }

        foreach (Match blockMatch in UniformBlockRegex.Matches(vert)) {
            string block = blockMatch.Groups[1].Value;
            foreach (Match fieldMatch in UniformFieldRegex.Matches(block)) {
                string type = fieldMatch.Groups[1].Value, name = fieldMatch.Groups[2].Value.Trim();
                if (!Std140Map.ContainsKey(type))
                    throw new FormatException($"Unknown type '{type}' in uniform data layout.");
                uniformDataSize = (uniformDataSize + Std140Map[type].Alignment - 1) / Std140Map[type].Alignment * Std140Map[type].Alignment;
                uniformData.Add(new(name, TypeMap[type].Type, TypeMap[type].Size, uniformDataSize));
                uniformDataSize += TypeMap[type].Size * 4;
            }
        }
        uniformDataSize = (uniformDataSize + 15) / 16 * 16;

        (_vertexData, _instanceData, _uniformData) = ([.. vertexData], [.. instanceData], [.. uniformData]);
        (_vertexDataSize, _instanceDataSize, _uniformDataSize) = (vertexDataSize, instanceDataSize, uniformDataSize);

        Debug.WriteLine($"Parsed shader layout: VertexData ({_vertexDataSize} bytes): {string.Join(", ", _vertexData.Select(e => $"{e.Name}({e.Type}, {e.Size}, {e.Offset})"))}");
        Debug.WriteLine($"                      InstanceData ({_instanceDataSize} bytes): {string.Join(", ", _instanceData.Select(e => $"{e.Name}({e.Type}, {e.Size}, {e.Offset})"))}");
        Debug.WriteLine($"                      UniformData ({_uniformDataSize} bytes): {string.Join(", ", _uniformData.Select(e => $"{e.Name}({e.Type}, {e.Size}, {e.Offset})"))}");
    }

    public record struct Entry(string Name, VertexAttribPointerType Type, int Size, int Offset);
}