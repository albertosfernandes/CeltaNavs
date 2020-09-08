using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeltaNavs.Repository
{
    public class ModelSaleRequestProductTemp
    {        
        public ModelSaleRequestProductTemp()
        {            
        }
        [Key]
        [Column("NavsSaleRequestProductsTempId")]
        public int SaleRequestProductTempId { get; set; }
        public string ProductPriceLookUpCode { get; set; }
        public decimal Value { get; set; }
        public decimal Quantity { get; set; }       
        public decimal TotalLiquid { get; set; }
        public virtual ModelProduct Product { get; set; }
        [ForeignKey("Product")]
        public int ProductInternalCodeOnErp { get; set; }
        public virtual ModelSaleRequestTemp SaleRequestTemp { get; set; }
        [ForeignKey("SaleRequestTemp")]
        public int SaleRequestTempId { get; set; }    
    }
}
