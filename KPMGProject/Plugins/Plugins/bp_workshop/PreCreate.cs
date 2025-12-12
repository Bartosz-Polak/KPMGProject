using System;
using KPMGProject.Plugins.Plugins.Constants;
using KPMGProject.Plugins.Services;
using Microsoft.Xrm.Sdk;

namespace KPMGProject.Plugins.Plugins.bp_workshop
{   
    public class PreCreate : PluginBase
    {
        private IWorkshopService _workshopService;
        public PreCreate(string unsecure, string secure)
            : base(typeof(PreCreate))
        {
        }

        protected override void ExecuteCdsPlugin(ILocalPluginContext localContext)
        {
            if(ShouldExecutePlugin(localContext.PluginExecutionContext))
            {
                _workshopService = new WorkshopService(localContext);
                _workshopService.UpdateNearbyWorkshops();
            }
        }

        private bool ShouldExecutePlugin(IPluginExecutionContext context)
        {
            return context.MessageName.Equals("Create", StringComparison.OrdinalIgnoreCase)
                && context.Stage == (int)PluginStage.PreOperation
                && context.PrimaryEntityName.Equals("bp_workshop", StringComparison.OrdinalIgnoreCase);
        }
    }
}
