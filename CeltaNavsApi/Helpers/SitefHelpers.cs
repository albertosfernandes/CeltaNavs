using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CeltaNavsApi.Helpers
{
    public class SitefHelpers
    {
        public static string ConvertToFlagCode(string tipoCard)
        {
            //a vista = 00
            //Debito = 01
            //CREDITO  = 02
            //outros = 99            
            if (tipoCard.ToUpperInvariant().Contains("DEBITO"))
            {
                return "0100"; //Debito a vista
            }

            if (tipoCard.ToUpperInvariant().Contains("CREDITO"))
            {
                return "0200"; //Credito a vista
            }

            return "0900"; //outros
        }
    }
}