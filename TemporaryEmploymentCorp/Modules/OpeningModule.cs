using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Nito.AsyncEx;
using TemporaryEmploymentCorp.DataAccess;
using TemporaryEmploymentCorp.DataAccess.EF;
using TemporaryEmploymentCorp.Helpers.ReportViewer;
using TemporaryEmploymentCorp.Models.CanQual;
using TemporaryEmploymentCorp.Models.Company;
using TemporaryEmploymentCorp.Models.History;
using TemporaryEmploymentCorp.Models.Opening;
using TemporaryEmploymentCorp.Models.Placement;
using TemporaryEmploymentCorp.Models.Qualification;
using TemporaryEmploymentCorp.Models.Session;
using TemporaryEmploymentCorp.Reports.Opening;
using TemporaryEmploymentCorp.Views.Opening;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace TemporaryEmploymentCorp.Modules
{
    public class OpeningModule : ObservableObject
    {
        public AddOpeningWindow _AddOpeningWindow;
        private Qualification _loadedQualification;
        private NewOpeningModel _newOpening;
        private readonly IRepository _repository;
        private CompanyModel _selectedCompany;
        private OpeningModel _selectedOpening;
        private PlacementModel _selectedPlacement;
        private NewPlacementModel _newPlacement;
        private NewHistoryModel _newHistory;
        private CanQualModel _selectedQualifiedCandidate;
        public ObservableCollection<OpeningModel> PrintOpenings { get; } = new ObservableCollection<OpeningModel>(); 
        

        public OpeningModule(IRepository repository)
        {
            _repository = repository;
            LoadAllOpenings();
//            PlacementLoading = NotifyTaskCompletion.Create(LoadPlacementsAsync);
        }

        public void LoadAllOpenings()
        {
            var companies = _repository.Company.GetRange();
            foreach (var company in companies)
            {
                var openings = _repository.Opening.GetRange(c => c.CompanyId == company.CompanyId);
                foreach (var opening in openings)
                {
                    var getEachOpening = _repository.Opening.Get(c => c.OpeningNumber == opening.OpeningNumber);
                    var comp = _repository.Company.Get(c => c.CompanyId == getEachOpening.CompanyId);
                    getEachOpening.Company = comp;
                    var qual = _repository.Qualification.Get(c => c.QualificationId == getEachOpening.QualificationId);
                    getEachOpening.Qualification = qual;
                    PrintOpenings.Add(new OpeningModel(getEachOpening));
                }
                

            }
        }
        

        public Qualification loadedQualification
        {
            get { return _loadedQualification; }
            set
            {
                _loadedQualification = value;
                RaisePropertyChanged(nameof(loadedQualification));
            }
        }

        public ICommand AssignCandidateCommand => new RelayCommand(AssignCandidateProc, AssignCandidateCondition);

        private bool AssignCandidateCondition()
        {
            return true;
        }

//        public PlacementModel SelectedPlacement
//        {
//            get { return _selectedPlacement; }
//            set
//            {
//                _selectedPlacement = value;
//                if (value != null)
//                {
//                    LoadRelatedInfo();
//                }
//                RaisePropertyChanged(nameof(SelectedPlacement));
//            }
//        }

        public NewPlacementModel NewPlacement
        {
            get { return _newPlacement; }
            set
            {
                _newPlacement = value;
                RaisePropertyChanged(nameof(NewPlacement));
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

        public CanQualModel SelectedQualifiedCandidate
        {
            get { return _selectedQualifiedCandidate; }
            set
            {
                _selectedQualifiedCandidate = value;
                RaisePropertyChanged(nameof(SelectedQualifiedCandidate));
            }
        }


        private void AssignCandidateProc()
        {
            var selectedCandidates = SelectedOpening.ToAssignList.Where(c => c.IsSelected).ToList();
            foreach (var candidate in selectedCandidates)
            {
                var qualId = SelectedOpening.Model.QualificationId;
                var candidateId = candidate.Model.CandidateId;

                var canQual =
                    _repository.CanQualify.Get(c => c.CandidateId == candidateId && c.QualificationId == qualId);


                try
                {
                    NewPlacement = new NewPlacementModel(new Placement(), _repository, _selectedOpening, candidate);
                    NewHistory = new NewHistoryModel(new History(), _repository, _selectedOpening, candidate,
                        NewPlacement);
                    SelectedOpening.Placements.Add(new PlacementModel(NewPlacement.ModelCopy, _repository));

                    canQual.IsAssigned = false;
                    canQual.OpeningNumber = SelectedOpening.Model.OpeningNumber;
                    _repository.CanQualify.Update(canQual);
                    _selectedOpening.ToAssignList.Remove(candidate);
                }
                catch (Exception e)
                {
                    MessageBox.Show("Unable to assign. Reason: " + e.Message);
                }
            }
            ViewModelLocatorStatic.Locator.CompanyModule.SelectedCompany =
                ViewModelLocatorStatic.Locator.CompanyModule.SelectedCompany;
            SelectedOpening = SelectedOpening;
        }



        public CompanyModel SelectedCompany
        {
            get { return _selectedCompany; }
            set
            {
                _selectedCompany = value;
                if (value != null)
                {
                    _selectedCompany.LoadOpenings();
                }
                RaisePropertyChanged(nameof(SelectedCompany));
            }
        }

        public OpeningModel SelectedOpening
        {
            get { return _selectedOpening; }
            set
            {
                _selectedOpening?.CancelEditCommand.Execute(null);
                _selectedOpening = value;


                if (value != null)
                {
                    
                    LoadQualificationInformation();
                    _selectedOpening.LoadRelatedInfo();
                    
                }
                RaisePropertyChanged(nameof(SelectedOpening));
            }
        }

        public NewOpeningModel NewOpening
        {
            get { return _newOpening; }
            set
            {
                _newOpening = value;
                RaisePropertyChanged(nameof(NewOpening));
            }
        }

        public ICommand AddOpeningCommand => new RelayCommand(AddOpeningProc, AddOpeningCondition);

        private bool AddOpeningCondition()
        {
            if (SelectedCompany == null) return false;
            return true;
        }

        public ICommand DeleteOpeningCommand => new RelayCommand(DeleteOpeningProc, DeleteOpeningCondition);

        private bool DeleteOpeningCondition()
        {
            if (SelectedOpening == null) return false;
            return true;

        }

        private void DeleteOpeningProc()
        {
            var result = MessageBox.Show("Are you sure you want to delete selected opening?", "Opening",
                MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var placements =
                        _repository.Placement.GetRange(c => c.OpeningNumber == SelectedOpening.Model.OpeningNumber);
                    if (placements.Count > 0)
                    {
                        MessageBox.Show("Unable to delete opening with assigned candidates");
                        
                    }

                    else
                    {
                        _repository.Opening.Remove(SelectedOpening.Model);
                        SelectedCompany.Openings.Remove(SelectedOpening);
                    }
                }
                catch (Exception e)
                {

                    MessageBox.Show("Unable to delete opening");
                }
            }


        }

        public ICommand CancelOpeningCommand => new RelayCommand(CancelProc);

        public ICommand SaveOpeningCommand => new RelayCommand(SaveOpeningProc, SaveOpeningCondition);

        private void LoadQualificationInformation()
        {
            var qualification =
                _repository.Qualification.Get(c => c.QualificationId == SelectedOpening.Model.QualificationId);
            loadedQualification = qualification;
        }

        

        private void AddOpeningProc()
        {
            NewOpening = new NewOpeningModel();
            _AddOpeningWindow = new AddOpeningWindow();
            _AddOpeningWindow.Owner = Application.Current.MainWindow;
            _AddOpeningWindow.ShowDialog();
        }

        private void CancelProc()
        {
            NewOpening?.Dispose();
            _AddOpeningWindow.Close();
        }

        private bool SaveOpeningCondition()
        {
            return true;
        }

        private void SaveOpeningProc()
        {
            if (NewOpening == null) return;
            if (!NewOpening.HasChanges) return;

            try
            {
                NewOpening.ModelCopy.CompanyId = SelectedCompany.Model.CompanyId;
                NewOpening.ModelCopy.OpeningNumber = NewPerson();
                _repository.Opening.Add(NewOpening.ModelCopy);
                SelectedCompany.Openings.Add(new OpeningModel(NewOpening.ModelCopy, _repository));
                _AddOpeningWindow.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show("An error occured during save. Reason: " + e.Message, "Opening");
            }
        }

        public string NewPerson()
        {
            var list = _repository.Opening.GetRange();
            var lastPersonId = "";


            try
            {
                lastPersonId = list.Last().OpeningNumber;
            }
            catch (InvalidOperationException)
            {
                lastPersonId = "OP-0000";
            }
            // gets the last id entry to increment

            if (lastPersonId != "")
            {
                var ids = lastPersonId.ToCharArray();
                var charResult = 0;
                var isChar = false;
                var idNumberPart = "";
                var charCount = 0;
                var idDigit = 4;
                var increment = 0;
                var procStr = "";

                foreach (var id in ids)
                {
                    isChar = int.TryParse(id.ToString(), out charResult);

                    if (isChar)
                    {
                        idNumberPart = idNumberPart + id;
                    }
                }

                increment = 0;
                charResult = int.Parse(idNumberPart);
                charResult = charResult + 1;
                idNumberPart = charResult.ToString();

                charCount = idNumberPart.Length;

                while (increment < idDigit - charCount)
                {
                    procStr = procStr + "0";
                    increment++;
                }

                idNumberPart = procStr + idNumberPart;
                lastPersonId = "OP-" + idNumberPart;
            }

            return lastPersonId;
        }

        public ICommand PrintAllOpeningsCommand => new RelayCommand(PrintAllOpeningsProc);
        private SingleInstanceWindowViewer<AllOpeningsReportWindow> _allOpeningsReportWindow = new SingleInstanceWindowViewer<AllOpeningsReportWindow>();

        private void PrintAllOpeningsProc()
        {
            _allOpeningsReportWindow.Show();

        }

        private string _searchOpening;
        public string SearchOpening
        {
            get { return _searchOpening; }
            set
            {
                _searchOpening = value;
                RaisePropertyChanged(nameof(SearchOpening));
                var opList = CollectionViewSource.GetDefaultView(SelectedCompany.Openings);
                if (string.IsNullOrWhiteSpace(SearchOpening))
                {
                    opList.Filter = null;
                }
                else
                {
                    opList.Filter = obj =>
                    {
                        var sessModel = obj as OpeningModel;
                        var sc = SearchOpening.ToLowerInvariant();
                        if (sessModel == null) return false;
                        return sessModel.Model.OpeningNumber.ToLowerInvariant().Contains(sc) ||
                               sessModel.Model.Qualification.QualificationCode.ToLowerInvariant().Contains(sc);
                        ;



                    };

                }
            }
        }
    }
}