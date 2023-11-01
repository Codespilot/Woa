namespace Woa.Common;

internal static class IdentifierGenerator
{
    private static readonly string[] _chars =
    {
        "0", "1", "2", "3", "4", "5", "6", "7", "8", "9",
        "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z",
        "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"
    };

    public static string Generate()
    {
        return Generate(DateTime.UtcNow.Ticks);
    }

    public static string Generate(long seed)
    {
        var key = GenerateKey();

        return Mixup(key, seed);
    }

    public static string GenerateRandomNumericCode(int length = 6)
    {
        var maximum = Math.Pow(10, length);

        while (true)
        {
            var guid = Guid.NewGuid();
            var hashCode = guid.GetHashCode();
            hashCode = Math.Abs(hashCode);
            if (hashCode < maximum)
            {
                return hashCode.ToString($"D{length}");
            }
        }
    }

    public static string GenerateSequentialNumericCode()
    {
        var number = (DateTime.UtcNow.Ticks - DateTime.Parse("2020-01-01").Ticks) / 100000;
        return $"{number}";
    }

    private static string GenerateKey()
    {
        var seek = unchecked((int)DateTime.UtcNow.Ticks);

        var random = new Random(seek);

        for (var i = 0; i < 100000; i++)
        {
            var number = random.Next(1, _chars.Length);
            var first = _chars[0];
            _chars[0] = _chars[number - 1];
            _chars[number - 1] = first;
        }

        return string.Join(string.Empty, _chars);
    }

    private static string Convert(string key, long value)
    {
        if (value < 62)
        {
            return key[(int)value].ToString();
        }

        var y = (int)(value % 62);
        var x = value / 62;
        return Convert(key, x) + key[y];
    }

    private static string Mixup(string key, long value)
    {
        var sequence = Convert(key, value);
        var salt = sequence.Aggregate(0, (current, seq) => current + seq);

        var x = salt % sequence.Length;

        var original = sequence.ToCharArray();
        var source = new char[original.Length];
        Array.Copy(original, x, source, 0, sequence.Length - x);
        Array.Copy(original, 0, source, sequence.Length - x, x);
        return source.Aggregate(string.Empty, ((current, @char) => current + @char));
    }
}