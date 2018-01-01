using SQLDatabase;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace JournalMatcher
{
   //public class XBRLElement
   //{
   //   public string Header1 { get; set; }
   //   public string Header2 { get; set; }
   //   public string Header3 { get; set; }
   //   public string Header4 { get; set; }
   //   public string Header5 { get; set; }
   //   public string Header6 { get; set; }

   //   public string ElementName { get; set; }
   //   public string Saldo { get; set; }
   //   public string Dokumentation { get; set; }

   //   public string Konton { get; set; }

   //}

   public class ExcelLoader
   {
      
      List<string> _errors;

      object misValue = System.Reflection.Missing.Value;

      public ExcelLoader(List<string> errors)
      {
         _errors = errors;
         object misValue = System.Reflection.Missing.Value;

      }

      public void Load(BASContext model, Excel.Workbook wb, int sheetId)
      {

         Excel.Worksheet sheet = wb.Worksheets.get_Item(sheetId);

         int abstraktKol=0, elementnamnKol=0, firstRefKol=0;
         if (sheetId == 2) // Förvaltningsberättelse
         {
            abstraktKol = 14;
            elementnamnKol = 11;
            firstRefKol = 20;
         }
         if (sheetId == 3) // Resultaträkning
         {
            abstraktKol = 12;
            elementnamnKol = 9;
            firstRefKol = 18;
         }
         else if (sheetId == 5) //Balansräkning
         {
            abstraktKol = 13;
            elementnamnKol = 10;
            firstRefKol = 19;
         }


         string name = sheet.Name;
         Console.WriteLine("Laddar blad {0}", name);


         var columns = sheet.Columns.Count;

         var rows = sheet.Rows.Count;

         int lastLevel = -1;

         string[] headers = new string[10];

         for (int row = 2; !string.IsNullOrWhiteSpace(GetStringValue(sheet, row, 2)); row++)
         {
            bool abstrakt = (GetStringValue(sheet, row, abstraktKol) == "true");

            int level = 1;
            while (string.IsNullOrWhiteSpace(GetStringValue(sheet, row, level + 4)))
            {
               level++;
            };

            if (abstrakt)
            {
               // Ny rubrik
               string headerName = GetStringValue(sheet, row, level + 4);
               Console.WriteLine("Rubrik: {0}", headerName);
               headers[level] = headerName;
            }
            else
            {
               if (level >= lastLevel)
               {
                  // Post

                  var element = new XBRLElement
                  {
                     Name = GetStringValue(sheet, row, level + 4),

                     ElementName = GetStringValue(sheet, row, elementnamnKol),
                     Domain = GetStringValue(sheet, row, elementnamnKol+1),
                     Standardrubrik = GetStringValue(sheet, row, elementnamnKol + 2),
                     Abstrakt = (GetStringValue(sheet, row, elementnamnKol+3) == "true"),
                     Saldo = GetStringValue(sheet, row, elementnamnKol + 4),
                     Periodtyp = GetStringValue(sheet, row, elementnamnKol + 5),
                     Dokumentation = GetStringValue(sheet, row, elementnamnKol + 7),
                     Konton = "",
                     Header1 = headers[1],
                     Header2 = headers[2],
                     Header3 = headers[3],
                     Header4 = headers[4],
                     Header5 = headers[5],
                     Header6 = headers[6],
                  };


                  for (int r = 0; r < 20; r++)
                  {
                     // Gå igenom referenserna

                     var cell = CellName(row, firstRefKol + r * 8 + 1);
                     var refNamn = GetStringValue(sheet, row, firstRefKol + r * 8 + 1);
                     if (!string.IsNullOrWhiteSpace(refNamn))
                     {
                        if (refNamn == "BAS-konto")
                        {
                           string nummer = GetStringValue(sheet, row, firstRefKol + r * 8 + 2);
                           Console.WriteLine("    Kontonr: {0}", nummer);

                           if (element.Konton != "")
                           {
                              element.Konton += ", ";
                           }

                           element.Konton += GetStringValue(sheet, row, firstRefKol + r * 8 + 2);
                        }
                     }

                  }

                  model.XbrlElements.Add(element);
               }
               else
               {
                  // Summarad

                  headers[level] = "";
               }
            }

            lastLevel = level;
         }
      }
 
      public string CellName(int row, int col)
      {
         if (col < 25)
         {
            char cCol = (char)((int)'A' + col - 1);
            return string.Format("{0}{1}", cCol, row);
         }
         else
         {
            char cCol1 = (char)((int)'A' + (int)((col - 1)/26) - 1);
            char cCol2 = (char)((int)'A' + (int)((col - 1) % 26));
            return string.Format("{0}{1}{2}", cCol1,cCol2, row);
         }
      }

      public int? GetIntValue(Excel.Worksheet sheet, int row, int col)
      {
         try
         {
            dynamic value = sheet.get_Range(CellName(row, col)).get_Value();

            return int.Parse(value);
         }
         catch (Exception e)
         {
            return null;
         }

      }

      public decimal GetDecimalValue(Excel.Worksheet sheet, int row, int col)
      {
         return (decimal)sheet.get_Range(CellName(row, col)).get_Value();
      }

      public bool CheckNullValue(Excel.Worksheet sheet, int row, int col)
      {
         return (sheet.get_Range(CellName(row, col)).get_Value() == null);
      }

      public DateTime GetDateValue(Excel.Worksheet sheet, int row, int col)
      {
         return sheet.get_Range(CellName(row, col)).get_Value();
      }
      public string GetStringValue(Excel.Worksheet sheet, int row, int col)
      {
         string cellname = CellName(row, col);
         var range = sheet.get_Range(cellname);

         try
         {
            string x = range.Text.ToString();

            //var type = range.GetType();
            return x;
         }
         catch (Exception)
         {
            return null;
         }
      }
   }
}
