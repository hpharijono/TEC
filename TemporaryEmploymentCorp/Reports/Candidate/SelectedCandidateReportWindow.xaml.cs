using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using TemporaryEmploymentCorp.DataAccess.EF;
using TemporaryEmploymentCorp.Helpers.ReportViewer;
using TemporaryEmploymentCorp.Modules;

namespace TemporaryEmploymentCorp.Reports.Candidate
{
    /// <summary>
    /// Interaction logic for SelectedCandidateReportWindow.xaml
    /// </summary>
    public partial class SelectedCandidateReportWindow : Window
    {
        private ReportViewBuilder _reportView;
        private string _titleFilter = string.Empty;

        public SelectedCandidateReportWindow()
        {
            InitializeComponent();

            _reportView = new ReportViewBuilder("TemporaryEmploymentCorp.Reports.Candidate.SelectedCandidateReport.rdlc", UpdateDatasetSource());
            _reportView.RefreshDataSourceCallback = UpdateDatasetSource;
            ReportContainer.Content = _reportView.ReportContent;

            DataContext = this;

        }
        public ObservableCollection<Qualification> quals= new ObservableCollection<Qualification>(); 
        private IReadOnlyCollection<DataSetValuePair> UpdateDatasetSource()
        {
            var sources = new List<DataSetValuePair>();
            quals.Clear();
            var selectedCandidate = ViewModelLocatorStatic.Locator.CandidateModule.SelectedCandidate;
            var history = selectedCandidate.History.Select(c => c.Model);
            var candQualifications = selectedCandidate.CandidateQualifications.Select(c => c.Model);
            
            foreach (var candQualification in candQualifications)
            {
                var qualification = candQualification.Qualification;
                quals.Add(qualification);
            }
            sources.Add(new DataSetValuePair("QualificationDataset", quals));
            sources.Add(new DataSetValuePair("CandidateDataset", selectedCandidate.Model));
            sources.Add(new DataSetValuePair("HistoryDataset", history));
            sources.Add(new DataSetValuePair("CanQualifyDataset", candQualifications));

            return sources;
        }
    }
}
