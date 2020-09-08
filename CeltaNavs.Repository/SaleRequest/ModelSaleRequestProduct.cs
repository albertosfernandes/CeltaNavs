using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using CeltaWare.CBS.PDV.Concentrator.Repository;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace CeltaNavs.Repository
{
    public class ModelSaleRequestProduct
    {
        public ModelSaleRequestProduct()
        {
            
        }

        [Key] 
        public int SaleRequestProductId { get; set; }        
        public string ProductPriceLookUpCode { get; set; }
        public decimal Value { get; set; }
        public decimal Quantity { get; set; }
        public string Comments { get; set; }
        public int UserId { get; set; }       
        public bool IsCancelled { get; set; }
        public bool IsDelivered { get; set; }
        public ProductionStatus ProductionStatus { get; set; }
        public decimal TotalLiquid { get; set; }        
        public virtual ModelProduct Product { get; set; }
        [ForeignKey("Product")]
        public int ProductInternalCodeOnErp { get; set; }
        public virtual ModelSaleRequest SaleRequest { get; set; }
        [ForeignKey("SaleRequest")]
        public int SaleRequestId { get; set; }        
    }
}
