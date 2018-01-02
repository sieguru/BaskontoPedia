using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLDatabase
{
   //public class AccountNumber
   //{
   //   public AccountNumber()
   //   {
   //      this.XbrlElements = new HashSet<XBRLElement>();
   //      this.Accounts = new HashSet<Account>();
   //      this.SubAccounts = new HashSet<AccountNumber>();
   //      this.Sokord = new HashSet<Sokord>();
   //   }

   //   [Key]
   //   public string AccountId { get; set; }
   //   public string Name { get; set; }
   //   public string LastYear { get; set; }
   //   public string SidaIBokforingsboken { get; set; }
   //   public string SidaIBokslutsboken { get; set; }

   //   public virtual ICollection<Account> Accounts { get; set; }    // BAS-konton som ingår under detta kontonummer:
   //   public virtual ICollection<XBRLElement> XbrlElements { get; set; }    // XBRL-element som omfattar detta kontonummer:
   //   public virtual ICollection<Sokord> Sokord { get; set; }    // Sökord som omfattar detta BAS-konto:

   //   public virtual string ParentId { get; set; }  // Self-reference
   //   [ForeignKey("ParentId")]
   //   public virtual ICollection<AccountNumber> SubAccounts { get; set; }    // BAS-konton som ingår under detta XBRL-element:
   //   public virtual AccountNumber Parent { get; set; }    // BAS-konton som ingår under detta XBRL-element:
   //}

   //public class Account
   //{
   //   public int Id { get; set; }

   //   public string AccountID { get; set; }
   //   public string Year { get; set; }
   //   public string Name { get; set; }
   //   public string Comment { get; set; }

   //   public bool NotK2 { get; set; }
   //   public bool Recommended { get; set; }

   //   public bool SubAccount { get; set; }

   //   public string MainAccount { get; set; }

   //   [ForeignKey("AccountNumberKey")]
   //   public virtual AccountNumber Number { get; set; }

   //   public string AccountNumberKey { get; set; }

   //   //public virtual ICollection<XBRLElement> XbrlElements { get; set; }    // XBRL-element som omfattar detta BAS-konto:
   //}

   public class UsedAccount
   {
      public int Id { get; set; }

      public string AccountID { get; set; }
      public string Name { get; set; }

      public string CompanyId { get; set; }

      public string DebitTransactions{ get; set; }
      public string CreditTransactions { get; set; }
   }
   public class XBRLElement
   {
      public XBRLElement()
      {
         this.References = new HashSet<XBRLReference>();
         this.Accounts = new HashSet<AccountNumber>();
      }

      public int Id { get; set; }

      public string Header1 { get; set; }
      public string Header2 { get; set; }
      public string Header3 { get; set; }
      public string Header4 { get; set; }
      public string Header5 { get; set; }
      public string Header6 { get; set; }
      public string Name { get; set; }
      public string Standardrubrik { get; set; }
      public string Periodtyp { get; set; }
      public int Level { get; set; }
      public bool Abstrakt { get; set; }
      public string Domain { get; set; }
      public string Datatyp { get; set; }

      public string ElementName { get; set; }
      public string Saldo { get; set; }
      public string Dokumentation { get; set; }

      public string Konton { get; set; }

      public virtual ICollection<AccountNumber> Accounts { get; set; }    // BAS-konton som ingår under detta XBRL-element:

      public virtual ICollection<XBRLReference> References { get; set; }    // BAS-konton som ingår under detta XBRL-element:

   }

   public class XBRLReference
   {
      public int Id { get; set; }
      public string Utgivare { get; set; }
      public string Namn { get; set; }
      public string Nummer { get; set; }
      public string Kapitel { get; set; }
      public string Paragraf { get; set; }
      public string Stycke { get; set; }
      public string Punkt { get; set; }
      public string Avsnitt { get; set; }
   }

   public class Sokord
   {
      public int Id { get; set; }
      public string Text { get; set; }
      public virtual ICollection<AccountNumber> Accounts { get; set; }    // BAS-konton som ingår under detta sökord:
   }
}
