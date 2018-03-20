namespace SaluteOnline.ConfigurationService.Domain
{
    public class BaseConfigurationItem
    {
        public bool HasContent { get; set; }
        public bool DragEnabled { get; set; }
        public bool ResizeEnabled { get; set; }
        public int Cols { get; set; }
        public int Rows { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }
}
