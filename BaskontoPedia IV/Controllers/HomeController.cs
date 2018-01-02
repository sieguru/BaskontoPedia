using SQLDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace BaskontoPedia_IV.Controllers
{
   public class History
   {
      public string Years { get; set; }
      public string Name { get; set; }

   }

   public class AccountViewModel
   {
      public AccountNumber Account { get; set; }

      //public IEnumerable<Account> HistoricAccounts { get; set; }

      public List<History> History;
      public IEnumerable<UsedAccount> UsedAccounts { get; set; }

      public AccountViewModel()
      {
         History = new List<History>();
      }

   }

   public class AccountGroupViewModel
   {
      public AccountNumber Group { get; set; }

      //public IEnumerable<Account> HistoricAccounts { get; set; }
    //  public IEnumerable<AccountNumber> Accounts { get; set; }
      //public IEnumerable<Account> UsedAccounts { get; set; }


   }

   public class XbrlViewModel
   {
      public XBRLElement Element { get; set; }

      public IEnumerable<AccountNumber> Accounts { get; set; }
   }

   public class AccountPlanViewModel
   {
      //public Account MainAccount { get; set; }

      //public IEnumerable<Account> HistoricAccounts { get; set; }
      public AccountNumber[] Classes { get; set; }
      public AccountNumber[,] Accounts { get; set; }
      //public IEnumerable<Account> UsedAccounts { get; set; }
      public AccountPlanViewModel()
      {
         Classes = new AccountNumber[10];
         Accounts = new AccountNumber[10,10];
      }

   }
   public class HomeController : Controller
   {
      BASContext model;

      public HomeController()
      {
         model = new BASContext(false);
      }

      public ActionResult Index()
      {
         var kontoPlanVM = new AccountPlanViewModel();

         var konton  = from k in model.AccountNumbers
                       select k;

         foreach (var k in konton)
         {
            if (k.AccountId.Length == 1)
            {
               int col = int.Parse(k.AccountId.Substring(0, 1));

               kontoPlanVM.Classes[col] = k;
            }

            if (k.AccountId.Length == 2)
            {
               int col = int.Parse(k.AccountId.Substring(0, 1));
               int row = int.Parse(k.AccountId.Substring(1, 1));

               kontoPlanVM.Accounts[row, col] = k;
            }
         }

         return View(kontoPlanVM);
      }

      public ActionResult Huvudkonto(string id)
      {
         ViewBag.Message = "Huvudkonto.";

         var kontoVM = new AccountViewModel();


         kontoVM.Account = (from k in model.AccountNumbers
                                where k.AccountId == id
                                select k).FirstOrDefault();

         if (kontoVM.Account.IntervalReference != null)
         {
            // Vi har träffat mitt i ett intervall - Gå till starten på intervallet
            kontoVM.Account = (from k in model.AccountNumbers
                               where k.AccountId == kontoVM.Account.IntervalReference
                               select k).FirstOrDefault();
         }


         var histKonton = from k in model.Accounts
                          where k.AccountID == kontoVM.Account.AccountId
                          orderby k.Year
                          select k;

         kontoVM.History = SummarizeHistory(histKonton);

         var usedAccounts = new List<UsedAccount>();

         foreach (var k in model.UsedAccounts)
         {
            if (k.AccountID.Substring(0, 3) == id.Substring(0, 3))
            {
               usedAccounts.Add(k);
            }
         }

         kontoVM.UsedAccounts = usedAccounts;


         return View(kontoVM);
      }

      public List<History> SummarizeHistory(IEnumerable<Account> histKonton)
      {
         List<History> history = new List<History>();

         string previousName = "";
         string lastchange = "";
         string lastyear = "";


         foreach (var k in histKonton)
         {
            if (k.Year == "2000")
            {
               previousName = k.Name.Trim();
               lastchange = k.Year;
            }
            else
            {
               if (previousName != k.Name.Trim())
               {
                  if (previousName != "")
                  {
                     history.Add(new History { Years = string.Format("{0}-{1}", lastchange, lastyear), Name = previousName });
                  }
                  previousName = k.Name.Trim();
                  lastchange = k.Year;
               }
            }
            lastyear = k.Year;
         }

         if (lastchange == "2000")
         {
            return null;
         }
         else
         {
            history.Add(new History { Years = string.Format("{0}-", lastchange), Name = previousName });

            return history;
         }
      }

      public ActionResult Kontogrupp(string id)
      {
         ViewBag.Message = "Kontogrupp.";

         var kontoGruppVM = new AccountGroupViewModel();


         kontoGruppVM.Group = (from k in model.AccountNumbers
                               where k.AccountId == id
                               select k).FirstOrDefault();

         if (kontoGruppVM.Group.IntervalReference != null)
         {
            // Vi har träffat mitt i ett intervall - Gå till starten på intervallet
            kontoGruppVM.Group = (from k in model.AccountNumbers
                               where k.AccountId == kontoGruppVM.Group.IntervalReference
                               select k).FirstOrDefault();
         }

         return View(kontoGruppVM);
      }
      public ActionResult Xbrl(string id)
      {
         ViewBag.Message = "Xbrl-element.";

         var kontoGruppVM = new XbrlViewModel();

         kontoGruppVM.Element = (from k in model.XbrlElements
                      where k.ElementName == id
                      select k).FirstOrDefault();

         var selectedAccounts = new List<AccountNumber>();

         foreach (var k in kontoGruppVM.Element.Accounts)
         {
                 selectedAccounts.Add(k);
          }


         kontoGruppVM.Accounts = selectedAccounts;

         return View(kontoGruppVM);
      }

      [HttpPost]
      public ActionResult QuickSearch(string searchWord)
      {
         if (Regex.Match(searchWord, "^[0-9]*$").Success)
         {
            if (searchWord.Length == 4)
            {
               return RedirectToAction("Huvudkonto/" + searchWord, "Home");
            }
            else if (searchWord.Length == 3)
            {
               return RedirectToAction("Huvudkonto/" + searchWord, "Home");
            }
            else if (searchWord.Length == 2)
            {
               return RedirectToAction("Kontogrupp/" + searchWord, "Home");
            }
         }

         return View("UnknownSearchString");
      }


      [HttpPost]
      public JsonResult AutoComplete(string Prefix)
      {
         //Note : you can bind same list from database  
         //List<SpeedAccount> ObjList = new List<SpeedAccount>()
         //{

         //   new SpeedAccount {Value="0999", Name="0999 Observationskonto" },
         //   new SpeedAccount {Value="1200", Name="1200  Maskiner och inventarier" },
         //   new SpeedAccount {Value="1209", Name="1209  Ack avskrivning Maskiner och" },
         //   new SpeedAccount {Value="1210", Name="1210  VM Maskiner och andra tekniska" },
         //   new SpeedAccount {Value="1219", Name="1219  Ack avskrivn maskiner o invent" },
         //   new SpeedAccount {Value="1220", Name="1220  Butiksinredning Torget 1" },
         //   new SpeedAccount {Value="1221", Name="1221  Butiksinredning, årets inköp" },
         //   new SpeedAccount {Value="1229", Name="1229  Ack avskrivning butiksinredn" },
         //   new SpeedAccount {Value="1232", Name="1232  Installation på annans fastigh" },
         //   new SpeedAccount {Value="12321", Name="12321 Inst på annans fastigh 10 år" },
         //   new SpeedAccount {Value="1239", Name="1239  Värdem. på förbättringsutg. på" },
         //   new SpeedAccount {Value="12391", Name="12391 Värdem inst på annans 10 år" },
         //   new SpeedAccount {Value="1410", Name="1410  Varulager" },
         //   new SpeedAccount {Value="1411", Name="1411  Beräknad varuförbrukning" },
         //   new SpeedAccount {Value="1480", Name="1480  Förskott till leverantörer" },
         //   new SpeedAccount {Value="1490", Name="1490  Beräknad lagerförändring" },
         //   new SpeedAccount {Value="1510", Name="1510  Kundfordringar" },
         //   new SpeedAccount {Value="1630", Name="1630  Skattekonto" },
         //   new SpeedAccount {Value="1640", Name="1640  Skattefordringar" },
         //   new SpeedAccount {Value="1650", Name="1650  Momsfordran" },
         //   new SpeedAccount {Value="1700", Name="1700  Förutbet kost, upplupna intäkt" },
         //   new SpeedAccount {Value="1760", Name="1760  Upplupna inkomsträntor" },
         //   new SpeedAccount {Value="1910", Name="1910  Kassa" },
         //   new SpeedAccount {Value="1911", Name="1911  Inhandlingskassa Lars" },
         //   new SpeedAccount {Value="1930", Name="1930  Checkräkningskonto" },
         //   new SpeedAccount {Value="2081", Name="2081  Aktiekapital" },
         //   new SpeedAccount {Value="2086", Name="2086  Reservfond" },
         //   new SpeedAccount {Value="2091", Name="2091  Balanserad vinst eller förlust" },
         //   new SpeedAccount {Value="2093", Name="2093  Erhållna aktieägartillskott" },
         //   new SpeedAccount {Value="2098", Name="2098  Vinst/förlust från föreg år" },
         //   new SpeedAccount {Value="2099", Name="2099  Årets resultat" },
         //   new SpeedAccount {Value="2126", Name="2126  Periodiseringsfond" },
         //   new SpeedAccount {Value="", Name="2330  Checkräkningskredit" },
         //   new SpeedAccount {Value="", Name="2390  Avräkn.konto Lars" },
         //   new SpeedAccount {Value="", Name="2391  Avräkn.konto Eva-Lena" },
         //   new SpeedAccount {Value="", Name="2392  Utvecklingslån ALMI" },
         //   new SpeedAccount {Value="", Name="2393  Avr.aktieägare" },
         //   new SpeedAccount {Value="", Name="2421  Ej inlösta presentkort" },
         //   new SpeedAccount {Value="", Name="2422  Försålda presentkort" },
         //   new SpeedAccount {Value="", Name="2423  Inlösta presentkort" },
         //   new SpeedAccount {Value="", Name="2440  Leverantörsskulder" },
         //   new SpeedAccount {Value="", Name="2510  Skatteskulder" },
         //   new SpeedAccount {Value="", Name="2518  Betald F-skatt" },
         //   new SpeedAccount {Value="", Name="2610  Utgående moms, 25%" },
         //   new SpeedAccount {Value="", Name="2615  Utg moms varuförvärv EU, ored" },
         //   new SpeedAccount {Value="", Name="2620  Utgående moms, 12 %" },
         //   new SpeedAccount {Value="", Name="2630  Utgående moms , 6 %" },
         //   new SpeedAccount {Value="", Name="2640  Ingående moms" },
         //   new SpeedAccount {Value="", Name="2650  Moms, redovisningskonto" },
         //   new SpeedAccount {Value="", Name="2710  Personalens källskatt" },
         //   new SpeedAccount {Value="", Name="2731  Avräkning lagstadgade sociala" },
         //   new SpeedAccount {Value="", Name="2890  Övriga kortfrisiga skulder" },
         //   new SpeedAccount {Value="", Name="2900  Upplupna kost, förutbet int" },
         //   new SpeedAccount {Value="", Name="2910  Upplupna löner" },
         //   new SpeedAccount {Value="", Name="2920  Upplupna semesterlöner" },
         //   new SpeedAccount {Value="", Name="2940  Uppl.arbetsgivaravgifter" },
         //   new SpeedAccount {Value="", Name="2941  Beräknade upplupna sociala avg" },
         //   new SpeedAccount {Value="", Name="2943  Uppl. löneskatt" },
         //   new SpeedAccount {Value="", Name="2950  Upplupna AMF-avgifter" },
         //   new SpeedAccount {Value="", Name="2960  Upplupna utgiftsräntor" },
         //   new SpeedAccount {Value="", Name="3010  Försäljning" },
         //   new SpeedAccount {Value="", Name="3021  Förs. herrdoft" },
         //   new SpeedAccount {Value="", Name="3022  Förs. damdoft" },
         //   new SpeedAccount {Value="", Name="3024  Förs. Kanebo/Sensai" },
         //   new SpeedAccount {Value="", Name="3025  Förs. E. Arden" },
         //   new SpeedAccount {Value="", Name="3031  Förs. Phytpmer" },
         //   new SpeedAccount {Value="", Name="3032  Förs. Max Factor" },
         //   new SpeedAccount {Value="", Name="3033  Förs. Isadora" },
         //   new SpeedAccount {Value="", Name="3034  Förs. Necessärer" },
         //   new SpeedAccount {Value="", Name="3035  Förs. övrigt" },
         //   new SpeedAccount {Value="", Name="3041  Förs. hudvård" },
         //   new SpeedAccount {Value="", Name="3042  Förs. kroppsvård" },
         //   new SpeedAccount {Value="", Name="3043  Förs. bijouterier" },
         //   new SpeedAccount {Value="", Name="3051  Förs. Olay" },
         //   new SpeedAccount {Value="", Name="3052  Förs. Depend" },
         //   new SpeedAccount {Value="", Name="3053  Förs. Ryding" },
         //   new SpeedAccount {Value="", Name="3081  Lämnade kassarabatter" },
         //   new SpeedAccount {Value="", Name="3110  Behandlingar" },
         //   new SpeedAccount {Value="", Name="3740  Öresutjämning" },
         //   new SpeedAccount {Value="", Name="3890  Annonsbidrag" },
         //   new SpeedAccount {Value="", Name="3910  Hyresintäkter" },
         //   new SpeedAccount {Value="", Name="3973  Vinst vid avyttring av mask. o" },
         //   new SpeedAccount {Value="", Name="3980  Erhållen bonus" },
         //   new SpeedAccount {Value="", Name="3990  Övriga ersättn och intäkter" },
         //   new SpeedAccount {Value="", Name="3994  Försäkringsersättning" },
         //   new SpeedAccount {Value="", Name="4010  Varuinköp ANVÄND INTE!!" },
         //   new SpeedAccount {Value="", Name="4011  Varuinköp EU" },
         //   new SpeedAccount {Value="", Name="4081  Erhållna kassarabatter" },
         //   new SpeedAccount {Value="", Name="4110  Inköp behandlingar" },
         //   new SpeedAccount {Value="", Name="4900  Beräknad lagerförändring" },
         //   new SpeedAccount {Value="", Name="4960  Förändr lager handelsvaror" },
         //   new SpeedAccount {Value="", Name="5010  Lokalhyra" },
         //   new SpeedAccount {Value="", Name="5070  Reparation och underhåll lokal" },
         //   new SpeedAccount {Value="", Name="5090  Övriga lokalkostnader" },
         //   new SpeedAccount {Value="", Name="5210  Hyra maskiner" },
         //   new SpeedAccount {Value="", Name="5410  Förbrukningsinventarier" },
         //   new SpeedAccount {Value="", Name="5440  Förbrukningsemballage" },
         //   new SpeedAccount {Value="", Name="5460  Förbrukningsmaterial" },
         //   new SpeedAccount {Value="", Name="5461  Förbrukning produktutveckling" },
         //   new SpeedAccount {Value="", Name="5480  Arbetskläder" },
         //   new SpeedAccount {Value="", Name="5500  Reparation och underhåll" },
         //   new SpeedAccount {Value="", Name="5710  Frakter, transport och försäkr" },
         //   new SpeedAccount {Value="", Name="5800  Resekostnader" },
         //   new SpeedAccount {Value="", Name="5900  Annonsering reklam" },
         //   new SpeedAccount {Value="", Name="5980  Sponsring" },
         //   new SpeedAccount {Value="", Name="6071  Representation, avdragsgill" },
         //   new SpeedAccount {Value="", Name="6072  Representation, ej avdragsgill" },
         //   new SpeedAccount {Value="", Name="6090  Övriga försäljningskostnader" },
         //   new SpeedAccount {Value="", Name="6110  Kontorsmaterial" },
         //   new SpeedAccount {Value="", Name="6211  Telefon" },
         //   new SpeedAccount {Value="", Name="6250  Postbefordran" },
         //   new SpeedAccount {Value="", Name="6310  Företagsförsäkringar" },
         //   new SpeedAccount {Value="", Name="6370  Bevakning och larm" },
         //   new SpeedAccount {Value="", Name="6420  Revisionsarvoden" },
         //   new SpeedAccount {Value="", Name="6530  Redovisningstjänster" },
         //   new SpeedAccount {Value="", Name="6560  Serviceavgifter branschorg" },
         //   new SpeedAccount {Value="", Name="6570  Bankkostnader" },
         //   new SpeedAccount {Value="", Name="6580  Advokat och rättegångskostn" },
         //   new SpeedAccount {Value="", Name="6590  Diverse främmande tjänster" },
         //   new SpeedAccount {Value="", Name="6970  Tidningar, facklitt." },
         //   new SpeedAccount {Value="", Name="6981  Föreningsavg, avdragsgilla" },
         //   new SpeedAccount {Value="", Name="6982  Föreningsavg, ej avdragsgilla" },
         //   new SpeedAccount {Value="", Name="6991  Övr externa kost,avdragsgilla" },
         //   new SpeedAccount {Value="", Name="6992  Övr externa kost, ej avdragsg" },
         //   new SpeedAccount {Value="", Name="7010  Löner till kollektivanställda" },
         //   new SpeedAccount {Value="", Name="7014  Löner t kollektivanst 10,21 %" },
         //   new SpeedAccount {Value="", Name="7090  Förändring av semesterlöneskul" },
         //   new SpeedAccount {Value="", Name="7220  Löner till företagsledare" },
         //   new SpeedAccount {Value="", Name="7331  Bilersättning, ej skattepl" },
         //   new SpeedAccount {Value="", Name="7410  Pensionsförsäkringsavgifter" },
         //   new SpeedAccount {Value="", Name="7510  Lagstadgade arbetsgivaravg" },
         //   new SpeedAccount {Value="", Name="7533  Särskild löneskatt, pens.kostn" },
         //   new SpeedAccount {Value="", Name="7541  Ber Soc. Avgifter" },
         //   new SpeedAccount {Value="", Name="7570  Arbetsmarknadsförsäkringar" },
         //   new SpeedAccount {Value="", Name="7580  Grupplivförsäkringspremier" },
         //   new SpeedAccount {Value="", Name="7581  Gruppförsäkring skattek K" },
         //   new SpeedAccount {Value="", Name="7610  Utbildning" },
         //   new SpeedAccount {Value="", Name="7620  Sjuk- och hälsovård" },
         //   new SpeedAccount {Value="", Name="7631  Personalrepr, avdragsgill" },
         //   new SpeedAccount {Value="", Name="7632  Personalrepr, ej avdragsgill" },
         //   new SpeedAccount {Value="", Name="7690  Övr personalkostnader" },
         //   new SpeedAccount {Value="", Name="7695  Förlikning" },
         //   new SpeedAccount {Value="", Name="7699  Övriga personalomkostnader" },
         //   new SpeedAccount {Value="", Name="7830  Avskr maskiner och inventarier" },
         //   new SpeedAccount {Value="", Name="8300  Ränteintäkter" },
         //   new SpeedAccount {Value="", Name="8314  Ränteintäkter skattefria" },
         //   new SpeedAccount {Value="", Name="8390  Övriga finansiella intäkter" },
         //   new SpeedAccount {Value="", Name="8400  Räntekostnader" },
         //   new SpeedAccount {Value="8414", Name="8414  Räntekostnad skattekonto" },
         //   new SpeedAccount {Value="8490", Name="8490  Övriga finansiella kostnader" },
         //   new SpeedAccount {Value="8811", Name="8811  Avsättning periodiseringsfond" },
         //   new SpeedAccount {Value="8819", Name="8819  Återföring periodiseringsfond" },
         //   new SpeedAccount {Value="8910", Name="8910  Skatt på årets resultat" },
         //   new SpeedAccount {Value="8920", Name="8920  Skatt pga ändrad beskattnig" },
         //   new SpeedAccount {Value="8999", Name="8999  Redovisat resultat" },
         //   new SpeedAccount {Value="9101", Name="9101  Statistikkonto antal kunder" },
         //   new SpeedAccount {Value="9199", Name="9199  Motkonto statistik" }
         //};
         ////Searching records from list using LINQ query  
         //var CityName = (from N in ObjList
         //                where N.Name.StartsWith(Prefix)
         //                select new { N.Name, N.Value });
         return Json("CityName", JsonRequestBehavior.AllowGet);
      }
   }
}