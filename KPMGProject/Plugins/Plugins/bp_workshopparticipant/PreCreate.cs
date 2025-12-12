using System;
using KPMGProject.Plugins.Plugins.Constants;
using KPMGProject.Plugins.Services;
using Microsoft.Xrm.Sdk;

namespace KPMGProject.Plugins.Plugins.bp_workshopparticipant
{   
    public class PreCreate : PluginBase
    {
        private IWorkshopParticipantService _workshopParticipantService;
        public PreCreate(string unsecure, string secure)
            : base(typeof(PreCreate))
        {
        }

        protected override void ExecuteCdsPlugin(ILocalPluginContext localContext)
        {
            if(ShouldExecutePlugin(localContext.PluginExecutionContext))
            {
                _workshopParticipantService = new WorkshopParticipantService(localContext);
                _workshopParticipantService.CheckAndUpdateParticipantLimit();
            }
        }

        private bool ShouldExecutePlugin(IPluginExecutionContext context)
        {
            return context.MessageName.Equals("Create", StringComparison.OrdinalIgnoreCase)
                && context.Stage == (int)PluginStage.PreOperation
                && context.PrimaryEntityName.Equals("bp_workshopparticipant", StringComparison.OrdinalIgnoreCase);
        }
    }
}
