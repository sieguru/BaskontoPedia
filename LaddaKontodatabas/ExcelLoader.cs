using SQLDatabase;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace JournalMatcher
{

   public class ExcelLoader
   {
      Excel.Application xl;
      Excel.Workbook wb;
      List<string> _errors;

      object misValue = System.Reflection.Missing.Value;

      public Dictionary<string, AccountNumber> allAccounts = new Dictionary<string, AccountNumber>();

      public ExcelLoader(List<string> errors)
      {
         _errors = errors;
         object misValue = System.Reflection.Missing.Value;

         xl = new Excel.Application();
         xl.SheetsInNewWorkbook = 1;
         xl.Visible = false;
      }

      public void Load(BASContext model, string year, string filnamn)
      {
         wb = xl.Workbooks.Open(filnamn, 0, true, 6, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, ";", false, false, 0, true, 1, 0);

         Excel.Worksheet sheet = wb.Worksheets.get_Item(1);


         //string name = sheet.Name;
         //Console.WriteLine("Laddar blad {0}", name);


         var columns = sheet.Columns.Count;

         var rows = sheet.Rows.Count;

         string huvudkonto = "";

         for (int row = 6; row < rows; row++)
         {
            string note = GetStringValue(sheet, row, 2);
            string kontonr = GetStringValue(sheet, row, 3).Trim();
            kontonr = kontonr.Replace("\n", ""); 


            string kontonamn = GetStringValue(sheet, row, 4).Trim();

            var parts = kontonr.Split(new[] { ' ' });

            if (parts.Count() > 1)
            {
               kontonr = parts[0];
               kontonamn = parts[1];
            }

            kontonr = kontonr.Replace((char)8211, '-');  // Typografiskt bindestreck ersätts med normalt


            if (!string.IsNullOrWhiteSpace(kontonr))
            {
               huvudkonto = kontonr;
            }

            bool notK2 = false;
            bool recommended = false;

            if (note == "[Ej K2]")
            {
               notK2 = true;
            }
            else if (note != null)
            {
               //Console.WriteLine("Not: {0}", note);
               recommended = true;
            }

            if (!string.IsNullOrWhiteSpace(kontonr))
            {
               if (kontonr == "END") return;

               if ((kontonr.Length == 4) && (kontonr.Substring(3, 1) == "0"))
               {
                  // Huvudkonto
                  huvudkonto = kontonr.Substring(0, 3);

                  AddAccount(model, year, kontonr.Substring(0, 3), null, kontonamn, huvudkonto, false, false);
                  // Underkonto (nollkonto)
                  AddAccount(model, year, kontonr, null, kontonamn, huvudkonto, false, false);
               }
               else
               {
                  // Kontoklass eller kontogrupp

                  if (kontonr.Contains('-'))  
                  {
                     var intervalParts = kontonr.Split(new[] { '-' });
                     kontonr = intervalParts[0];
                     string slutkonto= intervalParts[1];
                     huvudkonto = kontonr;

                     AddAccountInterval(model, year, kontonr, slutkonto, kontonamn, huvudkonto);
                  }
                  else
                  {

                     huvudkonto = kontonr;

                     AddAccount(model, year, kontonr, null, kontonamn, huvudkonto, false, false);
                  }
               }
            }

            note = GetStringValue(sheet, row, 5);
            kontonr = GetStringValue(sheet, row, 6);
            kontonamn = GetStringValue(sheet, row, 7);
            notK2 = false;
            recommended = false;

            if (note == "[Ej K2]")
            {
               notK2 = true;
            }
            else if (note != null)
            {
               //Console.WriteLine("Not: {0}", note);
               recommended = true;
            }

            if (!string.IsNullOrWhiteSpace(kontonr))
            {
               //Underkonto
               AddAccount(model, year, kontonr, null, kontonamn, huvudkonto, notK2, recommended);
            }


         }
         //Console.WriteLine("Laddat {0} transar från bankkontot.", bin.Count);

      }

      private void AddAccountInterval(BASContext model, string year, string kontonr, string slutkonto, string kontonamn, string huvudkonto)
      {
         string reference = kontonr;
         AddAccount(model, year, kontonr, slutkonto, kontonamn, huvudkonto, false, false);

         while (kontonr != slutkonto)
         {
            // Öka sistasiffran

            kontonr = kontonr.Substring(0, kontonr.Length - 1) + (char)(kontonr[kontonr.Length - 1] + 1);
            AddAccount(model, year, kontonr, null, kontonamn, huvudkonto, false, false, reference);
         }
      }

      private void AddAccount(BASContext model, string year, string kontonr, string intervallslut, string kontonamn, string huvudkonto, bool notK2, bool recommended, string reference = null)
      {
         kontonr = kontonr.Trim();
         kontonamn = kontonamn.Trim();

         if (!Regex.IsMatch(kontonr, @"^[1-8][0-9]{0,3}$"))
         {
            //Console.WriteLine("Ogiltigt konto {0} - {1}", kontonr, kontonamn);
            return;
         }

         //Console.WriteLine("{0} - {1}", kontonr, kontonamn);
         var acc = new Account
         {
            AccountID = kontonr,
            Year = year,
            Name = kontonamn,
            NotK2 = notK2,
            Recommended = recommended,
            SubAccount = (kontonr.Length == 4),
            MainAccount = huvudkonto,
         };
         model.Accounts.Add(acc);

         if (!allAccounts.ContainsKey(kontonr))
         {
            var accno = new AccountNumber
            {
               AccountId = kontonr,
               Name = kontonamn,
               LastYear = year,
               IntervalEnd = intervallslut,
               IntervalReference = reference
            };

            //accno.Accounts = new List<Account>();
            accno.Accounts.Add(acc);
//            model.AccountNumbers.Add(accno);

            allAccounts.Add(kontonr, accno);

            if (kontonr.Length > 1)
            {
               // Lägg till under "parent"

               string parentNr = kontonr.Substring(0, kontonr.Length - 1);

               if (!allAccounts.ContainsKey(parentNr))
               {
                  Console.WriteLine("Parent saknas för {0} - {1}", kontonr, kontonamn);
               }
               else
               {
                  var parent = allAccounts[parentNr];
                  if (parent.IntervalReference != null)
                  {
                     parent = allAccounts[parent.IntervalReference];
                  }
                  accno.Parent = parent;
                  parent.SubAccounts.Add(accno);
               }
            }
         }
         else
         {
            // Dubblettkonto
            var accno = allAccounts[kontonr];

            if (string.Compare(year, accno.LastYear) > 0)
            {
               // "Färskare" data

               accno.LastYear = year;
               accno.Name = kontonamn;
           }
         }

      }

      public void Load2000(BASContext model, string year, string filnamn)
      {
         wb = xl.Workbooks.Open(filnamn, 0, true, 6, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, ";", false, false, 0, true, 1, 0);

         Excel.Worksheet sheet = wb.Worksheets.get_Item(1);


         //string name = sheet.Name;
         //Console.WriteLine("Laddar blad {0}", name);


         var columns = sheet.Columns.Count;

         var rows = sheet.Rows.Count;


         for (int row = 2; row < rows; row++)
         {
            string text = GetStringValue(sheet, row, 1);
            if (text == "END") return;

            string huvudkonto = "";

            string kontonr = SplitAccount(model, year, text);
            if (!string.IsNullOrWhiteSpace(kontonr))
            {
               huvudkonto = kontonr;
            }
               text = GetStringValue(sheet, row, 2);
            SplitAccount(model, year, text, huvudkonto);

         }

      }

      public void Load2005(BASContext model, string year, string filnamn)
      {
         wb = xl.Workbooks.Open(filnamn, 0, true, 6, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, ";", false, false, 0, true, 1, 0);

         Excel.Worksheet sheet = wb.Worksheets.get_Item(1);


         //string name = sheet.Name;
         //Console.WriteLine("Laddar blad {0}", name);


         var columns = sheet.Columns.Count;

         var rows = sheet.Rows.Count;


         for (int row = 2; row < rows; row++)
         {
            string text = GetStringValue(sheet, row, 1);
            text = text.Trim();

            if (text == "END") return;
            
            string huvudkonto = "";

            if (text == "BAS-konton")
            {
               // Skippa rubrikrad
            }
            else
            {
               if (text.Contains(" "))
               {
                  string kontonr = SplitAccount(model, year, text);
                  if (!string.IsNullOrWhiteSpace(kontonr))
                  {
                     huvudkonto = kontonr;
                  }
               }
               else
               {
                  string kontonr = GetStringValue(sheet, row, 1);
                  string kontonamn = GetStringValue(sheet, row, 2);


                  if (!string.IsNullOrWhiteSpace(kontonr))
                  {
                     if ((kontonr.Length == 4) && (kontonr.Substring(3, 1) == "0"))
                     {
                        // Huvudkonto
                        huvudkonto = kontonr.Substring(0, 3);

                        AddAccount(model, year, kontonr.Substring(0, 3), null, kontonamn, huvudkonto, false, false);
                        AddAccount(model, year, kontonr, null, kontonamn, huvudkonto, false, false);
                     }
                     else
                     {
                        // Kontoklass eller kontogrupp
                        huvudkonto = kontonr;

                        AddAccount(model, year, kontonr, null, kontonamn, huvudkonto, false, false);
                     }

                  }

               }

               {
                  string kontonr = GetStringValue(sheet, row, 4);
                  string kontonamn = GetStringValue(sheet, row, 5);

                  if (!string.IsNullOrWhiteSpace(kontonr))
                  {
                     // Underkonto
                     AddAccount(model, year, kontonr, null, kontonamn, huvudkonto, false, false);
                  }
               }
            }
         }

      }


      private string SplitAccount(BASContext model, string year, string text, string huvudkonto = null)
      {
         if (!string.IsNullOrWhiteSpace(text))
         {
            string part1 = text.Split(new[] { ' ' })[0];

            if (int.TryParse(part1, out int x))
            {
               // Numeriskt kontonummer - OK
               string kontonr = part1.Trim();
               string kontonamn = text.Substring(part1.Length).Trim();
               Console.WriteLine("{0} - {1}", kontonr, kontonamn);

               if (huvudkonto == null) // Från vänsterspalten
               {
                  if ((kontonr.Length == 4) && (kontonr.Substring(3, 1) == "0"))
                  {
                     // Huvudkonto
                     huvudkonto = kontonr.Substring(0, 3);

                     AddAccount(model, year, kontonr.Substring(0, 3), null, kontonamn, huvudkonto, false, false);
                     AddAccount(model, year, kontonr, null, kontonamn, huvudkonto, false, false);
                  }
                  else
                  {
                     // Kontoklass eller kontogrupp
                     huvudkonto = kontonr;

                     AddAccount(model, year, kontonr, null, kontonamn, huvudkonto, false, false);
                  }

               }
               else
               {

                  AddAccount(model, year, kontonr, null, kontonamn, huvudkonto, false, false);
               }

               return huvudkonto;
            }
            else
            {
               // Icke-numeriskt
               Console.WriteLine("Error: {0}", text);
               return "";
            }
         }

         return "";
      }

      public string CellName(int row, int col)
      {
         char cCol = (char)((int)'A' + col - 1);
         return string.Format("{0}{1}", cCol, row);
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
