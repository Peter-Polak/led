using Led.Library;

namespace Led.BlazorServerWebApp.Models
{
    [Serializable]
    public class PageSettings
    {
        [NonSerialized]
        public const string FileName = "pages";
        public Page[] Pages { get; set; }

        public static PageSettings Load()
        {
            return Json.Load<PageSettings>(FileName);
        }
    }
}
