using ModelsRepository.IRepositries;
using ModelsRepository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelsRepository
{
    public interface IServiceUnit : IDisposable
    {
        IBaseRepository<User> Users { get; }
        IBaseRepository<ServiceType> ServiceType { get; }
        IBaseRepository<Service> Service { get; }
        IBaseRepository<Request> Request { get; }
        IBaseRepository<Report> Report { get; }
        IBaseRepository<AcceptedRequest> AcceptedRequest { get; }
        IBaseRepository<DeliveredRequest> DeliveredRequest { get; }
        IBaseRepository<FailedRequest> FailedRequest { get; }
        IBaseRepository<RequestReceivers> RequestReceivers { get; }
        IBaseRepository<ServiceAttachment> ServiceAttachment { get; }
        IBaseRepository<UserRating> UserRating { get; }
        IBaseRepository<VolunteerInfo> VolunteerInfo { get; }
        int Complete();
    }
}
