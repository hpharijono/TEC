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

namespace TemporaryEmploymentCorp.Reports.Qualifications
{
    /// <summary>
    /// Interaction logic for QualificationsReportWindow.xaml
    /// </summary>
    public partial class QualificationsReportWindow : Window
    {
        private ReportViewBuilder _reportView;
        private string _titleFilter = string.Empty;

        public QualificationsReportWindow()
        {
            InitializeComponent();
            _reportView = new ReportViewBuilder("TemporaryEmploymentCorp.Reports.Qualifications.QualificationsReport.rdlc", UpdateDatasetSource());
            _reportView.RefreshDataSourceCallback = UpdateDatasetSource;
            ReportContainer.Content = _reportView.ReportContent;

            DataContext = this;
        }

        private IReadOnlyCollection<DataSetValuePair> UpdateDatasetSource()
        {
            var sources = new List<DataSetValuePair>();
            //var selectedPublisher = (Application.Current.Resources["Locator"] as ViewModelLocator).PublisherModule;

            var qualifications = ViewModelLocatorStatic.Locator.QualificationModule.Qualifications.Select(c => c.Model);
           
            //var books = selectedPublisher.Books.Select(c => c.Model);
            //clear sources for filtering. use where.
            sources.Add(new DataSetValuePair("QualificationDataset", qualifications.Where(c => TitleFilter != null && c.QualificationCode.ToLowerInvariant().Contains(TitleFilter.ToLowerInvariant()))));
            //sources.Add(new DataSetValuePair("BooksDataset", books.Where(c => TitleFilter != null && c.Title.ToLowerInvariant().Contains(TitleFilter.ToLowerInvariant()))));
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
