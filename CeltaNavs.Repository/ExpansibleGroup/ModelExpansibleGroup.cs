using System.ComponentModel.DataAnnotations;

namespace CeltaNavs.Repository
{
    public class ModelExpansibleGroup
    {
        [Key]
        public int ExpansibleGroupId { get; set; }
        public string Name { get; set; }      
    }
}
