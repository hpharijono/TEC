using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using TemporaryEmploymentCorp.DataAccess;

namespace TemporaryEmploymentCorp.Models
{
    public class ModelBase<T> : ObservableObject
    {
        protected IRepository _Repository;

        public ModelBase(T model)
        {
            Model = model;
        }

        public ModelBase(T model, IRepository repository)
        {
            Model = model;
            _Repository = repository;
        }

        private T _model;

        public T Model
        {
            get { return _model; }
            protected set
            {
                _model = value;
                RaisePropertyChanged(nameof(Model));
            }
        }
    }
}
