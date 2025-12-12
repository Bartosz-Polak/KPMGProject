using KPMGProject.Plugins.Repositories;
using Microsoft.Xrm.Sdk;
using System.IdentityModel.Protocols.WSTrust;

namespace KPMGProject.Plugins.Services
{
    public class WorkshopParticipantService : ServiceBase, IWorkshopParticipantService
    {
        private IWorkshopRepository _workshopRepository;
        public WorkshopParticipantService(ILocalPluginContext context)
            : base(context)
        {
            _workshopRepository = new WorkshopRepository(_organizationServiceUser, _tracingService);
        }

        public void CheckAndUpdateParticipantLimit()
        {
            if (!_target.Contains("bp_workshopid"))
            {
                throw new InvalidPluginExecutionException("Participant must be associated with a workshop.");
            }
            EntityReference workshopRef = (EntityReference)_target.Attributes["bp_workshopid"];
            if (!_target.Contains("bp_contact"))
            {
                throw new InvalidPluginExecutionException("Participant must be associated with a contact.");
            }
            EntityReference contactRef = (EntityReference)_target.Attributes["bp_contact"];

            AquireLockOnWorkshop(workshopRef);

            var workshopCrm = _workshopRepository.GetById(workshopRef.Id);
            if (!workshopCrm.Contains("bp_maxparticipants"))
            {
                //Workshop maximum participant limit is not set.
                return;
            }
            var maxParticipants = (int)workshopCrm["bp_maxparticipants"];
            var currentParticipants = 0;
            if (workshopCrm.Contains("bp_currentparticipants"))
            {
                currentParticipants = (int)workshopCrm["bp_currentparticipants"];
            }
            if (maxParticipants > currentParticipants)
            {
                IncreaseParticipantsCount(workshopRef, currentParticipants);
            }
            else
            {
                throw new InvalidPluginExecutionException("Workshop has reached its maximum participant limit.");
            }
        }

        private void IncreaseParticipantsCount(EntityReference workshopRef, int currentCount)
        {
            Entity workshop = new Entity("bp_workshop");
            workshop["bp_workshopid"] = workshopRef.Id;
            workshop["bp_currentparticipants"] = currentCount + 1;
            workshop["bp_writelock"] = false;
            _workshopRepository.Update(workshop);
        }

        private void AquireLockOnWorkshop(EntityReference workshopRef)
        {
            var workshop = new Entity("bp_workshop");
            workshop["bp_workshopid"] = workshopRef.Id;
            workshop["bp_writelock"] = true;
            // Update to aquire write lock and avoid race
            _workshopRepository.Update(workshop);
            return;
        }
    }
}