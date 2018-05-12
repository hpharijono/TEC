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
using TemporaryEmploymentCorp.Models.Opening;
using TemporaryEmploymentCorp.Modules;
using TemporaryEmploymentCorp.Reports.Company;

namespace TemporaryEmploymentCorp.Reports.Opening
{
    /// <summary>
    /// Interaction logic for AllOpeningsReportWindow.xaml
    /// </summary>
    public partial class AllOpeningsReportWindow : Window
    {
        private ReportViewBuilder _reportView;
        private string _titleFilter = string.Empty;
        private readonly List<DataSetValuePair> _sources = new List<DataSetValuePair>();
    
        public AllOpeningsReportWindow()
        {
            InitializeComponent();

            _reportView = new ReportViewBuilder("TemporaryEmploymentCorp.Reports.Opening.AllOpeningsReport.rdlc", UpdateDatasetSource());
            _reportView.RefreshDataSourceCallback = UpdateDatasetSource;
            ReportContainer.Content = _reportView.ReportContent;

            DataContext = this;
        }
        public ObservableCollection<DataAccess.EF.Company> Comps { get; } = new ObservableCollection<DataAccess.EF.Company>(); 
        private IReadOnlyCollection<DataSetValuePair> UpdateDatasetSource()
        {
            _sources.Clear();
            var openings = ViewModelLocatorStatic.Locator.OpeningModule.PrintOpenings.Select(c => c.Model);
           
            _sources.Add(new DataSetValuePair("CompanyOpeningDataset", openings.Select(c => new CompanyOpening(c.Company, c, c.Qualification))));
//            foreach (var opening in openings)
//            {
//                var company = opening.Company;
//                Comps.Add(company);
//                _sources.Add(new DataSetValuePair("CompanyDataset", Comps));
//            }

            
            return _sources;
            

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
