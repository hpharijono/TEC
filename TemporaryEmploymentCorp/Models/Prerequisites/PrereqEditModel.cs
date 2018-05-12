using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemporaryEmploymentCorp.DataAccess.EF;
using TemporaryEmploymentCorp.Models.Editable;

namespace TemporaryEmploymentCorp.Models.Prerequisites
{
    public class NewPrereqModel : PrereqEditModel
    {
        public NewPrereqModel() : base(new Prerequisite())
        {
            InitializeRequiredFields();
        }

        private void InitializeRequiredFields()
        {
            

        }
    }

    public class PrereqEditModel : EditModelBase<Prerequisite>
    {
        public PrereqEditModel(Prerequisite model) : base(model)
        {
            ModelCopy = CreateCopy(model);
        }

        private Prerequisite CreateCopy(Prerequisite model)
        {
            var copy = new Prerequisite()
            {
                QualificationId = model.QualificationId,
                CourseId = model.CourseId,
                PrerequisitesId = model.PrerequisitesId
            };

            return copy;

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

        public string CourseId
        {
            get { return _ModelCopy.CourseId; }
            set
            {
                _ModelCopy.CourseId = value;
                RaisePropertyChanged(nameof(CourseId));
            }
        }
    }
}
