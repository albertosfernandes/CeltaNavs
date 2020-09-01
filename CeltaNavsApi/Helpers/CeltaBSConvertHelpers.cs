using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CeltaWare.CBS.PDV.Concentrator.Repository;

namespace CeltaNavsApi.Helpers
{
    public class CeltaBSConvertHelpers
    {
        public static SaleFiscalType ToSaleFiscalType(string fiscalType)
        {
            if(fiscalType.ToUpperInvariant() == "SAT")
            {
                return SaleFiscalType.SAT;
            }
            
            if (fiscalType.ToUpperInvariant() == "NFCE")
            {
                return SaleFiscalType.NFCe;
            }

            if (fiscalType.ToUpperInvariant() == "SATEMULADOR")
            {
                return SaleFiscalType.SATEmulador;
            }
            else
            {
                return SaleFiscalType.SATEmulador;
            }            
        }
    }
}