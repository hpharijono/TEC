using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemporaryEmploymentCorp.Models.CanQual
{
    public class QualificationSelection : ModelBase<DataAccess.EF.Qualification>
    {
        private bool _isSelected;

        public QualificationSelection(DataAccess.EF.Qualification model) : base(model)
        {
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                RaisePropertyChanged(nameof(IsSelected));
            }
        }
    }
}
