using JournalMatcher;
using SQLDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;


namespace ConsoleApp1
{
   class Program
   {
      static void Main(string[] args)
      {
         var errors = new List<string>();

         Excel.Application xl = new Excel.Application();
         xl.SheetsInNewWorkbook = 1;
         xl.Visible = false;
         Excel.Workbook wb = xl.Workbooks.Open(@"D:\BaskontoPedia\arsredovisning-2017-09-30.xlsx", 0, true, 6, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, ";", false, false, 0, true, 1, 0);

         var model = new BASContext(false);
         var loader = new ExcelLoader(errors);
         loader.Load(model, wb, 2);  //Förvaltningsberättelse
         loader.Load(model, wb, 3);  //Resultaträkning
         loader.Load(model, wb, 5);  //Balansräkning

         model.SaveChanges();
      }
   }
}
