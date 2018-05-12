using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TemporaryEmploymentCorp.DataAccess;

namespace TemporaryEmploymentCorp.Modules
{
    public class ViewModelLocator
    {
        private static readonly EfRepository _repository = new EfRepository();

        public ViewModelLocator()
        {
            CompanyModule = new CompanyModule(_repository);
            QualificationModule = new QualificationModule(_repository);
            CourseModule = new CourseModule(_repository);
            OpeningModule = new OpeningModule(_repository);
            CandidateModule = new CandidateModule(_repository);
            SessionModule = new SessionModule(_repository);
        }

        public SessionModule SessionModule { get; set; }

        public CandidateModule CandidateModule { get; set; }

        public OpeningModule OpeningModule { get; set; }

        public CourseModule CourseModule { get; set; }

        public QualificationModule QualificationModule { get; set; }

        public CompanyModule CompanyModule { get; set; }
    }

    public static class ViewModelLocatorStatic
    {
        public static ViewModelLocator Locator = Application.Current.Resources["Locator"] as ViewModelLocator;
    }
}
