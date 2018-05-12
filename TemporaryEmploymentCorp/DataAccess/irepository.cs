using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemporaryEmploymentCorp.DataAccess.EF;

namespace TemporaryEmploymentCorp.DataAccess
{
    public interface IRepository
    {
        IDataService<Attendance> Attendance { get; } 
        IDataService<Candidate> Candidate { get; } 
        IDataService<CanQualify> CanQualify { get; } 
        IDataService<Company> Company { get; } 
        IDataService<Course> Course { get; } 
        IDataService<Enrollment> Enrollment { get; } 
        IDataService<History> History { get; } 
        IDataService<Fee> Fee { get; }
        IDataService<Opening> Opening { get; } 
        IDataService<Placement> Placement { get; } 
        IDataService<Prerequisite> Prerequisite { get; } 
        IDataService<Qualification> Qualification { get; } 
        IDataService<Session> Session { get; } 
        IDataService<Payment> Payment { get; } 
    }
}
