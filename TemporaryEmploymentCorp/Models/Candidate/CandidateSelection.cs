using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using TemporaryEmploymentCorp.DataAccess;

namespace TemporaryEmploymentCorp.Models.Candidate
{
    public class CandidateSelection : ModelBase<DataAccess.EF.Candidate>
    {
        private bool _isSelected;


        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                RaisePropertyChanged(nameof(IsSelected));
            }
        }

        public CandidateSelection(DataAccess.EF.Candidate model) : base(model)
        {
        }

        public CandidateSelection(DataAccess.EF.Candidate model, IRepository repository) : base(model, repository)
        {
        }
    }
}
