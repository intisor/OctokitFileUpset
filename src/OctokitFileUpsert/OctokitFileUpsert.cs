using System.Net;
using System.Text;
using Octokit;

namespace OctokitFileUpsert;

public static class FileUpsertExtensions
{
    public static async Task<RepositoryContentChangeSet> UpsertFileAsync(
        this IRepositoryContentsClient client,
        string owner,
        string name,
        string filePath,
        string content,
        string commitMessage,
        string? branch = null,
        bool isContentBase64 = false)
    {
        if (isContentBase64)
        {
            var decodedBytes = Convert.FromBase64String(content);
            content = Encoding.UTF8.GetString(decodedBytes);
        }
        string? fileSha = null;
        try
        {
            var contents = await client.GetAllContentsByRef(owner, name, filePath, branch);
            if (contents.Count > 0)
            {
                fileSha = contents[0].Sha;
            }
        }
        catch (NotFoundException)
        {
            // File does not exist, fileSha remains null
        }

        if (fileSha == null)
        {
            return await client.CreateFile(owner, name, filePath, new CreateFileRequest(commitMessage, content, branch));
        }
        else
        {
            return await client.UpdateFile(owner, name, filePath, new UpdateFileRequest(commitMessage, content, fileSha, branch));
        }
    }
}