using Sie4Reader;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SieClient
{
   enum Id
   {
      S_FLAGGA, S_PROGRAM, S_FORMAT, S_GEN, S_FILTYP, S_SIETYP,
      S_PROSA, S_ENHET, S_FNR, S_FTYP, S_ORGNR, S_BKOD, S_ADRESS, S_FNAMN, S_RAR,
      S_TAXAR, S_OMFATTN, S_KONTO, S_KTYP, S_SRU, S_DIM, S_UNDERDIM,
      S_OBJEKT, S_IB, S_UB, S_RES, S_OUB, S_OIB, S_PSALDO, S_PBUDGET,
      S_VER, S_TRANS, S_RTRANS, S_BTRANS, S_KPTYP, S_LEV, S_LEVADR, S_LEVBETINFO, S_LEVREF,
      S_KUND, S_KUNDADR, S_KUNDBETINFO, S_KUNDREF, S_OKEND, S_KSUMMA, S_VALUTA
   }

   public enum FELHANTERING
   {
      FORTSETT_VID_FEL,
      AVBRYT_VID_FEL
   }



   class Posttyp
   {
      public string Namn { get; set; }

      public Id Id { get; set; }

      //Fält tre ange vilken typ fälten är i. En bokstav för varje fält som finns under posttypen.
      //Stor bokstav anger att fältet är obligatoriskt, liten bokstav anger att det kan förekomma.
      //B = Belopp
      //D = Datum
      //N = Numerisk
      //S = Sträng
      //A = Årsnummer
      //Y = År
      //M = Månad
      //O = Objekt lista
      //P = Period
      public string Pattern { get; set; }

      // O = Obligatorisk post. Saknas den ges ett fel.
      // o = Obligatorisk post. Saknas den ges en varning.
      // V = Valfri post. Får både förekomma och saknas
      // W = Förekommer normalt inte för denna filtyp. Varna om den förekommer.
      // F = Får ej förekomma för denna filtyp. Ge fel om den förekommer.
      public string Oblig { get; set; }

      public bool substart { get; set; }  // Anger att posten ska ha subposter
      public bool subpost { get; set; }   // Anger att posten ska vara en subpost

      public int antal { get; set; }     // Antal förekomster i filen
      public int errantal { get; set; }	// Antal felmeddelanden på denna posttyp

   }

   class CImportfelBas
   {
   }

   public delegate void DelItem(object item);

   public class SieMotor
   {
      DelItem _Callback;

      public void SetCallback(DelItem del)
      {
         _Callback += del;
      }

#if false
      static List<Posttyp> Posttyper = new List<Posttyp>{
         new Posttyp{},
         new Posttyp {Namn = "#FLAGGA",       Id = Id.S_FLAGGA,      Pattern = "N",        Oblig="OOOOO",  substart=false, subpost=false},

   new Posttyp {Namn = "#PROGRAM",      Id = Id.S_PROGRAM,     Pattern = "Ss",       Oblig="OOOOO",  substart=false, subpost=false},
   new Posttyp {Namn = "#FORMAT",       Id = Id.S_FORMAT,      Pattern = "S",        Oblig="OOOOO",  substart=false, subpost=false},
   new Posttyp {Namn = "#GEN",          Id = Id.S_GEN,         Pattern = "Ds",       Oblig="OOOOO",  substart=false, subpost=false},
   new Posttyp {Namn = "#FILTYP",       Id = Id.S_FILTYP,      Pattern = "S",        Oblig="WWWWW",  substart=false, subpost=false},      // Har utgått
   new Posttyp {Namn = "#SIETYP",       Id = Id.S_SIETYP,      Pattern = "N",        Oblig="VOOOO",  substart=false, subpost=false},
   new Posttyp {Namn = "#PROSA",        Id = Id.S_PROSA,       Pattern = "S",        Oblig="VVVVV",  substart=false, subpost=false},
   new Posttyp {Namn = "#ENHET",        Id = Id.S_ENHET,       Pattern = "NS",       Oblig="VVVVV",  substart=false, subpost=false},

   new Posttyp {Namn = "#FNR",          Id = Id.S_FNR,         Pattern = "S",        Oblig="VVVVV",  substart=false, subpost=false},
   new Posttyp {Namn = "#FTYP",         Id = Id.S_FTYP,         Pattern = "S",       Oblig="VVVVV",  substart=false, subpost=false},
   new Posttyp {Namn = "#ORGNR",        Id = Id.S_ORGNR,       Pattern = "Snn",      Oblig="oooVo",  substart=false, subpost=false},
   new Posttyp {Namn = "#BKOD",         Id = Id.S_BKOD,        Pattern = "N",        Oblig="VVVVV",  substart=false, subpost=false},
   new Posttyp {Namn = "#ADRESS",       Id = Id.S_ADRESS,      Pattern = "SSSS",     Oblig="VVVVV",  substart=false, subpost=false},
   new Posttyp {Namn = "#FNAMN",        Id = Id.S_FNAMN,       Pattern = "S",        Oblig="OOOOO",  substart=false, subpost=false},
   new Posttyp {Namn = "#RAR",          Id = Id.S_RAR,         Pattern = "ADD",      Oblig="OOOVO",  substart=false, subpost=false},
   new Posttyp {Namn = "#TAXAR",        Id = Id.S_TAXAR,       Pattern = "Ys",       Oblig="VVVFV",  substart=false, subpost=false},

   new Posttyp {Namn = "#OMFATTN",      Id = Id.S_OMFATTN,     Pattern = "D",        Oblig="WOOFW",  substart=false, subpost=false},

   new Posttyp {Namn = "#KONTO",        Id = Id.S_KONTO,       Pattern = "NS",       Oblig="OOOoO",  substart=false, subpost=false},
   new Posttyp {Namn = "#KTYP",         Id = Id.S_KTYP,        Pattern = "NS",       Oblig="VVVVV",  substart=false, subpost=false},
   new Posttyp {Namn = "#SRU",          Id = Id.S_SRU,         Pattern = "NN",       Oblig="OOOFV",  substart=false, subpost=false},

   new Posttyp {Namn = "#DIM",          Id = Id.S_DIM,         Pattern = "NS",       Oblig="FFVVV",  substart=false, subpost=false},
   new Posttyp {Namn = "#UNDERDIM",     Id = Id.S_UNDERDIM,    Pattern = "NSN",      Oblig="FFVVV",  substart=false, subpost=false},
   new Posttyp {Namn = "#OBJEKT",       Id = Id.S_OBJEKT,      Pattern = "NSS",      Oblig="FFVVV",  substart=false, subpost=false},



   new Posttyp {Namn = "#IB",           Id = Id.S_IB,          Pattern = "ANBb",      Oblig="oooFo",  substart=false, subpost=false},   // o->O ?
   new Posttyp {Namn = "#UB",           Id = Id.S_UB,          Pattern = "ANBb",      Oblig="oooFo",  substart=false, subpost=false},   // o->O ?
   new Posttyp {Namn = "#RES",          Id = Id.S_RES,         Pattern = "ANBb",      Oblig="OOOFO",  substart=false, subpost=false},
   new Posttyp {Namn = "#OUB",          Id = Id.S_OUB,         Pattern = "ANSBb",     Oblig="FFoFF",  substart=false, subpost=false},
   new Posttyp {Namn = "#OIB",          Id = Id.S_OIB,         Pattern = "ANSBb",     Oblig="FFoFF",  substart=false, subpost=false},
   new Posttyp {Namn = "#PSALDO",       Id = Id.S_PSALDO,      Pattern = "AMNOBb",    Oblig="FOOFF",  substart=false, subpost=false},
   new Posttyp {Namn = "#PBUDGET",      Id = Id.S_PBUDGET,     Pattern = "AMNOBb",    Oblig="FVVFF",  substart=false, subpost=false},

   new Posttyp {Namn = "#VER",          Id = Id.S_VER,         Pattern = "SSDss",    Oblig="FFFOO",  substart=true, subpost=false},  // Skulle varit SSDsd
                                                                  // men special för EkoMix
   new Posttyp {Namn = "#TRANS",        Id = Id.S_TRANS,       Pattern = "NOBdsbs",  Oblig="FFFOO",  substart=false, subpost=true},
   new Posttyp {Namn = "#RTRANS",       Id = Id.S_RTRANS,      Pattern = "NOBdsbs",  Oblig="FFFVV",  substart=false, subpost=true},
   new Posttyp {Namn = "#BTRANS",       Id = Id.S_BTRANS,      Pattern = "NOBdsbs",  Oblig="FFFVV",  substart=false, subpost=true},

   new Posttyp {Namn = "#KPTYP",        Id = Id.S_KPTYP,       Pattern = "S",        Oblig="VVVVV",  substart=false, subpost=false},

   new Posttyp {Namn = "#LEV",          Id = Id.S_LEV,         Pattern = "NSs",      Oblig="VVVVV",  substart=false, subpost=false},
   new Posttyp {Namn = "#LEVADR",       Id = Id.S_LEVADR,      Pattern = "Nssss",    Oblig="VVVVV",  substart=false, subpost=false},
   new Posttyp {Namn = "#LEVBETINFO",   Id = Id.S_LEVBETINFO,  Pattern = "NsN",      Oblig="VVVVV",  substart=false, subpost=false},
   new Posttyp {Namn = "#LEVREF",       Id = Id.S_LEVREF,      Pattern = "SSD",      Oblig="VVVVV",  substart=false, subpost=true},

   new Posttyp {Namn = "#KUND",         Id = Id.S_KUND,        Pattern = "NSss",     Oblig="VVVVV",  substart=false, subpost=false},
   new Posttyp {Namn = "#KUNDADR",      Id = Id.S_KUNDADR,     Pattern = "Nssss",    Oblig="VVVVV",  substart=false, subpost=false},
   new Posttyp {Namn = "#KUNDREF",      Id = Id.S_KUNDREF,     Pattern = "NDS",      Oblig="VVVVV",  substart=false, subpost=true},
   new Posttyp {Namn = "#KSUMMA",       Id = Id.S_KSUMMA,      Pattern = "n",        Oblig="VVVVV",  substart=false, subpost=false},

   new Posttyp {Namn = "#VALUTA",       Id = Id.S_VALUTA,      Pattern = "S",        Oblig="VVVVV",  substart=false, subpost=false},

};
#endif
      int Radnr;
      bool m_ver_flagga;
      bool m_klammer_aktiv;

 
      //public SieMotor()
      //{
      //   // Default constructor

      //   //m_oemkonv = true;

      //   //if (AfxGetApp()->GetProfileInt("AutoImport", "ANSI", 0))
      //   //{
      //   //   // Om man i registryt har angett att SIE-filerna innehåller ANSI
      //   //   // så sätts ANSI som default

      //   //   m_oemkonv = false;
      //   //}


      //   CImportfelBas m_felx = new CImportfelBas();
      //}

      public void LesFil(string filnamn, FELHANTERING felhantering)
      {
         Radnr = 0;
         string line;

         using (StreamReader file = new StreamReader(filnamn, Encoding.GetEncoding(437)))
         {
            while ((line = file.ReadLine()) != null)
            {
               Radnr++;  
               line = line.Trim();

               if (line == "{")
               {
                  Startklammer();
               }
               else if (line == "}")
               {
                  Slutklammer();
               }
               else
               {
                  ProcessLine(line);
               }
            }

            file.Close();
         }
      }


      void Startklammer()
      {
         //if (m_klammer_aktiv)
         //{
         //   // Det finns redan en aktiv klammer - två tillåts ej

         //   GeSyntaxfel(buffert, 0, DUBBEL_KLAMMER);

         //   if (felhantering == AVBRYT_VID_FEL)
         //   {
         //      return SIESTATUS_SYNTAXFEL;
         //   }
         //   else
         //   {
         //      m_felantal++;
         //   }
         //}
         //else
         //{
         //   // Kontrollera att föregående post var en #VER

         //   if (m_ver_flagga)
         //   {
         //      // Det är OK - Vi är nu inom klamrat område
         //      m_klammer_aktiv = 1;

         //      // Gör callback för att informera tillämpningen om
         //      // att verifikatet startar

         //      ParentesRutin(PARENTES_START);
         //   }
         //   else
         //   {
         //      // Fel - Klammer kan endast följa en post av typ #VER

         //      GeSyntaxfel(buffert, 0, KLAMMER_UTAN_VER);

         //      if (felhantering == AVBRYT_VID_FEL)
         //      {
         //         return SIESTATUS_SYNTAXFEL;
         //      }
         //      else
         //      {
         //         m_felantal++;
         //      }

         //      m_klammer_aktiv = 1;
         //   }
         //}
      }

      void Slutklammer()
      {
         //if (m_klammer_aktiv)
         //{
         //   // Det finns en aktiv klammer - OK

         //   m_klammer_aktiv = 0;
         //   m_ver_flagga = 0;    // det måste nu dyka upp en ny VER-post
         //                        // innan vi godkänner en startklammer

         //   // Gör callback för att informera tillämpningen om
         //   // att verifikatet är slut

         //   status = ParentesRutin(PARENTES_SLUT);

         //   if (status != SIESTATUS_OK)
         //   {
         //      // Applikationen har detekterat ett fel mellan parenteserna,
         //      // t ex obalans i ett verifikat

         //      if (felhantering == AVBRYT_VID_FEL)
         //      {
         //         return status;
         //      }
         //      else
         //      {
         //         m_datafelantal++;
         //      }
         //   }
         //}
         //else
         //{
         //   GeSyntaxfel(buffert, 0, KLAMMER_MATCHFEL);

         //   if (felhantering == AVBRYT_VID_FEL)
         //   {
         //      return SIESTATUS_SYNTAXFEL;
         //   }
         //   else
         //   {
         //      m_felantal++;
         //   }
         //}
      }

      void ProcessLine(string line)
      {

         //if (m_oemkonv == false)
         //{
         //   ::CharToOem(buffert, buffert);
         //}



         // Uppdatera progressbar

         //m_felx->ReportProgress(m_lesta_bytes * 100 / m_lengd);


         // Behandla raden



         var del_felt = Split(line);  // Dela upp raden i sina delar

         // Raden blev korrekt uppdelad

         if (del_felt.Count() == 0)
         {
            // Vi har hittat en tomrad
            return;
         }
         else
         {
            if (m_ver_flagga && !m_klammer_aktiv)
            {
               // Vi borde ha fått en startklammer här

               //GeSyntaxfel(buffert, 0, KLAMMER_SAKNAS);

               //m_ver_flagga = 0;

               //if (felhantering == AVBRYT_VID_FEL)
               //{
               //   DeallokeraDelfelt(del_felt);
               //   return SIESTATUS_SYNTAXFEL;
               //}
               //else
               //{
               //   m_felantal++;
            }
         }

#if false
         ret = KontrolleraPost(buffert, del_felt, &i);

               if (ret == false)
               {
                  // Gör callback till applikationen för att fråga den om
                  // inläsning ska avbrytas då en ogiltig post stötts på

                  ret = IgnoreraPostfel(SIE_posttyper[i].posttyp);

                  if (ret == false)
                  {
                     // Applikationsprogrammet behöver poster av denna typ och
                     // har därför valt att avbryta med ett felmeddelnade

                     DeallokeraDelfelt(del_felt);
                     return SIESTATUS_POSTFEL;
                  }
                  else
                  {
                     // Felet har visserligen rapporterats men ignoreras eftersom
                     // denna posttyp inte behövs för importen

                     //GeSyntaxfel(buffert, -1, OBETYDLIGT_SYNTAXFEL);
                  }
               }

               // Om man inte fått något allvarligt fel på posttypen ska vi här
               // göra en callback för att informera det anropande programmet om
               // postens innehåll
#endif
         switch (del_felt[0])
         {
            case "#FLAGGA": ProcessFLAGGA(del_felt); break;
            case "#PROGRAM": ProcessPROGRAM(del_felt); break;
            case "#FORMAT": ProcessFORMAT(del_felt); break;
            case "#GEN": ProcessGEN(del_felt); break;
            case "#FILTYP": ProcessFILTYP(del_felt); break;
            case "#SIETYP": ProcessSIETYP(del_felt); break;
            case "#PROSA": ProcessPROSA(del_felt); break;
            case "#ENHET": ProcessENHET(del_felt); break;
            case "#FNR": ProcessFNR(del_felt); break;
            case "#FTYP": ProcessFTYP(del_felt); break;
            case "#ORGNR": ProcessORGNR(del_felt); break;
            case "#BKOD": ProcessBKOD(del_felt); break;
            case "#ADRESS": ProcessADRESS(del_felt); break;
            case "#FNAMN": ProcessFNAMN(del_felt); break;
            case "#RAR": ProcessRAR(del_felt); break;
            case "#TAXAR": ProcessTAXAR(del_felt); break;
            case "#OMFATTN": ProcessOMFATTN(del_felt); break;
            case "#KONTO": ProcessKONTO(del_felt); break;
            case "#KTYP": ProcessKTYP(del_felt); break;
            case "#SRU": ProcessSRU(del_felt); break;
            case "#DIM": ProcessDIM(del_felt); break;
            case "#UNDERDIM": ProcessUNDERDIM(del_felt); break;
            case "#OBJEKT": ProcessOBJEKT(del_felt); break;
            case "#IB": ProcessIB(del_felt); break;
            case "#UB": ProcessUB(del_felt); break;
            case "#RES": ProcessRES(del_felt); break;
            case "#OUB": ProcessOUB(del_felt); break;
            case "#OIB": ProcessOIB(del_felt); break;
            case "#PSALDO": ProcessPSALDO(del_felt); break;
            case "#PBUDGET": ProcessPBUDGET(del_felt); break;
            case "#VER": ProcessVER(del_felt); break;
            case "#TRANS": ProcessTRANS(del_felt); break;
            case "#RTRANS": ProcessRTRANS(del_felt); break;
            case "#BTRANS": ProcessBTRANS(del_felt); break;
            case "#KPTYP": ProcessKPTYP(del_felt); break;
            case "#LEV": ProcessLEV(del_felt); break;
            case "#LEVADR": ProcessLEVADR(del_felt); break;
            case "#LEVBETINFO": ProcessLEVBETINFO(del_felt); break;
            case "#LEVREF": ProcessLEVREF(del_felt); break;
            case "#KUND": ProcessKUND(del_felt); break;
            case "#KUNDADR": ProcessKUNDADR(del_felt); break;
            case "#KUNDBETINFO": ProcessKUNDBETINFO(del_felt); break;
            case "#KUNDREF": ProcessKUNDREF(del_felt); break;
            case "#KSUMMA": ProcessKSUMMA(del_felt); break;
            case "#VALUTA": ProcessVALUTA(del_felt); break;

            default:
               // Okänd posttyp
               break;
         }


         //if (_strcmpi(del_felt[0].pek, "#SIETYP") == 0)
         //{
         //   m_sietyp = atoi(del_felt[1].pek);
         //}

         //if (strstr(m_filnamn, ".SI") == 0 && m_sietyp == 4)
         //{
         //   m_sietyp = 5;    // Typ 5 får symbolisera typ 4 med .SE
         //}
      }

      static Regex regexQuotedString = new Regex("^\\s*\"([^\"^}]*)\"", RegexOptions.CultureInvariant); // "^\\s*([\"'])(?:(?=(\\\\?))\\2.)*?\\1"); 
      static Regex regexCurlyBrackets = new Regex("^\\s*\\{(.+)\\}", RegexOptions.CultureInvariant);
      static Regex regexLiteral = new Regex("^\\s*([^\\s^\"]+)", RegexOptions.CultureInvariant);

      string[] Split(string line)
      {
         var parts = new List<string>();

         // Byt ut alla escapade fnuttar mot något unikt och identifierbart (dvs ngt som normalt inte förekommer i kontoamn, vertexter e.d.
         line = line.Replace("\\\"", "*#*#*#*");

         // ^\s*(["'])(?:(?=(\\?))\2.)*?\1   - Matchar inledande whitespace och quoted string
         // ^\s*([^\s^"]+)          - Matchar inledande sträng utan whitespace och fnuttar
         // ^\s*\{(.+)\}            - Matchar klamrar med godtyckliga tecken mellan dem
         while (line.Length > 0)
         {
            Match matchQuotedString = regexQuotedString.Match(line); // "^\\s*([\"'])(?:(?=(\\\\?))\\2.)*?\\1"); 
            Match matchCurlyBrackets = regexCurlyBrackets.Match(line);
            Match matchLiteral = regexLiteral.Match(line);

            if (matchQuotedString.Success)
            {
               parts.Add(matchQuotedString.Groups[1].Value);

               line = line.Substring(matchQuotedString.Groups[0].Length);
            }
            else if (matchCurlyBrackets.Success)
            {
               parts.Add(matchCurlyBrackets.Groups[1].Value);

               line = line.Substring(matchCurlyBrackets.Groups[0].Length);
            }
            else if (matchLiteral.Success)
            {
               parts.Add(matchLiteral.Groups[1].Value);

               line = line.Substring(matchLiteral.Groups[0].Length);
            }
            else
            {
               Console.WriteLine("Error: {0}", line);
               line = "";
            }
         }

         return parts.ToArray();
      }

      private void ProcessFLAGGA(string[] del_felt)
      {
         var item = new SieFLAGGA
         {
            //Kontonr = del_felt[1],
            //Namn = del_felt[2],
         };

         _Callback?.Invoke(item);
      }
      private void ProcessPROGRAM(string[] del_felt)
      {
         var item = new SiePROGRAM
         {
            //Kontonr = del_felt[1],
            //Namn = del_felt[2],
         };

         _Callback?.Invoke(item);
      }
      private void ProcessFORMAT(string[] del_felt)
      {
         var item = new SieFORMAT
         {
            //Kontonr = del_felt[1],
            //Namn = del_felt[2],
         };

         _Callback?.Invoke(item);
      }
      private void ProcessGEN(string[] del_felt)
      {
         var item = new SieGEN
         {
            //Kontonr = del_felt[1],
            //Namn = del_felt[2],
         };

         _Callback?.Invoke(item);
      }
      private void ProcessFILTYP(string[] del_felt)
      {
         var item = new SieFILTYP
         {
            //Kontonr = del_felt[1],
            //Namn = del_felt[2],
         };

         _Callback?.Invoke(item);
      }
      private void ProcessSIETYP(string[] del_felt)
      {
         var item = new SieSIETYP
         {
            //Kontonr = del_felt[1],
            //Namn = del_felt[2],
         };

         _Callback?.Invoke(item);
      }
      private void ProcessPROSA(string[] del_felt)
      {
         var item = new SiePROSA
         {
            //Kontonr = del_felt[1],
            //Namn = del_felt[2],
         };

         _Callback?.Invoke(item);
      }
      private void ProcessENHET(string[] del_felt)
      {
         var item = new SieENHET
         {
            //Kontonr = del_felt[1],
            //Namn = del_felt[2],
         };

         _Callback?.Invoke(item);
      }
      private void ProcessFNR(string[] del_felt)
      {
         var item = new SieFNR
         {
            //Kontonr = del_felt[1],
            //Namn = del_felt[2],
         };

         _Callback?.Invoke(item);
      }
      private void ProcessFTYP(string[] del_felt)
      {
         var item = new SieFTYP
         {
            //Kontonr = del_felt[1],
            //Namn = del_felt[2],
         };

         _Callback?.Invoke(item);
      }
      private void ProcessORGNR(string[] del_felt)
      {
         var item = new SieORGNR
         {
            //Kontonr = del_felt[1],
            //Namn = del_felt[2],
         };

         _Callback?.Invoke(item);
      }
      private void ProcessBKOD(string[] del_felt)
      {
         var item = new SieBKOD
         {
            //Kontonr = del_felt[1],
            //Namn = del_felt[2],
         };

         _Callback?.Invoke(item);
      }
      private void ProcessADRESS(string[] del_felt)
      {
         var item = new SieADRESS
         {
            //Kontonr = del_felt[1],
            //Namn = del_felt[2],
         };

         _Callback?.Invoke(item);
      }
      private void ProcessFNAMN(string[] del_felt)
      {
         var item = new SieFNAMN
         {
            //Kontonr = del_felt[1],
            //Namn = del_felt[2],
         };

         _Callback?.Invoke(item);
      }
      private void ProcessRAR(string[] del_felt)
      {
         var item = new SieRAR
         {
            //Kontonr = del_felt[1],
            //Namn = del_felt[2],
         };

         _Callback?.Invoke(item);
      }
      private void ProcessTAXAR(string[] del_felt)
      {
         var item = new SieTAXAR
         {
            //Kontonr = del_felt[1],
            //Namn = del_felt[2],
         };

         _Callback?.Invoke(item);
      }
      private void ProcessOMFATTN(string[] del_felt)
      {
         var item = new SieOMFATTN
         {
            //Kontonr = del_felt[1],
            //Namn = del_felt[2],
         };

         _Callback?.Invoke(item);
      }
      private void ProcessKONTO(string[] del_felt)
      {
         var item = new SieKONTO
         {
            Kontonr = del_felt[1],
            Namn = del_felt[2],
         };

         _Callback?.Invoke(item);
      }
      private void ProcessKTYP(string[] del_felt)
      {
         var item = new SieKPTYP
         {
            //Kontonr = del_felt[1],
            //Namn = del_felt[2],
         };

         _Callback?.Invoke(item);
      }
      private void ProcessSRU(string[] del_felt)
      {
         var item = new SieSRU
         {
            //Kontonr = del_felt[1],
            //Namn = del_felt[2],
         };

         _Callback?.Invoke(item);
      }
      private void ProcessDIM(string[] del_felt)
      {
         var item = new SieDIM
         {
            //Kontonr = del_felt[1],
            //Namn = del_felt[2],
         };

         _Callback?.Invoke(item);
      }
      private void ProcessUNDERDIM(string[] del_felt)
      {
         var item = new SieUNDERDIM
         {
            //Kontonr = del_felt[1],
            //Namn = del_felt[2],
         };

         _Callback?.Invoke(item);
      }
      private void ProcessOBJEKT(string[] del_felt)
      {
         var item = new SieOBJEKT
         {
            //Kontonr = del_felt[1],
            //Namn = del_felt[2],
         };

         _Callback?.Invoke(item);
      }
      private void ProcessIB(string[] del_felt)
      {
         var item = new SieIB
         {
            //Kontonr = del_felt[1],
            //Namn = del_felt[2],
         };

         _Callback?.Invoke(item);
      }
      private void ProcessUB(string[] del_felt)
      {
         var item = new SieUB
         {
            //Kontonr = del_felt[1],
            //Namn = del_felt[2],
         };

         _Callback?.Invoke(item);
      }
      private void ProcessRES(string[] del_felt)
      {
         var item = new SieRES
         {
            //Kontonr = del_felt[1],
            //Namn = del_felt[2],
         };

         _Callback?.Invoke(item);
      }
      private void ProcessOUB(string[] del_felt)
      {
         var item = new SieOUB
         {
            //Kontonr = del_felt[1],
            //Namn = del_felt[2],
         };

         _Callback?.Invoke(item);
      }
      private void ProcessOIB(string[] del_felt)
      {
         var item = new SieOIB
         {
            //Kontonr = del_felt[1],
            //Namn = del_felt[2],
         };

         _Callback?.Invoke(item);
      }
      private void ProcessPSALDO(string[] del_felt)
      {
         var item = new SiePSALDO
         {
            //Kontonr = del_felt[1],
            //Namn = del_felt[2],
         };

         _Callback?.Invoke(item);
      }
      private void ProcessPBUDGET(string[] del_felt)
      {
         var item = new SiePBUDGET
         {
            //Kontonr = del_felt[1],
            //Namn = del_felt[2],
         };

         _Callback?.Invoke(item);
      }
      private void ProcessVER(string[] del_felt)
      {
         var item = new SieVER
         {
            //Kontonr = del_felt[1],
            //Namn = del_felt[2],
         };

         _Callback?.Invoke(item);
      }
      private void ProcessTRANS(string[] del_felt)
      {
         var item = new SieTRANS
         {
            //Kontonr = del_felt[1],
            //Namn = del_felt[2],
         };
         //throw new NotImplementedException();
         var konto = del_felt[1];
         var belopp = decimal.Parse(del_felt[3], CultureInfo.InvariantCulture);

         _Callback?.Invoke(item);
      }
      private void ProcessRTRANS(string[] del_felt)
      {
         var item = new SieRTRANS
         {
            //Kontonr = del_felt[1],
            //Namn = del_felt[2],
         };

         _Callback?.Invoke(item);
      }
      private void ProcessBTRANS(string[] del_felt)
      {
         var item = new SieBTRANS
         {
            //Kontonr = del_felt[1],
            //Namn = del_felt[2],
         };

         _Callback?.Invoke(item);
      }
      private void ProcessKPTYP(string[] del_felt)
      {
         var item = new SieKPTYP
         {
            //Kontonr = del_felt[1],
            //Namn = del_felt[2],
         };

         _Callback?.Invoke(item);
      }
      private void ProcessLEV(string[] del_felt)
      {
         var item = new SieLEV
         {
            //Kontonr = del_felt[1],
            //Namn = del_felt[2],
         };

         _Callback?.Invoke(item);
      }
      private void ProcessLEVADR(string[] del_felt)
      {
         var item = new SieLEVADR
         {
            //Kontonr = del_felt[1],
            //Namn = del_felt[2],
         };

         _Callback?.Invoke(item);
      }
      private void ProcessLEVBETINFO(string[] del_felt)
      {
         var item = new SieLEVBETINFO
         {
            //Kontonr = del_felt[1],
            //Namn = del_felt[2],
         };

         _Callback?.Invoke(item);
      }
      private void ProcessLEVREF(string[] del_felt)
      {
         var item = new SieLEVREF
         {
            //Kontonr = del_felt[1],
            //Namn = del_felt[2],
         };

         _Callback?.Invoke(item);
      }
      private void ProcessKUND(string[] del_felt)
      {
         var item = new SieKUND
         {
            //Kontonr = del_felt[1],
            //Namn = del_felt[2],
         };

         _Callback?.Invoke(item);
      }
      private void ProcessKUNDADR(string[] del_felt)
      {
         var item = new SieKUNDADR
         {
            //Kontonr = del_felt[1],
            //Namn = del_felt[2],
         };

         _Callback?.Invoke(item);
      }
      private void ProcessKUNDBETINFO(string[] del_felt)
      {
         var item = new SieKUNDBETINFO
         {
            //Kontonr = del_felt[1],
            //Namn = del_felt[2],
         };

         _Callback?.Invoke(item);
      }
      private void ProcessKUNDREF(string[] del_felt)
      {
         var item = new SieKUNDREF
         {
            //Kontonr = del_felt[1],
            //Namn = del_felt[2],
         };

         _Callback?.Invoke(item);
      }
       private void ProcessKSUMMA(string[] del_felt)
      {
         var item = new SieKSUMMA
         {
            //Kontonr = del_felt[1],
            //Namn = del_felt[2],
         };

         _Callback?.Invoke(item);
      }
      private void ProcessVALUTA(string[] del_felt)
      {
         var item = new SieVALUTA
         {
            //Kontonr = del_felt[1],
            //Namn = del_felt[2],
         };

         _Callback?.Invoke(item);
      }



      void PostCheck()
      {
         /*------------------------------------------------------------------------*/
         /* Kolla att inga öppna klamrar kvarstår                                  */
         /*------------------------------------------------------------------------*/

         //if (m_ver_flagga && (m_klammer_aktiv == 0))
         //{
         //   // Vi har stött på en #VER-post som inte följs av en klammer

         //   GeSyntaxfel(buffert, 0, KLAMMER_SAKNAS);

         //   m_ver_flagga = 0;

         //   if (felhantering == AVBRYT_VID_FEL)
         //   {
         //      return SIESTATUS_SYNTAXFEL;
         //   }
         //   else
         //   {
         //      m_felantal++;
         //   }
         //}

         //if (m_klammer_aktiv)
         //{
         //   // Vi har stött på en startklammer men ingen slutklammer

         //   // Gör callback för att informera tillämpningen om
         //   // att verifikatet är slut

         //   //      status = ParentesRutin(PARENTES_SLUT);

         //   GeSyntaxfel(buffert, 0, SLUTKLAMM_SAKNAS);

         //   if (felhantering == AVBRYT_VID_FEL)
         //   {
         //      return SIESTATUS_SYNTAXFEL;
         //   }
         //   else
         //   {
         //      m_felantal++;
         //   }
         //}

      }
   }




#if false
      DEL_FELT* CSieMotor::DelaUppFelt(char* buffert)
{
   char* pek;
   char* feltstart;
   char* slutpek;
   char* slashpek;
   char temp;
   unsigned int i;              // Räknare för antalet delfält
   DEL_FELT* del_felt;


   // allokera minne för del_felt;
   del_felt = (DEL_FELT*)malloc(MAX_FELT * sizeof(DEL_FELT*));

   // Nollställ
   memset(del_felt, 0, MAX_FELT * sizeof(DEL_FELT*));

   pek = buffert;
   //buf2pek = buf2;

   // Gå igenom hela raden och dela upp den i sina beståndsdelar lagra pekare
   // till varje delfält i en array. arrayen avslutas med en nollpekare.

   i = 0;   // Nollställ räknaren för antalet delfält

   while (1)
   {
      // Skippa blanktecken och tabulatorer som föregår detta delfält

      while ((*pek == ' ') || (*pek == TAB)) pek++;

      if (*pek == 0)
      {
         // Om radslut detekteras
         del_felt[i].typ = SIE_NORM_FELT;
         del_felt[i].pek = 0;
         del_felt[i].pos = pek - buffert; // Spara pos för strängslut
         del_felt[i].objlista = NULL;
         return del_felt;
      }

      feltstart = pek;


      if (*pek == '"')      // Testa om delfältet är inneslutet inom fnuttar
      {
         // Delfältet är omgärdat av fnuttar

         pek++;     // Skippa startfnutten

         // Leta upp motsvarande slutfnutte. När detta görs ska vi ignorera
         // fnuttar som föregås av en backslash, eftersom dessa i så fall
         // tillhör själva fältinnehållet


         slutpek = pek + strcspn(pek, "\"");

         while (*slutpek == '"' && (*(slutpek - 1) == '\\'))
         {
            // Fnutten föregicks av en backslash - leta vidare

            slutpek += strcspn(slutpek + 1, "\"") + 1;
         }

         if (*slutpek != '"')
         {
            // Det fanns ingen matchande slutfnutte

            GeSyntaxfel(buffert, slutpek - buffert, SLUTFNUTT_SAKNAS);

            //del_felt[i].pos = slutpek-buffert;  // Spara pos för strängslut
            //return NULL;
         }

         // Slutfnutten är funnen - byt ut den mot ett strängslut

         *slutpek = 0;

         // Lägg in delfältet i arrayen med delfält

         /*
         strcpy(buf2pek, feltstart+1);  // +1 anges för att startfnutten ej
                                        // ska tas med
         del_felt[i] = buf2pek;
         buf2pek     += strlen(buf2pek)+1;
         */
         del_felt[i].typ = SIE_NORM_FELT;
         del_felt[i].pek = _strdup(feltstart + 1);
         del_felt[i].pos = feltstart - buffert + 1;
         del_felt[i].objlista = NULL;
         i++;

         *slutpek = '"';                // Återställ slutfnutten

         // Fortsätt leta efter nästa delfält med början på tecknet
         // efter slutfnutten.

         pek = slutpek + 1;

         if (*pek != 0 && *pek != ' ' && *pek != '\t')
         {
            GeSyntaxfel(buffert, pek - buffert, BLANK_SAKNAS);

            return NULL;

            //            // Skippa tecken tills blank detekteras

            //            while (*pek != 0 && *pek != ' ' && *pek != '\t')
            //            {
            //               pek++;
            //            }
         }
      }
      else if (*pek == '{')      // Testa om det är en objektlista
      {
         // Leta upp motsvarande slutkrumelur.

         slutpek = pek + strcspn(pek, "}");

         if (*slutpek != '}')
         {
            // Det fanns ingen matchande slutfnutte

            GeSyntaxfel(buffert, slutpek - buffert, SLUTKLAMM_SAKNAS);

            del_felt[i].pos = slutpek - buffert;  // Spara pos för strängslut
            return NULL;
         }

         // Slutkrumeluren är funnen - spara objektlistetexten

         temp = *(slutpek + 1);
         *(slutpek + 1) = 0;
         char* objliste_text = _strdup(feltstart);
         *(slutpek + 1) = temp;

         *slutpek = 0;

         // Lägg in delfältet i arrayen med delfält

         DEL_FELT* objlista = DelaUppFelt(pek + 1);

         del_felt[i].typ = SIE_OBJLISTA;
         del_felt[i].pek = objliste_text;
         del_felt[i].pos = feltstart - buffert;
         del_felt[i].objlista = objlista;
         i++;

         *slutpek = '}';                // Återställ slutkrumeluren

         // Fortsätt leta efter nästa delfält med början på tecknet
         // efter slutkrumeluren

         pek = slutpek + 1;

         if (*pek != 0 && *pek != ' ' && *pek != '\t')
         {
            CSieMotor::GeSyntaxfel(buffert, pek - buffert, BLANK_SAKNAS);

            // Skippa tecken tills blank detekteras

            while (*pek != 0 && *pek != ' ' && *pek != '\t')
            {
               pek++;
            }
         }
      }
      else
      {
         // Fältinnehållet är inte omgärdat av fnuttar
         // fältet sträcker sig då till första blank, tab eller radslut

         while ((*pek != ' ') && (*pek != TAB) && (*pek != 0)) pek++;

         temp = *pek;

         *pek = 0;      // Stoppa ner ett tillfälligt strängslut

         // Lägg in delfältet i arrayen med delfält

         del_felt[i].typ = SIE_NORM_FELT;
         del_felt[i].pek = _strdup(feltstart);
         del_felt[i].pos = feltstart - buffert;
         del_felt[i].objlista = NULL;
         i++;

         *pek = temp;       // Återställ sluttecknet
      }

      // Om delfältet innehåller någon backslash följd av en fnutte så ska
      // backslashen elimineras från strängen

      slashpek = strchr(del_felt[i - 1].pek, '\\');

      while (slashpek != 0)
      {
         if (*(slashpek + 1) == '"')
         {
            strcpy(slashpek, slashpek + 1);
         }
         else
         {
            // Slashen stod inte före en fnutte. I så fall är den en helt
            // naturlig del av fältinnehållet och ska lämnas orörd

            slashpek++;     // Fortsätt leta fr o m tecknet efter slashen
         }

         slashpek = strchr(slashpek, '\\');
      }
   }
}
#endif


#if false


      bool CSieMotor::KontrolleraPost(char* buffert, DEL_FELT* del_felt, unsigned int* index)
{
   int felt_slut;  // Boolean
   unsigned int i;
   unsigned int j;
   char felttyp;
   char* wintkn;
   char* pos;
   bool status;
   bool stat;

   // Kolla vad det är för posttyp

   i = 0;
   status = true;

   while ((SIE_posttyper[i].etikett != 0) &&
          (strcmp(SIE_posttyper[i].etikett, del_felt[0].pek) != 0))
   {
      i++;
   }

   if (SIE_posttyper[i].etikett == 0)
   {
      // Det är en okänd posttyp

      GeSyntaxfel(del_felt[0].pek, -1, OKEND_POSTTYP);

//#if 0
//      // Lägg upp posttypen så att vi inte får fler varningar på denna

//      SIE_posttyper[i].etikett   = strdup(del_felt[0]);
//      SIE_posttyper[i].posttyp   = OKEND;
//      SIE_posttyper[i].felt      = 0;
//      SIE_posttyper[i].oblig     = 0;
//      SIE_posttyper[i].substart  = 0;
//      SIE_posttyper[i].subpost   = 0;
//      SIE_posttyper[i].antal     = 0;
//      SIE_posttyper[i].errantal  = 0;

//      SIE_posttyper[i+1].etikett = 0;
//#endif

      *index = i;

      return true;
   }

   if (SIE_posttyper[i].felt == 0)
   {
      // Detta är en posttyp där vi inte har en aning om hur fälten ska
      // vara beskaffade. Därför kan vi inte göra någon kontroll av dem

      *index = i;
      return true;
   }

   // Kontrollera att fälten är rätt antal och giltiga
   // Vi kan endast göra detta om det är en posttyp vi känner till


   felt_slut = 0;

   for (j = 0; j < strlen(SIE_posttyper[i].felt); j++)
   {
      felttyp = SIE_posttyper[i].felt[j];

      if (felt_slut == 1 || del_felt[j + 1].pek == 0)
      {
         if (felt_slut == 0 && felttyp < 96)
         {
            // Om det är en obligatorisk parameter som saknas
            // Varna första gången det inträffar

            if (SIE_posttyper[i].errantal < MAX_ERR)
            {
               // Ge bara fel om man ej har gett mängder av
               // samma fel tidigare

               GeSyntaxfel(buffert, del_felt[j + 1].pos, FOR_FA_FELT);
               SIE_posttyper[i].errantal++;
            }

            status = false;

         }

         felt_slut = 1;
      }
      else
      {
         if ((felttyp >= 'a') &&
             (strlen(del_felt[j + 1].pek) == 0))
         {
            // %1 - Det är en frivillig parameter - vi godkänner då att den är en tomsträng

            stat = true;
         }
         else
         {

            // Det finns fler parametrar - kolla om de har rätt typ
            switch (felttyp)
            {
               case 'B':
               case 'b':   // Belopp

                  stat = KontrolleraFelt("-0123456789.", buffert,
                             del_felt[j + 1].pek, del_felt[j + 1].pos,
                             0, FEL_TKN_BELOPP, INGET_FEL, i);
                  break;

               case 'D':
               case 'd':       // Datum
                  stat = KontrolleraFelt("0123456789", buffert,
                             del_felt[j + 1].pek, del_felt[j + 1].pos,
                             8, FEL_TKN_DATUM, FEL_DATUM, i);
                  break;

               case 'M':
               case 'm':       // Månad
                  stat = KontrolleraFelt("0123456789", buffert,
                             del_felt[j + 1].pek, del_felt[j + 1].pos,
                             6, FEL_TKN_MANAD, FEL_MANAD, i);
                  break;

               case 'P':
               case 'p':       // Periodnummer
                  stat = KontrolleraFelt("0123456789", buffert,
                             del_felt[j + 1].pek, del_felt[j + 1].pos,
                             0, FEL_TKN_PERIODNR, FEL_PERIODNR, i);
                  break;

               case 'A':
               case 'a':   // Årsnummer
                  stat = KontrolleraFelt("-0123456789", buffert,
                             del_felt[j + 1].pek, del_felt[j + 1].pos,
                             0, FEL_TKN_ARSNR, INGET_FEL, i);
                  break;
               case 'Y':
               case 'y':   // År
                  stat = KontrolleraFelt("0123456789", buffert,
                             del_felt[j + 1].pek, del_felt[j + 1].pos,
                             4, FEL_TKN_AR, FEL_AR, i);
                  break;

               case 'S':
               case 's':       // Godtycklig sträng
                  for (wintkn = "ÅÄÖåäö"; *wintkn != 0; wintkn++)
                  {
                     pos = strchr(del_felt[j + 1].pek, *wintkn);
                     if (pos)
                     {
                        //GeSyntaxfel(buffert, del_felt[j+1].pos + (pos-del_felt[j+1].pek), WINTECKEN);
                        // Detta bör endast vara en varning eftersom den inte stoppar inäsning
                        break;
                     }
                  }
                  stat = true; // Detta ska ej få konsekvenser

                  break;

               case 'O':
               case 'o':       // Objektlista
                               /*
                               if (*del_felt[j+1].pek != '{')
                               {
                                  // Det är ingen objektlista inom klammer

                                  SIE_felmedd(buffert, felt_pos[j+1], FEL_OBJLISTA);
                                  stat = FEL;
                               }
                               else
                               {
                                  stat = RETT;
                               }
                               */
                  break;

               case 'N':
               case 'n':       // Numeriskt fält
                  stat = KontrolleraFelt("0123456789", buffert,
                              del_felt[j + 1].pek, del_felt[j + 1].pos,
                              0, FEL_TKN_NUM, INGET_FEL, i);
                  break;
            }
         }

         if (stat == false)
         {
            // Fältkontrollen har resulterat i ett fel

            status = false;
         }
      }
   }

   if (felt_slut == 0 && del_felt[j + 1].pek != 0)
   {
      // Det finns okända fält efter de kända

      if (SIE_posttyper[i].errantal < MAX_ERR)
      {
         // Ge bara fel om man ej har gett mängder av
         // samma fel tidigare

         GeSyntaxfel(buffert, del_felt[j + 1].pos, FOR_MANGA_FELT);
         SIE_posttyper[i].errantal++;
      }

      // Detta betraktar vi inte som något fel som påverkar status.
   }

   // Kolla om posttypen måste vara en subpost

   if (SIE_posttyper[i].subpost && (m_klammer_aktiv == 0))
   {
      // Subpost förekommer utan att klammer är aktiv

      GeSyntaxfel(del_felt[0].pek, -1, SUBPOST_FEL);
   }

   // Kolla om posttypen ej får vara en subpost

   if ((SIE_posttyper[i].subpost == 0) && (m_klammer_aktiv == 1))
   {
      // Denna posttyp ska ej förekomma som subpost

      GeSyntaxfel(del_felt[0].pek, -1, EJ_SUBPOST);
      m_klammer_aktiv = 0;
      m_ver_flagga = 0;
      status = false;
   }

   // Kolla om posttypen skall ha subposter

   if (SIE_posttyper[i].substart != 0)
   {
      // Denna posttyp ska ha subposter

      m_klammer_aktiv = 0;
      m_ver_flagga = 1;
   }

   SIE_posttyper[i].antal++;

   *index = i;

   return status;
}


bool CSieMotor::KontrolleraFelt(char* till_tkn, char* buffert, char* del_felt, int fel_pos,
                      unsigned int len, SIE_FELTYP tkn_fel, SIE_FELTYP len_fel,
                      unsigned int i)
{
   unsigned int feltkn;
   bool status;

   status = true;

   feltkn = strspn(del_felt, till_tkn);

   if (feltkn < strlen(del_felt))
   {
      // Det fanns ett tecken som vi inte tycker om

      if (SIE_posttyper[i].errantal < MAX_ERR)
      {
         // Ge bara fel om man ej har gett mängder av
         // samma fel tidigare

         GeSyntaxfel(buffert, fel_pos + feltkn, tkn_fel);

         SIE_posttyper[i].errantal++;
      }

      status = false;
   }

   if (len != 0 && strlen(del_felt) != len)
   {
      if (SIE_posttyper[i].errantal < MAX_ERR)
      {
         // Ge bara fel om man ej har gett mängder av samma fel tidigare

         GeSyntaxfel(buffert, fel_pos, len_fel);

         SIE_posttyper[i].errantal++;
      }

      status = false;
   }

   return status;
}

#endif

   //-------------------------------------------------------------------------
   // Virtuella funktioner som ska overridas av den som utnyttjar inläsnings-
   // motorn.
   //-------------------------------------------------------------------------


   //void GeSyntaxfel(string buffert, int felpos, SIE_FELTYP feltyp)
   //{
   //   int radnr = HemtaRadnummer();       // Radnr i filen där felet inträffat
   //   char* filnamn = HemtaFilnamn();    // Filnamn som innehåller felet

   //   return m_felx->GeFelmeddelande(filnamn, radnr, buffert, felpos, FELKATEGORI_SYNTAX, (int)feltyp, "");
   //}


#if false

      // Syntaxfel från CSieMotor

      typedef struct
{
   int id;
int allvar;         // 4 = lättare varning, 3 = varning, 1=fel
char* txt;
} FELINFO;


static FELINFO SIE_feltext[] =
{
   { BLANK_SAKNAS, 1, "Blanktecken saknas efter fält."},
   { SLUTFNUTT_SAKNAS, 3, "Avslutande citationstecken saknas."},
   { SLUTKLAMM_SAKNAS, 1, "Avslutande klammer saknas."},

   { OKEND_POSTTYP, 1, "Okänd posttyp."},

   { FOR_MANGA_FELT, 1, "För många fält."},
   { FOR_FA_FELT, 1, "Obligatoriskt fält saknas."},

   { OBL_POST_SAKNAS, 1, "Obligatorisk post '%s' måste finnas i denna filtyp."},
   { OBL_POST_SAKNAS_W, 3, "Post '%s' saknas. Bör förekomma i denna filtyp."},        // Varning
   { PTYP_FORBJUDEN, 1, "Post '%s' får inte förekomma i denna filtyp."},
   { PTYP_FORBJUDEN_W, 3, "Post '%s' bör inte förekomma i denna filtyp."},       // Varning
   { OBL_POST_SAKNAS_0, 1, "Obligatorisk post '%s' saknas för räkenskapsår 0."},
   { OBL_POST_SAKNAS_M1, 1, "Obligatorisk post '%s' saknas för räkenskapsår -1."},

   { FEL_TKN_BELOPP, 1, "Felaktigt tecken i beloppsfält"},
   { FEL_TKN_DATUM, 1, "Felaktigt tecken i datum."},
   { FEL_TKN_NUM, 1, "Felaktigt tecken i numeriskt fält."},
   { FEL_TKN_ARSNR, 1, "Felaktigt tecken i årsnummer."},
   { FEL_TKN_AR, 1, "Felaktigt tecken i årtal."},
   { FEL_TKN_MANAD, 1, "Felaktigt tecken i månadsangivelse."},
   { FEL_TKN_PERIODNR, 1, "Felaktigt tecken i periodnummer."},

   { FEL_DATUM, 1, "Felaktigt datum."},
   { FEL_AR, 1, "Felaktigt årtal."},
   { FEL_MANAD, 1, "Felaktig månadsangivelse."},
   { FEL_PERIODNR, 1, "Felaktigt periodnummer."},
   { FEL_OBJLISTA, 1, "Felaktig objektlista."},

   { DUBBEL_KLAMMER, 1, "Föregående post kan ej ha underposter."},
   { KLAMMER_UTAN_VER, 1, "Klammer har redan angetts."},
   { KLAMMER_MATCHFEL, 1, "Slutklammer saknar matchande startklammer."},
   { TEXT_EFTER_KLAMMER, 1, "Ogiltig text efter klammer."},

   { SUBPOST_FEL, 1, "Posttyp '%s' får endast anges inom klammer."},
   { EJ_SUBPOST, 1, "Posttyp '%s' får ej anges inom klammer."},
   { SUBPOST_SAKNAS, 1, "Underposter saknas."},
   { WINTECKEN, 3, "Ovanligt tecken. Förekomer normalt inte i PC8 teckenuppsättning."},
   { KLAMMER_SAKNAS, 1, "Klammer saknas efter en post som ska ha underposter."},
   {  INGET_FEL, 0, 0}          // Slutmärke
};

CString CSieMotor::FormateraSyntaxfel(int feltyp, const CString &info)
{
   int i = 0;

   while ((SIE_feltext[i].id != feltyp) &&
          (SIE_feltext[i].id != INGET_FEL))
   {
      i++;
   }


   CString x;

   if (SIE_feltext[i].id == INGET_FEL)
   {
      x.Format("Okänd felkod %i.", feltyp);
   }
   else
   {
      x.Format(SIE_feltext[i].txt, info);
   }

   return x;
}
#endif

}