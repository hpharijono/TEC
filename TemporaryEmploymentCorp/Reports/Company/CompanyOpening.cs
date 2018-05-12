using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemporaryEmploymentCorp.DataAccess.EF;

namespace TemporaryEmploymentCorp.Reports.Company
{
    public class CompanyOpening
    {
        private readonly DataAccess.EF.Company _selectedCompany;
        private readonly DataAccess.EF.Opening _opening;
        private readonly DataAccess.EF.Qualification _qualification;



        public CompanyOpening(DataAccess.EF.Company selectedCompany, DataAccess.EF.Opening opening, Qualification qualification)
        {
            _selectedCompany = selectedCompany;
            _opening = opening;
            _qualification = qualification;

        }

        public string QualificationCode
        {
            get { return _qualification.QualificationCode; }
            set { _qualification.QualificationCode = value; }
        }

        public string CompanyName
        {
            get { return _selectedCompany.CompanyName; }
            set { _selectedCompany.CompanyName = value; }
        }

        public string OpeningNumber
        {
            get { return _opening.OpeningNumber; }
            set { _opening.OpeningNumber = value; }
        }

        public DateTime? StartDate
        {
            get { return _opening.StartDate; }
            set { _opening.StartDate = value; }
        }

        public DateTime? EndDate
        {
            get { return _opening.EndDate; }
            set { _opening.EndDate = value; }
        }

        public string HourlyPay
        {
            get { return _opening.HourlyPay; }
            set { _opening.HourlyPay = value; }
        }
    }
}
