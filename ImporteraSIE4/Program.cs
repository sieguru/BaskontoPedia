using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Threading;
using SQLDatabase;

namespace SieClient
{

   class Program
   {
      //Encoding enc = Encoding.GetEncoding(1252);
      //string result = enc.GetString(bytes);
      //var asciiBytes = new byte[] { 0x94 }; // 94 corresponds represents 'ö' in code page 437.

      //Encoding OemEncoding = Encoding.GetEncoding(437);

      //var unicodeString = asciiEncoding.GetString(asciiBytes);

      // Programmet baserar sig på ett exempel från https://sieguru.se/2016/11/03/att-lasa-en-sie-fil/
      static void Main(string[] args)
      {
         //var model = new BASContext(false);
         var sieMotor = new SieMotor();
         sieMotor.SetCallback_Konto(KontoCallback);

         Console.WriteLine("Working...");
 //        var text = File.ReadAllText(@"D:\BaskontoPedia\SIE4\transaktioner_ovnbolag.se", Encoding.GetEncoding(437));


         sieMotor.LesFil(@"D:\BaskontoPedia\SIE4\transaktioner_ovnbolag.se", FELHANTERING.FORTSETT_VID_FEL);
         sieMotor.LesFil(@"D:\BaskontoPedia\SIE4\transaktioner_ovnbolag.se", FELHANTERING.FORTSETT_VID_FEL);
         sieMotor.LesFil(@"D:\BaskontoPedia\SIE4\transaktioner_ovnbolag.se", FELHANTERING.FORTSETT_VID_FEL);
         sieMotor.LesFil(@"D:\BaskontoPedia\SIE4\transaktioner_ovnbolag.se", FELHANTERING.FORTSETT_VID_FEL);
         sieMotor.LesFil(@"D:\BaskontoPedia\SIE4\transaktioner_ovnbolag.se", FELHANTERING.FORTSETT_VID_FEL);

         //model.SaveChanges();
      }

      static void KontoCallback(string kontonr, string namn)
      {
         Console.WriteLine("{0} {1}", kontonr, namn);
      }
   }
}
