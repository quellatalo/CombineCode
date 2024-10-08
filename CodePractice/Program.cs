using CodePractice.Euler.E001MultipleOf3And5;
using Quellatalo.CombineCode.CSharp;

// The combined source code file to be generated
const string TransformedFile = "Transformed.cs";
GenerateTransformedSolution<E001>();

// Open file using the OS's default behavior.
FileUtils.OpenFile(TransformedFile);
return;

static void GenerateTransformedSolution<T>()
{
    // The rewriter to add Solution class and main method required by HackerRank
    MainRewriter rewriter = new("Solution", $"new {typeof(T).Name}().Solve();");
    string[] pathsToSolution = typeof(T).FullName!.Split('.');
    File.WriteAllText(
        TransformedFile,
        CSharpSourceInfo
            .ReadFromFile(
                Path.Combine(
                [
                    CSharpSourceInfo.SolutionDirectory.FullName,
                    ..pathsToSolution[..^1],
                    $"{pathsToSolution[^1]}.cs",
                ]))
            .CompileToOneSource(rewriter));
}
