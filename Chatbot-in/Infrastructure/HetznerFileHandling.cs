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
        private readonly IConfiguration _config;
        public HetznerFileHandling(ILogger<HetznerFileHandling> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public Task DeleteFile(string path)
        {
            throw new NotImplementedException();
        }

        public Task LoadFile(string path)
        {
            throw new NotImplementedException();
        }

        public async Task<ContextFileInfo?> SaveFile(string filename, string ownerToken, byte[] binary)
        {
            _logger.Log(LogLevel.Information, "Saving File");

            await LocalFileHandling.Save(filename, binary);

            var client = new AsyncFtpClient(_config["ftp:url"], _config["ftp:user"], _config["ftp:password"]);

            // connect to the server and automatically detect working FTP settings
            await client.AutoConnect();

            // get a list of files and directories in the "/htdocs" folder
            foreach (FtpListItem item in await client.GetListing("/"))
            {

                // if this is a file
                if (item.Type == FtpObjectType.File)
                {

                    // get the file size
                    long size = await client.GetFileSize(item.FullName);

                    // calculate a hash for the file on the server side (default algorithm)
                    FtpHash hash = await client.GetChecksum(item.FullName);
                }

                // get modified date/time of the file or folder
                DateTime time = await client.GetModifiedTime(item.FullName);
            }

            // check if a folder exists
            if (!await client.DirectoryExists($"/{ownerToken}/"))
            {
                await client.CreateDirectory($"/{ownerToken}/");
            }

            // upload a file
            var sucess = await client.UploadFile(Directory.GetCurrentDirectory() + @"\" + filename, $"/{ownerToken}/{filename}");

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
            await client.Disconnect();

            if (sucess != FtpStatus.Success)
            {
                return null;
            }

            return new ContextFileInfo()
            {
                FileName = filename,
                OwnerToken = ownerToken
            };
        }


        static class LocalFileHandling
        {
            public async static Task Save(string filename, byte[] binary)
            {
                await File.WriteAllBytesAsync(Directory.GetCurrentDirectory() + @"\" + filename, binary);                
            }

        }

    }
}
