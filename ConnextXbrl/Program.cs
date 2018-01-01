using SQLDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnextXbrl
{
   class Program
   {
      static void Main(string[] args)
      {
         var model = new BASContext(false);

         foreach (var e in model.XbrlElements)
         {
            if (e.Konton != "")
            {
               var intervals = Intervals(e.Konton);

               foreach (var k in model.AccountNumbers)
               {
                  if (Match(intervals, k.AccountId))
                  {
                     // Lägg till referensen
                     k.XbrlElements.Add(e);
                  }
                 
               }
            }
         }

         model.SaveChanges();
      }

      private static bool Match(List<Interval> intervals, string accountID)
      {
         foreach (var i in intervals)
         {
            if ((string.Compare(accountID, i.Start) >= 0) && (string.Compare(accountID,i.End) <= 0))
            {
               return true;
            }
         }

         return false;
      }

      public static List<Interval> Intervals(string konton)
      {
         var intervals = konton.Split(new[] { ',' });

         List<Interval> _intervals = new List<Interval>();

         foreach (var i in intervals)
         {
            string start;
            string end;

            if (i.Contains("-"))
            {
               var ends = i.Split(new[] { '-' });
               start = ends[0].Trim();
               end = ends[1].Trim();
            }
            else
            {
               start = i.Trim();
               end = i.Trim();
            }

            _intervals.Add(new Interval
            {
               Start = start.Replace("x", ""),
               End = end.Replace("x", "9") 
            });
         }

         return _intervals;
      }
   }

   public class Interval
   {
      public string Start { get; set; }
      public string End { get; set; }

   }
}
