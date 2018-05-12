using TemporaryEmploymentCorp.Models.Editable;
using TemporaryEmploymentCorp.Models.Qualification;

namespace TemporaryEmploymentCorp.Models.Course
{
    public class NewCourseModel : CourseEditModel
    {
        public NewCourseModel() : base(new DataAccess.EF.Course())
        {
            InitializeRequiredFields();
        }

        private void InitializeRequiredFields()
        {
            CourseDescription = string.Empty;
            CourseName = string.Empty;
            CourseId = string.Empty;
        }
    }

    public class CourseEditModel:EditModelBase<DataAccess.EF.Course>
    {
        private QualificationModel _selectedQualificiation;

        protected CourseEditModel() : base(new DataAccess.EF.Course())
        {

        }

        public CourseEditModel(DataAccess.EF.Course model) : base(model)
        {
            ModelCopy = CreateCopy(model);
        }

        private DataAccess.EF.Course CreateCopy(DataAccess.EF.Course model)
        {
            var copy = new DataAccess.EF.Course()
            {
                CourseId = model.CourseId,
                CourseDescription = model.CourseDescription,
                CourseName = model.CourseName,
                QualificationId = model.QualificationId
            };
            return copy;
        }

        public string CourseId
        {
            get { return _ModelCopy.CourseId; }
            set
            {
                string tmp = value;
                string newValue = ValidateInputAndAddErrors(ref tmp, value, nameof(CourseId),
                    () => string.IsNullOrWhiteSpace(value), "Course ID should not be empty");

                _ModelCopy.CourseId = newValue;
            }
        }

        public string CourseName
        {
            get { return _ModelCopy.CourseName; }
            set
            {
                string tmp = value;
                string newValue = ValidateInputAndAddErrors(ref tmp, value, nameof(CourseName),
                    () => string.IsNullOrWhiteSpace(value), "Course Name should not be empty");

                _ModelCopy.CourseName = newValue;
            }
        }

        public string CourseDescription
        {
            get { return _ModelCopy.CourseDescription; }
            set
            {
                string tmp = value;
                string newValue = ValidateInputAndAddErrors(ref tmp, value, nameof(CourseDescription),
                    () => string.IsNullOrWhiteSpace(value), "Description should not be empty");

                _ModelCopy.CourseDescription = newValue;
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

        public int QualificationId
        {
            get { return _ModelCopy.QualificationId; }
            set
            {
                _ModelCopy.QualificationId = value;
                
            }
        }
    }
}
