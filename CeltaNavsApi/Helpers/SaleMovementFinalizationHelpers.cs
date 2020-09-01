using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CeltaWare.CBS.PDV.Concentrator.Repository;

namespace CeltaNavsApi.Helpers
{
    public class SaleMovementFinalizationHelpers
    {
        public static FinalizationIdentification ConvertToBSFinalizationId(int id)
        {
            if(id == 5)
            {
                return FinalizationIdentification.DebitCard;
            }

            if (id == 6)
            {
                return FinalizationIdentification.CreditCard;
            }

            return FinalizationIdentification.Money;
        }
    }
}