using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeltaNavs.Repository
{
    public enum SaleRequestOrigin
    {
        BS = 1,
        Concentrator = 2,
    }

    public enum ProductionStatus
    {
        New = 0,
        InProduction = 1,
        Delivered = 2
    }
}
