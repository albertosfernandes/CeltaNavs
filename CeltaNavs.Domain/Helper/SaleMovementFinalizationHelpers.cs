﻿using CeltaWare.CBS.PDV.Concentrator.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeltaNavs.Domain
{
    public class SaleMovementFinalizationHelpers
    {
        public static FinalizationIdentification ConvertToBSFinalizationId(int id)
        {
            if (id == 5)
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
