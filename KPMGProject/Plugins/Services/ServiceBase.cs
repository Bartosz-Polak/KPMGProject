using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Services;
using System.Text;
using System.Threading.Tasks;

namespace KPMGProject.Plugins.Services
{
    public abstract class ServiceBase
    {
        protected readonly IPluginExecutionContext _pluginExecutionContext;
        protected readonly IOrganizationService _organizationServiceUser;
        protected readonly IOrganizationService _organizationServiceAdmin;
        protected readonly ITracingService _tracingService;
        protected readonly Entity _target;
        protected readonly Entity _preImage;
        protected ServiceBase(ILocalPluginContext localPluginContext)
        {
            if (localPluginContext == null)
            {
                throw new InvalidPluginExecutionException(nameof(_pluginExecutionContext));
            }
            // Obtain the tracing service
            _tracingService = localPluginContext.TracingService;

            try
            {
                _pluginExecutionContext = localPluginContext.PluginExecutionContext;
                _organizationServiceUser = localPluginContext.CurrentUserService;
                _organizationServiceAdmin = localPluginContext.SystemUserService;
                _target = (Entity)_pluginExecutionContext.InputParameters["Target"];
                if (_target == null)
                {
                    throw new InvalidPluginExecutionException("Target entity is null.");
                }

            }
            catch (Exception ex)
            {
                _tracingService?.Trace("An error occurred executing Plugin KPMGProject.Plugins.PreOperationbp_workshopparticipantCreate : {0}", ex.ToString());
                throw new InvalidPluginExecutionException("An error occurred executing Plugin KPMGProject.Plugins.PreOperationbp_workshopparticipantCreate .", ex);
            }
        }
    }
}
