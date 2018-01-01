using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;
using System.Reflection;
using System.Threading;
using SQLDatabase;

namespace SieClient
{
   class Program
   {
      // Programmet baserar sig på ett exempel från https://sieguru.se/2016/11/03/att-lasa-en-sie-fil/
      static void Main(string[] args)
      {
         using (Stream siefile = Assembly.GetExecutingAssembly().GetManifestResourceStream("SieClient.Resources.Caz 1605.sie"))
         {
            XmlSerializer mySerializer = new XmlSerializer(typeof(Sie));
            Sie siemodel = (Sie)mySerializer.Deserialize(siefile);
            var model = new BASContext(false);

            Console.WriteLine("Working...");

            foreach (var a in siemodel.Accounts)
            {
               var acc = new UsedAccount
               {
                  AccountID = a.id,
                  Name = a.name,
               };

               model.UsedAccounts.Add(acc);

            }

            model.SaveChanges();
         }
      }
   }
}
