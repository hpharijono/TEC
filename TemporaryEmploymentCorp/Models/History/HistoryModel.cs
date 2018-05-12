using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TemporaryEmploymentCorp.DataAccess;
using TemporaryEmploymentCorp.Models.Editable;

namespace TemporaryEmploymentCorp.Models.History
{
    public class HistoryModel:ModelBase<DataAccess.EF.History>, IEditableModel<DataAccess.EF.History>
    {
        private bool _isEditing;
        private EditModelBase<DataAccess.EF.History> _editModel;

        public HistoryModel(DataAccess.EF.History model) : base(model)
        {
        }

        public HistoryModel(DataAccess.EF.History model, IRepository repository) : base(model, repository)
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

        public EditModelBase<DataAccess.EF.History> EditModel
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
