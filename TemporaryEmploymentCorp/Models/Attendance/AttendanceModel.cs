using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TemporaryEmploymentCorp.DataAccess;
using TemporaryEmploymentCorp.Models.Editable;

namespace TemporaryEmploymentCorp.Models.Attendance
{
    public class AttendanceModel : ModelBase<DataAccess.EF.Attendance>, IEditableModel<DataAccess.EF.Attendance>
    {
        private bool _isEditing;
        private EditModelBase<DataAccess.EF.Attendance> _editModel;

        public AttendanceModel(DataAccess.EF.Attendance model) : base(model)
        {
        }

        public AttendanceModel(DataAccess.EF.Attendance model, IRepository repository) : base(model, repository)
        {
        }

        public bool isEditing
        {
            get { return _isEditing; }
            set { _isEditing = value; }
        }

        public EditModelBase<DataAccess.EF.Attendance> EditModel
        {
            get { return _editModel; }
            set { _editModel = value; }
        }

        public ICommand EditCommand { get; }
        public ICommand SaveEditCommand { get; }
        public ICommand CancelEditCommand { get; }
    }
}
