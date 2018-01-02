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
   public class AccountNumber
   {
      public AccountNumber()
      {
         this.XbrlElements = new HashSet<XBRLElement>();
         this.Accounts = new HashSet<Account>();
         this.SubAccounts = new HashSet<AccountNumber>();
         this.Sokord = new HashSet<Sokord>();
      }

      [Key]
      public string AccountId { get; set; }
      public string IntervalEnd { get; set; }
      public string IntervalReference { get; set; }

      public string Name { get; set; }
      public bool Recommended { get; set; }

      public string LastYear { get; set; }
      public string SidaIBokforingsboken { get; set; }
      public string SidaIBokslutsboken { get; set; }

      public virtual ICollection<Account> Accounts { get; set; }    // BAS-konton som ingår under detta kontonummer:
      public virtual ICollection<XBRLElement> XbrlElements { get; set; }    // XBRL-element som omfattar detta kontonummer:
      public virtual ICollection<Sokord> Sokord { get; set; }    // Sökord som omfattar detta BAS-konto:

      public virtual string ParentId { get; set; }  // Self-reference
      [ForeignKey("ParentId")]
      public virtual ICollection<AccountNumber> SubAccounts { get; set; }    // BAS-konton som ingår under detta XBRL-element:
      public virtual AccountNumber Parent { get; set; }    // BAS-konton som ingår under detta XBRL-element:

      public string Number
      {
         get
         {
            string presentation = AccountId;

            if (AccountId.Length == 3)
            {
               presentation += "x";
            }

            if (IntervalEnd != null)
            {
               presentation += "-" + IntervalEnd;

               if (AccountId.Length == 3)
               {
                  presentation += "x";
               }
            }

            return presentation;
         }
      }

      public override string ToString()
      {
         return Number + " " + Name;
      }
   }

  
}
