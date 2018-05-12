using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Nito.AsyncEx;
using TemporaryEmploymentCorp.DataAccess;
using TemporaryEmploymentCorp.Helpers.ReportViewer;
using TemporaryEmploymentCorp.Models.Candidate;
using TemporaryEmploymentCorp.Models.Course;
using TemporaryEmploymentCorp.Models.Qualification;
using TemporaryEmploymentCorp.Reports.Qualifications;
using TemporaryEmploymentCorp.Views.Qualification.Display;

namespace TemporaryEmploymentCorp.Modules
{
    public class QualificationModule : ObservableObject
    {
        public ObservableCollection<QualificationModel> Qualifications { get; } =
            new ObservableCollection<QualificationModel>();

        private IRepository _repository;
        private QualificationModel _selectedQualification;
        private NewQualificationModel _newQualification;
        public INotifyTaskCompletion QualificationLoading { get; private set; }

        public QualificationModule(IRepository repository)
        {
            _repository = repository;
            QualificationLoading = NotifyTaskCompletion.Create(() => LoadQualificationsAsync());
        }

        private async Task LoadQualificationsAsync()
        {
            Qualifications.Clear();
            var qualifications = await _repository.Qualification.GetRangeAsync();
            foreach (var qualification in qualifications)
            {
                Qualifications.Add(new QualificationModel(qualification, _repository));
                await Task.Delay(500);
            }

        }

        public QualificationModel SelectedQualification
        {
            get { return _selectedQualification; }
            set
            {
                _selectedQualification?.CancelEditCommand.Execute(null);

                _selectedQualification = value;
                if (value != null)
                {
                    _selectedQualification.LoadRelatedInfo();
                }

                RaisePropertyChanged(nameof(SelectedQualification));
            }
        }

        public NewQualificationModel NewQualification
        {
            get { return _newQualification; }
            set
            {
                _newQualification = value;
                RaisePropertyChanged(nameof(NewQualification));
            }
        }

        public ICommand AddQualificationCommand => new RelayCommand(AddQualificationProc);
        public AddQualificationWindow _AddQualificationWindow;

        private void AddQualificationProc()
        {
            NewQualification = new NewQualificationModel();
            _AddQualificationWindow = new AddQualificationWindow();
            _AddQualificationWindow.Owner = Application.Current.MainWindow;
            _AddQualificationWindow.ShowDialog();
        }

        public ICommand SaveQualificationCommand => new RelayCommand(SaveQualificationProc, SaveQualificationCondition);

        private bool SaveQualificationCondition()
        {
            return (NewQualification != null) && NewQualification.HasChanges && !NewQualification.HasErrors;

        }

        public ICommand PrintQualificationsCommand => new RelayCommand(PrintQualificationsProc);

        private SingleInstanceWindowViewer<QualificationsReportWindow> _qualificationsWindow =
            new SingleInstanceWindowViewer<QualificationsReportWindow>();

        private void PrintQualificationsProc()
        {
            _qualificationsWindow.Show();

        }

        public ICommand DeleteQualificationCommand => new RelayCommand(DeleteQualProc, DeleteQualCondition);

        private async Task DeleteQualAsync()
        {
            var result = MessageBox.Show("Are you sure you want to remove selected qualification?", "Qualification",
                MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var openings = await Task.Run(()=>_repository.Opening.GetRangeAsync(c => c.QualificationId == SelectedQualification.Model.QualificationId, CancellationToken.None));
                    if (openings.Count > 0)
                    {
                        MessageBox.Show("Unable to delete openings with qualifications.");
                    }

                    else
                    {
                        await RemoveQualificationAsync();
                    }

                    
                }
                catch (Exception)
                {
                    MessageBox.Show("Unable to delete.");
                }
            }

            else
            {
                return;
            }
        }

        private async void DeleteQualProc()
        {
            await DeleteQualAsync();
        }

        private async Task RemoveQualificationAsync()
        {
            await Task.Run(() => _repository.Qualification.RemoveAsync(SelectedQualification.Model));
            Qualifications.Remove(SelectedQualification);
        }

        private bool DeleteQualCondition()
        {
            if (SelectedQualification == null)
            {
                return false;
            }
            return true;
        }

        private void SaveQualificationProc()
        {
            if (NewQualification == null) return;
            if (!NewQualification.HasChanges) return;
            NotifyTaskCompletion.Create(() => SaveAddQualificationAsync());
            _AddQualificationWindow.Close();
        }

        private async Task SaveAddQualificationAsync()
        {
            try
            {
                await Task.Run(() => _repository.Qualification.AddAsync(NewQualification.ModelCopy)) ;
                Qualifications.Add(new QualificationModel(NewQualification.ModelCopy, _repository));                
            }
            catch (Exception e)
            {
                MessageBox.Show("Unable to add. Reason: " + e.Message);
            }
        }





        public ICommand CancelCommand => new RelayCommand(CancelProc);

        private void CancelProc()
        {
            NewQualification?.Dispose();
            _AddQualificationWindow.Close();
        }

        private string _searchQualification;

        public string SearchQualification
        {
            get { return _searchQualification; }
            set
            {
                _searchQualification = value;
                RaisePropertyChanged(nameof(SearchQualification));
                var CandidateList = CollectionViewSource.GetDefaultView(Qualifications);
                if (string.IsNullOrWhiteSpace(SearchQualification))
                {
                    CandidateList.Filter = null;
                }
                else
                {
                    CandidateList.Filter = obj =>
                    {
                        var qualModel = obj as QualificationModel;
                        var sc = SearchQualification.ToLowerInvariant();
                        if (qualModel == null) return false;
                        return qualModel.Model.QualificationCode.ToLowerInvariant().Contains(sc);



                    };

                }
            }
        }
    }
}
