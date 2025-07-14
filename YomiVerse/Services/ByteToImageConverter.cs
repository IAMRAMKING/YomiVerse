using Microsoft.Maui.Controls;
using System;
using System.Globalization;
using System.IO;

namespace YomiVerse.Services
{
    public class ByteToImageConverter : IValueConverter
    {
        // 1x1 transparent PNG base64
        private const string DefaultBase64Image =
            "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVQI12P4//8/AwAI/AL+GM6NEwAAAABJRU5ErkJggg==";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value is byte[] bytes && bytes.Length > 0)
                {
                    return ImageSource.FromStream(() => new MemoryStream(bytes));
                }

                // Use fallback 1x1 transparent image
                var fallback = System.Convert.FromBase64String(DefaultBase64Image);
                return ImageSource.FromStream(() => new MemoryStream(fallback));
            }
            catch
            {
                // As a last fallback, show base64 transparent image
                var fallback = System.Convert.FromBase64String(DefaultBase64Image);
                return ImageSource.FromStream(() => new MemoryStream(fallback));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
