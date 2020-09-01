using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeltaNavs.Repository
{
    public class ModelProduct
    {
        public ModelProduct()
        {

        }

        
        [Key]
        public int ProductsId { get; set; }
//        [Key]
        public int InternalCodeOnERP { get; set; }
        public int EnterpriseId { get; set; }
        public string PriceLookupCode { get; set; }
        public string PersonalizedCode { get; set; }
        public string EanCode { get; set; }
        public string Name { get; set; }
        public string NameReduced { get; set; }
        public string PackingAbbreviation { get; set; }
        public double PackingQuantity { get; set; }
        public decimal SaleRetailPrice { get; set; }
        public decimal OfferRetailPrice { get; set; }
        public decimal WholeSalePrice { get; set; }
        public decimal WholeOfferPrice { get; set; }
        public decimal LiquidCost { get; set; }
        public decimal LiquidCostMiddle { get; set; }
        public decimal LiquidCostReal { get; set; }
        public decimal RepositionCost { get; set; }
        public decimal RepositionCostMiddle { get; set; }
        public decimal RepositionCostReal { get; set; }
        public string CFOP { get; set; }
        public string FiscalClassificationNCM { get; set; }
        public string TributarySituationCode { get; set; }
        public decimal IcmsAliquot { get; set; }
        public decimal IcmsIVAMargin { get; set; }
        public decimal IcmsReductionMargin { get; set; }
        public decimal IcmsStave { get; set; }
        public decimal PercImpNBPTNational { get; set; }
        public decimal PercImpNBPTState { get; set; }
        public decimal PercImpNBPTMunicipal { get; set; }
        public decimal CofinsPercentage { get; set; }
        public decimal PisPercentage { get; set; }
        public int CofinsType { get; set; }
        public int PisType { get; set; }
        public string CSTPisCofins { get; set; }
        public int ProductIsFatherComposition { get; set; }
        public double QuantityMinimumWholeSale { get; set; }
        public bool AllowFractionate { get; set; }
        public bool IsBalance { get; set; }
        public bool CanBeWeightyInPdv { get; set; }
        public bool QuantityInBalance { get; set; }
        public bool AllowQuantityInPdv { get; set; }
        public bool GoesForFastTable { get; set; }
        public bool IsAlcoholicProduct { get; set; }
        public double BalanceTare { get; set; }
        public string CodeClass { get; set; }
        public int DepartmentCode { get; set; }
        public int SectionCode { get; set; }
        public int GroupCode { get; set; } 
        public int SubGroupCode { get; set; }
        public string DateOfLastChanged { get; set; }
        public double StockQuantity { get; set; }
        

        public virtual string SaleRetailPraticedString
        {
            get
            {
                if (OfferRetailPrice > 0)
                    return OfferRetailPrice.ToString("N2");
                else if (SaleRetailPrice > 0)
                    return SaleRetailPrice.ToString("N2");
                else
                    return String.Empty;
            }
        }

        //public virtual ICollection<ModelSaleRequestProducts> ModelSaleRequestProduct { get; set; }
    }
}
