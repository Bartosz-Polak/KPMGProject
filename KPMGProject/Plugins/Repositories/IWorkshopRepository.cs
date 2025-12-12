using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;

namespace KPMGProject.Plugins.Repositories
{
    public interface IWorkshopRepository : IRepositoryBase
    {
        Entity GetById(Guid id);
        EntityCollection GetWorkshopsInDateRange(DateTime minDate, DateTime maxDate);
    }
}
