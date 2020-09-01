using CeltaNavs.Repository;
using MySql.Data.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeltaNavs.Domain
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class NavsContext : DbContext
    {
        public DbSet<ModelProduct> Products { get; set; }     
        public DbSet<ModelSaleRequest> SaleRequests { get; set; }
        public DbSet<ModelSaleRequestProduct> SaleRequestProducts { get; set; }        
        public DbSet<ModelSaleMovementFinalization> NavsFinalizations { get; set; }
        public DbSet<ModelNavsSetting> NavsSettings { get; set; }
        public DbSet<ModelEnterprise> Enterprises { get; set; }
        public DbSet<ModelPdv> Pdvs { get; set; }        
        public DbSet<ModelExpansibleGroup> ExpansibleGroups { get; set; }
        public DbSet<ModelExpansibleGroupProduct> ExpansibleGroupsProducts { get; set; }        

        public NavsContext() : base("connDbCBS")
        {
        }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ModelNavsSetting>().ToTable("navssettings");
            modelBuilder.Entity<ModelSaleRequest>().ToTable("salerequests");
            modelBuilder.Entity<ModelSaleRequestProduct>().ToTable("salerequestproducts");            
            modelBuilder.Entity<ModelSaleMovementFinalization>().ToTable("navsfinalizations");
            modelBuilder.Entity<ModelProduct>().ToTable("products");
            modelBuilder.Entity<ModelEnterprise>().ToTable("enterprises");
            modelBuilder.Entity<ModelPdv>().ToTable("pdvs");
            modelBuilder.Entity<ModelExpansibleGroup>().ToTable("expansiblegroups");
            modelBuilder.Entity<ModelExpansibleGroupProduct>().ToTable("expansiblegroupsproducts");

            modelBuilder.Entity<ModelSaleRequestProduct>().HasKey(t => t.SaleRequestProductId);
            modelBuilder.Entity<ModelSaleRequest>().HasKey(t => t.SaleRequestId);            

            modelBuilder.Entity<ModelProduct>().HasKey(t => t.InternalCodeOnERP);                       

           // modelBuilder.Entity<ModelSaleRequestProducts>()
           //.HasRequired<ModelSaleRequests>(s => s.SaleRequest)
           //.WithMany(g => g.Products)
           //.HasForeignKey<int>(s => s.SaleRequestId);

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
