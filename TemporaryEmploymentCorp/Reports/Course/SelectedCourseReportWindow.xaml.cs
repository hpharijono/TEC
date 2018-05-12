using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TemporaryEmploymentCorp.Helpers.ReportViewer;
using TemporaryEmploymentCorp.Modules;

namespace TemporaryEmploymentCorp.Reports.Course
{
    /// <summary>
    /// Interaction logic for SelectedCourseReportWindow.xaml
    /// </summary>
    public partial class SelectedCourseReportWindow : Window
    {
        private ReportViewBuilder _reportView;

        public SelectedCourseReportWindow()
        {
            InitializeComponent();
            

            _reportView = new ReportViewBuilder("TemporaryEmploymentCorp.Reports.Course.SelectedCourseReport.rdlc", UpdateDatasetSource());
            _reportView.RefreshDataSourceCallback = UpdateDatasetSource;
            ReportContainer.Content = _reportView.ReportContent;

            DataContext = this;
        }
        public ObservableCollection<DataAccess.EF.Qualification> Prereqs { get; } = new ObservableCollection<DataAccess.EF.Qualification>();
        private IReadOnlyCollection<DataSetValuePair> UpdateDatasetSource()
        {

            var sources = new List<DataSetValuePair>();
            var selectedCourse = ViewModelLocatorStatic.Locator.CourseModule.SelectedCourse;
            var associatedQual = selectedCourse.AssociatedQualification;
            var prereqs = selectedCourse.Prerequisites.Select(c => c.Model);
            foreach (var item in prereqs)
            {
                var qual = item.Qualification;
                Prereqs.Add(qual);
            }
            sources.Add(new DataSetValuePair("CourseDataset", selectedCourse.Model));
            sources.Add(new DataSetValuePair("QualificationDataset", associatedQual));
            sources.Add(new DataSetValuePair("PQualDataset", Prereqs));



            return sources;
        }
    }
}
