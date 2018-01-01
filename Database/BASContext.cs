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

   public class BASContext : DbContext
   {
      bool _recreate;
      public BASContext() : base("BasBas")
      {
         Database.SetInitializer<BASContext>(new DropCreateDatabaseAlways<BASContext>());
      }

      public BASContext(bool recreate = false) : base("BasBas")
      {
         _recreate = recreate;

         if (recreate)
         {
            Database.SetInitializer<BASContext>(new DropCreateDatabaseAlways<BASContext>());
         }
      }
      protected override void OnModelCreating(DbModelBuilder modelBuilder)
      {
         modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
      }
      public DbSet<Account> Accounts { get; set; }
      public DbSet<UsedAccount> UsedAccounts { get; set; }
      public DbSet<XBRLElement> XbrlElements { get; set; }

      public DbSet<AccountNumber> AccountNumbers { get; set; }
      
   }
   //public class BASContextInitializer : DropCreateDatabaseIfModelChanges<BASContext>
   //{
   //   protected override void Seed(BASContext context)
   //   {
   //      //Category cat1 = new Category { Id = Guid.NewGuid(), Name = ".NET Framework" };
   //      //Category cat2 = new Category { Id = Guid.NewGuid(), Name = "SQL Server" };
   //      //Category cat3 = new Category { Id = Guid.NewGuid(), Name = "jQuery" };

   //      //context.Categories.Add(cat1);
   //      //context.Categories.Add(cat2);
   //      //context.Categories.Add(cat3);

   //      //context.SaveChanges();

   //   }
   //}
}
