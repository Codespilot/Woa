namespace Woa.Common;

public static class CaptchaHelper
{
    public static string Generate(int length = 6)
    {
        if (length < 3)
        {
            throw new ArgumentException("字符串长度不能小于3");
        }

        var code = string.Empty;

        var random = new Random();

        while (code.Length < length)
        {
            var index = random.Next(0, Characters.Numbers.Length - 1);
            code += Characters.Numbers[index];
        }

        return code;
    }
}