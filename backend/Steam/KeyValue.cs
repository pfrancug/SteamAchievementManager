namespace SAM.Backend.Steam;

internal enum KeyValueType : byte
{
    None = 0,
    String = 1,
    Int32 = 2,
    Float32 = 3,
    Pointer = 4,
    WideString = 5,
    Color = 6,
    UInt64 = 7,
    End = 8,
}

internal class KeyValue
{
    private static readonly KeyValue _invalid = new();
    public string Name = "<root>";
    public KeyValueType Type = KeyValueType.None;
    public object? Value;
    public bool Valid;
    public List<KeyValue>? Children;
    private Dictionary<string, KeyValue>? _childrenLookup;

    public KeyValue this[string key]
    {
        get
        {
            if (Children is null)
            {
                return _invalid;
            }
            if (_childrenLookup is null)
            {
                _childrenLookup = new Dictionary<string, KeyValue>(
                    StringComparer.InvariantCultureIgnoreCase
                );
                foreach (var child in Children)
                {
                    _childrenLookup.TryAdd(child.Name, child);
                }
            }
            return _childrenLookup.TryGetValue(key, out var result) ? result : _invalid;
        }
    }

    public string AsString(string defaultValue)
    {
        if (!Valid || Value is null)
        {
            return defaultValue;
        }
        return Value.ToString() ?? defaultValue;
    }

    public int AsInteger(int defaultValue)
    {
        if (!Valid)
        {
            return defaultValue;
        }
        return Type switch
        {
            KeyValueType.String or KeyValueType.WideString => int.TryParse(
                (string)Value!,
                out int v
            )
                ? v
                : defaultValue,
            KeyValueType.Int32 => (int)Value!,
            KeyValueType.Float32 => (int)(float)Value!,
            KeyValueType.UInt64 => (int)((ulong)Value! & 0xFFFFFFFF),
            _ => defaultValue,
        };
    }

    public float AsFloat(float defaultValue)
    {
        if (!Valid)
        {
            return defaultValue;
        }
        return Type switch
        {
            KeyValueType.String or KeyValueType.WideString => float.TryParse(
                (string)Value!,
                out float v
            )
                ? v
                : defaultValue,
            KeyValueType.Int32 => (int)Value!,
            KeyValueType.Float32 => (float)Value!,
            KeyValueType.UInt64 => (ulong)Value! & 0xFFFFFFFF,
            _ => defaultValue,
        };
    }

    public bool AsBoolean(bool defaultValue)
    {
        if (!Valid)
        {
            return defaultValue;
        }
        return Type switch
        {
            KeyValueType.String or KeyValueType.WideString => int.TryParse(
                (string)Value!,
                out int v
            )
                ? v != 0
                : defaultValue,
            KeyValueType.Int32 => (int)Value! != 0,
            KeyValueType.Float32 => (int)(float)Value! != 0,
            KeyValueType.UInt64 => (ulong)Value! != 0,
            _ => defaultValue,
        };
    }

    public static KeyValue? LoadAsBinary(string path)
    {
        if (!File.Exists(path))
        {
            return null;
        }
        try
        {
            using var input = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            KeyValue kv = new();
            if (!kv.ReadAsBinary(input))
            {
                return null;
            }
            return kv;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public bool ReadAsBinary(Stream input)
    {
        Children = [];
        try
        {
            while (true)
            {
                var type = (KeyValueType)input.ReadValueU8();
                if (type == KeyValueType.End)
                {
                    break;
                }

                KeyValue current = new() { Type = type, Name = input.ReadStringUnicode() };
                switch (type)
                {
                    case KeyValueType.None:
                        current.ReadAsBinary(input);
                        break;
                    case KeyValueType.String:
                        current.Valid = true;
                        current.Value = input.ReadStringUnicode();
                        break;
                    case KeyValueType.WideString:
                        current.Valid = true;
                        current.Value = input.ReadStringWide();
                        break;
                    case KeyValueType.Int32:
                        current.Valid = true;
                        current.Value = input.ReadValueS32();
                        break;
                    case KeyValueType.UInt64:
                        current.Valid = true;
                        current.Value = input.ReadValueU64();
                        break;
                    case KeyValueType.Float32:
                        current.Valid = true;
                        current.Value = input.ReadValueF32();
                        break;
                    case KeyValueType.Color:
                    case KeyValueType.Pointer:
                        current.Valid = true;
                        current.Value = input.ReadValueU32();
                        break;
                    default:
                        throw new FormatException($"Unknown KeyValue type: {type}");
                }

                if (input.Position >= input.Length)
                {
                    throw new FormatException("Unexpected end of stream");
                }

                Children.Add(current);
            }
            Valid = true;
            return input.Position == input.Length;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
