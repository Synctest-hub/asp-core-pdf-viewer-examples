using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Syncfusion.DocIO;
using System;
using System.IO;
using EJ2DocumentEditor = Syncfusion.EJ2.DocumentEditor;
namespace DocumentEditor.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumenteditorController : ControllerBase
{
    private IHostEnvironment hostEnvironment;
        public DocumenteditorController(IHostEnvironment environment)
        {
            this.hostEnvironment = environment;
        }
        //Import file from client side.
        [Route("Import")]
        public string Import(IFormCollection data)
        {
            if (data.Files.Count == 0)
                return null;
            Stream stream = new MemoryStream();
            IFormFile file = data.Files[0];
            int index = file.FileName.LastIndexOf('.');
            string type = index > -1 && index < file.FileName.Length - 1 ?
                file.FileName.Substring(index) : ".docx";
            file.CopyTo(stream);
            stream.Position = 0;

            EJ2DocumentEditor.WordDocument document = EJ2DocumentEditor.WordDocument.Load(stream, GetFormatType(type.ToLower()));
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(document);
            document.Dispose();
            return json;
        }
        //Import documents from web server.
        [Route("ImportFile")]
        public string ImportFile([FromBody]CustomParams param)
        {
            string path = this.hostEnvironment.ContentRootPath + "\\Files\\" + param.fileName;
            try
            {
                Stream stream = System.IO.File.Open(path, FileMode.Open, FileAccess.ReadWrite);
                Syncfusion.EJ2.DocumentEditor.WordDocument document = Syncfusion.EJ2.DocumentEditor.WordDocument.Load(stream, GetFormatType(path));
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(document);
                document.Dispose();
                stream.Dispose();
                return json;
            }
            catch
            {
                return "Failure";
            }
        }
        public class CustomParams
        {
            public string fileName
            {
                get;
                set;
            }
        }

        [Route("LoadDefault")]
        public string LoadDefault()
        {
            string path = this.hostEnvironment.ContentRootPath + "\\Files\\" + "sample.docx";
            Stream stream = System.IO.File.OpenRead(path);
            stream.Position = 0;

            Syncfusion.EJ2.DocumentEditor.WordDocument document = Syncfusion.EJ2.DocumentEditor.WordDocument.Load(stream, EJ2DocumentEditor.FormatType.Docx);
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(document);
            document.Dispose();
            return json;
        }

        //Save document in web server.
        [Route("Save")]
        public string Save([FromBody]CustomParameter param)
        {
            string path = this.hostEnvironment.ContentRootPath + "\\Files\\" + param.fileName;
            Byte[] byteArray = Convert.FromBase64String(param.documentData);
            Stream stream = new MemoryStream(byteArray);
            EJ2DocumentEditor.FormatType type = GetFormatType(path);
            try
            {
                FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);

                if (type != EJ2DocumentEditor.FormatType.Docx)
                {
                    Syncfusion.DocIO.DLS.WordDocument document = new Syncfusion.DocIO.DLS.WordDocument(stream, Syncfusion.DocIO.FormatType.Docx);
                    document.Save(fileStream, GetDocIOFomatType(type));
                    document.Close();
                }
                else
                {
                    stream.Position = 0;
                    stream.CopyTo(fileStream);
                }
                stream.Dispose();
                fileStream.Dispose();
                return "Sucess";
            }
            catch
            {
                Console.WriteLine("err");
                return "Failure";
            }
        }
        public class CustomParameter
        {
            public string fileName
            {
                get;
                set;
            }
            public string documentData
            {
                get;
                set;
            }
        }

        internal static EJ2DocumentEditor.FormatType GetFormatType(string fileName)
        {
            int index = fileName.LastIndexOf('.');
            string format = index > -1 && index < fileName.Length - 1 ? fileName.Substring(index + 1) : "";

            if (string.IsNullOrEmpty(format))
                throw new NotSupportedException("EJ2 Document editor does not support this file format.");
            switch (format.ToLower())
            {
                case "dotx":
                case "docx":
                case "docm":
                case "dotm":
                    return EJ2DocumentEditor.FormatType.Docx;
                case "dot":
                case "doc":
                    return EJ2DocumentEditor.FormatType.Doc;
                case "rtf":
                    return EJ2DocumentEditor.FormatType.Rtf;
                case "txt":
                    return EJ2DocumentEditor.FormatType.Txt;
                case "xml":
                    return EJ2DocumentEditor.FormatType.WordML;
                default:
                    throw new NotSupportedException("EJ2 Document editor does not support this file format.");
            }
        }

        internal static Syncfusion.DocIO.FormatType GetDocIOFomatType(EJ2DocumentEditor.FormatType type)
        {
            switch (type)
            {
                case EJ2DocumentEditor.FormatType.Docx:
                    return FormatType.Docx;
                case EJ2DocumentEditor.FormatType.Doc:
                    return FormatType.Doc;
                case EJ2DocumentEditor.FormatType.Rtf:
                    return FormatType.Rtf;
                case EJ2DocumentEditor.FormatType.Txt:
                    return FormatType.Txt;
                case EJ2DocumentEditor.FormatType.WordML:
                    return FormatType.WordML;
                default:
                    throw new NotSupportedException("DocIO does not support this file format.");
            }
        }

}
