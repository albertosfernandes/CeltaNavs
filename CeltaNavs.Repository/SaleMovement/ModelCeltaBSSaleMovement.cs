using CeltaWare.CBS.PDV.Concentrator.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeltaNavs.Repository
{
    public class ModelCeltaBSSaleMovement
    {
        [Key]
        public int SaleMovementId { get; set; }
        public int EnterpriseId { get; set; }
        public int PdvId { get; set; }
        public string PersonalizedCode { get; set; }
        public int CouponFiscalNumber { get; set; }
        public string DateOfCreation { get; set; }
        public DateTime DateHourOfCreation { get; set; }
        public string CpfCnpj { get; set; }
        public int DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public int IncrementType { get; set; }
        public decimal IncrementValue { get; set; }
        public int DeliveryTaxType { get; set; }
        public decimal DeliveryTaxValue { get; set; }
        public string XmlSatNfce { get; set; }
        public string XmlSatNfceCancel { get; set; }
        public string EletronicAccessKey { get; set; }
        public string SaleRequestPersonalizedCode { get; set; }
        public SaleFiscalType SaleFiscalType { get; set; }
        public int CovenantCode { get; set; }
        public int CustomerCode { get; set; }
        public int CustomerAdditionalCode { get; set; }
        public int SellerCode { get; set; }
        public int RoundingType { get; set; }
        public int UserIdOfCancellation { get; set; }
        public bool IsCancelled { get; set; }
        public int MotiveJustificationId { get; set; }
        public bool IsDelivery { get; set; }
        public decimal FidelityControlsPoints { get; set; }
        public int SalePricePraticed { get; set; }
        public decimal TotalLiquid { get; set; }
        public int FlagStatusRead { get; set; }
    }
}
