using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemporaryEmploymentCorp.DataAccess.EF;

namespace TemporaryEmploymentCorp.DataAccess
{
    public class EfRepository : IRepository
    {
        public IDataService<Attendance> Attendance { get; } = new EfDataService<Attendance>();
        public IDataService<Candidate> Candidate { get; } = new EfDataService<Candidate>();
        public IDataService<CanQualify> CanQualify { get; } = new EfDataService<CanQualify>();
        public IDataService<Company> Company { get; } = new EfDataService<Company>();
        public IDataService<Course> Course { get; } = new EfDataService<Course>();
        public IDataService<Enrollment> Enrollment { get; } = new EfDataService<Enrollment>();
        public IDataService<History> History { get; } = new EfDataService<History>();
        public IDataService<Fee> Fee { get; } = new EfDataService<Fee>();
        public IDataService<Opening> Opening { get; } = new EfDataService<Opening>();
        public IDataService<Placement> Placement { get; } = new EfDataService<Placement>();
        public IDataService<Prerequisite> Prerequisite { get; } = new EfDataService<Prerequisite>();
        public IDataService<Qualification> Qualification { get; } = new EfDataService<Qualification>();
        public IDataService<Session> Session { get; } = new EfDataService<Session>();
        public IDataService<Payment> Payment { get; } = new EfDataService<Payment>();
    }
}
