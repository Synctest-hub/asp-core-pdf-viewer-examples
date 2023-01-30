using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Syncfusion.EJ2.PdfViewer;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Reflection;
using Syncfusion.Pdf.Parsing;
using SkiaSharp;
using System.Drawing;

namespace ExportImageindotnet6.Pages
{
    [IgnoreAntiforgeryToken(Order = 1001)]
    public class IndexModel : PageModel
    {

        private readonly IWebHostEnvironment _hostingEnvironment;
        private IMemoryCache _cache;

        public IndexModel(IWebHostEnvironment hostingEnvironment, IMemoryCache cache)
        {
            _hostingEnvironment = hostingEnvironment;
            _cache = cache;
        }

        public IActionResult OnGetImage()
        {
            PdfRenderer pdfExportImage = new PdfRenderer();
            pdfExportImage.Load("wwwroot/Data/hive_succinctly.pdf");
            SKBitmap bitmapimage = pdfExportImage.ExportAsImage(0);
            SKImage image = SKImage.FromBitmap(bitmapimage);
            using (var stream = new FileStream(Path.Combine("wwwroot/Data/Page1.png"), FileMode.Create))
            {
                var imageData = image.Encode(SKEncodedImageFormat.Png, 100);
                imageData.SaveTo(stream);
            }
            var imageFileStream = System.IO.File.OpenRead(Path.Combine("wwwroot/Data/Page1.png"));
            return File(imageFileStream, "image/png");
        }
    }
}