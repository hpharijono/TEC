using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TemporaryEmploymentCorp.DataAccess;
using TemporaryEmploymentCorp.DataAccess.EF;
using TemporaryEmploymentCorp.Models.Editable;

namespace TemporaryEmploymentCorp.Models.Prerequisites
{
    public class PrereqModel : ModelBase<Prerequisite>, IEditableModel<Prerequisite>
    {
        private bool _isEditing;
        private EditModelBase<Prerequisite> _editModel;

        public PrereqModel(Prerequisite model) : base(model)
        {
        }

        public PrereqModel(Prerequisite model, IRepository repository) : base(model, repository)
        {
        }

        public bool isEditing
        {
            get { return _isEditing; }
            set { _isEditing = value; }
        }

        public EditModelBase<Prerequisite> EditModel
        {
            get { return _editModel; }
            set { _editModel = value; }
        }

        public ICommand EditCommand { get; }
        public ICommand SaveEditCommand { get; }
        public ICommand CancelEditCommand { get; }
    }
}
