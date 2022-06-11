using DataAccessRepository.Context;
using DataAccessRepository.Repositries;
using ModelsRepository;
using ModelsRepository.IRepositries;
using ModelsRepository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessRepository
{
    public class ServiceUnit : IServiceUnit
    {
        private readonly MainContext _context;

        public IBaseRepository<User> Users { get; private set; }

        public IBaseRepository<ServiceType> ServiceType { get; private set; }

        public IBaseRepository<Service> Service { get; private set; }

        public IBaseRepository<Request> Request { get; private set; }

        public IBaseRepository<Report> Report { get; private set; }

        public IBaseRepository<AcceptedRequest> AcceptedRequest { get; private set; }

        public IBaseRepository<DeliveredRequest> DeliveredRequest { get; private set; }

        public IBaseRepository<FailedRequest> FailedRequest { get; private set; }

        public IBaseRepository<RequestReceivers> RequestReceivers { get; private set; }

        public IBaseRepository<ServiceAttachment> ServiceAttachment { get; private set; }

        public IBaseRepository<UserRating> UserRating { get; private set; }

        public IBaseRepository<VolunteerInfo> VolunteerInfo { get; private set; }


        public ServiceUnit(MainContext context)
        {
            _context = context;

            Users = new BaseRepository<User>(_context);
            ServiceType = new BaseRepository<ServiceType>(_context);
            Service = new BaseRepository<Service>(_context);
            Request = new BaseRepository<Request>(_context);
            Report = new BaseRepository<Report>(_context);
            AcceptedRequest = new BaseRepository<AcceptedRequest>(_context);
            UserRating = new BaseRepository<UserRating>(_context);
            ServiceAttachment = new BaseRepository<ServiceAttachment>(_context);
            RequestReceivers = new BaseRepository<RequestReceivers>(_context);
            DeliveredRequest = new BaseRepository<DeliveredRequest>(_context);
            FailedRequest = new BaseRepository<FailedRequest>(_context);
            VolunteerInfo = new BaseRepository<VolunteerInfo>(_context);
        }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
