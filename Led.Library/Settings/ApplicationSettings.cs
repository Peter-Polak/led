namespace Led.Library.Settings
{
    [Serializable]
    public class ApplicationSettings
    {
        [NonSerialized]
        public const string FileName = "settings";
        public Hub75MatrixSettings Hub75Matrix { get; set; } = new Hub75MatrixSettings();
        public TurnSignalSettings TurnSignal { get; set; } = new TurnSignalSettings();

        public static ApplicationSettings Load()
        {
            return Json.Load<ApplicationSettings>(FileName);
        }

        public void Save()
        {
            Json.Save(this, FileName);
        }
    }
}
