﻿using Microsoft.AspNetCore.Http;
using WebTotalComander.Service.ViewModels;

namespace WebTotalComander.Service.Services;

public interface IFileService
{
    public Task<bool> SaveFileAsync(Stream stream,  string fileName ,string filePath);
    public Task<bool> DeleteFileAsync(string fileName, string path);
    public Task<MemoryStream> DownloadFileAsync(string filePath);
    public Task<bool> ReplaceFileAsync(Stream stream, string fileName, string filePath);
}
