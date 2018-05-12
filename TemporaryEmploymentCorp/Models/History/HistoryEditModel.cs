using System;
using System.Runtime.InteropServices;
using TemporaryEmploymentCorp.DataAccess;
using TemporaryEmploymentCorp.Models.Candidate;
using TemporaryEmploymentCorp.Models.CanQual;
using TemporaryEmploymentCorp.Models.Editable;
using TemporaryEmploymentCorp.Models.Opening;
using TemporaryEmploymentCorp.Models.Placement;
using TemporaryEmploymentCorp.Modules;

namespace TemporaryEmploymentCorp.Models.History
{
    public class NewHistoryModel: HistoryEditModel
    {
        private IRepository _repository;
        private CandidateModel _selectedCandidate;

        public NewHistoryModel() : base(new DataAccess.EF.History())
        {
            InitializeRequiredFields();
        }

        public NewHistoryModel(DataAccess.EF.History model, IRepository repository, OpeningModel selectedOpening, CandidateSelection selectedCandidate, NewPlacementModel selectedPlacement ) : base(model)
        {
            _repository = repository;
            ModelCopy.DateStarted = selectedOpening.Model.StartDate;
            ModelCopy.DateEnded = selectedOpening.Model.EndDate;
            ModelCopy.CandidateId = selectedCandidate.Model.CandidateId;
            ModelCopy.PlacementId = selectedPlacement.ModelCopy.PlacementId;
            ModelCopy.HistoryDescription = selectedOpening.Model.Qualification.QualificationCode;
            _repository.History.Add(ModelCopy);
            //var candidate = _repository.Candidate.Get(c => c.CandidateId == selectedCandidate.Model.CandidateId);
            //SelectedCandidate = candidate;

        }

        public CandidateModel SelectedCandidate
        {
            get { return _selectedCandidate; }
            set
            {
                _selectedCandidate = value;
                RaisePropertyChanged(nameof(SelectedCandidate));
            }
        }

        private void InitializeRequiredFields()
        {
            

        }
    }

    public class HistoryEditModel:EditModelBase<DataAccess.EF.History>
    {
        protected HistoryEditModel() : base(new DataAccess.EF.History())
        {

        }

        public HistoryEditModel(DataAccess.EF.History model) : base(model)
        {
            ModelCopy = CreateCopy(model);
        }

        private DataAccess.EF.History CreateCopy(DataAccess.EF.History model)
        {
            var copy = new DataAccess.EF.History()
            {
                DateStarted = model.DateStarted,
                DateEnded =  model.DateEnded,
                HistoryDescription = model.HistoryDescription,
                HistoryId = model.HistoryId,
                CandidateId = model.CandidateId,
                PlacementId = model.PlacementId
            };

            return copy;
        }

        public DateTime? DateStarted
        {
            get { return _ModelCopy.DateStarted; }
            set
            {
                _ModelCopy.DateStarted = value;
                RaisePropertyChanged(nameof(DateStarted));
            }
        }

        public DateTime? DateEnded
        {
            get { return _ModelCopy.DateEnded; }
            set
            {
                _ModelCopy.DateEnded = value;
                RaisePropertyChanged(nameof(DateEnded));
            }
        }

        public string HistoryDescription
        {
            get { return _ModelCopy.HistoryDescription; }
            set
            {
                string tmp = value;
                string newValue = ValidateInputAndAddErrors(ref tmp, value, nameof(HistoryDescription),
                    () => string.IsNullOrWhiteSpace(value), "mailing address should not be empty");

                _ModelCopy.HistoryDescription = newValue;
            }
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

        public int? PlacementId
        {
            get { return _ModelCopy.PlacementId; }
            set
            {
                var temp = _ModelCopy.PlacementId;
                _ModelCopy.PlacementId = temp;
                RaisePropertyChanged(nameof(PlacementId));
            }
        }
    }
}
