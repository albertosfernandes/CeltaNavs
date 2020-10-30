using CeltaNavs.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeltaNavs.Domain
{
    public class ProductDao : Persistent
    {
        public ModelProduct FindByInternalCode(string productCode, ModelNavsSetting navsSettings)
        {            
            int productInt = Convert.ToInt32(productCode);
            return context.Products.Where(prod =>
            prod.InternalCodeOnERP == productInt && prod.EnterpriseId == navsSettings.EnterpriseId).FirstOrDefault();         
        }

        public ModelProduct FindByPlu(string productCode, ModelNavsSetting navsSettings)
        {
            string productCodeWithDigit;
            if (productCode.Contains("-"))
            {
                productCodeWithDigit = productCode;
            }
            else
            {
                productCodeWithDigit = productCode;                
                productCodeWithDigit = productCodeWithDigit.PadLeft(Convert.ToInt32(navsSettings.NumberOfCharacteresPLU), '0');
                productCodeWithDigit += "-" + CheckDigit(productCode);
            }

            return context.Products.Where(prod =>
            prod.PriceLookupCode == productCodeWithDigit && prod.EnterpriseId == navsSettings.EnterpriseId).FirstOrDefault();
        }

        public ModelProduct FindByEan(string productCode, ModelNavsSetting navsSettings)
        {            
            return context.Products.Where(prod =>
            prod.EanCode == productCode && prod.EnterpriseId == navsSettings.EnterpriseId).FirstOrDefault();            
        }

        public List<ModelProduct> GetAll(ModelNavsSetting navsSettings)
        {
            try
            {
                return context.Products
                        .Where(p => p.EnterpriseId == navsSettings.EnterpriseId)
                        .ToList();

            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public List<ModelProduct> GetByGroup(ModelNavsSetting navsSettings, string groupCode)
        {
            int groupCodeInt = Convert.ToInt32(groupCode);
            try
            {
                var productsGroup = from product in context.Products
                                    join groupProduct in context.ExpansibleGroupsProducts
                                    on product.InternalCodeOnERP equals groupProduct.ProductInternalCodeOnErp
                                    where product.EnterpriseId == navsSettings.EnterpriseId && groupProduct.ExpansibleGroupId == groupCodeInt
                                    orderby product.NameReduced descending
                                    select product;

                return productsGroup.ToList();
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public string CheckDigit(string number)
        {
            int sum = 0;
            bool oddParity = true;

            for (int i = number.Length - 1; i >= 0; i--)
            {
                if (oddParity)
                    sum += 3 * (number[i] - Constants.ValueZero[0]);
                else
                    sum += number[i] - Constants.ValueZero[0];

                oddParity = !oddParity;
            }

            int checkDigit = 10 - (sum % 10);

            if (checkDigit == 10)
                checkDigit = 0;

            return Convert.ToChar(checkDigit + Constants.ValueZero[0]).ToString();
        }

        private struct Constants
        {
            public const string ValueZero = "0";
            public const string ValueNine = "9";
            public const string ValueHypen = "-";
            public const string ValueSpace = " ";
        }
    }
}
