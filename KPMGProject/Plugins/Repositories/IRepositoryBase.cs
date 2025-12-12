using Microsoft.Xrm.Sdk;

namespace KPMGProject.Plugins.Repositories
{
    public interface IRepositoryBase
    {
        void Create(Entity entity);
        void Update(Entity entity);
    }
}