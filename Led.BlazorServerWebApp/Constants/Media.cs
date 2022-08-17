using System.Reflection;

namespace Led.BlazorServerWebApp.Constants
{
    public static class Media
    {
        public const string RootFolder = "media";
        public const string ImageFolder = "images";
        public const string GifFolder = "gifs";

        private const string separator = "_";
        public static class Image
        {
            public static class Arrow
            {
                public const string Left = $"arrow{separator}left.png";
                public const string Right = $"arrow{separator}right.png";
            }

            public static class Flag
            {
                public const string Ukraine = $"flag{separator}ukraine.png";
            }

            public const string FeelsGoodMan = $"feelsgoodman.png";
            //public const string NetRobot = $"netrobot.png";
            //public const string MGF = $"mgf.png";
            public const string Bike = $"bike.png";

            public static string GetPath(string fileName, int size = 64)
            {
                return Path.Combine(RootFolder, $"{size}x{size}",ImageFolder, fileName);
            }

            public static string GetWwwPath(string fileName, int size = 64)
            {
                return Path.Combine("wwwroot", RootFolder, $"{size}x{size}", ImageFolder, fileName);
            }
        }

        public class Gif
        {
            public const string Wave = $"wave.gif";

            public static string GetPath(string fileName, int size = 64)
            {
                return Path.Combine(RootFolder, $"{size}x{size}", GifFolder, fileName);
            }

            public static string GetWwwPath(string fileName, int size = 64)
            {
                return Path.Combine("wwwroot", RootFolder, $"{size}x{size}", GifFolder, fileName);
            }
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
}
