using TemporaryEmploymentCorp.Models.Editable;

namespace TemporaryEmploymentCorp.Models.Company
{
    public class NewCompanyModel : CompanyEditModel
    {
        public NewCompanyModel() : base(new DataAccess.EF.Company())
        {
            InitializeRequiredFields();
        }

        private void InitializeRequiredFields()
        {
            CompanyName = string.Empty;
            CompanyAddress = string.Empty;
        }
    }

    public class CompanyEditModel:EditModelBase<DataAccess.EF.Company>
    {
        protected CompanyEditModel() : base(new DataAccess.EF.Company())
        {

        }

        public CompanyEditModel(DataAccess.EF.Company model) : base(model)
        {
            ModelCopy = CreateCopy(model);
        }

        public string CompanyName
        {
            get { return _ModelCopy.CompanyName; }
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
                string newValue = ValidateInputAndAddErrors(ref tmp, value, nameof(CompanyName),
                    () => string.IsNullOrWhiteSpace(value), "Company name should not be empty.");

                _ModelCopy.CompanyName = newValue;
            }
        }

        public string CompanyAddress
        {
            get { return _ModelCopy.CompanyAddress; }
            set
            {
                string tmp = value;
                string newValue = ValidateInputAndAddErrors(ref tmp, value, nameof(CompanyAddress),
                    () => string.IsNullOrWhiteSpace(value), "Company address should not be empty.");

                _ModelCopy.CompanyAddress = newValue;
            }
        }

        public string CompanyPhoneNo
        {
            get { return _ModelCopy.CompanyPhoneNo; }
            set
            {
                string tmp = value;
                string newValue = ValidateInputAndAddErrors(ref tmp, value, nameof(CompanyPhoneNo),
                    () =>
                    {
                        long x;
                        var result = long.TryParse(value, out x);
                        return !result;
                    }, "Telephone number should not contain letters");

                _ModelCopy.CompanyPhoneNo = newValue;
            }
        }

        private DataAccess.EF.Company CreateCopy(DataAccess.EF.Company model)
        {
            var copy = new DataAccess.EF.Company()
            {
                CompanyId = model.CompanyId,
                CompanyName = model.CompanyName,
                CompanyAddress = model.CompanyAddress,
                CompanyPhoneNo = model.CompanyPhoneNo
            };
            return copy;
        }
    }
}
