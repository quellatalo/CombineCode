CombineCode
===========

This repository's main content is the [Quellatalo.CombineCode.CSharp](Quellatalo.CombineCode.CSharp) package.

It's available on [NuGet](https://www.nuget.org/packages/Quellatalo.CombineCode.CSharp).

The other two projects [CodePractice](CodePractice) and [Computing](Computing) are examples demonstrated below.

# Usage

```csharp
const string TransformedFile = "Transformed.cs";
CSharpSourceInfo sourceInfo = CSharpSourceInfo.ReadFromFile("Path/To/File.cs");

// combine the targeted source file and its dependencies to one string.
string combinedCode = sourceInfo.CompileToOneSource();

// write the combined code to file
File.WriteAllText(TransformedFile, combinedCode);

// open the newly written file by using the OS's default behavior
FileUtils.OpenFile(TransformedFile);
```

# Example

Imagine we want to challenge [HackerRank's Euler project](https://www.hackerrank.com/contests/projecteuler), starting with [the first problem](https://www.hackerrank.com/contests/projecteuler/challenges/euler001/problem).

Still, we want to code in a standard environment, writing multiple code files with reusable types.

## The written code

First of all, let's create a console project `CodePractice` for online practice code, and write an interface for all problems:

```csharp
namespace CodePractice;

/// <summary>
/// Represents a solution for an online practice problem.
/// </summary>
public interface ISolution
{
    /// <summary>
    /// Solves the problem (by reading from Console.In, then printing the results to Console.Out).
    /// </summary>
    void Solve();
}
```

When identifying the [E001 problem](https://www.hackerrank.com/contests/projecteuler/challenges/euler001/problem), we noticed that an implementation of [SumOfArithmeticSeries](https://www.mathsisfun.com/algebra/sequences-sums-arithmetic.html) would be very helpful.
So, we decided to create a `Computing` library and write such utility method:
```csharp
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
```

Now we solve the problem:
```csharp
using System;
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
```

In `CodePractice` project's `Program.cs`, we can execute the scenario:
```csharp
using CodePractice.Euler.E001MultipleOf3And5;

new E001().Solve();
```

In summary, we have the following code structure:

- (Console) CodePractice
    - Euler
        - E001MultipleOf3And5
            - E001.cs
    - ISolution.cs
    - Program.cs
- (Library) Computing
    - Extensions
        - ArithmeticExtensions.cs

## Submitting to the online editor

Now to submit our solution, we can add nuget reference to this package (`Quellatalo.CombineCode.CSharp`) and change our `Program.cs` to:
```csharp
using System.IO;
using Quellatalo.CombineCode.CSharp;

// The combined source code file to be generated
const string TransformedFile = "Transformed.cs";

// The rewriter to add Solution class and main method required by HackerRank
MainRewriter rewriter = new("Solution", $"new E001().Solve();");

File.WriteAllText(
    TransformedFile,
    CSharpSourceInfo
        .ReadFromFile(
            Path.Combine(
                CSharpSourceInfo.SolutionDirectory.FullName,
                "CodePractice",
                "Euler",
                "E001MultipleOf3And5",
                "E001.cs"))
        .CompileToOneSource(rewriter));

// Open file using the OS's default behavior.
FileUtils.OpenFile(TransformedFile);
```

Executing it will open up a `Transformed.cs` file with the following content:
```csharp
namespace Computing.Extensions
{
    using System;
    using System.Linq;
    using System.Numerics;

    /// <summary>
    /// Common computations.
    /// </summary>
    public static class ArithmeticExtensions
    {
        /// <summary>
        /// Calculates the sum of an arithmetic sequence.
        /// </summary>
        /// <param name = "firstNumber">The first term.</param>
        /// <param name = "difference">The common difference.</param>
        /// <param name = "numberOfSeries">The number of series.</param>
        /// <typeparam name = "T">Number type.</typeparam>
        /// <returns>The sum of the series.</returns>
        /// <remarks>https://www.mathsisfun.com/algebra/sequences-sums-arithmetic.html.</remarks>
        public static T SumOfArithmeticSeries<T>(this T firstNumber, T difference, T numberOfSeries)
            where T : IBinaryInteger<T> => (numberOfSeries * ((firstNumber << 1) + ((numberOfSeries - T.One) * difference))) >> 1;
    }
}
namespace CodePractice
{
    /// <summary>
    /// Represents a solution for an online practice problem.
    /// </summary>
    public interface ISolution
    {
        /// <summary>
        /// Solves the problem.
        /// </summary>
        void Solve();
    }
}
namespace CodePractice.Euler.E001MultipleOf3And5
{
    using System;
    using System.Globalization;
    using System.Text;
    using Computing.Extensions;

    class E001 : ISolution
    {
        public const int MaxN = 1_000_000_000;
        public const int MaxT = 100_000;
        public const int Min = 1;
        public static long MultipleOf3Or5(int upToExclusive)
        {
            int largest = upToExclusive - 1;
            return 3L.SumOfArithmeticSeries(3, largest / 3) + 5L.SumOfArithmeticSeries(5, largest / 5) - 15L.SumOfArithmeticSeries(15, largest / 15);
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

    public class Solution
    {
        public static void Main()
        {
            new E001().Solve();
        }
    }
}
```

Now we can copy its content and submit to HackerRank.
