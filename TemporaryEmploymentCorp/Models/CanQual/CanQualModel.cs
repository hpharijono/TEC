using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TemporaryEmploymentCorp.DataAccess;
using TemporaryEmploymentCorp.DataAccess.EF;
using TemporaryEmploymentCorp.Models.Editable;

namespace TemporaryEmploymentCorp.Models.CanQual
{
    public class CanQualModel:ModelBase<CanQualify>, IEditableModel<CanQualify>
    {
        private bool _isEditing;
        private EditModelBase<CanQualify> _editModel;

        public CanQualModel(CanQualify model) : base(model)
        {
        }

        public CanQualModel(CanQualify model, IRepository repository) : base(model, repository)
        {
        }

        public bool isEditing
        {
            get { return _isEditing; }
            set
            {
                _isEditing = value;
                RaisePropertyChanged(nameof(isEditing));
            }
        }

        public EditModelBase<CanQualify> EditModel
        {
            get { return _editModel; }
            set
            {
                _editModel = value;
                RaisePropertyChanged(nameof(EditModel));
            }
        }

        public ICommand EditCommand { get; }
        public ICommand SaveEditCommand { get; }
        public ICommand CancelEditCommand { get; }
    }
}
