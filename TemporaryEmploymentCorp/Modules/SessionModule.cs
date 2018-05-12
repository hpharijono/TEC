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
using TemporaryEmploymentCorp.DataAccess.EF;
using TemporaryEmploymentCorp.Helpers.ReportViewer;
using TemporaryEmploymentCorp.Models.Attendance;
using TemporaryEmploymentCorp.Models.Candidate;
using TemporaryEmploymentCorp.Models.Enrollees;
using TemporaryEmploymentCorp.Models.Qualification;
using TemporaryEmploymentCorp.Models.Session;
using TemporaryEmploymentCorp.Reports.Course;
using TemporaryEmploymentCorp.Reports.Session;
using TemporaryEmploymentCorp.Views.Session;
using TemporaryEmploymentCorp.Views.Session.Attendance;
using TemporaryEmploymentCorp.Views.Session.Enrollees;

namespace TemporaryEmploymentCorp.Modules
{
    public class SessionModule : ObservableObject
    {
        private IRepository _repository;
        private SessionModel _selectedSession;
        private NewSessionModel _newSession;
        public ObservableCollection<SessionModel> Sessions { get; } = new ObservableCollection<SessionModel>(); 
        public SessionModule(IRepository repository)
        {
            _repository = repository;
            SessionLoading = NotifyTaskCompletion.Create(LoadSessionAsync);
        }

        private async Task LoadSessionAsync()
        {
            var sessions = await _repository.Session.GetRangeAsync();
            foreach (var session in sessions)
            {
                var course = _repository.Course.Get(c => c.CourseId == session.CourseId);
                session.Course = course;
                Sessions.Add(new SessionModel(session, _repository));
            }
        }

        public INotifyTaskCompletion SessionLoading { get; private set; }

        public SessionModel SelectedSession
        {
            get { return _selectedSession; }
            set
            {
                _selectedSession?.CancelEditCommand.Execute(null);

                _selectedSession = value;
                if (value != null)
                {
                    LoadSessionDetails();
                    _selectedSession.LoadRelatedInfo();
                    GetNumberOfEnrollees();
                }
                RaisePropertyChanged(nameof(SelectedSession));
            }
        }

        private void LoadSessionDetails()
        {
            var course = _repository.Course.Get(c => c.CourseId == SelectedSession.Model.CourseId);
            LoadedCourse = course;

        }

        public Course LoadedCourse
        {
            get { return _loadedCourse; }
            set
            {
                _loadedCourse = value;
                RaisePropertyChanged(nameof(LoadedCourse));
            }
        }

        public NewSessionModel NewSession
        {
            get { return _newSession; }
            set
            {
                _newSession = value;
                RaisePropertyChanged(nameof(NewSession));
            }
        }

        public EnrolleeModel SelectedEnrollee
        {
            get { return _selectedEnrollee; }
            set
            {
                _selectedEnrollee = value;
                RaisePropertyChanged(nameof(SelectedEnrollee));
            }
        }

        public NewEnrollmentModel NewEnrollment
        {
            get { return _newEnrollment; }
            set
            {
                _newEnrollment = value; 
                RaisePropertyChanged(nameof(NewEnrollment));
            }
        }

        public ICommand AddEnrolleeCommand => new RelayCommand(AddEnrolleeProc);
        public AddEnrolleesWindow _AddEnrolleesWindow;
        private void AddEnrolleeProc()
        {
            NewEnrollment = new NewEnrollmentModel(new Enrollment(), _repository, SelectedSession);
            _AddEnrolleesWindow = new AddEnrolleesWindow();
            _AddEnrolleesWindow.Owner = Application.Current.MainWindow;
            _AddEnrolleesWindow.ShowDialog();

        }

        public ICommand CancelEnrolleeCommand => new RelayCommand(CancelEnrolleeProc);

        private void CancelEnrolleeProc()
        {
            NewEnrollment?.Dispose();
            _AddEnrolleesWindow.Close();
        }

        public ICommand SaveEnrolleeCommand => new RelayCommand(SaveEnrolleeProc, SaveEnrolleeCondition);

        public int NumberOfEnrollees
        {
            get { return _numberOfEnrollees; }
            set
            {
                _numberOfEnrollees = value;
                RaisePropertyChanged(nameof(NumberOfEnrollees));
            }
        }

        private void GetNumberOfEnrollees()
        {
            var enrollments = _repository.Enrollment.GetRange(c => c.SessionCode == SelectedSession.Model.SessionId);
            NumberOfEnrollees = enrollments.Count;

        }

        private bool SaveEnrolleeCondition()
        {
            return true;
        }

