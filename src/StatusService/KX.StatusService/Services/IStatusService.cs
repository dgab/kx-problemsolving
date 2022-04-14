using System.Collections.Generic;
using System.Threading.Tasks;

namespace KX.StatusService.Services
{
    public interface IStatusService
    {
        Task<IEnumerable<ContainerStatus>> GetContainerStatuses(string id, string label);
    }
}