using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Nito.AsyncEx;
using TemporaryEmploymentCorp.DataAccess;
using TemporaryEmploymentCorp.DataAccess.EF;
using TemporaryEmploymentCorp.Helpers.ReportViewer;
using TemporaryEmploymentCorp.Models.Candidate;
using TemporaryEmploymentCorp.Models.CanQual;
using TemporaryEmploymentCorp.Models.History;
using TemporaryEmploymentCorp.Reports.Candidate;
using TemporaryEmploymentCorp.Views.Candidate;
using TemporaryEmploymentCorp.Views.Candidate.CanQual;
using TemporaryEmploymentCorp.Views.Candidate.History;

namespace TemporaryEmploymentCorp.Modules
{
    public class CandidateModule : ObservableObject
    {
        public AddCandidateWindow _addCandidateWindow;

        public AddCanQualWindow _AddCanQualWindow;
        public AddHistoryWindow _AddHistoryWindow;
        private NewCandidateModel _newCandidate;

        private NewCanQualModel _newCanQual;
        private NewHistoryModel _newHistory;
        private readonly IRepository _repository;
        private CandidateModel _selectedCandidate;
        private CanQualModel _selectedCanQual;
        private HistoryModel _selectedHistory;
        private string _searchCandidate;

        public CandidateModule(IRepository repository)
        {
            _repository = repository;
            CandidateLoading = NotifyTaskCompletion.Create(LoadCandidatesAsync);

        }

        public INotifyTaskCompletion CandidateLoading { get; set; }


        public ObservableCollection<QualificationSelection> QualificationsList { get; } =
            new ObservableCollection<QualificationSelection>();

        

        public CandidateModel SelectedCandidate
        {
            get { return _selectedCandidate; }
            set
            {
                _selectedCandidate?.CancelEditCommand.Execute(null);

                _selectedCandidate = value;
                if (value != null)
                {
                    _selectedCandidate.LoadRe5latedInfo();
                    GetNumberOfQualifications();
                    GetNumberOfEnrolledClasses();
                }
                RaisePropertyChanged(nameof(SelectedCandidate));
            }
        }

        public NewCandidateModel NewCandidate
        {
            get { return _newCandidate; }
            set
            {
                _newCandidate = value;
                RaisePropertyChanged(nameof(NewCandidate));
            }
        }

        public NewCanQualModel NewCanQual
        {
            get { return _newCanQual; }
            set
            {
                _newCanQual = value;
                LoadQualificationsList();
                RaisePropertyChanged(nameof(NewCanQual));
            }
        }

        public CanQualModel SelectedCanQual
        {
            get { return _selectedCanQual; }
            set
            {
                _selectedCanQual = value;
                RaisePropertyChanged(nameof(SelectedCanQual));
            }
        }

        public NewHistoryModel NewHistory
        {
            get { return _newHistory; }
            set
            {
                _newHistory = value;
                RaisePropertyChanged(nameof(NewHistory));
            }
        }

        public HistoryModel SelectedHistory
        {
            get { return _selectedHistory; }
            set
            {
                _selectedHistory = value;
                RaisePropertyChanged(nameof(SelectedHistory));
            }
        }

        public ICommand AddCandidateCommand => new RelayCommand(AddCandidateProc);
        public ICommand SaveCandidateCommand => new RelayCommand(SaveCandidateProc, SaveCandidateCondition);
        public ICommand CancelCandidateCommand => new RelayCommand(CancelProc);

        public ICommand AddCanQualCommand => new RelayCommand(AddCanQualProc);
        public ICommand SaveCanQualCommand => new RelayCommand(SaveCanQualProc, SaveCanQualCondition);
        public ICommand CancelCanQualCommand => new RelayCommand(CancelCanQualProc);

//        public ICommand AddHistoryCommand => new RelayCommand(AddHistoryProc);
//        public ICommand SaveHistoryCommand => new RelayCommand(SaveHistoryProc, SaveHistoryCondition);
//        public ICommand CancelHistoryCommand => new RelayCommand(CancelHistoryProc);
//        public ICommand RemoveHistoryCommand => new RelayCommand(RemoveHistoryProc);

        public ICommand RemoveCandidateCommand => new RelayCommand(RemoveCandidateProc, RemoveCandidateCondition);
        public ICommand RemoveCanQualCommand => new RelayCommand(RemoveCanQualProc, RemoveCanQualCondition);

        private void RemoveCanQualProc()
        {
            var result = MessageBox.Show("Are you sure you want to delete selected candidate qualification?", "Candidate qualification",
                MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    NotifyTaskCompletion.Create(RemoveCanQualAsync);
                }
                catch (Exception e)
                {

                    MessageBox.Show("Unable to delete candidate qualification");
                }
            }
        }

