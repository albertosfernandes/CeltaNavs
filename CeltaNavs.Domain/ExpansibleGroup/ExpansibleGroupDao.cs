using CeltaNavs.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeltaNavs.Domain
{
    public class ExpansibleGroupDao : Persistent
    {
        public List<ModelExpansibleGroup> Get(ModelNavsSetting settings)
        {            
            List<ModelExpansibleGroup> groupList = new List<ModelExpansibleGroup>();
            try
            {
                var resp = from expandableGroup in context.ExpansibleGroups
                           join expandableGroupProd in context.ExpansibleGroupsProducts
                           on expandableGroup.ExpansibleGroupId equals expandableGroupProd.ExpansibleGroupId
                           join product in context.Products
                           on expandableGroupProd.ProductInternalCodeOnErp equals product.InternalCodeOnERP
                           where product.EnterpriseId == settings.EnterpriseId
                           group expandableGroup by new
                           {                               
                               expandableGroup.ExpansibleGroupId,
                               expandableGroup.Name

                           } into nsGroup                     
                           select new
                           {                            
                               nsGroup.Key.ExpansibleGroupId,
                               nsGroup.Key.Name                          
                           };
                foreach (var groupitem in resp)
                {
                    ModelExpansibleGroup g = new ModelExpansibleGroup();
                    g.ExpansibleGroupId = groupitem.ExpansibleGroupId;                    
                    g.Name = groupitem.Name;
                    groupList.Add(g);
                }

                return groupList;

            }
            catch(Exception err)
            {
                throw err;
            }
        }
    }
}
