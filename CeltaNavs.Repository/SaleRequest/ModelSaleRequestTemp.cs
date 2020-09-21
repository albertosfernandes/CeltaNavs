using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeltaNavs.Repository
{
    public class ModelSaleRequestTemp
    {
        public ModelSaleRequestTemp()
        {
            this.Products = new List<ModelSaleRequestProductTemp>();
        }

        [Key]
        [Column("NavsSaleRequestsTempId")]
        public int SaleRequestTempId { get; set; }
        public string PersonalizedCode { get; set; }        
        public int EnterpriseId { get; set; }        
        public decimal TotalLiquid { get; set; }        
        public virtual List<ModelSaleRequestProductTemp> Products { get; set; }

    }
}
