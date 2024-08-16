namespace Final_project.Utilities.Extensions
{
    public static class FileUpload
    {
        public static async Task<string> CreateImage(this IFormFile file, string imagesFolderPath, string folder)
        {
            var destinationPath = Path.Combine(imagesFolderPath, folder);
            Random r = new();
            int random = r.Next(0, 1000);
            var fileName = string.Concat(random, file.FileName);
            var path = Path.Combine(destinationPath, fileName);

            using (FileStream stream = new(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fileName;
        }

        public static bool IsValidFile(this IFormFile file, string type)
        {
            return file.ContentType.Contains(type);
        }

        public static async Task DeleteImage(string imagePath)
        {
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
        }

        public static void DeleteFile(string fileName, string targetDirectory)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), Path.Combine("wwwroot", fileName));

            if (!File.Exists(path)) return;

            File.Delete(path);
        }
    }
}
