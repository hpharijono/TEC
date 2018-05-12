using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Nito.AsyncEx;
using TemporaryEmploymentCorp.DataAccess;
using TemporaryEmploymentCorp.Helpers.ReportViewer;
using TemporaryEmploymentCorp.Models.Company;
using TemporaryEmploymentCorp.Reports.Company;
using TemporaryEmploymentCorp.Views.Company;

namespace TemporaryEmploymentCorp.Modules
{
    public class CompanyModule : ObservableObject
    {
        public ObservableCollection<CompanyModel> Companies { get; } = new ObservableCollection<CompanyModel>();
        private IRepository _repository;
        private CompanyModel _selectedCompany;
        private NewCompanyModel _newCompany;
        public INotifyTaskCompletion CompanyLoading { get; private set; }

        public CompanyModule(IRepository repository)
        {
            _repository = repository;
            //            LoadPublishers();

            CompanyLoading = NotifyTaskCompletion.Create(() => LoadCompaniesAsync());

        }

        public CompanyModule()
        {
            
        }

        private async Task LoadCompaniesAsync()
        {
            var companies = await _repository.Company.GetRangeAsync();
            Companies.Clear();
            foreach (var company in companies)
            {
                Companies.Add(new CompanyModel(company, _repository));
                await Task.Delay(10);
            }
        }

        public CompanyModel SelectedCompany
        {
            get { return _selectedCompany; }
            set
            {
                //will return to the before-editing stage
                _selectedCompany?.CancelEditCommand.Execute(null);
                _selectedCompany = value;

                if (_selectedCompany != null)
                {
                    _selectedCompany.LoadRelatedInfo();
                }

                RaisePropertyChanged(nameof(SelectedCompany));
            }
        }

        public NewCompanyModel NewCompany
        {
            get { return _newCompany; }
            set
            {
                _newCompany = value;
                RaisePropertyChanged(nameof(NewCompany));
            }
        }

        public ICommand AddCompanyCommand => new RelayCommand(AddCompanyProc);
        public ICommand CancelCompanyCommand => new RelayCommand(CancelCompanyProc);
        public ICommand PrintCompanyCommand => new RelayCommand(PrintCompanyProc);
        private SingleInstanceWindowViewer<AllCompanyReportWindow> _AllCompanyReportWindow = new SingleInstanceWindowViewer<AllCompanyReportWindow>();

        private void PrintCompanyProc()
        {
            _AllCompanyReportWindow.Show();

        }

        public ICommand SaveCompanyCommand => new RelayCommand(SaveCompanyProc, SaveCompanyCondition);
        public ICommand DeleteCompanyCommand => new RelayCommand(DeleteCompanyProc, DeleteCompanyCondition);

        private bool DeleteCompanyCondition()
        {
            if (SelectedCompany == null)
            {
                return false;
            }
            return true;
        }

        private async Task DeleteCompanyProcAsync()
        {
            var value = MessageBox.Show("Are you sure you want to delete?", "", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (value == MessageBoxResult.Yes)
            {
                try
                {
                    NotifyTaskCompletion.Create(RemoveCompanyAsync);
                }
                catch (Exception e)
                {

                    MessageBox.Show("Companies with openings cannot be deleted.");
                }
                       
                        
                
                
            }

            else
            {
                return;
            }
        }

        public async void DeleteCompanyProc()
        {
            await DeleteCompanyProcAsync();
        }




        private async Task RemoveCompanyAsync()
        {
            try
            {
                await Task.Run(() => _repository.Company.RemoveAsync(SelectedCompany.Model));
                Companies.Remove(SelectedCompany);
            }
            catch (Exception e)
            {
                MessageBox.Show("Unable to delete company with openings.", "Company");
                throw;
            }
            
        }

        private void CancelCompanyProc()
        {
            NewCompany?.Dispose();
            _AddCompanyWindow.Close();
        }

        

        private bool SaveCompanyCondition()
        {
            return (NewCompany != null) && NewCompany.HasChanges && !NewCompany.HasErrors;
        }

        private void SaveCompanyProc()
        {
            if (NewCompany == null) return;
            if (!NewCompany.HasChanges) return;
            NotifyTaskCompletion.Create(() => SaveAddCompanyAsync());
            _AddCompanyWindow.Close();

        }

        private async Task SaveAddCompanyAsync()
        {
            try
            {
                
                await Task.Run(() => _repository.Company.AddAsync(NewCompany.ModelCopy)) ;
                Companies.Add(new CompanyModel(NewCompany.ModelCopy, _repository));
                
            }
            catch (Exception e)
            {
                MessageBox.Show("An error occured during save. Reason: " + e.Message, "Company");

            }
            
        }

        public AddCompanyWindow _AddCompanyWindow;
        
        

        private void AddCompanyProc()
        {
            NewCompany = new NewCompanyModel();
            _AddCompanyWindow = new AddCompanyWindow();
            _AddCompanyWindow.Owner = Application.Current.MainWindow;
            _AddCompanyWindow.ShowDialog();

        }

        private string _searchCompany;
        public string SearchCompany
        {
            get { return _searchCompany; }
            set
            {
                _searchCompany = value;
                RaisePropertyChanged(nameof(SearchCompany));

                var viewCompanyList = CollectionViewSource.GetDefaultView(Companies);
                if (string.IsNullOrWhiteSpace(SearchCompany))
                {
                    viewCompanyList.Filter = null;
                }

                else
                {
                    viewCompanyList.Filter = obj =>
                    {
                        var cm = obj as CompanyModel;
                        var sc = SearchCompany.ToLowerInvariant();
                        if (cm == null) return false;
                        return cm.Model.CompanyAddress.ToLowerInvariant().Contains(sc) ||
                               cm.Model.CompanyName.ToLowerInvariant().Contains(sc);
                        ;
                    };

                }
            }
        }
    }
}
