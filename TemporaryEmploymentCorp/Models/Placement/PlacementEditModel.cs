using System;
using TemporaryEmploymentCorp.DataAccess;
using TemporaryEmploymentCorp.Models.Candidate;
using TemporaryEmploymentCorp.Models.CanQual;
using TemporaryEmploymentCorp.Models.Editable;
using TemporaryEmploymentCorp.Models.Opening;

namespace TemporaryEmploymentCorp.Models.Placement
{
    public class NewPlacementModel : PlacementEditModel
    {
        public NewPlacementModel() : base(new DataAccess.EF.Placement())
        {
            InitializeRequiredFields();
        }

        private IRepository _repository;
        public NewPlacementModel(DataAccess.EF.Placement model, IRepository repository, OpeningModel selectedOpening, CandidateSelection selectedCandidate) : base(model)
        {
            _repository = repository;
            ModelCopy.TotalHoursWorked = GetTotalHoursWorked(selectedOpening);
            ModelCopy.CandidateId = selectedCandidate.Model.CandidateId;
            ModelCopy.OpeningNumber = selectedOpening.Model.OpeningNumber;
            _repository.Placement.Add(ModelCopy);

        }

        private string GetTotalHoursWorked(OpeningModel selectedOpening)
        {
            var dateStarted = selectedOpening.Model.StartDate;
            var dateEnded = selectedOpening.Model.EndDate;

            TimeSpan? difference = dateEnded - dateStarted;
            var days = difference.Value.Days;
            var hours = days*8; //8 hours a day
            return Convert.ToString(hours);

        }

        private void InitializeRequiredFields()
        {
            

        }
    }

    public class PlacementEditModel : EditModelBase<DataAccess.EF.Placement>
    {
        public PlacementEditModel(DataAccess.EF.Placement model) : base(model)
        {
            ModelCopy = CreateCopy(model);
        }

        private DataAccess.EF.Placement CreateCopy(DataAccess.EF.Placement model)
        {
            var copy = new DataAccess.EF.Placement()
            {
                PlacementId = model.PlacementId,
                OpeningNumber = model.OpeningNumber,
                CandidateId = model.CandidateId,
                TotalHoursWorked = model.TotalHoursWorked
            };
            return copy;

        }

        public string OpeningNumber
        {
            get { return _ModelCopy.OpeningNumber; }
            set
            {
                _ModelCopy.OpeningNumber = value;
                RaisePropertyChanged(nameof(OpeningNumber));
            }
        }

        public int CandidateId
        {
            get { return _ModelCopy.CandidateId; }
            set
            {
                _ModelCopy.CandidateId = value;
                RaisePropertyChanged(nameof(OpeningNumber));
            }
        }

        public string TotalHoursWorked
        {
            get { return _ModelCopy.TotalHoursWorked; }
            set
            {
                _ModelCopy.TotalHoursWorked = value;
                RaisePropertyChanged(nameof(TotalHoursWorked));
            }
        }






    }
}
