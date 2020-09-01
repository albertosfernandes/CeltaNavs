using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace CeltaNavs.Repository
{
    public class ModelSaleRequest
    {
        public ModelSaleRequest()
        {
            Products = new HashSet<ModelSaleRequestProduct>();
        }

        [Key]
        public int SaleRequestId { get; set; }
        public string PersonalizedCode { get; set; }
        public DateTime DateOfCreation { get; set; }
        public DateTime DateHourOfCreation { get; set; }
        public int EnterpriseId { get; set; }
        public bool IsCancelled { get; set; }
        public bool IsUsing { get; set; }
        public int Peoples { get; set; }
        public string FlagStatus { get; set; }
        public decimal TotalLiquid { get; set; }        
        public SaleRequestOrigin FlagOrigin { get; set; }
        public virtual ICollection<ModelSaleRequestProduct> Products { get; set; }        
    }
}
