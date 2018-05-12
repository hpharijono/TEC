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
using TemporaryEmploymentCorp.Models.CanQual;
using TemporaryEmploymentCorp.Models.Editable;
using TemporaryEmploymentCorp.Models.Enrollees;
using TemporaryEmploymentCorp.Models.History;
using TemporaryEmploymentCorp.Reports.Candidate;

namespace TemporaryEmploymentCorp.Models.Candidate
{
    public class CandidateModel:ModelBase<DataAccess.EF.Candidate>, IEditableModel<DataAccess.EF.Candidate>
    {
        private bool _isEditing;
        private EditModelBase<DataAccess.EF.Candidate> _editModel;

        public ObservableCollection<HistoryModel> History { get; } = new ObservableCollection<HistoryModel>();
        public ObservableCollection<EnrolleeModel> EnrolledIn { get; } = new ObservableCollection<EnrolleeModel>();
        public ObservableCollection<CanQualModel> CandidateQualifications { get; } = new ObservableCollection<CanQualModel>();

        public INotifyTaskCompletion CanQualLoading { get; private set; }
        public INotifyTaskCompletion HistoryLoading { get; private set; }
        public INotifyTaskCompletion EnrolledSessionLoading { get; private set; }

        public void LoadRelatedInfo()
        {
            EnrolledSessionLoading = NotifyTaskCompletion.Create(LoadEnrolledSessionsAsync);
            CanQualLoading = NotifyTaskCompletion.Create(LoadCanQualAsync);
            HistoryLoading = NotifyTaskCompletion.Create(LoadHistoriesAsync);
        }

        private async Task LoadEnrolledSessionsAsync()
        {
            EnrolledIn.Clear();
            var enrolledSessions = await _Repository.Enrollment.GetRangeAsync(c => c.CandidateId == Model.CandidateId);
            foreach (var enrolledSession in enrolledSessions)
            {
                var getSession = _Repository.Session.Get(c => c.SessionId == enrolledSession.SessionCode);
                var getCourse = _Repository.Course.Get(c => c.CourseId == getSession.CourseId);
                getSession.Course = getCourse;
                enrolledSession.Session = getSession;
                
                EnrolledIn.Add(new EnrolleeModel(enrolledSession, _Repository));
                await Task.Delay(100);
            }
        }

        private async Task LoadCanQualAsync()
        {
            CandidateQualifications.Clear();
            var canQual = await _Repository.CanQualify.GetRangeAsync(c => c.CandidateId == Model.CandidateId);

            foreach (var cq in canQual)
            {
                var getQual = _Repository.Qualification.Get(c => c.QualificationId == cq.QualificationId);                
                
                cq.Qualification = getQual;
                CandidateQualifications.Add(new CanQualModel(cq, _Repository));
                await Task.Delay(100);
            }
        }

        public ICommand PrintSelectedCandidate => new RelayCommand(PrintSelectedCandidateProc);
        private SingleInstanceWindowViewer<SelectedCandidateReportWindow> _selectedCandWindow = new SingleInstanceWindowViewer<SelectedCandidateReportWindow>();

        private void PrintSelectedCandidateProc()
        {
            _selectedCandWindow.Show();
        }

        private async Task LoadHistoriesAsync()
        {
            var histories = await _Repository.History.GetRangeAsync(c => c.CandidateId == Model.CandidateId);
            History.Clear();
            foreach (var history in histories)
            {
                History.Add(new HistoryModel(history, _Repository));
                await Task.Delay(100);
            }
        }

        public CandidateModel(DataAccess.EF.Candidate model) : base(model)
        {
        }

        public CandidateModel(DataAccess.EF.Candidate model, IRepository repository) : base(model, repository)
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

        public EditModelBase<DataAccess.EF.Candidate> EditModel
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
            EditModel = new CandidateEditModel(Model);
        }

        public ICommand SaveEditCommand => new RelayCommand(SaveEditProc, SaveEditCondition);

        private bool SaveEditCondition()
        {
            return (EditModel != null) && EditModel.HasChanges && !EditModel.HasErrors;
        }

        private void SaveEditProc()
        {
            NotifyTaskCompletion.Create(SaveEditCandidateAsync);
        }

        private async Task SaveEditCandidateAsync()
        {
            if (EditModel == null) return;
            if (!EditModel.HasChanges) return;

            try
            {
                await _Repository.Candidate.UpdateAsync(EditModel.ModelCopy);

                //replace the model with the edited copy
                Model = EditModel.ModelCopy;

                isEditing = false;
            }
            catch (Exception e)
            {

                MessageBox.Show("Unable to save. Reason" + e.Message);
            }
        }

        public ICommand CancelEditCommand => new RelayCommand(CancelProc);

        private void CancelProc()
        {
            isEditing = false;
            EditModel?.Dispose();
        }
    }
}
