using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using Nito.AsyncEx;
using TemporaryEmploymentCorp.DataAccess;
using TemporaryEmploymentCorp.Models.Course;
using TemporaryEmploymentCorp.Models.Editable;

namespace TemporaryEmploymentCorp.Models.Qualification
{
    public class QualificationModel:ModelBase<DataAccess.EF.Qualification>,IEditableModel<DataAccess.EF.Qualification>
    {
        private bool _isEditing;
        private EditModelBase<DataAccess.EF.Qualification> _editModel;
        public ObservableCollection<CourseModel> CourseDeveloped { get; } = new ObservableCollection<CourseModel>();

        public QualificationModel(DataAccess.EF.Qualification model) : base(model)
        {
        }

        public QualificationModel(DataAccess.EF.Qualification model, IRepository repository) : base(model, repository)
        {
        }
        public INotifyTaskCompletion LoadCourseAsync { get; private set; }
        public void LoadRelatedInfo()
        {
            LoadCourseAsync = NotifyTaskCompletion.Create(LoadCoursesDevelopedAsync);
        }

        private async Task LoadCoursesDevelopedAsync()
        {
            CourseDeveloped.Clear();
            var requiredCourses =
                await _Repository.Course.GetRangeAsync(c => c.QualificationId == Model.QualificationId);

            foreach (var item in requiredCourses)
            {
                CourseDeveloped.Add(new CourseModel(item));
            }
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

        public EditModelBase<DataAccess.EF.Qualification> EditModel
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
            EditModel = new QualificationEditModel(Model);
        }

        public ICommand SaveEditCommand => new RelayCommand(SaveProc, SaveCondition);

        private bool SaveCondition()
        {
            return (EditModel != null) && EditModel.HasChanges && !EditModel.HasErrors;
        }

        private void SaveProc()
        {
            

            NotifyTaskCompletion.Create(SaveEditAsync);
        }

        private async Task SaveEditAsync()
        {
            if (EditModel == null) return;
            if (!EditModel.HasChanges) return;

            try
            {
                await _Repository.Qualification.UpdateAsync(EditModel.ModelCopy);

                //replace the model with the edited copy
                Model = EditModel.ModelCopy;

                isEditing = false;
            }
            catch (Exception e)
            {

                MessageBox.Show("Unable to save. Reason" + e.Message);
            }
        }


        public ICommand CancelEditCommand => new RelayCommand(CancelProc);

        private void CancelProc()
        {
            isEditing = false;
            EditModel?.Dispose();
            
        }
    }
}
