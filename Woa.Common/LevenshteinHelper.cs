namespace Woa.Common;

public class LevenshteinHelper
{
	/// <summary>
	/// 取最小的一位数
	/// </summary>
	/// <param name="first"></param>
	/// <param name="second"></param>
	/// <param name="third"></param>
	/// <returns></returns>
	private static int LowerOfThree(int first, int second, int third)
	{
		var min = Math.Min(first, second);
		return Math.Min(min, third);
	}

	/// <summary>
	/// 计算字符串相似度
	/// </summary>
	/// <param name="first"></param>
	/// <param name="second"></param>
	/// <returns></returns>
	public static decimal CalculateSimilarity(string first, string second)
	{
		var n = first.Length;
		var m = second.Length;

		int i;
		int j;
		if (n == 0)
		{
			return m;
		}

		if (m == 0)
		{
			return n;
		}

		var matrix = new int[n + 1, m + 1];

		for (i = 0; i <= n; i++)
		{
			//初始化第一列
			matrix[i, 0] = i;
		}

		for (j = 0; j <= m; j++)
		{
			//初始化第一行
			matrix[0, j] = j;
		}

		for (i = 1; i <= n; i++)
		{
			var ch1 = first[i - 1];
			for (j = 1; j <= m; j++)
			{
				var ch2 = second[j - 1];
				var temp = ch1.Equals(ch2) ? 0 : 1;
				matrix[i, j] = LowerOfThree(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1, matrix[i - 1, j - 1] + temp);
			}
		}

		var val = matrix[n, m];
		return 1 - (decimal)val / Math.Max(first.Length, second.Length);
	}
}