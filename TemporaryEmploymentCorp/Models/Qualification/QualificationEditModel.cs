using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemporaryEmploymentCorp.Models.Editable;

namespace TemporaryEmploymentCorp.Models.Qualification
{
    public class NewQualificationModel : QualificationEditModel
    {
        public NewQualificationModel() : base(new DataAccess.EF.Qualification())
        {
            InitializeRequiredFields();
        }

        private void InitializeRequiredFields()
        {

            QualificationCode = string.Empty;
            QualificationDescription = string.Empty;


        }
    }

    public class QualificationEditModel:EditModelBase<DataAccess.EF.Qualification>
    {
        protected QualificationEditModel() : base(new DataAccess.EF.Qualification())
        {

        }

        public QualificationEditModel(DataAccess.EF.Qualification model) : base(model)
        {
            ModelCopy = CreateCopy(model);
        }

        public string QualificationCode
        {
            get { return _ModelCopy.QualificationCode; }
            set
            {
                //                if(value.Contains("z"))
                //                    SetErrors(nameof(Name), "Name should not contain z");
                //                else
                //                {
                //                    ClearErrors(nameof(Name));
                //                    
                //                }

                string tmp = value;
                string newValue = ValidateInputAndAddErrors(ref tmp, value, nameof(QualificationCode),
                    () => string.IsNullOrWhiteSpace(value), "Qualification Code should not be empty.");

                _ModelCopy.QualificationCode = newValue;
            }
        }

        public string QualificationDescription
        {
            get { return _ModelCopy.QualificationDescription; }
            set
            {
                string tmp = value;
                string newValue = ValidateInputAndAddErrors(ref tmp, value, nameof(QualificationDescription),
                    () => string.IsNullOrWhiteSpace(value), "Description should not be empty");

                _ModelCopy.QualificationDescription = newValue;
            }
        }

        private DataAccess.EF.Qualification CreateCopy(DataAccess.EF.Qualification model)
        {
            var copy = new DataAccess.EF.Qualification()
            {
                QualificationId = model.QualificationId,
                QualificationCode = model.QualificationCode,
                QualificationDescription = model.QualificationDescription
            };
            return copy;


        }
    }
}
