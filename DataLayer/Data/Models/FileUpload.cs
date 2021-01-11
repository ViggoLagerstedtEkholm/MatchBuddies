using Microsoft.AspNetCore.Http;

namespace DataLayer.Data.Models
{
    public class FileUpload
    {
        public IFormFile file { get; set; }
    }
}
