using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using Nito.AsyncEx;
using TemporaryEmploymentCorp.DataAccess;
using TemporaryEmploymentCorp.Helpers.ReportViewer;
using TemporaryEmploymentCorp.Models.Candidate;
using TemporaryEmploymentCorp.Models.CanQual;
using TemporaryEmploymentCorp.Models.Editable;
using TemporaryEmploymentCorp.Models.Placement;
using TemporaryEmploymentCorp.Modules;
using TemporaryEmploymentCorp.Reports.Company;
using TemporaryEmploymentCorp.Reports.Opening;

namespace TemporaryEmploymentCorp.Models.Opening
{
    public class OpeningModel: ModelBase<DataAccess.EF.Opening>, IEditableModel<DataAccess.EF.Opening>
    {
        private bool _isEditing;
        private EditModelBase<DataAccess.EF.Opening> _editModel;
        //public ObservableCollection<CanQualModel> QualifiedCandidatesList { get; }= new ObservableCollection<CanQualModel>();

        //for multiple assignment
        public ObservableCollection<CandidateSelection> ToAssignList { get; } = new ObservableCollection<CandidateSelection>();
        public INotifyTaskCompletion PlacementLoading { get; set; }
        public INotifyTaskCompletion QualifiedCandidatesLoading { get; set; }
        public void LoadRelatedInfo()
        {
            PlacementLoading = NotifyTaskCompletion.Create(LoadPlacementAsync);
         
            //PlacementLoading = NotifyTaskCompletion.Create(LoadPlacementsAsync);
            QualifiedCandidatesLoading = NotifyTaskCompletion.Create(LoadQualifiedCandidatesListAsync);
        }
        public ObservableCollection<PlacementModel> Placements { get; } = new ObservableCollection<PlacementModel>();

        private async Task LoadPlacementAsync()
        {
            Placements.Clear();
            var placements = _Repository.Placement.GetRange(c => c.OpeningNumber == Model.OpeningNumber);

            foreach (var item in placements)
            {
                //for display purposes
                var getCandidate =  _Repository.Candidate.Get(c => c.CandidateId == item.CandidateId);
                item.Candidate = getCandidate;
                var getOpening =  _Repository.Opening.Get(c => c.OpeningNumber == item.OpeningNumber);
                item.Opening = getOpening;


                Placements.Add(new PlacementModel(item, _Repository));
            }
        }
        //        private void LoadQualifiedCandidatesList()
        //        {
        //            QualifiedCandidatesList.Clear();
        //
        //            var qualifiedCandidates =
        //                _Repository.CanQualify.GetRange(c => c.QualificationId == Model.QualificationId && c.IsAssigned == true);
        //
        //            foreach (var candidate in qualifiedCandidates)
        //            {
        //                var qualifiedCandidate = _Repository.CanQualify.Get(c => c.CanQualifyId == candidate.CanQualifyId);
        //
        //                //to display Candidate
        //                var getCandidateInfo = _Repository.Candidate.Get(c => c.CandidateId == qualifiedCandidate.CandidateId);
        //                qualifiedCandidate.Candidate = getCandidateInfo;
        //                //end
        //
        //                QualifiedCandidatesList.Add(new CanQualModel(qualifiedCandidate, _Repository));
        //            }
        //        }

        private async Task LoadQualifiedCandidatesListAsync()
        {
            ToAssignList.Clear();

            var qualifiedCandidates =
                 _Repository.CanQualify.GetRange(c => c.QualificationId == Model.QualificationId);

            foreach (var candidate in qualifiedCandidates)
            {
                var qualifiedCandidate =  _Repository.CanQualify.Get(c => c.CanQualifyId == candidate.CanQualifyId);

                //to display Candidate
                var getCandidateInfo =  _Repository.Candidate.Get(c => c.CandidateId == qualifiedCandidate.CandidateId);
                qualifiedCandidate.Candidate = getCandidateInfo;
                //end

                ToAssignList.Add(new CandidateSelection(getCandidateInfo));
            }
            
            foreach (var item in Placements)
            {
                var toRemove = ToAssignList.FirstOrDefault(c => c.Model.CandidateId == item.Model.CandidateId);
                ToAssignList.Remove(toRemove);
            }
        }
        public OpeningModel(DataAccess.EF.Opening model) : base(model)
        {
        }

        public OpeningModel(DataAccess.EF.Opening model, IRepository repository) : base(model, repository)
        {
        }

        public bool isEditing
        {
            get { return _isEditing; }
            set
            {
                _isEditing = value;
                RaisePropertyChanged(nameof(isEditing));
            }
        }

        public EditModelBase<DataAccess.EF.Opening> EditModel
        {
            get { return _editModel; }
            set
            {
                _editModel = value;
                RaisePropertyChanged(nameof(EditModel));
            }
        }

        public ICommand EditCommand => new RelayCommand(EditProc);

        private void EditProc()
        {
            isEditing = true;
            EditModel?.Dispose();
            EditModel = new OpeningEditModel(Model);

        }

        public ICommand SaveEditCommand => new RelayCommand(SaveEditProc, SaveEditCondition);

        private bool SaveEditCondition()
        {
            return (EditModel != null) && EditModel.HasChanges && !EditModel.HasErrors;
        }

        private void SaveEditProc()
        {
            if (EditModel == null) return;
            if (!EditModel.HasChanges) return;
            NotifyTaskCompletion.Create(SaveEditOpeningAsync);
        }

        private async Task SaveEditOpeningAsync()
        {
            try
            {
                
                await _Repository.Opening.UpdateAsync(EditModel.ModelCopy);

                //replace the model with the edited copy
                Model = EditModel.ModelCopy;

                isEditing = false;
            }
            catch (Exception e)
            {

                MessageBox.Show("Unable to save. Reason" + e.Message);
            }
        }

        public ICommand CancelEditCommand => new RelayCommand(CancelEditProc);

        private void CancelEditProc()
        {
            isEditing = false;
            EditModel?.Dispose();
        }

        public ICommand PrintSelectedOpeningCommand => new RelayCommand(PrintSelectedOpeningProc);
        private SingleInstanceWindowViewer<SelectedOpeningReportWindow> _selectedOpeningReportWindow = new SingleInstanceWindowViewer<SelectedOpeningReportWindow>();

        private void PrintSelectedOpeningProc()
        {
            _selectedOpeningReportWindow.Show();

        }
    }
}
