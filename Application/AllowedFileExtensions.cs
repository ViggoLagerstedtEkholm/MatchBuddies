using DataLayer.Data.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
// A custom made exception for handling exceptions that could happen if users upload a file with a invalid extension
namespace Application
{
    public static class ValidationExtenstion
    {
        // checks the image and throws a exception if the image is null or has the wrong fileformat
        public static FileUpload CheckImage(this FileUpload image)
        {

            if (image.file == null || image == null)
                throw new WrongFileExtensionException();

            if (CheckBytes(image))
                return image;

            throw new WrongFileExtensionException();

        }

        // This method will check if a file that is uploaded is either a jpg, bmp, gif or png file
        // this is done by reading the eight first byte of the file.
        private static bool CheckBytes(FileUpload image)
        {
            Stream stream = new StreamReader(image.file.OpenReadStream()).BaseStream;

            stream.Seek(0, SeekOrigin.Begin);

            List<string> jpgFile = new List<string> { "FF", "D8" };
            List<string> bmpFile = new List<string> { "42", "4D" };
            List<string> gifFile = new List<string> { "47", "49", "46" };
            List<string> pngFile = new List<string> { "89", "50", "4E", "47", "0D", "0A", "1A", "0A" };
            List<List<string>> imgTypes = new List<List<string>> { jpgFile, bmpFile, gifFile, pngFile };

            List<string> bytesIterated = new List<string>();

            for (int i = 0; i < 8; i++)
            {
                string bit = stream.ReadByte().ToString("X2");
                bytesIterated.Add(bit);

                bool isImage = imgTypes.Any(img => !img.Except(bytesIterated).Any());
                if (isImage)
                {
                    return true;
                }
            }

            return false;
        }
    }

}


