using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YomiVerse.Services
{
    internal class YomiService
    {
        private static readonly Lazy<YomiService> _instance = new Lazy<YomiService>(() => new YomiService());

        public static YomiService Instance => _instance.Value;

        private YomiService() { }

        public ImageSource GetImageSource(byte[] imageBytes)
        {
            return ImageSource.FromStream(() => new MemoryStream(imageBytes));
        }
    }
}
