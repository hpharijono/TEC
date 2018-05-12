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
using TemporaryEmploymentCorp.Models.Session;
using TemporaryEmploymentCorp.Modules;

namespace TemporaryEmploymentCorp.Reports.Session
{
    /// <summary>
    /// Interaction logic for AllSessionReportWindow.xaml
    /// </summary>
    public partial class AllSessionReportWindow : Window
    {
        private ReportViewBuilder _reportView;
        private string _titleFilter = string.Empty;
        private string _codeFilter;
        private string _courseFilter;

        public AllSessionReportWindow()
        {
            InitializeComponent();

            _reportView = new ReportViewBuilder("TemporaryEmploymentCorp.Reports.Session.AllSessionReport.rdlc", UpdateDatasetSource());
            _reportView.RefreshDataSourceCallback = UpdateDatasetSource;
            ReportContainer.Content = _reportView.ReportContent;

            DataContext = this;
        }

        private IReadOnlyCollection<DataSetValuePair> UpdateDatasetSource()
        {

            var sources = new List<DataSetValuePair>();
            var sessions = ViewModelLocatorStatic.Locator.SessionModule.Sessions.Select(c => c.Model);

            if (CodeFilter != null)
            {
                var sessionsfiltered =
                    sessions.Where(
                        c => c.SessionId.ToLowerInvariant().Contains(CodeFilter.ToLowerInvariant()));
                sessions = sessionsfiltered;
            }

            if (CourseFilter != null)
            {
                var cf = CourseFilter.ToLowerInvariant();
                var sessionsfiltered =
                    sessions.Where(
                        c => c.Course.CourseId.ToLowerInvariant().Contains(cf) || c.Course.CourseName.ToLowerInvariant().Contains(cf));
                sessions = sessionsfiltered;
            }

            sources.Add(new DataSetValuePair("SessionDataset", sessions));

            return sources;
        }

        public string CodeFilter
        {
            get { return _codeFilter; }
            set
            {
                _codeFilter = value;
                _reportView.ReportContent.UpdateDataSource(UpdateDatasetSource());
            }
        }

       

        public string CourseFilter
        {
            get { return _courseFilter; }
            set
            {
                _courseFilter = value;
                _reportView.ReportContent.UpdateDataSource(UpdateDatasetSource());
            }
        }
    }
}
