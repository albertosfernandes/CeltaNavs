using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeltaNavs.Repository
{
    public class ModelPdv
    {
        [Key]
        public int PdvId { get; set; }
        public int PdvNumber { get; set; }
        public string IpAddress { get; set; }
        public bool IsUsing { get; set; }
        public string MacAddress { get; set; }
        public string VersionNumber { get; set; }
        public int EnterpriseId { get; set; }
    }
}
