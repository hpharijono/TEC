using System;
using System.Collections.Generic;
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
    /// Interaction logic for AllCourseReportWindow.xaml
    /// </summary>
    public partial class AllCourseReportWindow : Window
    {
        private ReportViewBuilder _reportView;
        private string _titleFilter = string.Empty;

        public AllCourseReportWindow()
        {
            InitializeComponent();

            _reportView = new ReportViewBuilder("TemporaryEmploymentCorp.Reports.Course.AllCourseReport.rdlc", UpdateDatasetSource());
            _reportView.RefreshDataSourceCallback = UpdateDatasetSource;
            ReportContainer.Content = _reportView.ReportContent;

            DataContext = this;
        }

        private IReadOnlyCollection<DataSetValuePair> UpdateDatasetSource()
        {
            var sources = new List<DataSetValuePair>();

            var courses = ViewModelLocatorStatic.Locator.CourseModule.Courses.Select(c => c.Model);
            sources.Add(new DataSetValuePair("CourseDataset", courses.Where(c => TitleFilter != null && c.CourseName.ToLowerInvariant().Contains(TitleFilter.ToLowerInvariant())
            || c.CourseId.ToLowerInvariant().Contains(TitleFilter.ToLowerInvariant()))));


            return sources;
        }

        public string TitleFilter
        {
            get { return _titleFilter; }
            set
            {
                _titleFilter = value;
                _reportView.ReportContent.UpdateDataSource(UpdateDatasetSource());
            }
        }
    }
}
