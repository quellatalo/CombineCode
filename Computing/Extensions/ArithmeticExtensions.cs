using System.Numerics;

namespace Computing.Extensions;

/// <summary>
/// Common computations.
/// </summary>
public static class ArithmeticExtensions
{
    /// <summary>
    /// Calculates the sum of an arithmetic sequence.
    /// </summary>
    /// <param name="firstNumber">The first term.</param>
    /// <param name="difference">The common difference.</param>
    /// <param name="numberOfSeries">The number of series.</param>
    /// <typeparam name="T">Number type.</typeparam>
    /// <returns>The sum of the series.</returns>
    /// <remarks>https://www.mathsisfun.com/algebra/sequences-sums-arithmetic.html.</remarks>
    public static T SumOfArithmeticSeries<T>(this T firstNumber, T difference, T numberOfSeries)
        where T : IBinaryInteger<T>
        => (numberOfSeries * ((firstNumber << 1) + ((numberOfSeries - T.One) * difference))) >> 1;
}
