using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace ProgrammingPOE.Controllers
{
    public class DocumentsController : Controller
    {
        private readonly IWebHostEnvironment _environment;

        public DocumentsController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public IActionResult Download(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return NotFound("File name is required.");
            }

            // Security: Prevent directory traversal attacks
            var safeFileName = Path.GetFileName(fileName);
            if (string.IsNullOrEmpty(safeFileName))
            {
                return NotFound("Invalid file name.");
            }

            var filePath = Path.Combine(_environment.WebRootPath, "uploads", safeFileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound($"File not found: {safeFileName}");
            }

            // Determine content type based on file extension
            var contentType = GetContentType(safeFileName);
            var originalFileName = safeFileName.Contains("_")
                ? safeFileName.Substring(safeFileName.IndexOf("_") + 1)
                : safeFileName;

            // Return the file for download
            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, contentType, originalFileName);
        }

        private string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".pdf" => "application/pdf",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".doc" => "application/msword",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".xls" => "application/vnd.ms-excel",
                ".txt" => "text/plain",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                _ => "application/octet-stream"
            };
        }
    }
}