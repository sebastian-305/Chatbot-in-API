using System.Text;
using Chatbot_in.Domain.Models;
using Chatbot_in.Domain.Service;
using FluentFTP;
using Microsoft.Extensions.Logging;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Chatbot_in.Infrastructure
{
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
            bool success = true;
            _logger.Log(LogLevel.Information, "Deleting File");
            try
            {
                await _ftpClient.DeleteFile($"/{owner}/{filename}");
            }
            catch (Exception ex)
            {
                _logger.LogError("{Error}", ex.Message);
                await _ftpClient.Disconnect();
                success =  false;
            }

            var list = await _ftpClient.GetListing($"/{owner}");

            if (list == null || list.Length == 0)
            {
                await _ftpClient.DeleteDirectory($"/{owner}");
            }

            await _ftpClient.Disconnect();
            return success;
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

            // get a list of files and directories in the "/htdocs" folder
            foreach (FtpListItem item in await _ftpClient.GetListing("/"))
            {

                // if this is a file
                if (item.Type == FtpObjectType.File)
                {

                    // get the file size
                    long size = await _ftpClient.GetFileSize(item.FullName);

                    // calculate a hash for the file on the server side (default algorithm)
                    FtpHash hash = await _ftpClient.GetChecksum(item.FullName);
                }

                // get modified date/time of the file or folder
                DateTime time = await _ftpClient.GetModifiedTime(item.FullName);
            }

            // check if a folder exists
            if (!await _ftpClient.DirectoryExists($"/{owner}/"))
            {
                await _ftpClient.CreateDirectory($"/{owner}/");
            }

            // upload a file
            var sucess = await _ftpClient.UploadFile(Directory.GetCurrentDirectory() + @"\" + filename, $"/{owner}/{filename}");

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

            if (sucess != FtpStatus.Success)
            {
                return null;
            }

            return new ContextFileInfo()
            {
                FileName = filename,
                OwnerToken = owner
            };
        }

        static class LocalFileHandling
        {
            public async static Task Save(string filename, byte[] binary)
            {
                await File.WriteAllBytesAsync(Directory.GetCurrentDirectory() + @"\" + filename, binary);
            }

            public async static Task<string[]> LoadAndDeleteTextFile(string filename)
            {
                var data = await File.ReadAllLinesAsync(Directory.GetCurrentDirectory() + @"\" + filename);

                File.Delete(Directory.GetCurrentDirectory() + @"\" + filename);

                return data;
            }

        }

    }
}
