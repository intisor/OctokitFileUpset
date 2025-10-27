using System.Net;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Octokit;
using Xunit;

namespace OctokitFileUpsert.Tests;

public class OctokitFileUpsertTests
{
    [Fact]
    public async Task TestUpsertFileAsync_CreatesFileWhenNotExists()
    {
        //Arrange
        var fakeClient = Substitute.For<IRepositoryContentsClient>();
        var owner = "testOwner";
        var repo = "testRepo";
        var filePath = "testFile.txt";
        var content = "Hello, World!";
        var commitMessage = "Add testFile.txt";

        fakeClient.GetAllContentsByRef(owner, repo, filePath, null)
            .Throws(new NotFoundException("File not found", HttpStatusCode.NotFound));

        var expectedResponse = Substitute.For<RepositoryContentChangeSet>();
        fakeClient.CreateFile(owner, repo, filePath, Arg.Any<CreateFileRequest>())
            .Returns(expectedResponse);
    }
}