        private void SaveEnrolleeProc()
        {
            if (NewEnrollment == null) return;

            try
            {
                NewEnrollment.ModelCopy.Balance = GetBalance(SelectedSession.Model.Fee);
                NewEnrollment.ModelCopy.SessionCode = SelectedSession.Model.SessionId;
                _repository.Enrollment.Add(NewEnrollment.ModelCopy);
                _selectedSession.EnrolledCandidatesList.Add(new EnrolleeModel(NewEnrollment.ModelCopy, _repository));
            }
            catch (Exception e)
            {
                MessageBox.Show("Unable to save. Reason: " + e.Message);
                throw;
            }

//            var selectedEnrollees = NewEnrollment.ToEnrollList.Where(c => c.IsSelected).ToList();
//            foreach (var selectedEnrollee in selectedEnrollees)
//            {
//                try
//                {
//                    NewEnrollment.ModelCopy.CandidateId = selectedEnrollee.Model.CandidateId;
//                    NewEnrollment.ModelCopy.SessionCode = SelectedSession.Model.SessionId;                    
//
//                    _selectedSession.EnrolledCandidatesList.Add(new EnrolleeModel(NewEnrollment.ModelCopy, _repository));
//                    _repository.Enrollment.Add(NewEnrollment.ModelCopy);
//
//
//                }
//                catch (Exception)
//                {
//                    MessageBox.Show("An error occured during save", "Candidate");
//                }
//            }

            SelectedSession = _selectedSession;
            ViewModelLocatorStatic.Locator.CandidateModule.SelectedCandidate =
                ViewModelLocatorStatic.Locator.CandidateModule.SelectedCandidate;
            _AddEnrolleesWindow.Close();
        }

        public ICommand RemoveEnrolleeCommand => new RelayCommand(RemoveEnrolleeProc, RemoveEnrolleCondition);

        private void RemoveEnrolleeProc()
        {
            var result = MessageBox.Show("Are you sure you want to delete selected enrollee?", "Enrollment",
                MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    
                    
                        NotifyTaskCompletion.Create(RemoveEnrolleeCommandAsync);
                    
                }
                catch (Exception e)
                {

                    MessageBox.Show("Unable to delete opening");
                }
            }

        }

        private async Task RemoveEnrolleeCommandAsync()
        {
            await _repository.Enrollment.RemoveAsync(SelectedEnrollee.Model);
            SelectedSession.EnrolledCandidatesList.Remove(SelectedEnrollee);
        }

        private bool RemoveEnrolleCondition()
        {
            if(SelectedEnrollee == null) return false;
            return true;
        }

        private string GetBalance(string fee)
        {
            int zero = 0;
            var newBalance = Convert.ToInt32(fee) - Convert.ToInt32(NewEnrollment.ModelCopy.Balance);
            if (newBalance < 0)
            {
                return zero.ToString();
            }
            return newBalance.ToString();
        }

        public ICommand AddSessionCommand => new RelayCommand(AddSessionProc);
        public ICommand RemoveSessionCommand => new RelayCommand(RemoveSessionProc, RemoveSessionCondition);
        public ICommand RemoveAttendeeCommand => new RelayCommand(RemoveAttendeeProc, RemoveAttendeeCondition);

        private void RemoveAttendeeProc()
        {
            try
            {
                _repository.Attendance.Remove(SelectedAttendance.Model);
                SelectedSession.PresentCandidatesList.Remove(SelectedAttendance);
            }
            catch (Exception)
            {
                MessageBox.Show("Unable to Delete Attendee");
                throw;
            }
        }

        private bool RemoveAttendeeCondition()
        {
            if (SelectedAttendance == null) return false;
            return true;
          
        }

