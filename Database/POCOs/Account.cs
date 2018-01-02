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
 
   public class Account
   {
      public int Id { get; set; }

      public string AccountID { get; set; }
      public string Year { get; set; }
      public string Name { get; set; }
      public string Comment { get; set; }

      public bool NotK2 { get; set; }
      public bool Recommended { get; set; }

 //     public bool SubAccount { get; set; } // Todo: Möjlighet att skilja på olika (under)kontotyper

      [ForeignKey("AccountNumberKey")]
      public virtual AccountNumber Number { get; set; }

      public string AccountNumberKey { get; set; }

      //public virtual ICollection<XBRLElement> XbrlElements { get; set; }    // XBRL-element som omfattar detta BAS-konto:
   }

}
