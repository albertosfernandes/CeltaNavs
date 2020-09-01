using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeltaNavs.Repository
{
    public class ModelNavsSetting
    {
        [Key]
        public int NavsSettingsId { get; set; }
        public ModelEnterprise Enterprises { get; set; }
        public int EnterpriseId { get; set; }
        public ModelPdv Pdvs { get; set; }
        public int PdvId { get; set; }
        public string PosSerial { get; set; }
        public string NumberOfCharacteresPLU { get; set; }
        public bool SaveXMLSat { get; set; }
        public string SitefAddressIp { get; set; }
        public string SitefPort { get; set; }
        public string SitefStoreCode { get; set; }
        public string ConcentratorAddress { get; set; }
        public string ConcentratorPort { get; set; }
        public string SatAddressSharePdv { get; set; }
        public string SatPortSharePdv { get; set; }
    }
}