        private void RemoveSessionProc()
        {
            var result = MessageBox.Show("Are you sure you want to delete selected session?", "Session",
                MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var enrollments =
                        _repository.Enrollment.GetRange(c => c.SessionCode == SelectedSession.Model.SessionId);
                   
                    if (enrollments.Count > 0)
                    {
                        MessageBox.Show("Unable to delete sessions with enrollees");

                    }

                    else
                    {
                        NotifyTaskCompletion.Create(RemoveSessionAsync);
                    }
                }
                catch (Exception e)
                {

                    MessageBox.Show("Unable to delete opening");
                }
            }
        }

        private async Task RemoveSessionAsync()
        {
            await _repository.Session.RemoveAsync(SelectedSession.Model);
            Sessions.Remove(SelectedSession);
        }

        private bool RemoveSessionCondition()
        {
            if (SelectedSession == null) return false;
            return true;
        } 

        public AddSessionWindow _AddSessionWindow;
        private EnrolleeModel _selectedEnrollee;
        private NewEnrollmentModel _newEnrollment;
        private AttendanceModel _selectedAttendance;
        private NewAttendanceModel _newAttendance;

        private void AddSessionProc()
        {
            NewSession = new NewSessionModel();
            _AddSessionWindow = new AddSessionWindow();
            _AddSessionWindow.Owner = Application.Current.MainWindow;
            _AddSessionWindow.ShowDialog();

        }

        public ICommand CancelSessionCommand => new RelayCommand(CancelSessionProc);

        private void CancelSessionProc()
        {
            NewSession?.Dispose();
            _AddSessionWindow.Close();
        }

        public ICommand SaveSessionCommand => new RelayCommand(SaveSessionProc, SaveSessionCondition);

        private bool SaveSessionCondition()
        {
            return true;
        }

        private void SaveSessionProc()
        {
            if (NewSession == null) return;
            if (!NewSession.HasChanges) return;

            try
            {

                _repository.Session.Add(NewSession.ModelCopy);
                Sessions.Add(new SessionModel(NewSession.ModelCopy, _repository));
                _AddSessionWindow.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show("An error occured during save. Reason:" + e.Message, "Company");

            }

            SelectedSession = _selectedSession;
        }

        public AttendanceModel SelectedAttendance
        {
            get { return _selectedAttendance; }
            set
            {
                _selectedAttendance = value;
                RaisePropertyChanged(nameof(SelectedAttendance));
            }
        }

        public NewAttendanceModel NewAttendance
        {
            get { return _newAttendance; }
            set
            {
                _newAttendance = value;
                RaisePropertyChanged(nameof(NewAttendance));
            }
        }

        public ICommand AddAttendanceCommand => new RelayCommand(AddAttendanceProc);
        public AddAttendanceWindow _AddAttendanceWindow;
        private int _numberOfEnrollees;
        private Course _loadedCourse;

        private void AddAttendanceProc()
        {
            NewAttendance = new NewAttendanceModel(new Attendance(), _repository, SelectedSession);
            _AddAttendanceWindow = new AddAttendanceWindow();
            _AddAttendanceWindow.Owner = Application.Current.MainWindow;
            _AddAttendanceWindow.ShowDialog();
        }

        public ICommand CancelAttendanceCommand => new RelayCommand(CancelAttendanceProc);

        private void CancelAttendanceProc()
        {
            NewAttendance?.Dispose();
            _AddAttendanceWindow.Close();
        }

        public ICommand SaveAttendanceCommand => new RelayCommand(SaveAttendanceProc, SaveAttendanceCondition);

        private bool SaveAttendanceCondition()
        {
            return true;
        }

        private void SaveAttendanceProc()
        {
            if(NewAttendance == null) return;

            var selectedPresentEnrollees = NewAttendance.ToCheckAttendance.Where(c => c.IsSelected).ToList();
            foreach (var selectedStudent in selectedPresentEnrollees)
            {
                try
                {
                    NewAttendance.ModelCopy.SessionId = SelectedSession.Model.SessionId;
                    NewAttendance.ModelCopy.CandidateId = selectedStudent.Model.CandidateId;
                    NewAttendance.ModelCopy.IsPresent = true;

                    _repository.Attendance.Add(NewAttendance.ModelCopy);
                    _selectedSession.PresentCandidatesList.Add(new AttendanceModel(NewAttendance.ModelCopy, _repository));
                }
                catch (Exception e)
                {

                    MessageBox.Show("Unable to save. Reason: " + e.Message);
                }
            }

            SelectedSession = _selectedSession;
            _AddAttendanceWindow.Close();


        }

        public ICommand PrintAllSessionsCommand => new RelayCommand(PrintAllSessionsProc);
        private SingleInstanceWindowViewer<AllSessionReportWindow> _allSessionWindow = new SingleInstanceWindowViewer<AllSessionReportWindow>();

        private void PrintAllSessionsProc()
        {
            _allSessionWindow.Show();
        }

        public ICommand PrintAttendanceCommand => new RelayCommand(PrintAttendanceProc);
        private SingleInstanceWindowViewer<AttendanceReportWindow> _printAttendanceWindow = new SingleInstanceWindowViewer<AttendanceReportWindow>();

        private void PrintAttendanceProc()
        {
            _printAttendanceWindow.Show();
        }



        private string _searchSession;

        public string SearchSession
        {
            get { return _searchSession; }
            set
            {
                _searchSession = value;
                RaisePropertyChanged(nameof(SearchSession));
                var SessionList = CollectionViewSource.GetDefaultView(Sessions);
                if (string.IsNullOrWhiteSpace(SearchSession))
                {
                    SessionList.Filter = null;
                }
                else
                {
                    SessionList.Filter = obj =>
                    {
                        var sessModel = obj as SessionModel;
                        var sc = SearchSession.ToLowerInvariant();
                        if (sessModel == null) return false;
                        return sessModel.Model.SessionId.ToLowerInvariant().Contains(sc) ||
                               sessModel.Model.Course.CourseName.ToLowerInvariant().Contains(sc);
                        



                    };

                }
            }
        }
    }
}
