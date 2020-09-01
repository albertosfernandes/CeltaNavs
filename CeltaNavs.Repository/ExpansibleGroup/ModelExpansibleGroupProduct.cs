
using System.ComponentModel.DataAnnotations;

namespace CeltaNavs.Repository
{
    public class ModelExpansibleGroupProduct
    {
        [Key]
        public int ExpansibleGroupId { get; set; }       
        public int ProductInternalCodeOnErp { get; set; }
    }
}
