using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CeltaNavs.Repository;

namespace CeltaNavs.Domain
{
    public class NavsSettingDao : Persistent
    {
        public void Add(ModelNavsSetting settings)
        {
            context.NavsSettings.Add(settings);
            context.SaveChanges();
        }

        public ModelNavsSetting GetByEnterpriseAndPdv(string _emp, string _pdv)
        {
            if(_emp == "undefined")
            {
                _emp = "0";
            }
            
            if(_pdv == "undefined")
            {
                _pdv = "0";
            }
            int emp = Convert.ToInt32(_emp);
            int pdv = Convert.ToInt32(_pdv);
            return context.NavsSettings
                .Where(s => s.EnterpriseId == emp && s.PdvId == pdv)
                .FirstOrDefault<ModelNavsSetting>();
        }

        public ModelNavsSetting GetById(string posSerial)
        {            
            return context.NavsSettings
                            .Where(s => s.PosSerial == posSerial)
                            .FirstOrDefault<ModelNavsSetting>();
        }

        public ModelNavsSetting Get(string posSerial)
        {
            try
            {
                ModelNavsSetting newNavsSettings = new ModelNavsSetting();

                var resp = from nSettings in context.NavsSettings
                           join ent in context.Enterprises
                           on nSettings.EnterpriseId equals ent.EnterpriseId
                           join pdv in context.Pdvs
                           on nSettings.PdvId equals pdv.PdvId
                           where nSettings.PosSerial == posSerial
                           select new
                           {
                               nSettings,                           
                               empresas = ent,
                               pdvs = pdv
                           };

                foreach (var item in resp)
                {
                    newNavsSettings = item.nSettings;                
                    newNavsSettings.Enterprises = item.empresas;
                    newNavsSettings.Pdvs = item.pdvs;
                }
                return newNavsSettings;
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public List<ModelEnterprise> GetAllEnterprises()
        {
            return context.Enterprises.ToList<ModelEnterprise>();
        }

        public List<ModelPdv> GetAllPdvs(string _enterpriseid)
        {
            int ent = Convert.ToInt32(_enterpriseid);
            return context.Pdvs.Where(p => p.EnterpriseId == ent && p.VersionNumber == "NAVS").ToList<ModelPdv>();
        }

        public void UpdateNavsSettings(ModelNavsSetting modelNavsSettings)
        {
            try
            {
                var setting = context.NavsSettings.Find(modelNavsSettings.NavsSettingsId);

                setting.ConcentratorAddress = modelNavsSettings.ConcentratorAddress;
                setting.ConcentratorPort = modelNavsSettings.ConcentratorPort;
                setting.EnterpriseId = modelNavsSettings.EnterpriseId;
                setting.NumberOfCharacteresPLU = modelNavsSettings.NumberOfCharacteresPLU;
                setting.PdvId = modelNavsSettings.PdvId;
                setting.PosSerial = modelNavsSettings.PosSerial;
                setting.SatAddressSharePdv = modelNavsSettings.SatAddressSharePdv;
                setting.SatPortSharePdv = modelNavsSettings.SatPortSharePdv;
                setting.SaveXMLSat = modelNavsSettings.SaveXMLSat;
                setting.SitefAddressIp = modelNavsSettings.SitefAddressIp;
                setting.SitefPort = modelNavsSettings.SitefPort;
                setting.SitefStoreCode = modelNavsSettings.SitefStoreCode;                
                
                context.SaveChanges();
            }
            catch(Exception er)
            {
                throw er;
            }
        }
    }
}
