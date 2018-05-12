using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TemporaryEmploymentCorp.DataAccess;
using TemporaryEmploymentCorp.Models.Editable;

namespace TemporaryEmploymentCorp.Models.Placement
{
    public class PlacementModel : ModelBase<DataAccess.EF.Placement>, IEditableModel<DataAccess.EF.Placement>
    {
        public PlacementModel(DataAccess.EF.Placement model) : base(model)
        {
        }

        public PlacementModel(DataAccess.EF.Placement model, IRepository repository) : base(model, repository)
        {
        }

        public bool isEditing { get; }
        public EditModelBase<DataAccess.EF.Placement> EditModel { get; }
        public ICommand EditCommand { get; }
        public ICommand SaveEditCommand { get; }
        public ICommand CancelEditCommand { get; }
    }
}
