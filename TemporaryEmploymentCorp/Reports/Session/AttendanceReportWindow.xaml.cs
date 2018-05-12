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
    /// Interaction logic for AttendanceReportWindow.xaml
    /// </summary>
    public partial class AttendanceReportWindow : Window
    {
        private ReportViewBuilder _reportView;

        public AttendanceReportWindow()
        {
            InitializeComponent();

            _reportView = new ReportViewBuilder("TemporaryEmploymentCorp.Reports.Session.AttendanceReport.rdlc", UpdateDatasetSource());
            _reportView.RefreshDataSourceCallback = UpdateDatasetSource;
            ReportContainer.Content = _reportView.ReportContent;

            DataContext = this;
        }
        public ObservableCollection<DataAccess.EF.Candidate> Cands { get; } = new ObservableCollection<DataAccess.EF.Candidate>();
        public ObservableCollection<DataAccess.EF.Candidate> AbsentCands { get; } = new ObservableCollection<DataAccess.EF.Candidate>();

        private IReadOnlyCollection<DataSetValuePair> UpdateDatasetSource()
        {
            var _sources = new List<DataSetValuePair>();

            var selectedSession = ViewModelLocatorStatic.Locator.SessionModule.SelectedSession;
            _sources.Add(new DataSetValuePair("SessionDataset", selectedSession.Model));

            var course = selectedSession.Model.Course;
            _sources.Add(new DataSetValuePair("CourseDataset", course));

            var attendees = selectedSession.PresentCandidatesList.Select(c => c.Model);
            foreach (var item in attendees)
            {
                var candidates = item.Candidate;
                Cands.Add(candidates);
            }
            _sources.Add(new DataSetValuePair("CandidateDataset", Cands));

            var absentees = selectedSession.AbsentCandidatesList.Select(c => c.Model);
            foreach (var item in absentees)
            {
                var candidates = item.Candidate;
                AbsentCands.Add(candidates);
            }
            _sources.Add(new DataSetValuePair("AbsentCandidateDataset", AbsentCands));

            return _sources;
        }
    }
}
