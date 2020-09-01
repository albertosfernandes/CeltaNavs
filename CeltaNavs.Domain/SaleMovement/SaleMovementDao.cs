using CeltaNavs.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CeltaNavs.Domain
{
    public class SaleMovementDao : Persistent
    {
        //xmlsat Resposta do SAT
        // caminho original desse metodo = namespace CeltaWare.CBS.PDV.Service > metodo ExportSaleMovement(ModelSaleMovement pdvSaleMovement)
        public static int CreateSaleMovementBS(string xmlsat, ModelSaleMovement navsSaleMov, ICollection<ModelSaleRequestProduct> listOfItens, List<ModelSaleMovementFinalization> listOfnSaleOrderFinalization, ModelNavsSetting _navsSettings)
        {
            try
            {
                ModelCeltaBSSaleMovement currentSale = new ModelCeltaBSSaleMovement();

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlsat);               
                XmlNodeList xmlData = doc.GetElementsByTagName("XmlSale");
                string xmlresult = xmlData[0].FirstChild.InnerText; ;

                //var resultKey = (XCData)elem.FirstNode;
                //for (int i = 0; i < xmlData.Count; i++)
                //{
                //    xmlresult = (xmlData[i].InnerXml);
                //}
                currentSale.XmlSatNfce = xmlresult;

                currentSale.EletronicAccessKey = NavsSaleHelpers.GetFiscalNoteAccessKey(xmlsat);
                string saleFiscalTypeOff = NavsSaleHelpers.GetSaleFiscalType(xmlsat);
                currentSale.SaleFiscalType = CeltaBSConvertHelpers.ToSaleFiscalType(saleFiscalTypeOff);            
                currentSale.CouponFiscalNumber = Convert.ToInt32(currentSale.EletronicAccessKey.Substring(34, 6));


                /*
                if (currentSale.IsSAT)
                    currentSale.CouponFiscalNumber = Convert.ToInt32(StringHelpers.GetOnlyDigits(currentSale.EletronicAccessKey).Substring(31, 6));
                else
                    currentSale.CouponFiscalNumber = Convert.ToInt32(StringHelpers.GetOnlyDigits(currentSale.EletronicAccessKey).Substring(25, 9));
                */

                #region nSaleMove
                CeltaWare.CBS.PDV.Concentrator.Repository.ModelSaleMovement bsSaleMovement = new CeltaWare.CBS.PDV.Concentrator.Repository.ModelSaleMovement()
                {
                    PdvId = navsSaleMov.Pdvs.PdvId,
                    EnterpriseId = navsSaleMov.Enterprises.EnterpriseId,
                    CouponFiscalNumber = currentSale.CouponFiscalNumber,
                    PersonalizedCode = DateTime.Now.ToString("yyyyMMdd") + navsSaleMov.PdvId.ToString() + currentSale.CouponFiscalNumber.ToString(),// YYYY-MM-DD -PDV2 dig. + numero cupom fiscal
                    //DateOfCreation = DateTime.Now.ToString("yyyy-MM-dd"),
                    DateHourOfCreation = DateTime.Now,
                    CpfCnpj = navsSaleMov.CPFCNPJ,
                    SaleFiscalType = CeltaBSConvertHelpers.ToSaleFiscalType(saleFiscalTypeOff),                    
                    RoundingType = CeltaWare.Utilities.RoundingType.AwayFromZero, 
                    XmlSatNfce = currentSale.XmlSatNfce,
                    //XmlSatNfceCancel = xmlsat, - nao faço cancelamento no NAVS
                    EletronicAccessKey = currentSale.EletronicAccessKey,
                    SaleRequestPersonalizedCode = navsSaleMov.PersonalizedCode,
                    TotalLiquid = navsSaleMov.TotalLiquid, //
                                                           //SalePricePraticed = pdvSaleMovement.SalePricePraticed,
                    Products = new List<CeltaWare.CBS.PDV.Concentrator.Repository.ModelSaleMovementProduct>(),
                    Finalizations = new List<CeltaWare.CBS.PDV.Concentrator.Repository.ModelSaleMovementFinalization>()
                };
                #endregion


                #region nProducts
                int sequenceOfProduct = 1;
                foreach (var saleMovementProduct in listOfItens)
                {
                    CeltaWare.CBS.PDV.Concentrator.Repository.ModelSaleMovementProduct product = new CeltaWare.CBS.PDV.Concentrator.Repository.ModelSaleMovementProduct();
                    product.SaleMovementProductId = 0;
                    product.SaleMovement = null;
                    product.SaleMovementProducOfFatherComposition = null;
                    product.SaleMovementProductCompositionId = null;
                    product.Position = sequenceOfProduct++;
                    product.ProductInternalCodeOnERP = saleMovementProduct.Product.InternalCodeOnERP.ToString();
                    product.ProductPluCode = saleMovementProduct.Product.PriceLookupCode;
                    product.ProductEan = saleMovementProduct.Product.EanCode;
                    product.ProductName = saleMovementProduct.Product.Name;
                    product.Quantity = saleMovementProduct.Quantity;
                    product.UnitValue = saleMovementProduct.Product.SaleRetailPrice;
                    product.IsCancelled = saleMovementProduct.IsCancelled;
                    product.IcmsTributarySituationCode = saleMovementProduct.Product.TributarySituationCode;
                    product.IcmsAliquot = saleMovementProduct.Product.IcmsAliquot;
                    product.IcmsStave = saleMovementProduct.Product.IcmsStave;
                    product.IcmsReductionMargin = saleMovementProduct.Product.IcmsReductionMargin;
                    product.IcmsIVAMargin = saleMovementProduct.Product.IcmsIVAMargin;
                    product.SaleRetailPrice = saleMovementProduct.Product.SaleRetailPrice;
                    product.OfferRetailPrice = saleMovementProduct.Product.OfferRetailPrice;
                    product.WholeSalePrice = saleMovementProduct.Product.WholeSalePrice;
                    product.WholeOfferPrice = saleMovementProduct.Product.WholeOfferPrice;
                    product.RepositionCost = saleMovementProduct.Product.RepositionCost;
                    product.RepositionCostMiddle = saleMovementProduct.Product.RepositionCostMiddle;
                    product.LiquidCost = saleMovementProduct.Product.LiquidCost;
                    product.LiquidCostMiddle = saleMovementProduct.Product.LiquidCostMiddle;
                    product.RepositionCostReal = saleMovementProduct.Product.RepositionCostReal;
                    product.LiquidCostReal = saleMovementProduct.Product.LiquidCostReal;
                    product.CFOP = saleMovementProduct.Product.CFOP;
                    product.PisType = saleMovementProduct.Product.PisType;
                    product.CofinsType = saleMovementProduct.Product.CofinsType;
                    product.PisPercentage = saleMovementProduct.Product.PisPercentage;
                    product.CofinsPercentage = saleMovementProduct.Product.CofinsPercentage;
                    product.CSTPisCofins = saleMovementProduct.Product.CSTPisCofins;
                    //product.SaleOfferCode = saleMovementProduct.SaleOfferCode > 0 ? saleMovementProduct.SaleOfferCode : (int?)null;
                    //product.SaleOfferQuantity = saleMovementProduct.SaleOfferQuantity > 0 ? saleMovementProduct.SaleOfferQuantity : (decimal?)null;
                    //product.SaleOfferDiscountType = saleMovementProduct.SaleOfferDiscount != null ? (int)saleMovementProduct.SaleOfferDiscount.Type : (int?)null;
                    //product.SaleOfferDiscountValue = saleMovementProduct.SaleOfferDiscount != null ? (saleMovementProduct.SaleOfferDiscount.Type == PercentageOrValueType.Percentage ? saleMovementProduct.SaleOfferDiscount.Value : saleMovementProduct.TotalSaleOfferDiscount) : (decimal?)null;

                    bsSaleMovement.Products.Add(product);
                }
                #endregion


                #region Finalizations
                foreach (var saleMovementFinalization in listOfnSaleOrderFinalization)
                {
                    CeltaWare.CBS.PDV.Concentrator.Repository.ModelSaleMovementFinalization finalizations = new CeltaWare.CBS.PDV.Concentrator.Repository.ModelSaleMovementFinalization();
                    
                    finalizations.SaleMovementFinalizationId = 0;
                    finalizations.SaleMovement = null;

                    finalizations.FinalizationId = CeltaNavsApi.Helpers.SaleMovementFinalizationHelpers.ConvertToBSFinalizationId(saleMovementFinalization.FinalizationId);
                    
                    //finalizations.FinalizationPdvId = saleMovementFinalization.FinalizationId; id da finalizadora vinculada ao pdv
                    finalizations.Value = saleMovementFinalization.Value;
                    finalizations.ValueLiquid = saleMovementFinalization.Value;
                    //finalizations.PayBackValue = saleMovementFinalization.p; Houve Troco ????

                    finalizations.AuthorizationCode = saleMovementFinalization.NSUAUT;
                    
                    finalizations.TEFNetWorkCode = saleMovementFinalization.CODADQ;
                    finalizations.TEFFlagCode = saleMovementFinalization.TIPOCART;
                    finalizations.TEFFlagDescription = saleMovementFinalization.TIPOCART;
                    finalizations.TEFModalityPaymentCode = saleMovementFinalization.TIPOTRANS != null ? CeltaNavsApi.Helpers.SitefHelpers.ConvertToFlagCode(saleMovementFinalization.TIPOTRANS) : null;
                    finalizations.TEFModalityDescription = saleMovementFinalization.TIPOTRANS;
                    
                    finalizations.TEFNSUHost = saleMovementFinalization.NSUAUT;
                    finalizations.TEFNSUSitef = saleMovementFinalization.NSUSIT;
                    finalizations.TEFAuthorizationCode = saleMovementFinalization.NSUSIT;

                    bsSaleMovement.Finalizations.Add(finalizations);
                }
                #endregion
                //mandar o movimento por web-api

                CeltaNavsApi.Services.ConcentradorServices _concentradorServices = new CeltaNavsApi.Services.ConcentradorServices(_navsSettings);
                int result = _concentradorServices.ExportSaleMovement(bsSaleMovement);

                return result;                                
            }
            catch (Exception err)
            {
                throw err;
            }
        }

      

               
    }
}
