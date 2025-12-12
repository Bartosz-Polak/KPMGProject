using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPMGProject.Plugins.Repositories
{
    public class WorkshopRepository : RepositoryBase, IWorkshopRepository
    {
        public WorkshopRepository(IOrganizationService organizationService, ITracingService tracingService) : base(organizationService, tracingService)
        {
        }

        public Entity GetById(Guid id)
        {
            return _organizationService.Retrieve("bp_workshop", id, new ColumnSet("bp_maxparticipants", "bp_currentparticipants"));
        }

        public EntityCollection GetWorkshopsInDateRange(DateTime minDate, DateTime maxDate)
        {
            QueryExpression query = new QueryExpression("bp_workshop");
            query.ColumnSet.AddColumns("bp_workshopid", "bp_nearbyworkshops", "bp_workshopname");

            query.Criteria.AddCondition("bp_date", ConditionOperator.Between, minDate, maxDate);
            query.Criteria.FilterOperator = LogicalOperator.And;

            return _organizationService.RetrieveMultiple(query);
        }
    }
}
