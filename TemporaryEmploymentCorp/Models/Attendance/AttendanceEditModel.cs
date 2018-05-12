using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemporaryEmploymentCorp.DataAccess;
using TemporaryEmploymentCorp.Models.Candidate;
using TemporaryEmploymentCorp.Models.Editable;
using TemporaryEmploymentCorp.Models.Session;

namespace TemporaryEmploymentCorp.Models.Attendance
{
    public class NewAttendanceModel : AttendanceEditModel
    {
        public NewAttendanceModel() : base(new DataAccess.EF.Attendance())
        {
            InitializeRequiredFields();
        }

        public ObservableCollection<CandidateSelection> ToCheckAttendance { get; } = new ObservableCollection<CandidateSelection>();
        private IRepository _repository;

        public NewAttendanceModel(DataAccess.EF.Attendance model, IRepository repository, SessionModel selectedSession) : base(model)
        {
            _repository = repository;
            ToCheckAttendance.Clear();
            var enrollments = _repository.Enrollment.GetRange(c => c.SessionCode == selectedSession.Model.SessionId);
            foreach (var enrollment in enrollments)
            {
                var candidates = _repository.Candidate.Get(c => c.CandidateId == enrollment.CandidateId);
                ToCheckAttendance.Add(new CandidateSelection(candidates)); //converts candidate into CandidateSelection for displaying in add attendance window
            }

            //removes candidates that are already marked present
            foreach (var item in selectedSession.PresentCandidatesList)
            {
                var toRemove = ToCheckAttendance.FirstOrDefault(c => c.Model.CandidateId == item.Model.CandidateId);
                ToCheckAttendance.Remove(toRemove);
            }
        }

        private void InitializeRequiredFields()
        {
            

        }
    }

    public class AttendanceEditModel : EditModelBase<DataAccess.EF.Attendance>
    {
        public AttendanceEditModel(DataAccess.EF.Attendance model) : base(model)
        {
            ModelCopy = CreateCopy(model);
        }

        private DataAccess.EF.Attendance CreateCopy(DataAccess.EF.Attendance model)
        {
            var copy = new DataAccess.EF.Attendance()
            {
                AttendanceId = model.AttendanceId,
                CandidateId = model.CandidateId,
                SessionId = model.SessionId,
                IsPresent = model.IsPresent
            };
            return copy;


        }

        public int CandidateId
        {
            get { return _ModelCopy.CandidateId; }
            set
            {
                _ModelCopy.CandidateId = value;
                RaisePropertyChanged(nameof(CandidateId));
            }
        }

        public string SessionId
        {
            get { return _ModelCopy.SessionId; }
            set
            {
                _ModelCopy.SessionId = value;
                RaisePropertyChanged(nameof(SessionId));
            }
        }

        public bool? IsPresent
        {
            get { return _ModelCopy.IsPresent; }
            set
            {
                _ModelCopy.IsPresent = value;
                RaisePropertyChanged(nameof(IsPresent));
            }
        }

    }
}
