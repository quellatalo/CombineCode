Quellatalo.CombineCode.CSharp
=============================

***A library to combine multiple written CSharp source files into one file.***

**Author:** *Quellatalo Nin*

# Overview

The project is still in early stage.
It does not scan the local folders as much as IDEs/compilers do.

Please see the [requirements](#requirements) to follow the expected code style.

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

Please visit [the project repository](https://github.com/quellatalo/CombineCode) to see some examples.

# Requirements

It's just some standard styling rules:
- [SA1402FileMayOnlyContainASingleType](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1402.md)
- [SA1649FileNameMustMatchTypeName](https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/documentation/SA1649.md)
- [IDE0130: Namespace does not match folder structure](https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/style-rules/ide0130)

Basically, namespaces should have matching folders, and types should have matching `.cs` files.
