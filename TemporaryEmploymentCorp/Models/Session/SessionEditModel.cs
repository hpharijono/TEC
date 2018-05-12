using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemporaryEmploymentCorp.Models.Course;
using TemporaryEmploymentCorp.Models.Editable;

namespace TemporaryEmploymentCorp.Models.Session
{
    public class NewSessionModel : SessionEditModel
    {
         
        public NewSessionModel() : base(new DataAccess.EF.Session())
        {
            InitializeRequiredFields();
        }

        private void InitializeRequiredFields()
        {
            

        }
    }

    public class SessionEditModel : EditModelBase<DataAccess.EF.Session>
    {
        private CourseModel _selectedCourse;

        public SessionEditModel(DataAccess.EF.Session model) : base(model)
        {
            ModelCopy = CreateCopy(model);
        }

        private DataAccess.EF.Session CreateCopy(DataAccess.EF.Session model)
        {
            var copy = new DataAccess.EF.Session()
            {
                SessionId = model.SessionId,
                SessionTime = model.SessionTime,
                SessionDate = model.SessionDate,
                SessionLocation = model.SessionLocation,
                CourseId = model.CourseId,
                Fee = model.Fee,
            };
            return copy;

        }

        public CourseModel SelectedCourse
        {
            get { return _selectedCourse; }
            set
            {
                _selectedCourse = value;
                CourseId = SelectedCourse.Model.CourseId;
            }
        }

        public string CourseId
        {
            get { return _ModelCopy.CourseId; }
            set
            {
                _ModelCopy.CourseId = value;
                RaisePropertyChanged(nameof(CourseId));
            }
        }

        public string SessionId
        {
            get { return _ModelCopy.SessionId; }
            set
            {
                string tmp = value;
                string newValue = ValidateInputAndAddErrors(ref tmp, value, nameof(SessionId),
                    () => string.IsNullOrWhiteSpace(value), "ID should not be empty.");

                _ModelCopy.SessionId = newValue;
            }
        }

        public string SessionLocation
        {
            get { return _ModelCopy.SessionLocation; }
            set
            {
                string tmp = value;
                string newValue = ValidateInputAndAddErrors(ref tmp, value, nameof(SessionLocation),
                    () => string.IsNullOrWhiteSpace(value), "Location should not be empty");

                _ModelCopy.SessionLocation = newValue;
            }
        }

        public DateTime? SessionTime
        {
            get { return _ModelCopy.SessionTime; }
            set
            {
                _ModelCopy.SessionTime = value;
                RaisePropertyChanged(nameof(SessionTime));
            }
        }

        public DateTime? SessionDate
        {
            get { return _ModelCopy.SessionDate; }
            set
            {
                _ModelCopy.SessionDate = value;
                RaisePropertyChanged(nameof(SessionDate));
            }
        }

        public string Fee
        {
            get { return _ModelCopy.Fee; }
            set
            {
                string tmp = value;
                string newValue = ValidateInputAndAddErrors(ref tmp, value, nameof(Fee),
                    () =>
                    {
                        long x;
                        var result = long.TryParse(value, out x);
                        return !result;
                    }, "Fee should not contain letters");

                _ModelCopy.Fee = newValue;
            }
        }
    }
}
