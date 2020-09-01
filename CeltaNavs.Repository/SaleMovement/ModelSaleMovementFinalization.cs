using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeltaNavs.Repository
{
    public class ModelSaleMovementFinalization
    {
        [Key]
        public int NavsFinalizationId { get; set; }
        public int SaleRequestId { get; set; }
        public int EnterpriseId { get; set; }
        public int PdvId { get; set; }
        public string PersonalizedCode { get; set; }
        public int FinalizationId { get; set; }
        public decimal Value { get; set; }
        public decimal PayBackValue { get; set; }
        public string PaymentResponse { get; set; }
        public string BinCard { get; set; }
        public string InstitutionalName { get; set; }
        public string NSUAUT { get; set; }
        public string NSUSIT { get; set; }
        public string NSUCAN { get; set; }
        public string CODAUT { get; set; }
        public string NPARCEL { get; set; }
        public string TIPOTRANS { get; set; }
        public string TIPOCART { get; set; }
        public string CODADQ { get; set; }
        
    }
}
