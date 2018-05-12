using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using Nito.AsyncEx;
using TemporaryEmploymentCorp.DataAccess;
using TemporaryEmploymentCorp.Helpers.ReportViewer;
using TemporaryEmploymentCorp.Models.Editable;
using TemporaryEmploymentCorp.Models.Opening;
using TemporaryEmploymentCorp.Reports.Company;

namespace TemporaryEmploymentCorp.Models.Company
{
    public class CompanyModel:ModelBase<DataAccess.EF.Company>, IEditableModel<DataAccess.EF.Company>
    {
        public ObservableCollection<OpeningModel> Openings { get; } = new ObservableCollection<OpeningModel>();
        private IRepository _repository;
        private int _numberOfOpenings;

        public void LoadRelatedInfo()
        {
            //LoadOpenings();
            LoadNumberOfOpenings();
        }

        public void LoadOpenings()
        {
            NotifyTaskCompletion.Create(() => LoadOpeningsAsync());
        }

        public void LoadNumberOfOpenings()
        {
            NotifyTaskCompletion.Create(() => LoadNumberOfOpeningsAsync());
        }
        



        public int NumberOfOpenings
        {
            get { return _numberOfOpenings; }
            set
            {
                _numberOfOpenings = value;
                RaisePropertyChanged(nameof(NumberOfOpenings));
            }
        }

        public int NumberOfAssigned
        {
            get { return _numberOfAssigned; }
            set
            {
                _numberOfAssigned = value;
                RaisePropertyChanged(nameof(NumberOfAssigned));
            }
        }



        private async Task LoadNumberOfOpeningsAsync()
        {
            var openings = await Task.Run(() => _repository.Opening.GetRangeAsync(c => c.CompanyId == Model.CompanyId));
            NumberOfOpenings = openings.Count;
        }

        private async Task LoadOpeningsAsync()
        {
//            CancellationToken token = new CancellationToken();
//            token.ThrowIfCancellationRequested();
//
//            CancellationTokenSource.CreateLinkedTokenSource()
            Openings.Clear();
            var openings =  await Task.Run(() => _Repository.Opening.GetRange(c => c.CompanyId == Model.CompanyId ));
            
            foreach (var opening in openings)
            {
                var qual = await Task.Run(() => _repository.Qualification.Get(c => c.QualificationId == opening.QualificationId));
                opening.Qualification = qual;
               
                Openings.Add(new OpeningModel(opening, _repository ));
                Task.Delay(10);
            }
        }

        private bool _isEditing;
        private EditModelBase<DataAccess.EF.Company> _editModel;
        private int _numberOfAssigned;

        public CompanyModel(DataAccess.EF.Company model) : base(model)
        {
        }

        public CompanyModel(DataAccess.EF.Company model, IRepository repository) : base(model, repository)
        {
            _repository = repository;
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

        public EditModelBase<DataAccess.EF.Company> EditModel
        {
            get { return _editModel; }
            set
            {
                _editModel = value;
                RaisePropertyChanged(nameof(EditModel));
            }
        }

        public ICommand EditCommand => new RelayCommand(EditProc);

        private void EditProc()
        {
            isEditing = true;
            EditModel?.Dispose();
            EditModel = new CompanyEditModel(Model);
        }

        public ICommand SaveEditCommand => new RelayCommand(SaveEditProc, SaveEditCondition);

        private bool SaveEditCondition()
        {
            return (EditModel != null) && EditModel.HasChanges && !EditModel.HasErrors;
        }

        private void SaveEditProc()
        {
            NotifyTaskCompletion.Create(SaveEditCompanyAsync);            
        }

        private async Task SaveEditCompanyAsync()
        {
            if (EditModel == null) return;
            if (!EditModel.HasChanges) return;

            try
            {
                await _Repository.Company.UpdateAsync(EditModel.ModelCopy);

                //replace the model with the edited copy
                Model = EditModel.ModelCopy;

                isEditing = false;
            }
            catch (Exception e)
            {

                MessageBox.Show("Unable to save. Reason" + e.Message);
            }
        }

        public ICommand CancelEditCommand => new RelayCommand(CancelEditProc);

        private void CancelEditProc()
        {
            isEditing = false;
            EditModel?.Dispose();
        }

        
    }
}
