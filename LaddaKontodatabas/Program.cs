using JournalMatcher;
using SQLDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Microsoft.Interop.eXCEL;


namespace ConsoleApp1
{
   class Program
   {
      static void Main(string[] args)
      {
         var errors = new List<string>();

         var model = new BASContext(true);
         var loader = new ExcelLoader(errors);
         loader.Load2000(model, "2000", @"D:\BaskontoPedia\BAS 2000.xlsx");
         //loader.Load2005(model, "2005", @"D:\BaskontoPedia\BAS-2005-kontotabell.xls");
         //loader.Load2005(model, "2006", @"D:\BaskontoPedia\BAS-2006-kontotabell.xls");
         //loader.Load2005(model, "2007", @"D:\BaskontoPedia\BAS-2007-kontotabell.xls");
         //loader.Load2005(model, "2008", @"D:\BaskontoPedia\BAS-2008-kontotabell.xls");
         //loader.Load(model, "2009", @"D:\BaskontoPedia\BAS-2009-kontotabell.xls");
         //loader.Load(model, "2010", @"D:\BaskontoPedia\BAS-2010-kontotabell.xls");
         //loader.Load(model, "2011", @"D:\BaskontoPedia\BAS-2011-kontotabell.xls");
         //loader.Load(model, "2012", @"D:\BaskontoPedia\BAS-2012-kontotabell.xls");
         //loader.Load(model, "2013", @"D:\BaskontoPedia\BAS-2013-kontotabell.xls");
         //loader.Load(model, "2014", @"D:\BaskontoPedia\BAS-2014-kontotabell.xls");
         //loader.Load(model, "2015", @"D:\BaskontoPedia\BAS-2015-kontotabell.xls");
         //loader.Load(model, "2016", @"D:\BaskontoPedia\BAS-2016-kontotabell.xls");
         loader.Load(model, "2017", @"D:\BaskontoPedia\BAS-2017-kontotabell.xls");

         foreach (var accno in loader.allAccounts.Values)
         {
            model.AccountNumbers.Add(accno);
         }

         model.SaveChanges();
      }
   }
}
