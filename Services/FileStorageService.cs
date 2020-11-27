using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace TodoListMVC.Services
{
    public interface IFileStorageService
    {
        string GetFileUrl(string fileName);
        Task<string> SaveFileAsync(IFormFile file);
        Task DeleteFileAsync(string fileName);
    }
    public class FileStorageService : IFileStorageService
    {
        private readonly string _fileFolder;
        private const string FILE_FOLDER_NAME = "files";

        public FileStorageService(IWebHostEnvironment webHostEnvironment)
        {
            _fileFolder = Path.Combine(webHostEnvironment.WebRootPath, FILE_FOLDER_NAME);
        }

        public string GetFileUrl(string fileName)
        {
            return $"/{FILE_FOLDER_NAME}/{fileName}";
        }

        public async Task<string> SaveFileAsync(IFormFile file)
        {
            var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";

            var filePath = Path.Combine(_fileFolder, fileName);

            using var output = new FileStream(filePath, FileMode.Create);

            await file.OpenReadStream().CopyToAsync(output);

            return fileName;
        }

        public async Task DeleteFileAsync(string fileName)
        {
            var filePath = Path.Combine(_fileFolder, fileName);
            if (File.Exists(filePath))
            {
                await Task.Run(() => File.Delete(filePath));
            }
        }
    }
}
