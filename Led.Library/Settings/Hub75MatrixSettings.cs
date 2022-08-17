namespace Led.Library.Settings
{
    [Serializable]
    public class Hub75MatrixSettings
    {
        public int PwmDuration { get; set; } = 100;
        public int RenderDelay { get; set; } = 1;
    }
}
