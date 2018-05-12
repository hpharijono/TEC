using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemporaryEmploymentCorp.Models.Editable;

namespace TemporaryEmploymentCorp.Models.Candidate
{
    public class NewCandidateModel : CandidateEditModel
    {
        public NewCandidateModel() : base(new DataAccess.EF.Candidate())
        {
            InitializeRequiredFields();
        }

        private void InitializeRequiredFields()
        {
            //            Name = string.Empty;
            //            Address = string.Empty;
            //            BirthDate = null; //last will show up first in errors
        }
    }

    public class CandidateEditModel : EditModelBase<DataAccess.EF.Candidate>
    {
        protected CandidateEditModel() : base(new DataAccess.EF.Candidate())
        {

        }

        public CandidateEditModel(DataAccess.EF.Candidate model) : base(model)
        {
            ModelCopy = CreateCopy(model);
        }

        private DataAccess.EF.Candidate CreateCopy(DataAccess.EF.Candidate model)
        {
            var copy = new DataAccess.EF.Candidate()
            {
                CandidateId = model.CandidateId,
                CandidateFirstName = model.CandidateFirstName,
                CandidateMiddleName = model.CandidateMiddleName,
                CandidateLastName = model.CandidateLastName,
                CandidateAddress = model.CandidateAddress,
                CandidateBirthdate = model.CandidateBirthdate,
                CandidatePhoneNo = model.CandidatePhoneNo

            };
            return copy;

        }

        public string CandidateFirstName
        {
            get { return _ModelCopy.CandidateFirstName; }
            set
            {
                string tmp = value;
                string newValue = ValidateInputAndAddErrors(ref tmp, value, nameof(CandidateFirstName),
                    () => string.IsNullOrWhiteSpace(value), "First Name should not be empty");

                _ModelCopy.CandidateFirstName = newValue;
            }
        }

        public string CandidateMiddleName
        {
            get { return _ModelCopy.CandidateMiddleName; }
            set
            {
                string tmp = value;
                string newValue = ValidateInputAndAddErrors(ref tmp, value, nameof(CandidateMiddleName),
                    () => string.IsNullOrWhiteSpace(value), "Middle Name should not be empty");

                _ModelCopy.CandidateMiddleName = newValue;
            }
        }

        public string CandidateLastName
        {
            get { return _ModelCopy.CandidateLastName; }
            set
            {
                string tmp = value;
                string newValue = ValidateInputAndAddErrors(ref tmp, value, nameof(CandidateLastName),
                    () => string.IsNullOrWhiteSpace(value), "Last Name should not be empty");

                _ModelCopy.CandidateLastName = newValue;
            }
        }

        public string CandidateAddress
        {
            get { return _ModelCopy.CandidateAddress; }
            set
            {
                string tmp = value;
                string newValue = ValidateInputAndAddErrors(ref tmp, value, nameof(CandidateAddress),
                    () => string.IsNullOrWhiteSpace(value), "Address should not be empty");

                _ModelCopy.CandidateAddress = newValue;
            }
        }

        public string CandidatePhoneNo
        {
            get { return _ModelCopy.CandidatePhoneNo; }
            set
            {
                string tmp = value;
                string newValue = ValidateInputAndAddErrors(ref tmp, value, nameof(CandidatePhoneNo),
                    () =>
                    {
                        int x;
                        var result = int.TryParse(value, out x);
                        return !result;
                    }, "Telephone number should not contain letters");

                _ModelCopy.CandidatePhoneNo = newValue;
            }
        }

        public DateTime? CandidateBirthdate
        {
            get { return _ModelCopy.CandidateBirthdate; }
            set
            {
                _ModelCopy.CandidateBirthdate = value;
                RaisePropertyChanged(nameof(CandidateBirthdate));
            }
        }
    }
}
