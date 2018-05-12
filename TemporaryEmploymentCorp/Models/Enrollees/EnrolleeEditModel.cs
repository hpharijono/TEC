using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemporaryEmploymentCorp.DataAccess;
using TemporaryEmploymentCorp.DataAccess.EF;
using TemporaryEmploymentCorp.Models.Candidate;
using TemporaryEmploymentCorp.Models.Editable;
using TemporaryEmploymentCorp.Models.Session;

namespace TemporaryEmploymentCorp.Models.Enrollees
{
    public class NewEnrollmentModel : EnrolleeEditModel
    {
        public ObservableCollection<CandidateModel> ToEnrollList { get; } =
            new ObservableCollection<CandidateModel>();

        private IRepository _repository;

        public NewEnrollmentModel() : base(new Enrollment())
        {
            InitializeRequiredFields();
        }

        public NewEnrollmentModel(Enrollment model, IRepository repository, SessionModel selectedSession) : base(model)
        {
            _repository = repository;

            var prerequisites = _repository.Prerequisite.GetRange(c => c.CourseId == selectedSession.Model.CourseId);
                //gets all the prerequisite subjects for that course
            var requiredQualIds = prerequisites.Select(c => c.QualificationId).ToList();


            if (prerequisites.Count != 0)
            {
                ToEnrollList.Clear();
                var prerequisiteQualifications = new List<CanQualify>();
                foreach (var prerequisite in prerequisites)
                {
                    var canQuals =
                        _repository.CanQualify.GetRange(c => c.QualificationId == prerequisite.QualificationId);
                        //gets all the candidates from the CandidateQualification entity (not candidate Entity) that has all the required prerequisites to enroll in that session

                    foreach (var canQualify in canQuals)
                    {
                        prerequisiteQualifications.Add(canQualify);
                    }
//                    foreach (var candidates in canQuals)
//                    {
//                        //gets the candidate from the Candidate entity and converts (.Select) it to CandidateSelection and added to the AddEnrolleeWindow listbox.
//                        var candidate =
//                            _repository.Candidate.GetRange(c => c.CandidateId == candidates.CandidateId)
//                                .Select(converted => new CandidateSelection(converted));
//                        foreach (var candidateSelection in candidate)
//                        {
//                            ToEnrollList.Add(candidateSelection);
//                        }
//
//                    }
                }

                var canQualificationsGrouped =
                    prerequisiteQualifications.GroupBy(c => c.CandidateId).ToList();

                var eligibleCandidates = new List<CandidateModel>();
                foreach (var item in canQualificationsGrouped)
                {
                    var result = item.Select(c => c.QualificationId).Intersect(requiredQualIds);
                    if (result.Count() != requiredQualIds.Count) continue;
                    {
                        // get the associated candidate using the key since key == CandidateID;
                        var candidate = _repository.Candidate.Get(c => c.CandidateId == item.Key);
                        eligibleCandidates.Add(new CandidateModel(candidate));
                    }
                }


                foreach (var item in eligibleCandidates)
                {
                    ToEnrollList.Add(item);
                }
            }
            else
            {
                //if no prerequisites, load all candidates to the list. (all candidates can enroll)
                var candidates = _repository.Candidate.GetRange().Select(c => new CandidateModel(c));
                ToEnrollList.Clear();
                foreach (var candidate in candidates)
                {
                    ToEnrollList.Add(candidate);
                }
            }

            //removes candidates that are already enrolled (selectedSession.EnrolledCandidatesList is binded to listbox in EnrolleesView)
            foreach (var item in selectedSession.EnrolledCandidatesList)
            {
                var toRemove = ToEnrollList.FirstOrDefault(c => c.Model.CandidateId == item.Model.CandidateId);
                ToEnrollList.Remove(toRemove);
            }
        }


        private void InitializeRequiredFields()
        {
        }
    }

    public class EnrolleeEditModel : EditModelBase<Enrollment>
    {
        private CandidateModel _selectedCandidate;

        public EnrolleeEditModel(Enrollment model) : base(model)
        {
            ModelCopy = CreateCopy(model);
        }

        private Enrollment CreateCopy(Enrollment model)
        {
            var copy = new Enrollment()
            {
                EnrollmentId = model.EnrollmentId,
                SessionCode = model.SessionCode,
                CandidateId = model.CandidateId,
                Balance = model.Balance
            };

            return copy;
        }

        public CandidateModel SelectedCandidate
        {
            get { return _selectedCandidate; }
            set
            {
                _selectedCandidate = value;
                CandidateId = SelectedCandidate.Model.CandidateId;
            }
        }

        public int CandidateId
        {
            get { return _ModelCopy.CandidateId; }
            set
            {
                _ModelCopy.CandidateId = value;
                //RaisePropertyChanged(nameof(CandidateId));
            }
        }

        public string SessionCode
        {
            get { return _ModelCopy.SessionCode; }
            set
            {
                _ModelCopy.SessionCode = value;
                RaisePropertyChanged(nameof(SessionCode));
            }
        }

        public string Balance
        {
            get { return _ModelCopy.Balance; }
            set
            {
                string tmp = value;
                string newValue = ValidateInputAndAddErrors(ref tmp, value, nameof(Balance),
                    () =>
                    {
                        long x;
                        var result = long.TryParse(value, out x);
                        return !result;
                    }, "Balance should not contain letters");

                _ModelCopy.Balance = newValue;
            }
        }
    }
}