using Chatbot_in.Application.Service;
using Chatbot_in.Domain.Models;
using FluentFTP;

namespace Chatbot_in.Infrastructure.HetznerFTP;

public class HetznerFileHandling : IFileHandlingTemporary
{
    private readonly ILogger<HetznerFileHandling> _logger;
    private readonly AsyncFtpClient _ftpClient;
    public HetznerFileHandling(ILogger<HetznerFileHandling> logger, IConfiguration config)
    {
        _logger = logger;
        _ftpClient = new AsyncFtpClient(config["ftp:url"], config["ftp:user"], config["ftp:password"]);
    }

    public async Task<bool> DeleteFile(string owner, string filename)
    {
        _logger.Log(LogLevel.Information, "Deleting File");
        await _ftpClient.AutoConnect();

        try
        {
            await _ftpClient.DeleteFile($"/{owner}/{filename}");
        }
        catch (Exception ex)
        {
            _logger.LogError("{Error}", ex.Message);
            await _ftpClient.Disconnect();
            return false;
        }

        var list = await _ftpClient.GetListing($"/{owner}");

        if (list == null || list.Length == 0)
        {
            await _ftpClient.DeleteDirectory($"/{owner}");
        }

        await _ftpClient.Disconnect();
        return true;
    }

    public async Task<string[]> LoadFile(string owner, string filename)
    {
        _logger.Log(LogLevel.Information, "Loading File");

        await _ftpClient.AutoConnect();

        try
        {
            if (await _ftpClient.DownloadFile(Directory.GetCurrentDirectory() + @"\" + filename, $"/{owner}/{filename}") != FtpStatus.Success)
            {
                throw new Exception("Error in downloading File");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("{Error}", ex.Message);
            await _ftpClient.Disconnect();
            return [];
        }
        await _ftpClient.Disconnect();

        return await LocalFileHandling.LoadAndDeleteTextFile(filename);
    }

    public async Task<ContextFileInfo?> SaveFile(string owner, string filename, byte[] binary)
    {
        _logger.Log(LogLevel.Information, "Saving File");

        await LocalFileHandling.Save(filename, binary);

        // connect to the server and automatically detect working FTP settings
        await _ftpClient.AutoConnect();

        // check if a folder exists
        if (!await _ftpClient.DirectoryExists($"/{owner}/"))
        {
            await _ftpClient.CreateDirectory($"/{owner}/");
        }

        // upload a file
        var success = await _ftpClient.UploadFile(Directory.GetCurrentDirectory() + @"\" + filename, $"/{owner}/{filename}");
        
        try
        {
            File.Delete(Directory.GetCurrentDirectory() + @"\" + filename);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
        }

        //// upload a file and retry 3 times before giving up
        //client.Config.RetryAttempts = 3;
        //await client.UploadFile(@"C:\MyVideo.mp4", "/htdocs/big.txt", FtpRemoteExists.Overwrite, false, FtpVerify.Retry);

        // disconnect! good bye!
        await _ftpClient.Disconnect();

        if (success != FtpStatus.Success)
        {
            return null;
        }

        return new ContextFileInfo()
        {
            FileName = filename,
            ShortUID = owner
        };
    }

    static class LocalFileHandling
    {
        public async static Task Save(string filename, byte[] binary)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), filename);
            await File.WriteAllBytesAsync(filePath, binary);
        }

        public async static Task<string[]> LoadAndDeleteTextFile(string filename)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), filename);
            string[] data = await File.ReadAllLinesAsync(filePath);

            File.Delete(filePath);

            return data;
        }

    }

}
