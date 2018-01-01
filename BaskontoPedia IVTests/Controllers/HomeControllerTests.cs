using Microsoft.VisualStudio.TestTools.UnitTesting;
using BaskontoPedia_IV.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLDatabase;

namespace BaskontoPedia_IV.Controllers.Tests
{
   [TestClass()]
   public class HomeControllerTests
   {
      [TestMethod()]
      public void SummarizeHistoryTest()
      {
         var c = new HomeController();

         var accounts = new List<Account> {
            new Account { Year="2000", Name="A" },
            new Account { Year="2001", Name="A" },
            new Account { Year="2005", Name="A" },
            new Account { Year="2006", Name="A" },
            new Account { Year="2007", Name="B" },
            new Account { Year="2008", Name="B" },

         };

         var summary = c.SummarizeHistory(accounts);

         Assert.AreEqual(2, summary.Count);

      }

      [TestMethod()]
      public void SummarizeHistoryTest2()
      {
         var c = new HomeController();

         var accounts = new List<Account> {
            new Account { Year="2005", Name="A" },
            new Account { Year="2006", Name="A" },
            new Account { Year="2007", Name="A" },
            new Account { Year="2008", Name="A" },

         };

         var summary = c.SummarizeHistory(accounts);

         Assert.AreEqual(1, summary.Count);

      }
   }
}