        private async Task RemoveCanQualAsync()
        {
            await _repository.CanQualify.RemoveAsync(SelectedCanQual.Model);
            SelectedCandidate.CandidateQualifications.Remove(SelectedCanQual);
        }

        private bool RemoveCanQualCondition()
        {
            if (SelectedCanQual == null) return false;
            return true;
        }

        private void RemoveCandidateProc()
        {
            var result = MessageBox.Show("Are you sure you want to delete selected candidate?", "Candidate",
                MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var history =
                        _repository.History.GetRange(c => c.CandidateId == SelectedCandidate.Model.CandidateId);
                    var canQual =
                        _repository.CanQualify.GetRange(c => c.CandidateId == SelectedCandidate.Model.CandidateId);
                    if (history.Count > 0)
                    {
                        MessageBox.Show("Unable to delete candidates with history");

                    }

                    else if (canQual.Count > 0)
                    {
                        MessageBox.Show("Unable to delete candidates with qualifications.");
                    }

                    else
                    {
                        NotifyTaskCompletion.Create(RemoveCandidateAsync);
                    }
                }
                catch (Exception e)
                {

                    MessageBox.Show("Unable to delete opening");
                }
            }
        }

        private async Task RemoveCandidateAsync()
        {
            await _repository.Candidate.RemoveAsync(SelectedCandidate.Model);
            Candidates.Remove(SelectedCandidate);

        }

        private bool RemoveCandidateCondition()
        {
            if (SelectedCandidate == null) return false;
            return true;
        }

        private void CancelCanQualProc()
        {
            NewCanQual?.Dispose();
            _AddCanQualWindow.Close();
        }

//        private void RemoveHistoryProc()
//        {
//            var value = MessageBox.Show("Are you sure you want to delete?", "", MessageBoxButton.YesNo,
//                MessageBoxImage.Question);
//
//            if (value == MessageBoxResult.Yes)
//            {
//                NotifyTaskCompletion.Create(RemoveHistoryAsync);
//            }
//        }

//        private async Task RemoveHistoryAsync()
//        {
//            await _repository.History.RemoveAsync(SelectedHistory.Model);
//            _selectedCandidate.History.Remove(SelectedHistory);
//        }

//        private void CancelHistoryProc()
//        {
//            NewHistory?.Dispose();
//            _AddHistoryWindow.Close();
//        }

        private bool SaveCanQualCondition()
        {
            return true;
        }

        public int NumberOfQualifications
        {
            get { return _numberOfQualifications; }
            set
            {
                _numberOfQualifications = value;
                RaisePropertyChanged(nameof(NumberOfQualifications));
            }
        }

        private void GetNumberOfQualifications()
        {
            var candQuals = _repository.CanQualify.GetRange(c => c.CandidateId == SelectedCandidate.Model.CandidateId);
            NumberOfQualifications = candQuals.Count;
        }

        public int NumberOfEnrolledClasses
        {
            get { return _numberOfEnrolledClasses; }
            set
            {
                _numberOfEnrolledClasses = value;
                RaisePropertyChanged(nameof(NumberOfEnrolledClasses));
            }
        }

        private void GetNumberOfEnrolledClasses()
        {
            var numberOfEnrolledClasses =
                _repository.Enrollment.GetRange(c => c.CandidateId == SelectedCandidate.Model.CandidateId);
            NumberOfEnrolledClasses = numberOfEnrolledClasses.Count;
        }

        public void LoadQualificationsList() //this code is to filter the list of qualifications that a candidate can earn
        {
            QualificationsList.Clear();

            var qualifications = _repository.Qualification.GetRange().Select(c => new QualificationSelection(c));
            foreach (var qualificationSelection in qualifications)
            {
                QualificationsList.Add(qualificationSelection);
            }

            foreach (var item in SelectedCandidate.CandidateQualifications)
            {
                var toRemove =
                    QualificationsList.FirstOrDefault(c => c.Model.QualificationId == item.Model.QualificationId);
                QualificationsList.Remove(toRemove);
            }
        }


        private void SaveCanQualProc()
        {
            if (NewCanQual == null) return;

            var selectedQuals = QualificationsList.Where(c => c.IsSelected).ToList();
            foreach (var selectedQualification in selectedQuals)
            {
                try
                {
                    NewCanQual.ModelCopy.CandidateId = SelectedCandidate.Model.CandidateId;
                    NewCanQual.ModelCopy.QualificationId = selectedQualification.Model.QualificationId;
                    NewCanQual.ModelCopy.IsAssigned = true;

                    _repository.CanQualify.Add(NewCanQual.ModelCopy);
                    _selectedCandidate.CandidateQualifications.Add(new CanQualModel(NewCanQual.ModelCopy, _repository));
                    

                    
                }
                catch (Exception e)
                {
                    MessageBox.Show("An error occured during save. Reason: " + e.Message, "Candidate");
                }
            }

            SelectedCandidate = _selectedCandidate;
            ViewModelLocatorStatic.Locator.OpeningModule.SelectedOpening = ViewModelLocatorStatic.Locator.OpeningModule.SelectedOpening;
            _AddCanQualWindow.Close();


        }

        private async Task SaveCanQualAsync()
        {
            
        }


