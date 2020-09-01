using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeltaNavs.Repository
{
    public class ModelSaleMovement
    {
        public ModelSaleMovement()
        {
            this.Pdvs = new ModelPdv();
            this.Product = new ModelProduct();
            this.Enterprises = new ModelEnterprise();
        }

        [Key]
        public int SaleMovementId { get; set; }
        public string PersonalizedCode { get; set; }        
        public string CPFCNPJ { get; set; }
        public ModelEnterprise Enterprises { get; set; }
        public int EnterpriseId { get; set; }
        public ModelPdv Pdvs { get; set; }
        public int PdvId { get; set; }
        //public virtual ICollection<Products> Product { get; set; }
        public ModelProduct Product { get; set; }
        public int ProductsId { get; set; }
        public string ProductInternalCodeOnErp { get; set; }
        public int Quantity { get; set; }
        public bool IsCancel { get; set; }
        public decimal TotalLiquid { get; set; }
    }
}
