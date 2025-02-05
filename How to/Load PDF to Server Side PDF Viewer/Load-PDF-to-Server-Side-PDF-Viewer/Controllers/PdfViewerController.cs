using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Syncfusion.EJ2.PdfViewer;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json.Nodes;

namespace CoreSample.Controllers
{
    public class PdfViewerController : Controller
    {
        private IWebHostEnvironment _hostingEnvironment;
        //Initialize the memory cache object   
        public IMemoryCache _cache;
        public PdfViewerController(IWebHostEnvironment hostingEnvironment, IMemoryCache cache)
        {
            _hostingEnvironment = hostingEnvironment;
            _cache = cache;
            Console.WriteLine("PdfViewerController initialized");
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        //Get action for obtaing the byte array of the PDF document
        public IActionResult GetPDFByte(string fileName)
        {
            string documentPath = GetDocumentPath(fileName);
            // Read the file content into a byte array
            byte[] fileBytes = System.IO.File.ReadAllBytes(documentPath);

            // Return the byte array as a file
            return File(fileBytes, "application/pdf", fileName);
        }

        //Gets the path of the PDF document
        private string GetDocumentPath(string document)
        {
            string documentPath = string.Empty;
            if (!System.IO.File.Exists(document))
            {
                var path = _hostingEnvironment.ContentRootPath;
                if (System.IO.File.Exists(path + "/Data/" + document))
                    documentPath = path + "/Data/" + document;
            }
            else
            {
                documentPath = document;
            }
            Console.WriteLine(documentPath);
            return documentPath;
        }

    }
}
