using iText.Kernel;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Filter;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PDFReader
{

 
   class AccountNumberFontFilter : TextRegionEventFilter
   {
      public AccountNumberFontFilter(Rectangle filterRect) : base(filterRect)
      {
      }

      public override bool Accept(IEventData data, EventType type)
      {
         if (type == EventType.RENDER_TEXT)
         {
            TextRenderInfo renderInfo = (TextRenderInfo)data;

            PdfFont font = renderInfo.GetFont();

            string fontName = font.GetFontProgram().GetFontNames().GetFontName().Substring(7);
            float fontSize = renderInfo.GetTextMatrix().Get(0);


            if (fontName == "HelveticaNeueLTStd-Bd")
            {
               // Sökord i sökordsregister
               // Marginaltext
               //   bl.a. kontonummer för huvudkonton
               // Underkonton

               string text = renderInfo.GetText().Trim();

               if (Regex.IsMatch(text, @"^[1-8][0-9]{0,3}.*"))
               {
                  if (fontSize >= 9)
                  {
                     //Console.WriteLine("GetText: {0} {1}", renderInfo.GetText(), fontName);
                     return true;
                  }
                  else
                  {
                     // Skräp extraherat från brödtext
                     //WriteRenderInfo(renderInfo);
                     return false;
                  }
               }

               return false;
            }

            if (fontName == "HelveticaNeueLTStd-Roman")
            {
               return false;
            }

            if (fontName == "HelveticaNeueLTStd-BdCn")
            {
               // Text i tumflik
               return false;
            }
            if (fontName == "HelveticaNeueLTStd-BdIt")
            {
               //Console.WriteLine("Y: {0}", renderInfo.GetText());
               return false;
            }
            if (fontName == "HelveticaNeueLTStd-It")
            {
               //Console.WriteLine("Z: {0}", renderInfo.GetText());
               return false;
            }
            if (fontName == "HelveticaNeueLTStd-Md")
            {
               return false;
            }
            if (fontName == "Wingdings-Regular")
            {
               return false;
            }

            if (fontName == "HelveticaNeueLTStd-MdCn")
            {
               return false;
            }

            if (fontName == "HelveticaNeueLTStd-Cn")
            {
               return false;
            }

            if (fontName == "SymbolStd")
            {
               return false;
            }

            if (fontName == "Helvetica")
            {
               // Tryckanvisninngar
               return false;
            }

            Console.WriteLine("Font: {0} {1}", fontName, fontSize);

         }

         return false;
      }

      private static void WriteRenderInfo(TextRenderInfo renderInfo)
      {
         Console.WriteLine("------");
         Console.WriteLine("GetActualText: {0}", renderInfo.GetActualText());
         Console.WriteLine("GetAscentLine: {0}", renderInfo.GetAscentLine());

         LineSegment baseline = renderInfo.GetBaseline();
         Console.WriteLine("GetBaseline: {0}", renderInfo.GetBaseline().GetStartPoint());


         if (renderInfo.GetCharSpacing() != 0)
         {
            Console.WriteLine("GetCharSpacing: {0}", renderInfo.GetCharSpacing());
         }
         Console.WriteLine("GetDescentLine: {0}", renderInfo.GetDescentLine());
         Console.WriteLine("GetExpansionText: {0}", renderInfo.GetExpansionText());
         if (renderInfo.GetFillColor().GetType() == typeof(DeviceGray))
         {
            Console.WriteLine("GetFillColor : {0}", ((DeviceGray)renderInfo.GetFillColor()).GetColorValue());
         }
         else
         {
            Console.WriteLine("GetFillColor: {0}", renderInfo.GetFillColor());
         }
         Console.WriteLine("GetFontSize: {0}", renderInfo.GetFontSize());
         if (renderInfo.GetHorizontalScaling() != 100)
         {
            Console.WriteLine("GetHorizontalScaling: {0}", renderInfo.GetHorizontalScaling());
         }

         if (renderInfo.GetLeading() != 0)
         {
            Console.WriteLine("GetLeading: {0}", renderInfo.GetLeading());
         }
         if (renderInfo.GetMcid() != 0)
         {
            Console.WriteLine("GetMcid: {0}", renderInfo.GetMcid());
         }
         Console.WriteLine("GetPdfString: {0}", renderInfo.GetPdfString());
         if (renderInfo.GetRise() != 0)
         {
            Console.WriteLine("GetRise: {0}", renderInfo.GetRise());
         }
         Console.WriteLine("GetSingleSpaceWidth: {0}", renderInfo.GetSingleSpaceWidth());
         Console.WriteLine("GetStrokeColor: {0}", renderInfo.GetStrokeColor());
         Console.WriteLine("GetText: {0}", renderInfo.GetText());
         Console.WriteLine("GetTextMatrix: {0}", renderInfo.GetTextMatrix());
         Console.WriteLine("GetTextMatrix: {0}", renderInfo.GetTextMatrix());
         Console.WriteLine("GetUnscaledBaseline: {0}", renderInfo.GetUnscaledBaseline());
         Console.WriteLine("GetUnscaledWidth: {0}", renderInfo.GetUnscaledWidth());
         if (renderInfo.GetWordSpacing() != 0)
         {
            Console.WriteLine("GetWordSpacing: {0}", renderInfo.GetWordSpacing());
         }
         //Console.WriteLine("IsReversedChars: {0}", renderInfo.IsReversedChars());
      }
   }
}

