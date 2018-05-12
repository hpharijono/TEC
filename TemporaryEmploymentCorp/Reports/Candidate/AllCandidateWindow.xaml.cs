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
using Microsoft.Reporting.WinForms.Internal.Soap.ReportingServices2005.Execution;
using TemporaryEmploymentCorp.Helpers.ReportViewer;
using TemporaryEmploymentCorp.Modules;
using ReportParameter = Microsoft.Reporting.WinForms.ReportParameter;

namespace TemporaryEmploymentCorp.Reports.Candidate
{
    /// <summary>
    /// Interaction logic for AllCandidateWindow.xaml
    /// </summary>
    public partial class AllCandidateWindow : Window
    {

        private readonly ReportViewBuilder _reportView;
        private readonly List<DataSetValuePair> _sources = new List<DataSetValuePair>();
        private bool _isBirthdateChecked = true;
        private bool _isAddressChecked = true;
        private bool _isPhoneNumberChecked = true;
        ReportParameter[] parameters = new ReportParameter[4];
        private bool _isMiddleNameChecked = true;
        private string _nameFilter;
        private string _phoneNumberFilter;
        private string _address;
        private string _addressFilter;

        public AllCandidateWindow()
        {
            InitializeComponent();

            _reportView = new ReportViewBuilder("TemporaryEmploymentCorp.Reports.Candidate.AllCandidateReport.rdlc", UpdateDatasetSource());
            _reportView.RefreshDataSourceCallback = UpdateDatasetSource;
            ReportContainer.Content = _reportView.ReportContent;

            DataContext = this;

            SetParameters();
        }

        private void SetParameters()
        {
            parameters[0] = new ReportParameter("BirthdateVisibility", _isBirthdateChecked.ToString());
            parameters[1] = new ReportParameter("AddressVisibility", _isAddressChecked.ToString());
            parameters[2] = new ReportParameter("PhoneNumberVisibility", _isPhoneNumberChecked.ToString());
            parameters[3] = new ReportParameter("MiddleNameVisibility", _isMiddleNameChecked.ToString());

            _reportView.ReportContent.Viewer.LocalReport.SetParameters(parameters);
        }

        private IReadOnlyCollection<DataSetValuePair> UpdateDatasetSource()
        {
            _sources.Clear();
            var candidates = ViewModelLocatorStatic.Locator.CandidateModule.Candidates.Select(c => c.Model);

            if (NameFilter != null)
            {
                var n = NameFilter.ToLowerInvariant();
                var filteredpersons = candidates.Where(c => c.CandidateFirstName.ToLowerInvariant().Contains(n) ||
                                                            c.CandidateMiddleName.ToLowerInvariant().Contains(n) ||
                                                            c.CandidateLastName.ToLowerInvariant().Contains(n));
                candidates = filteredpersons;
            }

            if (AddressFilter != null)
            {
                var n = AddressFilter.ToLowerInvariant();
                var filteredpersons = candidates.Where(c => c.CandidateAddress.ToLowerInvariant().Contains(n)
                                                         );
                candidates = filteredpersons;
            }

            if (PhoneNumberFilter != null)
            {
                var n = PhoneNumberFilter.ToLowerInvariant();
                var filteredpersons = candidates.Where(c => c.CandidatePhoneNo.Contains(n));
                candidates = filteredpersons;
            }

         

            _sources.Add(new DataSetValuePair("CandidateDataset", candidates));
            return _sources;
        }

        public string NameFilter
        {
            get { return _nameFilter; }
            set
            {
                _nameFilter = value;
                _reportView.ReportContent.UpdateDataSource(UpdateDatasetSource());
            }
        }

        public string AddressFilter
        {
            get { return _addressFilter; }
            set
            {
                _addressFilter = value;
                _reportView.ReportContent.UpdateDataSource(UpdateDatasetSource());
            }
        }

        public string PhoneNumberFilter
        {
            get { return _phoneNumberFilter; }
            set
            {
                _phoneNumberFilter = value;
                _reportView.ReportContent.UpdateDataSource(UpdateDatasetSource());

            }
        }

        public bool IsBirthdateChecked
        {
            get { return _isBirthdateChecked; }
            set
            {
                _isBirthdateChecked = value;

                parameters[0] = new ReportParameter("BirthdateVisibility", _isBirthdateChecked.ToString());

                _reportView.ReportContent.Viewer.LocalReport.SetParameters(parameters);
                _reportView.ReportContent.Viewer.RefreshReport();
            }
        }

        public bool IsAddressChecked
        {
            get { return _isAddressChecked; }
            set
            {
                _isAddressChecked = value;

                parameters[1] = new ReportParameter("AddressVisibility", _isAddressChecked.ToString());

                _reportView.ReportContent.Viewer.LocalReport.SetParameters(parameters);
                _reportView.ReportContent.Viewer.RefreshReport();
            }
        }

        public bool IsPhoneNumberChecked
        {
            get { return _isPhoneNumberChecked; }
            set
            {
                _isPhoneNumberChecked = value;

                parameters[2] = new ReportParameter("PhoneNumberVisibility", _isPhoneNumberChecked.ToString());

                _reportView.ReportContent.Viewer.LocalReport.SetParameters(parameters);
                _reportView.ReportContent.Viewer.RefreshReport();
            }
        }

        public bool IsMiddleNameChecked
        {
            get { return _isMiddleNameChecked; }
            set
            {
                _isMiddleNameChecked = value;

                parameters[3] = new ReportParameter("MiddleNameVisibility", _isMiddleNameChecked.ToString());

                _reportView.ReportContent.Viewer.LocalReport.SetParameters(parameters);
                _reportView.ReportContent.Viewer.RefreshReport();
            }
        }
    }
}
