namespace Training_Quest3.ViewModel
{
    internal class ImageSaveOptions
    {
        private object png;

        public ImageSaveOptions(object png)
        {
            this.png = png;
        }

        public int PageNumber { get; set; }
        public int Width { get; set; }
    }
}