//        private bool SaveHistoryCondition()
//        {
//            return true;
//        }

//        private void SaveHistoryProc()
//        {
//            if (NewHistory == null) return;
//            if (!NewHistory.HasChanges) return;
//            NotifyTaskCompletion.Create(SaveHistoryAsync);
//        }
//
//        private async Task SaveHistoryAsync()
//        {
//            try
//            {
//                NewHistory.ModelCopy.CandidateId = SelectedCandidate.Model.CandidateId;
//                await _repository.History.AddAsync(NewHistory.ModelCopy);
//                _selectedCandidate.History.Add(new HistoryModel(NewHistory.ModelCopy, _repository));
//                _AddHistoryWindow.Close();
//            }
//            catch (Exception e)
//            {
//                MessageBox.Show("An error occured during save. Reason: " + e.Message, "Candidate");
//            }
//        }
        public ObservableCollection<CandidateModel> Candidates { get; } = new ObservableCollection<CandidateModel>();
        
        private async Task LoadCandidatesAsync()
        {
            var candidates = await Task.Run(() => _repository.Candidate.GetRangeAsync());
            Candidates.Clear();
            foreach (var candidate in candidates)
            {
                Candidates.Add(new CandidateModel(candidate, _repository));
                await Task.Delay(500);
            }
        }

//        private void AddHistoryProc()
//        {
//            NewHistory = new NewHistoryModel();
//            _AddHistoryWindow = new AddHistoryWindow();
//            _AddHistoryWindow.Owner = Application.Current.MainWindow;
//            _AddHistoryWindow.ShowDialog();
//        }

        private void AddCanQualProc()
        {
            NewCanQual = new NewCanQualModel();
            _AddCanQualWindow = new AddCanQualWindow();
            _AddCanQualWindow.Owner = Application.Current.MainWindow;
            _AddCanQualWindow.ShowDialog();
        }


        private void AddCandidateProc()
        {
            NewCandidate = new NewCandidateModel();
            _addCandidateWindow = new AddCandidateWindow();
            _addCandidateWindow.Owner = Application.Current.MainWindow;
            _addCandidateWindow.ShowDialog();
        }


        private void CancelProc()
        {
            NewCandidate?.Dispose();
            _addCandidateWindow.Close();
        }


        private bool SaveCandidateCondition()
        {
            return true;
        }

        private void SaveCandidateProc()
        {
            if (NewCandidate == null) return;
            if (!NewCandidate.HasChanges) return;
            NotifyTaskCompletion.Create(SaveCandidateAsync);
        }

        private async Task SaveCandidateAsync()
        {
            try
            {
                await _repository.Candidate.AddAsync(NewCandidate.ModelCopy);
                Candidates.Add(new CandidateModel(NewCandidate.ModelCopy, _repository));
                _addCandidateWindow.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("An error occured during save", "Candidate");
            }
        }

        public string SearchCandidate
        {
            get { return _searchCandidate; }
            set
            {
                _searchCandidate = value;
                RaisePropertyChanged(nameof(SearchCandidate));
                var CandidateList = CollectionViewSource.GetDefaultView(Candidates);
                if (string.IsNullOrWhiteSpace(SearchCandidate))
                {
                    CandidateList.Filter = null;
                }
                else
                {
                    CandidateList.Filter = obj =>
                    {
                        var candModel = obj as CandidateModel;
                        var sc = SearchCandidate.ToLowerInvariant();
                        if (candModel == null) return false;
                        return candModel.Model.CandidateFirstName.ToLowerInvariant().Contains(sc) ||
                               candModel.Model.CandidateMiddleName.ToLowerInvariant().Contains(sc) ||
                               candModel.Model.CandidateLastName.ToLowerInvariant().Contains(sc);


                    };

                }
            }
        }

        public ICommand PrintAllCandidates => new RelayCommand(PrintAllCandidatesProc);
        

        private SingleInstanceWindowViewer<AllCandidateWindow> _allCandWindow = new SingleInstanceWindowViewer<AllCandidateWindow>();
        private int _numberOfQualifications;
        private int _numberOfEnrolledClasses;

        private void PrintAllCandidatesProc()
        {
            _allCandWindow.Show();
        }
    }
}