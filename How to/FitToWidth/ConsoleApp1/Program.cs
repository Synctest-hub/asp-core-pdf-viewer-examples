using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using System.IO;

class Program
{
    static void Main()
    {
        // Create a new PDF document
        PdfDocument doc = new PdfDocument();

        // Add a new page to the document
        PdfPage page = doc.Pages.Add();

        // Set the document’s viewer preferences
        doc.ViewerPreferences.FitWindow = true; // Fit the document to the window
        doc.ViewerPreferences.PageLayout = PdfPageLayout.SinglePage; // Display one page at a time

        // Save and close the document
        using (FileStream outputFileStream = new FileStream(Path.GetFullPath(@"../../../Output.pdf"), FileMode.Create, FileAccess.ReadWrite))
        {
            doc.Save(outputFileStream); // Save the document to a file
        }

        // Close the document after saving
        doc.Close(true);
    }
}
