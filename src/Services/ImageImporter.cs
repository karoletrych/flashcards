using System;
using System.IO;
using System.Threading.Tasks;
using Tesseract;

namespace FlashCards.Services
{
    public interface IImageImporter
    {
        Task<string> Import(Stream stream);
    }

    public class ImageImporter : IImageImporter
    {
        private readonly ITesseractApi _tesseractApi;

        public ImageImporter(ITesseractApi tesseractApi)
        {
            _tesseractApi = tesseractApi;
        }

        public async Task<string> Import(Stream stream)
        {
            var initialized = await _tesseractApi.Init("eng+pol");
            if (!initialized)
                throw new Exception("Tesseract cannot be initialized");

            var recognized = await _tesseractApi.SetImage(stream);
            if (!recognized)
                throw new Exception("Tesseract cannot be initialized");
            return _tesseractApi.Text;
        }
    }
}