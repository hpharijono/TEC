using System.Collections.Generic;
using System.Linq;
using System.Windows;
using TemporaryEmploymentCorp.Helpers.ReportViewer;
using TemporaryEmploymentCorp.Modules;

namespace TemporaryEmploymentCorp.Reports.Company
{
    /// <summary>
    /// Interaction logic for AllCompanyReportWindow.xaml
    /// </summary>
    public partial class AllCompanyReportWindow : Window
    {
        private ReportViewBuilder _reportView;
        private string _titleFilter = string.Empty;

        public AllCompanyReportWindow()
        {
            InitializeComponent();
            _reportView = new ReportViewBuilder("TemporaryEmploymentCorp.Reports.Company.AllCompanyReport.rdlc", UpdateDatasetSource());
            _reportView.RefreshDataSourceCallback = UpdateDatasetSource;
            ReportContainer.Content = _reportView.ReportContent;

            DataContext = this;
        }


        private IReadOnlyCollection<DataSetValuePair> UpdateDatasetSource()
        {

            var sources = new List<DataSetValuePair>();
            var companies = ViewModelLocatorStatic.Locator.CompanyModule.Companies.Select(c => c.Model);

            sources.Add(new DataSetValuePair("CompanyDataset", companies.Where(c => TitleFilter != null && c.CompanyName.ToLowerInvariant().Contains(TitleFilter.ToLowerInvariant()))));

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
