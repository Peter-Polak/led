using System.Reflection;

namespace Led.BlazorServerWebApp.Constants;

public static class Media
{
    public const string MediaFolderName = "media";
    public const string SystemFolderName = "System";

    private const string separator = "_";

    public static List<string> GetFilesFrom(string searchFolder, string[] filters, bool isRecursive)
    {
        List<string> filesFound = new List<string>();
        var searchOption = isRecursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        foreach (var filter in filters)
        {
            filesFound.AddRange(Directory.GetFiles(searchFolder, $"*.{filter}", searchOption));
        }
        return filesFound.ToList();
    }

    public class Image
    {
        public const string ImageFolderName = "images";

        public static Image ArrowLeft = new ($"arrow{separator}left.png", SystemFolderName);
        public static Image ArrowRight = new ($"arrow{separator}right.png", SystemFolderName);
        public static Image Bike = new ($"bike.png", SystemFolderName);

        public string FullName { get; set; }
        public string Subfolder { get; set; }
        public string Name => Path.GetFileNameWithoutExtension(FullName);
        public string Extension => Path.GetExtension(FullName);
        public string GetRelativePath(int size = 64) => GetRelativePath(FullName, Subfolder, size);
        public string GetRelativePathWeb(int size = 64) => GetRelativePathWeb(FullName, Subfolder, size);

        public Image(string fullName, string subfolder = "")
        {
            FullName = fullName;
            Subfolder = subfolder;
        }

        private static string GetFolderRelativePath(string subfolder = "", int size = 64) => Path.Combine(MediaFolderName, $"{size}x{size}", ImageFolderName, subfolder);
        public static string GetRelativePath(string fileName, string subfolder = "", int size = 64) => Path.Combine(GetFolderRelativePath(subfolder, size), fileName);
        public static string GetRelativePathWeb(string fileName, string subfolder = "", int size = 64) => Path.Combine("wwwroot", GetRelativePath(fileName, subfolder, size));

        public static Dictionary<string, List<Image>> GetAllImages(int size = 64)
        {
            Dictionary<string, List<Image>> images = new();
            var relativePath = Path.Combine("wwwroot", MediaFolderName, $"{size}x{size}", ImageFolderName);
            var extensions = new string[] { "jpg", "jpeg", "png", "tiff", "bmp", "svg" };
            var subfolders = Directory.GetDirectories(relativePath);

            foreach (var folderRelativePath in subfolders)
            {
                var folderName = new DirectoryInfo(folderRelativePath).Name;
                images.Add(folderName, new());

                foreach (var imageRelativePath in GetFilesFrom(folderRelativePath, extensions, false))
                {
                    var imageName = new DirectoryInfo(imageRelativePath).Name;
                    images[folderName].Add(new(imageName, folderName));
                }
            }

            string otherFolderName = "Other";
            images.Add(otherFolderName, new());
            foreach (var imageRelativePath in GetFilesFrom(relativePath, extensions, false))
            {
                var imageName = new DirectoryInfo(imageRelativePath).Name;
                images[otherFolderName].Add(new(imageName));
            }

            return images;
        }
    }

    public class Gif
    {
        public const string GifFolderName = "gifs";
    }

}

public static class TypeUtilities
{
    public static List<T?> GetAllPublicConstantValues<T>(this Type type)
    {
        return type
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(T))
            .Select(x => (T?)x.GetRawConstantValue())
            .ToList();
    }
}
