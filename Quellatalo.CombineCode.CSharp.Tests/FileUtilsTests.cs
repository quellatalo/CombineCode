namespace Quellatalo.CombineCode.CSharp.Tests;

public class FileUtilsTests
{
    [Test]
    public async Task FindFromParentDirectories_SolutionFile_Success()
    {
        var searchResult = FileUtils.FindFromParentDirectories("*.sln");
        using var multipleAssertion = Assert.Multiple();
        await Assert.That(searchResult).HasCount().EqualTo(1);
        await Assert.That(searchResult[0].Name).IsEqualTo("CombineCode.sln");
    }

    [Test]
    public async Task FindFromParentDirectories_SubDir_Success()
    {
        var searchResult = FileUtils.FindFromParentDirectories("*.csproj", subDirectories: ["CombineCode", "*"]);
        using var multipleAssertion = Assert.Multiple();
        await Assert.That(searchResult).HasCount().EqualTo(4);
    }

    [Test]
    public async Task FindFromParentDirectories_NotExist_NotFound()
    {
        var searchResult = FileUtils.FindFromParentDirectories(Guid.NewGuid().ToString());
        await Assert.That(searchResult).IsEmpty();
    }
}
