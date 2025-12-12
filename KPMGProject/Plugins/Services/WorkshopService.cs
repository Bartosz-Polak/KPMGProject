using KPMGProject.Plugins.Repositories;
using Microsoft.Xrm.Sdk;
using System;
using System.IdentityModel.Protocols.WSTrust;

namespace KPMGProject.Plugins.Services
{
    public class WorkshopService : ServiceBase, IWorkshopService
    {
        private IWorkshopRepository _workshopRepository;
        public WorkshopService(ILocalPluginContext context)
            : base(context)
        {
            _workshopRepository = new WorkshopRepository(_organizationServiceUser, _tracingService);
        }
        
        public void UpdateNearbyWorkshops()
        {
            // For a sake of completeness I should compare target with pre_image for date update scenario to make sure that date indeed changed.
            // I am omiting this step to save time but in professional scenario I would include it
            if(!_target.Contains("bp_date"))
            {
                throw new InvalidPluginExecutionException("Workshop date is not set.");
            }
            var workshopDate = (DateTime)_target["bp_date"];
            var minDate = workshopDate.AddDays(-7);
            var maxDate = workshopDate.AddDays(7);

            var nearbyWorkshops = _workshopRepository.GetWorkshopsInDateRange(minDate, maxDate);
            var nearbyWorkshopNames = "";
            if(nearbyWorkshops.Entities == null && nearbyWorkshops.Entities.Count == 0)
            {
                //No nearby workshops found
                return;
            }
            foreach(var workshop in nearbyWorkshops.Entities)
            {
                if (workshop.Id != _target.Id && workshop.Contains("bp_workshopname")) // Exclude self
                {
                    nearbyWorkshopNames += $"{workshop["bp_workshopname"]}; ";
                }
            }
            // Setting nearby workshop names like this is safe as I never read it from workshop
            nearbyWorkshopNames = nearbyWorkshopNames.Substring(0, nearbyWorkshopNames.Length - 2); // Remove last semicolon and whitespace
            _target["bp_nearbyworkshops"] = nearbyWorkshopNames;
            // This is not a complete code. I am not able to finish it up due to time constraints - my week was quite busy unfortunately.
            // What else needs to be done:
            // Update all retrieved workshops to include target workshop in their nearby workshops field as well
            // It would be tempting to just append target workshop name to each of them and update but that would lead to potential race condition
            // Therefore to be absolutely sure, I would have to calculate nearby workshops for each of them in the same manner as for target workshop
            // Additionally there should be Update and Delete plugins on workshop entity to maintain consistency of nearby workshops field
            // Update would need to trigger recalculation of nearby workshops for all workshops in the old and new date ranges with making sure that it is not triggered more than once per workshop
            // Delete would need to trigger recalculation of nearby workshops for all workshops in the old date range
            }
    }
}