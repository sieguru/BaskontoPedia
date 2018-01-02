using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Threading;
using SQLDatabase;
using Sie4Reader;

namespace SieClient
{
   class Program
   {
      // Programmet baserar sig på ett exempel från https://sieguru.se/2016/11/03/att-lasa-en-sie-fil/
      static void Main(string[] args)
      {
         var model = new BASContext(false);
 
         Console.WriteLine("Working...");


         Reader.Read(model, @"D:\BaskontoPedia\SIE4\BL0001_typ4.SE");
         Reader.Read(model, @"D:\BaskontoPedia\SIE4\Bokslut Norstedts SIE 4E.se");
         Reader.Read(model, @"D:\BaskontoPedia\SIE4\live2011.se");
         Reader.Read(model, @"D:\BaskontoPedia\SIE4\magenta_bokföring_SIE4E.se");
         Reader.Read(model, @"D:\BaskontoPedia\SIE4\MAMUT_SIE4_EXPORT.SE");
         Reader.Read(model, @"D:\BaskontoPedia\SIE4\sie 4.SE");
         Reader.Read(model, @"D:\BaskontoPedia\SIE4\SIE_exempelfil.se");
         Reader.Read(model, @"D:\BaskontoPedia\SIE4\Sie4 (1).se");
         Reader.Read(model, @"D:\BaskontoPedia\SIE4\sie4 (2).se");
         //new Reader(model, @"D:\BaskontoPedia\SIE4\SIE4 (3).SE");  // Trasig fil
         Reader.Read(model, @"D:\BaskontoPedia\SIE4\SIE4 (4).se");
         Reader.Read(model, @"D:\BaskontoPedia\SIE4\Sie4.se");
         Reader.Read(model, @"D:\BaskontoPedia\SIE4\Test4.SE");
         Reader.Read(model, @"D:\BaskontoPedia\SIE4\transaktioner_ovnbolag.se");
         Reader.Read(model, @"D:\BaskontoPedia\SIE4\typ4 (1).se");
        // sieMotor.LesFil(@"D:\BaskontoPedia\SIE4\XE_SIE_4_20151125095119.SE", FELHANTERING.FORTSETT_VID_FEL);

         model.SaveChanges();
      }
    }

   public class Reader
   {
      BASContext Model { get; set; }
      string Filename { get; set; }
      public static void Read(BASContext model, string filename)
      {
         var reader = new Reader
         {
            Model = model,
            Filename = filename,
         };

         reader.ReadFile();
      }
      public void ReadFile()
      {
         var sieMotor = new SieMotor();
         sieMotor.SetCallback(ItemCallback);
         sieMotor.LesFil(Filename, FELHANTERING.FORTSETT_VID_FEL);
      }

      void ItemCallback(object item)
      {
         if (item.GetType() == typeof(SieKONTO))
         {
            var konto = (SieKONTO)item;

            var acc = new UsedAccount
            {
               AccountID = konto.Kontonr,
               Name = konto.Namn,
            };

            Model.UsedAccounts.Add(acc);


            Console.WriteLine("{0} {1}", konto.Kontonr, konto.Namn);
         }
         else
         {
            Console.WriteLine("Item: {0}", item);
         }
      }
   }
}
