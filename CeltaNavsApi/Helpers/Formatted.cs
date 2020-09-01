using CeltaNavs.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CeltaNavsApi.Helpers
{
    public class Formatted
    {
        public static string FormatDescriptionSummary(ModelProduct product)
        {
            string desc;
            if (product.NameReduced.Length > 20)
            {
                desc = product.NameReduced.Substring(0, 20);
            }
            else
            {
                desc = product.NameReduced.Substring(0);
                for (int i = product.NameReduced.Length; i < 20; i++)
                {
                    desc += " ";
                }
            }
            return desc;
        }

        public static string FormatDescription(ModelProduct product)
        {
            string desc;
            if (product.NameReduced.Length > 30)
            {
                desc = product.NameReduced.Substring(0, 30);
            }
            else
            {
                desc = product.NameReduced.Substring(0);
                for (int i = product.NameReduced.Length; i < 30; i++)
                {
                    desc += "%20";
                }

            }
            // desc = product.NameReduced;            

            return desc;
        }

        public static string FormatDescription20positions(ModelProduct product)
        {
            string desc;
            if (product.NameReduced.Length > 20)
            {
                desc = product.NameReduced.Substring(0, 20);
            }
            else
            {
                desc = product.NameReduced.Substring(0);
                for (int i = product.NameReduced.Length; i < 20; i++)
                {
                    desc += "%20";
                }

            }
            // desc = product.NameReduced;            

            return desc;
        }

        public static string FormatCode(ModelProduct product)
        {
            string cod;
            if (product.PriceLookupCode.Length >= 8)
            {
                cod = product.PriceLookupCode.Substring(0, 7);
            }
            else
            {
                if (product.PriceLookupCode.Contains("-"))
                {
                    cod = product.PriceLookupCode.Substring(0, product.PriceLookupCode.IndexOf("-"));
                    cod += "%20";
                }
                else
                {
                    cod = product.PriceLookupCode.Substring(0);
                }
                //for (int i = product.PersonalizedCode.Length; i < 7; i++)
                //{
                //    cod += "%20";
                //}
            }

            return cod;
        }

        public static string FormatError(string message)
        {
            try
            {
                int tamanhoTotal = message.Length;
                int inicioCorte = 0;
                int fimCorte = 30;
                int quantoFalta = 1;
                string newMessage = "";
                while (quantoFalta > 0)
                {
                    if (message.Length < 30)
                    {
                        return message;
                    }
                    newMessage += message.Substring(inicioCorte, 30) + "<BR>";
                    inicioCorte = fimCorte - 1;
                    fimCorte = inicioCorte + 30;
                    quantoFalta = (message.Length - 1) - (newMessage.Length - 1);
                    if (quantoFalta < 30)
                    {
                        newMessage += message.Substring(inicioCorte, quantoFalta);
                        return newMessage;
                    }

                }
                return newMessage;
            }
            catch (Exception err)
            {
                throw err;
            }
        }
    }
}