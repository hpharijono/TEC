using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using Nito.AsyncEx;
using TemporaryEmploymentCorp.DataAccess;
using TemporaryEmploymentCorp.Helpers.ReportViewer;
using TemporaryEmploymentCorp.Models.Editable;
using TemporaryEmploymentCorp.Models.Prerequisites;
using TemporaryEmploymentCorp.Reports.Course;

namespace TemporaryEmploymentCorp.Models.Course
{
    public class CourseModel:ModelBase<DataAccess.EF.Course>, IEditableModel<DataAccess.EF.Course>
    {
        public ObservableCollection<PrereqModel> Prerequisites { get; } = new ObservableCollection<PrereqModel>(); 
        private bool _isEditing;
        private EditModelBase<DataAccess.EF.Course> _editModel;
        private DataAccess.EF.Qualification _associatedQualification;
        public INotifyTaskCompletion PrereqsLoading { get; private set; }
        public CourseModel(DataAccess.EF.Course model) : base(model)
        {
        }

        public CourseModel(DataAccess.EF.Course model, IRepository repository) : base(model, repository)
        {
        }

        public void LoadRelatedInfo()
        {
            //PrereqsLoading = NotifyTaskCompletion.Create(LoadPrereqsAsync);
            //await Task.Run(() => LoadPrereqsAsync());
            PrereqsLoading = NotifyTaskCompletion.Create(() => LoadPrereqsAsync());
            NotifyTaskCompletion.Create(() => LoadQualificationDisplayAsync());
        }

        private async Task LoadPrereqsAsync()
        {
            Prerequisites.Clear();
            var prereqs = await Task.Run(() => _Repository.Prerequisite.GetRange(c => c.CourseId == Model.CourseId));
            foreach (var prerequisite in prereqs)
            {
                //for displaying in list. Gets qualification
                var prereq = await Task.Run(() => _Repository.Qualification.Get(c => c.QualificationId == prerequisite.QualificationId));
                prerequisite.Qualification = prereq;
                //end

                Prerequisites.Add(new PrereqModel(prerequisite, _Repository));
            }
        }

        public DataAccess.EF.Qualification AssociatedQualification
        {
            get { return _associatedQualification; }
            set
            {
                _associatedQualification = value;
                RaisePropertyChanged(nameof(AssociatedQualification));
            }
        }

        private async Task LoadQualificationDisplayAsync()
        {
            var getQualification = await Task.Run(() => _Repository.Qualification.GetAsync(c => c.QualificationId == Model.QualificationId));
            AssociatedQualification = getQualification;
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

        public EditModelBase<DataAccess.EF.Course> EditModel
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
            EditModel = new CourseEditModel(Model);
        }

        public ICommand SaveEditCommand => new RelayCommand(SaveEditProc, SaveEditCondition);

        private bool SaveEditCondition()
        {
            return (EditModel != null) && EditModel.HasChanges && !EditModel.HasErrors;
        }

        private void SaveEditProc()
        {
            if (EditModel == null) return;
            if (!EditModel.HasChanges) return;

            try
            {
                _Repository.Course.Update(EditModel.ModelCopy);

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

        public ICommand PrintSelectedCourse => new RelayCommand(PrintSelectedCourseProc);
        public SingleInstanceWindowViewer<SelectedCourseReportWindow> _selCourseWindow = new SingleInstanceWindowViewer<SelectedCourseReportWindow>();
        private void PrintSelectedCourseProc()
        {
            _selCourseWindow.Show();

        }
    }
}
