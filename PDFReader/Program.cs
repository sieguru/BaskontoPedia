using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using SQLDatabase;
using System;
using System.Text;
using System.Linq;

namespace PDFReader
{

   // The coordinate of the lower-left corner is x = mediabox.getLeft() and y = mediabox.getBottom() the coordinate of the upper-right corner is x = mediabox.getRight() and y = mediabox.getTop().

   // The values of x increase from left to right; the values of y increase from bottom to top.The unit of the measurement system in PDF is called "user unit". By default one user unit coincides with one point(this can change, but you won't find many PDFs with a different UserUnit value). In normal circumstances, 72 user units = 1 inch.

   class Program
   {
      static void Main(string[] args)
      {
         StringBuilder text = new StringBuilder();
         PdfDocument pdfDoc = new PdfDocument(new PdfReader(@"D:\BaskontoPedia\Bokforingsboken manus den 23 oktober 2017.pdf"));
         var model = new BASContext(false);

         ExtraheraKontonummer(model, pdfDoc, 62, 433);

         model.SaveChanges();

         pdfDoc.Close();
      }

      private static void ExtraheraKontonummer(BASContext model, PdfDocument pdfDoc, int startPage, int endPage)
      {
         //startPage = 265;
         //endPage = 265;

         // Gå igenom konteringsinstruktionerna och extrahera alla konton
         for (int page = startPage; page <= endPage; page++)
         {
            PdfPage p = pdfDoc.GetPage(page);
            Rectangle rect = p.GetCropBox();

            AccountNumberFontFilter fontFilter = new AccountNumberFontFilter(rect);

            FilteredEventListener listener = new FilteredEventListener();
            LocationTextExtractionStrategy extractionStrategy = listener.AttachEventListener(new LocationTextExtractionStrategy(), fontFilter);
            var processor = new PdfCanvasProcessor(listener);
            processor.ProcessPageContent(p);

            string extractedText = extractionStrategy.GetResultantText().Trim();

            if (extractedText != "")
            {
               //Console.WriteLine("Page: {0}", page);
               
               foreach (var kontotext in extractedText.Split(new[] { '\n' }))
               {
                  string kontonr = kontotext.Trim().Split()[0].Trim();

                  // Console.WriteLine("{0} <- {1}", kontonr, kontotext);


                  if (kontonr != "")
                  {
                     UpdateAccountInDatabase(model, page, kontonr);

                     if ((kontonr.Length == 4) && (kontonr.Substring(3, 1) == "0"))
                     {
                        UpdateAccountInDatabase(model, page, kontonr.Substring(0,3));
                     }
                  }
               }
            }
         }
      }

      private static void UpdateAccountInDatabase(BASContext model, int page, string kontonr)
      {
         //Console.WriteLine(kontonr);
         var konto = (from k in model.AccountNumbers
                      where k.AccountId == kontonr
                      select k).FirstOrDefault();

         if (konto == null)
         {
            Console.WriteLine("Konto saknas i databasen: {0}", kontonr);
         }
         else
         {

            if (string.IsNullOrEmpty(konto.SidaIBokforingsboken))
            {
               //Console.WriteLine("Konto {0} uppdaterat.", kontonr);
               konto.SidaIBokforingsboken = page.ToString();
            }
            else
            {
               if (konto.SidaIBokforingsboken == page.ToString())
               {
                  // Kontot förekommer flera gånger på samma sida - upprepa inte sidreferensen
                  //Console.WriteLine("Konto {0} har redan korrekt sidreferens.", kontonr);
               }
               else
               {
                  konto.SidaIBokforingsboken += ", " + page.ToString();
                  Console.WriteLine("Konto {0} förekommer på flera sidor: {1}", kontonr, konto.SidaIBokforingsboken);
               }
            }
         }
      }

      private static void ExtraheraNyckelord(PdfDocument pdfDoc, int startPage, int endPage)
      {
         // Gå igenom sökordsregistret och extrahera alla sökord

         for (int page = startPage; page <= endPage; page++)
         {
            PdfPage p = pdfDoc.GetPage(page);
            Rectangle rect = p.GetCropBox();

            AccountNumberFontFilter fontFilter = new AccountNumberFontFilter(rect);

            FilteredEventListener listener = new FilteredEventListener();
            LocationTextExtractionStrategy extractionStrategy = listener.AttachEventListener(new LocationTextExtractionStrategy(), fontFilter);
            var processor = new PdfCanvasProcessor(listener);
            processor.ProcessPageContent(p);

            string actualText = extractionStrategy.GetResultantText();

            if (actualText != "")
            {
               Console.WriteLine("Page: {0}", page);
               Console.WriteLine(actualText);
            }
         }
      }

   }
}

