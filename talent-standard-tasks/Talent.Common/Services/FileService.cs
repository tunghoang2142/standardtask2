using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talent.Common.Aws;
using Talent.Common.Contracts;

namespace Talent.Common.Services
{
    public class FileService : IFileService
    {
        private readonly IHostingEnvironment _environment;
        private readonly string _tempFolder;
        private IAwsService _awsService;

        public FileService(IHostingEnvironment environment,
            IAwsService awsService)
        {
            _environment = environment;
            _tempFolder = "images\\";
            _awsService = awsService;
        }

        public FileStreamResult GetImage(string id)
        {
            if (id == null) id = "matthew.png";
            string filePath = _environment.ContentRootFileProvider.GetFileInfo(Path.Combine(_tempFolder, id)).PhysicalPath;
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return new FileStreamResult(fileStream, "image/" + Path.GetExtension(filePath));
        }

        public async Task<string> GetFileURL(string id, FileType type)
        {
            switch (type)
            {
                case FileType.ProfilePhoto:
                    string filePath = _environment.ContentRootFileProvider.GetFileInfo(Path.Combine(_tempFolder, id)).PhysicalPath;
                    if (File.Exists(filePath))
                    {
                        return filePath;
                    }
                    return null;
                //case FileType.UserVideo:
                //    break;
                //case FileType.UserCV:
                //    break;
                default:
                    break;
            }
            throw new NotImplementedException();
        }

        public async Task<string> SaveFile(IFormFile file, FileType type)
        {
            switch (type)
            {
                case FileType.ProfilePhoto:
                    string folderPath = Path.Combine(_environment.ContentRootPath, _tempFolder);
                    Directory.CreateDirectory(folderPath);
                    string fileName = Path.GetRandomFileName() + Path.GetExtension(file.FileName);
                    string filePath = Path.Combine(folderPath, fileName);
                    using (Stream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        await file.CopyToAsync(fileStream); 
                    }
                    return fileName;
                //case FileType.UserVideo:
                //    break;
                //case FileType.UserCV:
                //    break;
                default:
                    return null;
            }
        }

        public async Task<bool> DeleteFile(string id, FileType type)
        {
            switch (type)
            {
                case FileType.ProfilePhoto:
                    string folderPath = Path.Combine(_environment.ContentRootPath, _tempFolder);
                    string filePath = Path.Combine(folderPath, id);
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                        return true;
                    }
                    return false;
                //case FileType.UserVideo:
                //    break;
                //case FileType.UserCV:
                //    break;
                default:
                    return false;
            }
        }


        #region Document Save Methods

        private async Task<string> SaveFileGeneral(IFormFile file, string bucket, string folder, bool isPublic)
        {
            //Your code here;
            throw new NotImplementedException();
        }

        private async Task<bool> DeleteFileGeneral(string id, string bucket)
        {
            //Your code here;
            throw new NotImplementedException();
        }
        #endregion
    }
}
