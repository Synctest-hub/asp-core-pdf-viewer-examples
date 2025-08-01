using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Syncfusion.EJ2.PdfViewer;
using System.IO;
using Newtonsoft.Json;
using Syncfusion.Pdf.Parsing;
using System.Security.Cryptography.X509Certificates;
using Syncfusion.Pdf.Security;
using Syncfusion.Pdf;
using System.Net;
#if REDIS
using Microsoft.Extensions.Caching.Distributed;
#endif

namespace ESigningSimpleSample.Controllers
{
    public partial class HomeController : Controller
    {
        private IMemoryCache _cache;
        private readonly IWebHostEnvironment _hostingEnvironment;
#if REDIS
        private IDistributedCache _distributedCache;
        public PdfViewerController(IMemoryCache memoryCache, IDistributedCache distributedCache, IWebHostEnvironment hostingEnvironment)
#else
        public HomeController(IMemoryCache memoryCache, IWebHostEnvironment hostingEnvironment)
#endif
        {
            _cache = memoryCache;
#if REDIS
            _distributedCache = distributedCache;
#endif
            _hostingEnvironment = hostingEnvironment;
        }
        // GET: Default
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ESigningPdfForms()
        {
            return View();
        }

        [AcceptVerbs("Post")]
        [HttpPost]
        [Route("api/[controller]/FlattenDownload")]
        public IActionResult FlattenDownload([FromBody] Dictionary<string, string> jsonObject)
        {
            try
            {
                string documentBase = "";
                
                // Check if the request contains base64String (from custom JavaScript call)
                if (jsonObject != null && jsonObject.ContainsKey("base64String"))
                {
                    documentBase = jsonObject["base64String"];
                }

                // Handle base64 string processing safely
                string base64String = "";
                if (documentBase.Contains("data:application/pdf;base64,"))
                {
                    base64String = documentBase.Split(new string[] { "data:application/pdf;base64," }, StringSplitOptions.None)[1];
                }
                else
                {
                    // Assume it's already just the base64 data without the prefix
                    base64String = documentBase;
                }

                byte[] byteArray = Convert.FromBase64String(base64String);
                PdfLoadedDocument loadedDocument = new PdfLoadedDocument(byteArray);
                
                if (loadedDocument.Form != null)
                {
                    loadedDocument.FlattenAnnotations();
                    loadedDocument.Form.Flatten = true;
                }
                
                //Save the PDF document.
                MemoryStream stream = new MemoryStream();
                //Save the PDF document
                loadedDocument.Save(stream);
                stream.Position = 0;
                //Close the document
                loadedDocument.Close(true);
                
                string updatedDocumentBase = Convert.ToBase64String(stream.ToArray());
                documentBase = "data:application/pdf;base64," + updatedDocumentBase;
                return Content(documentBase);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error processing PDF: {ex.Message}");
            }
        }

    }
}