using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemporaryEmploymentCorp.DataAccess.EF;
using TemporaryEmploymentCorp.Models.Editable;

namespace TemporaryEmploymentCorp.Models.CanQual
{
    public class NewCanQualModel:CanQualEditModel
    {
        public NewCanQualModel() : base(new CanQualify())
        {
            InitializeRequiredFields();
        }

        private void InitializeRequiredFields()
        {
            

        }
    }

    public class CanQualEditModel:EditModelBase<CanQualify>
    {
        public CanQualEditModel(CanQualify model) : base(model)
        {
            ModelCopy = CreateCopy(model);
            
        }

        private CanQualify CreateCopy(CanQualify model)
        {
            var copy = new CanQualify()
            {
                CanQualifyId = model.CanQualifyId,
                QualificationId = model.QualificationId,
                CandidateId = model.CandidateId,
                IsAssigned = model.IsAssigned,
                OpeningNumber = model.OpeningNumber
                
                
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

        public string OpeningNumber
        {
            get { return _ModelCopy.OpeningNumber; }
            set
            {
                _ModelCopy.OpeningNumber = value;
                RaisePropertyChanged(nameof(OpeningNumber));
            }
        }

        public int QualificationId
        {
            get { return _ModelCopy.QualificationId; }
            set
            {
                _ModelCopy.QualificationId = value;
                RaisePropertyChanged(nameof(QualificationId));
            }
        }

        public bool IsAssigned
        {
            get { return _ModelCopy.IsAssigned; }
            set
            {
                _ModelCopy.IsAssigned = value;
                RaisePropertyChanged(nameof(IsAssigned));
            }
        }
    }
}
