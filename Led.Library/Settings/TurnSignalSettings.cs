namespace Led.Library.Settings
{
    [Serializable]
    public class TurnSignalSettings
    {
        public int OnDuration { get; set; } = 350;
        public int OffDuration { get; set; } = 350;
    }
}
