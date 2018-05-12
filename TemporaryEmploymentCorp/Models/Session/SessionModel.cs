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
using TemporaryEmploymentCorp.Models.Attendance;
using TemporaryEmploymentCorp.Models.Editable;
using TemporaryEmploymentCorp.Models.Enrollees;
using TemporaryEmploymentCorp.Modules;
using TemporaryEmploymentCorp.Reports.Session;

namespace TemporaryEmploymentCorp.Models.Session
{
    public class SessionModel : ModelBase<DataAccess.EF.Session>, IEditableModel<DataAccess.EF.Session>
    {
        private bool _isEditing;
        private EditModelBase<DataAccess.EF.Session> _editModel;
        public ObservableCollection<EnrolleeModel> EnrolledCandidatesList { get; } = new ObservableCollection<EnrolleeModel>();
        public ObservableCollection<AttendanceModel> PresentCandidatesList { get; } = new ObservableCollection<AttendanceModel>();
        public ObservableCollection<EnrolleeModel> AbsentCandidatesList { get; } = new ObservableCollection<EnrolleeModel>();
        public SessionModel(DataAccess.EF.Session model) : base(model)
        {
        }

        public SessionModel(DataAccess.EF.Session model, IRepository repository) : base(model, repository)
        {

        }

        public void LoadPresentCandidates()
        {
            var attendances = _Repository.Attendance.GetRange(c => c.SessionId == Model.SessionId);
            PresentCandidatesList.Clear();
            
            foreach (var attendance in attendances)
            {
                var candidate = _Repository.Candidate.Get(c => c.CandidateId == attendance.CandidateId);
                attendance.Candidate = candidate;
                PresentCandidatesList.Add(new AttendanceModel(attendance));
            }

            foreach (var item in PresentCandidatesList)
            {
                var toRemove = AbsentCandidatesList.FirstOrDefault(c => c.Model.CandidateId == item.Model.CandidateId);
                AbsentCandidatesList.Remove(toRemove);
            }
        }

       

        public void LoadRelatedInfo()
        {
           
            LoadEnrollees();
            LoadPresentCandidates();
        }

        

        public void LoadEnrollees()
        {
            var enrollees = _Repository.Enrollment.GetRange(c => c.SessionCode == Model.SessionId);
            EnrolledCandidatesList.Clear();
            AbsentCandidatesList.Clear();
            foreach (var enrollee in enrollees)
            {
                var candidate = _Repository.Candidate.Get(c => c.CandidateId == enrollee.CandidateId);
                enrollee.Candidate = candidate;

                EnrolledCandidatesList.Add(new EnrolleeModel(enrollee, _Repository));
                AbsentCandidatesList.Add(new EnrolleeModel(enrollee, _Repository));
              
                
            }
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

        public EditModelBase<DataAccess.EF.Session> EditModel
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
            EditModel = new SessionEditModel(Model);
        }

        public ICommand SaveEditCommand => new RelayCommand(SaveEditProc, SaveEditCondition);

        private bool SaveEditCondition()
        {
            return (EditModel != null) && EditModel.HasChanges && !EditModel.HasErrors;
        }

        private void SaveEditProc()
        {
            NotifyTaskCompletion.Create(SaveEditSessionAsync);
        }

        private async Task SaveEditSessionAsync()
        {
            if (EditModel == null) return;
            if (!EditModel.HasChanges) return;

            try
            {
                await _Repository.Session.UpdateAsync(EditModel.ModelCopy);

                //replace the model with the edited copy
                Model = EditModel.ModelCopy;

                isEditing = false;
            }
            catch (Exception e)
            {

                MessageBox.Show("Unable to save. Reason" + e.Message);
            }

            ViewModelLocatorStatic.Locator.SessionModule.SelectedSession =
                ViewModelLocatorStatic.Locator.SessionModule.SelectedSession;
        }

        public ICommand CancelEditCommand => new RelayCommand(CancelProc);

        private void CancelProc()
        {
            isEditing = false;
            EditModel?.Dispose();
            
        }

        public ICommand PrintSelectedSessionCommand => new RelayCommand(PrintSelectedSessionProc);
        private SingleInstanceWindowViewer<SelectedSesssionReportWindow> _selectedSessionWindow = new SingleInstanceWindowViewer<SelectedSesssionReportWindow>();

        private void PrintSelectedSessionProc()
        {
            _selectedSessionWindow.Show();
        }
    }
}
