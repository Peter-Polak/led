using System.Reflection;
using Iot.Device.Imu;

namespace Led.BlazorServerWebApp.Constants;

public class Media
{
    private const string separator = "_";
    private int height;
    private int width;

    public const string MediaFolderName = "media";
    public const string SystemFolderName = "System";

    public static Media ArrowLeft = new($"arrow{separator}left.png", SystemFolderName);
    public static Media ArrowRight = new($"arrow{separator}right.png", SystemFolderName);
    public static Media Bike = new($"bike.png", SystemFolderName);

    public string FullName { get; set; }
    public string Subfolder { get; set; }
    public string Name => Path.GetFileNameWithoutExtension(FullName);
    public string Extension => Path.GetExtension(FullName);
    public string Dimensions => $"{width}x{height}";

    public Media(string fullName, string subfolder = "", int width = 64, int height = 64)
    {
        this.width = width;
        this.height = height;

        FullName = fullName;
        Subfolder = subfolder;
    }

    
    public string GetMediaFolderPathRelative(bool wwwroot) => Path.Combine(wwwroot ? "wwwroot" : "", MediaFolderName, Dimensions);
    public string GetPathRelative(bool wwwroot) => Path.Combine(GetMediaFolderPathRelative(wwwroot), Subfolder, FullName);
    public static string GetMediaFolderPathRelative(bool wwwroot, int width = 64, int height = 64) => Path.Combine(wwwroot ? "wwwroot" : "", MediaFolderName, $"{width}x{height}");

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

    public static string[] ImageExtensions = { "jpg", "jpeg", "png", "tiff", "bmp", "svg" };
    public static string[] GifExtensions = { "gif" };

    public static Dictionary<string, List<Media>> GetAll(string[] extensions, int width = 64, int height = 64)
    {
        Dictionary<string, List<Media>> images = new();
        var absolutePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Media.GetMediaFolderPathRelative(true, width, height));
        var subfolders = GetSubfolders(width, height);

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
        foreach (var imageRelativePath in GetFilesFrom(absolutePath, extensions, false))
        {
            var imageName = new DirectoryInfo(imageRelativePath).Name;
            images[otherFolderName].Add(new(imageName));
        }

        return images;
    }

    public static Dictionary<string, List<Media>> GetAllImages(int width = 64, int height = 64) => GetAll(ImageExtensions, width, height);
    public static Dictionary<string, List<Media>> GetAllGifs(int width = 64, int height = 64) => GetAll(GifExtensions, width, height);
    public static string[] GetSubfolders(int width = 64, int height = 64)
    {
        var absolutePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot", MediaFolderName, $"{width}x{height}");
        var subfolders = Directory.GetDirectories(absolutePath);

        return subfolders;
    }
}
