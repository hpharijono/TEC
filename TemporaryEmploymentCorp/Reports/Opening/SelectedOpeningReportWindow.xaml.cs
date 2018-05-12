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
using TemporaryEmploymentCorp.Models.Company;
using TemporaryEmploymentCorp.Modules;

namespace TemporaryEmploymentCorp.Reports.Opening
{
    /// <summary>
    /// Interaction logic for SelectedOpeningReportWindow.xaml
    /// </summary>
    public partial class SelectedOpeningReportWindow : Window
    {
        private ReportViewBuilder _reportView;
        private string _titleFilter = string.Empty;
        private readonly List<DataSetValuePair> _sources = new List<DataSetValuePair>();
        private CompanyModel _selectedCompany;

        public SelectedOpeningReportWindow()
        {
            InitializeComponent();

            _reportView = new ReportViewBuilder("TemporaryEmploymentCorp.Reports.Opening.SelectedOpeningReport.rdlc", UpdateDatasetSource());
            _reportView.RefreshDataSourceCallback = UpdateDatasetSource;
            ReportContainer.Content = _reportView.ReportContent;

            DataContext = this;
        }
        public ObservableCollection<DataAccess.EF.Candidate> Cands { get; } = new ObservableCollection<DataAccess.EF.Candidate>();
        public ObservableCollection<DataAccess.EF.Candidate> potentialCands { get; } = new ObservableCollection<DataAccess.EF.Candidate>();
        private IReadOnlyCollection<DataSetValuePair> UpdateDatasetSource()
        {
            _sources.Clear();
            var selectedOpening = ViewModelLocatorStatic.Locator.OpeningModule.SelectedOpening;
            _sources.Add(new DataSetValuePair("OpeningDataset", selectedOpening.Model));
            var associatedQualification = selectedOpening.Model.Qualification;
            _sources.Add(new DataSetValuePair("QualificationDataset", associatedQualification));
            var placements = selectedOpening.Placements.Select(c => c.Model);
            foreach (var item in placements)
            {
                var candidate = item.Candidate;
                Cands.Add(candidate);
            }
            _sources.Add(new DataSetValuePair("CandidateDataset", Cands));
            _sources.Add(new DataSetValuePair("PlacementDataset", placements));
            
          
            return _sources;

        }
    }
}
