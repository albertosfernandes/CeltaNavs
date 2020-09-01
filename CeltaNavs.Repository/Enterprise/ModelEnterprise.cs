using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeltaNavs.Repository
{
    public class ModelEnterprise
    {
        [Key]
        public int EnterpriseId { get; set; }
        public string FantasyName { get; set; }
        public int EnterpriseCode { get; set; }
        public string PersonalizedCode { get; set; }
        public int FederalSimpleType { get; set; }
        public string CNPJ { get; set; }
        public string InscriptionState { get; set; }
        public string AddressName { get; set; }
        public string AddressDistrict { get; set; }
        public string AddressCity { get; set; }
        public string AddressZip { get; set; }
        public string AddressState { get; set; }
        public string Telephone { get; set; }
    }
}
