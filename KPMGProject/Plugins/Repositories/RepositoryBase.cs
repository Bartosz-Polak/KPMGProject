using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPMGProject.Plugins.Repositories
{
    public abstract class RepositoryBase : IRepositoryBase
    {
        protected readonly IOrganizationService _organizationService;
        protected readonly ITracingService _tracingService;
        protected RepositoryBase(IOrganizationService organizationService, ITracingService tracingService) {
            _organizationService = organizationService;
            _tracingService = tracingService;
        }

        public void Create(Entity entity)
        {
            _organizationService.Create(entity);
        }

        public void Update(Entity entity)
        {
            _organizationService.Update(entity);
        }
    }
}
