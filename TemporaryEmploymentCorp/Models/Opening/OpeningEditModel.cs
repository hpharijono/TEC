using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemporaryEmploymentCorp.Models.Editable;
using TemporaryEmploymentCorp.Models.Qualification;
using TemporaryEmploymentCorp.Modules;

namespace TemporaryEmploymentCorp.Models.Opening
{
    public class NewOpeningModel : OpeningEditModel
    {
        public NewOpeningModel() : base(new DataAccess.EF.Opening())
        {
            InitializeRequiredFields();
        }

        private void InitializeRequiredFields()
        {
            

        }
    }
    public class OpeningEditModel:EditModelBase<DataAccess.EF.Opening>
    {
        private QualificationModel _selectedQualificiation;

        public OpeningEditModel(DataAccess.EF.Opening model) : base(model)
        {
            ModelCopy = CreateCopy(model);
//            _selectedQualificiation =
//                ViewModelLocatorStatic.Locator.QualificationModule.Qualifications.FirstOrDefault(
//                    c => c.Model.QualificationCode == ModelOriginal.Qualification.QualificationCode);
        }


        protected OpeningEditModel() : base(new DataAccess.EF.Opening())
        {

        }

        

        public int CompanyId
        {
            get { return _ModelCopy.CompanyId; }
            set
            {
                _ModelCopy.CompanyId = value;
                RaisePropertyChanged(nameof(CompanyId));
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

        public bool? isAssigned
        {
            get { return _ModelCopy.IsAssigned; }
            set
            {
                _ModelCopy.IsAssigned = value;
                RaisePropertyChanged(nameof(isAssigned));
            }
        }

        public DateTime? StartDate
        {
            get { return _ModelCopy.StartDate; }
            set
            {
                _ModelCopy.StartDate = value;
                RaisePropertyChanged(nameof(StartDate));
            }
        }

        public DateTime? EndDate
        {
            get { return _ModelCopy.EndDate; }
            set
            {
                _ModelCopy.EndDate = value;
                RaisePropertyChanged(nameof(EndDate));
            }
        }

        public string HourlyPay
        {
            get { return _ModelCopy.HourlyPay; }
            set
            {
                string tmp = value;
                string newValue = ValidateInputAndAddErrors(ref tmp, value, nameof(HourlyPay),
                    () =>
                    {
                        int x;
                        var result = int.TryParse(value, out x);
                        return !result;
                    }, "Hourly Pay should not include letters");

                _ModelCopy.HourlyPay = newValue;
            }
        }

        public int QualificationId
        {
            get { return _ModelCopy.QualificationId; }
            set
            {
                _ModelCopy.QualificationId = value;
               
            }
        }

        public QualificationModel SelectedQualification
        {
            get { return _selectedQualificiation; }
            set
            {
                _selectedQualificiation = value;
                QualificationId = SelectedQualification.Model.QualificationId;
            }
        }


        private DataAccess.EF.Opening CreateCopy(DataAccess.EF.Opening model)
        {
            var copy = new DataAccess.EF.Opening
            {
                CompanyId = model.CompanyId,
                QualificationId = model.QualificationId,
                OpeningNumber = model.OpeningNumber,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                HourlyPay = model.HourlyPay,
                IsAssigned = model.IsAssigned


            };

            return copy;

        }
    }
}
