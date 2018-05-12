using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using TemporaryEmploymentCorp.DataAccess;
using TemporaryEmploymentCorp.DataAccess.EF;
using TemporaryEmploymentCorp.Models.Editable;
using TemporaryEmploymentCorp.Modules;
using TemporaryEmploymentCorp.Views.Session.Enrollees;

namespace TemporaryEmploymentCorp.Models.Enrollees
{
    public class EnrolleeModel : ModelBase<Enrollment>, IEditableModel<Enrollment>
    {
        private bool _isEditing;
        private EditModelBase<Enrollment> _editModel;

        public EnrolleeModel(Enrollment model) : base(model)
        {
        }

        public EnrolleeModel(Enrollment model, IRepository repository) : base(model, repository)
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

        public EditModelBase<Enrollment> EditModel
        {
            get { return _editModel; }
            set
            {
                _editModel = value;
                RaisePropertyChanged(nameof(EditModel));
            }
        }

        public ICommand EditCommand => new RelayCommand(EditProc, EditCondition);

        private bool EditCondition()
        {
            if (Model == null) return false;
            return true;
        }


        public EditEnrolleeWindow _editWindow;
        private void EditProc()
        {
            _editWindow = new EditEnrolleeWindow();
            EditModel?.Dispose();
            EditModel = new EnrolleeEditModel(Model);

            _editWindow.ShowDialog();

        }

        public ICommand SaveEditCommand => new RelayCommand(SaveEditProc, SaveEditCondition);

        private void SaveEditProc()
        {
            if (EditModel == null) return;
            if (!EditModel.HasChanges) return;

            try
            {
                EditModel.ModelCopy.Balance = GetNewBalance();
                _Repository.Enrollment.Update(EditModel.ModelCopy);

                //replace the model with the edited copy
                Model = EditModel.ModelCopy;

                isEditing = false;
            }
            catch (Exception e)
            {

                MessageBox.Show("Unable to save. Reason" + e.Message);
            }

            _editWindow.Close();

            var again = ViewModelLocatorStatic.Locator.SessionModule.SelectedSession;
            ViewModelLocatorStatic.Locator.SessionModule.SelectedSession = null;
            ViewModelLocatorStatic.Locator.SessionModule.SelectedSession = again;

        }

        private string GetNewBalance()
        {
            int zero = 0;
            var oldBal = EditModel.ModelOriginal.Balance;
            var newBal = Convert.ToInt32(oldBal) - Convert.ToInt32(EditModel.ModelCopy.Balance);
            if (newBal < 0)
            {
                return zero.ToString();
            }
            return newBal.ToString();


        }

        private bool SaveEditCondition()
        {
            return (EditModel != null) && EditModel.HasChanges && !EditModel.HasErrors;
        }

        public ICommand CancelEditCommand => new RelayCommand(CancelProc);

        private void CancelProc()
        {
            isEditing = false;
            EditModel?.Dispose();
            _editWindow.Close();
        }
    }
}
