namespace Quellatalo.CombineCode.CSharp.Tests;

public class FileUtilsTests
{
    [Test]
    public async Task FindFromParentDirectories_SolutionFile_Success()
    {
        var searchResult = FileUtils.FindFromParentDirectories("*.sln");
        using var multipleAssertion = Assert.Multiple();
        await Assert.That(searchResult).IsNotNull();
        await Assert.That(searchResult!.Length).IsEqualTo(1);
        await Assert.That(searchResult[0].Name).IsEqualTo("CombineCode.sln");
    }

    [Test]
    public async Task FindFromParentDirectories_NotExist_NotFound()
    {
        var searchResult = FileUtils.FindFromParentDirectories(Guid.NewGuid().ToString());
        await Assert.That(searchResult).IsNull();
    }
}
