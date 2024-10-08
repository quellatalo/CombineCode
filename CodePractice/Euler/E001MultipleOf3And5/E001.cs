using System.Globalization;
using System.Text;
using Computing.Extensions;

namespace CodePractice.Euler.E001MultipleOf3And5;

class E001 : ISolution
{
    public const int MaxN = 1_000_000_000;
    public const int MaxT = 100_000;
    public const int Min = 1;

    public static long MultipleOf3Or5(int upToExclusive)
    {
        int largest = upToExclusive - 1;
        return 3L.SumOfArithmeticSeries(3, largest / 3) +
               5L.SumOfArithmeticSeries(5, largest / 5) -
               15L.SumOfArithmeticSeries(15, largest / 15);
    }

    public void Solve()
    {
        StringBuilder results = new();

        // up to 100,000
        int t = Convert.ToInt32(Console.ReadLine(), CultureInfo.InvariantCulture);
        for (int i = 0; i < t; i++)
        {
            // up to 1,000,000,000
            int n = Convert.ToInt32(Console.ReadLine(), CultureInfo.InvariantCulture);
            results.AppendLine(MultipleOf3Or5(n).ToString(CultureInfo.InvariantCulture));
        }

        Console.WriteLine(results.ToString());
    }
}
