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
         Console.WriteLine("Laddar BAS {0}", year);
         wb = xl.Workbooks.Open(filnamn, 0, true, 6, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, ";", false, false, 0, true, 1, 0);

         Excel.Worksheet sheet = wb.Worksheets.get_Item(1);


         var columns = sheet.Columns.Count;

         var rows = sheet.Rows.Count;

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

               if (kontonr.Contains('-'))
               {
                  var intervalParts = kontonr.Split(new[] { '-' });
                  kontonr = intervalParts[0];
                  string slutkonto = intervalParts[1];
                  //Console.WriteLine("Adding interval: {0}-{1}", kontonr, slutkonto);

                  if (!slutkonto.StartsWith(kontonr.Substring(0, kontonr.Length - 1)))
                  {
                     Console.WriteLine("Ogiltigt intervall: {0}-{1}", kontonr, slutkonto);
                  }
                  else
                  {
                     AddAccountInterval(model, year, kontonr, slutkonto, kontonamn, true, notK2, recommended);
                  }
               }
               else
               {
                  AddAccount(model, year, kontonr, null, kontonamn, true, notK2, recommended);
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
               AddAccount(model, year, kontonr, null, kontonamn, false, notK2, recommended);
            }
         }
      }

      private void AddAccountInterval(BASContext model, string year, string kontonr, string slutkonto, string kontonamn, bool firstcol, bool notK2, bool recommended)
      {
         string reference = kontonr;
         AddAccount(model, year, kontonr, slutkonto, kontonamn, firstcol, notK2, recommended);

         while (kontonr != slutkonto)
         {
            // Öka sistasiffran

            kontonr = kontonr.Substring(0, kontonr.Length - 1) + (char)(kontonr[kontonr.Length - 1] + 1);
            AddAccount(model, year, kontonr, null, kontonamn, firstcol, notK2, recommended, reference);
         }
      }

      private void AddAccount(BASContext model, string year, string kontonr, string intervallslut, string kontonamn, bool firstcol, bool notK2, bool recommended, string reference = null)
      {
         kontonr = kontonr.Trim();
         kontonamn = kontonamn.Trim();

         if (!Regex.IsMatch(kontonr, @"^[1-8][0-9]{0,3}$"))
         {
            //Console.WriteLine("Ogiltigt konto {0} - {1}", kontonr, kontonamn);
            return;
         }

         if ((firstcol == true) && (kontonr.Length == 4) && (kontonr.Substring(3, 1) == "0"))
         {
            // Ett huvudkonto i första kolumnen ska läggas upp både som ett huvudkonto och som ett underkonto
            // Lägg upp huvudkontot genom rekursion

            AddAccount(model, year, kontonr.Substring(0, 3), null, kontonamn, false, notK2, recommended);
         }


         //Console.WriteLine("{0} - {1}", kontonr, kontonamn);
         var acc = new Account
         {
            AccountID = kontonr,
            Year = year,
            Name = kontonamn,
            NotK2 = notK2,
            Recommended = recommended,
         };
         model.Accounts.Add(acc);

         AddToAccountNumber(year, kontonr, intervallslut, kontonamn, recommended, reference, acc);
      }

      private void AddToAccountNumber(string year, string kontonr, string intervallslut, string kontonamn, bool recommended, string reference, Account acc)
      {
         if ((kontonr.Length < 4) && (year != "2017"))
         {
            // Hummer för klasser, grupper och huvudkonton lägger vi bara upp om de avser aktuellt år
         }

         if (!allAccounts.ContainsKey(kontonr))
         {
            var accno = new AccountNumber
            {
               AccountId = kontonr,
               Name = kontonamn,
               LastYear = year,
               IntervalEnd = intervallslut,
               Recommended = recommended,
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
         Console.WriteLine("Laddar BAS {0}", year);
         wb = xl.Workbooks.Open(filnamn, 0, true, 6, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, ";", false, false, 0, true, 1, 0);

         Excel.Worksheet sheet = wb.Worksheets.get_Item(1);

         var columns = sheet.Columns.Count;

         var rows = sheet.Rows.Count;

         for (int row = 2; row < rows; row++)
         {
            string text = GetStringValue(sheet, row, 1);
            if (text == "END") return;

            SplitAccount(model, year, text, true);

            text = GetStringValue(sheet, row, 2);
            SplitAccount(model, year, text, false);
         }
      }

      public void Load2005(BASContext model, string year, string filnamn)
      {
         Console.WriteLine("Laddar BAS {0}", year);
         wb = xl.Workbooks.Open(filnamn, 0, true, 6, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, ";", false, false, 0, true, 1, 0);

         Excel.Worksheet sheet = wb.Worksheets.get_Item(1);

         var columns = sheet.Columns.Count;

         var rows = sheet.Rows.Count;


         for (int row = 2; row < rows; row++)
         {
            string text = GetStringValue(sheet, row, 1);
            text = text.Trim();

            if (text == "END") return;

            if (text == "BAS-konton")
            {
               // Skippa rubrikrad
            }
            else
            {
               if (text.Contains(" "))
               {
                  SplitAccount(model, year, text, true);
               }
               else
               {
                  string kontonr = GetStringValue(sheet, row, 1);
                  string kontonamn = GetStringValue(sheet, row, 2);


                  if (!string.IsNullOrWhiteSpace(kontonr))
                  {
                     AddAccount(model, year, kontonr, null, kontonamn, true, false, false);
                  }

               }

               {
                  string kontonr = GetStringValue(sheet, row, 4);
                  string kontonamn = GetStringValue(sheet, row, 5);

                  if (!string.IsNullOrWhiteSpace(kontonr))
                  {
                     // Underkonto
                     AddAccount(model, year, kontonr, null, kontonamn, false, false, false);
                  }
               }
            }
         }

      }


      private void SplitAccount(BASContext model, string year, string text, bool firstcol)
      {
         if (!string.IsNullOrWhiteSpace(text))
         {
            string part1 = text.Split(new[] { ' ' })[0];

            if (int.TryParse(part1, out int x))
            {
               // Numeriskt kontonummer - OK
               string kontonr = part1.Trim();
               string kontonamn = text.Substring(part1.Length).Trim();
               //Console.WriteLine("{0} - {1}", kontonr, kontonamn);

               AddAccount(model, year, kontonr, null, kontonamn, firstcol, false, false);
            }
            else
            {
               // Icke-numeriskt
              // Console.WriteLine("Error: {0}", text);
            }
         }
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
