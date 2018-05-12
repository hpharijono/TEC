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

namespace TemporaryEmploymentCorp.Reports.Session
{
    /// <summary>
    /// Interaction logic for SelectedSesssionReportWindow.xaml
    /// </summary>
    public partial class SelectedSesssionReportWindow : Window
    {
        private ReportViewBuilder _reportView;
        private string _titleFilter = string.Empty;

        public SelectedSesssionReportWindow()
        {
            InitializeComponent();

            _reportView = new ReportViewBuilder("TemporaryEmploymentCorp.Reports.Session.SelectedSessionReport.rdlc", UpdateDatasetSource());
            _reportView.RefreshDataSourceCallback = UpdateDatasetSource;
            ReportContainer.Content = _reportView.ReportContent;

            DataContext = this;
        }
        public ObservableCollection<DataAccess.EF.Candidate> Cands { get; } = new ObservableCollection<DataAccess.EF.Candidate>(); 
        private IReadOnlyCollection<DataSetValuePair> UpdateDatasetSource()
        {
            var sources = new List<DataSetValuePair>();

            var selectedSession = ViewModelLocatorStatic.Locator.SessionModule.SelectedSession;
            sources.Add(new DataSetValuePair("SessionDataset", selectedSession.Model));

            var course = selectedSession.Model.Course;
            sources.Add(new DataSetValuePair("CourseDataset", course));

            var enrollments = selectedSession.EnrolledCandidatesList.Select(c => c.Model);
            foreach (var item in enrollments)
            {
                var candidates = item.Candidate;
                Cands.Add(candidates);
            }
            sources.Add(new DataSetValuePair("CandidateDataset", Cands));
            return sources;
        }
    }
}
