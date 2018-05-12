using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Nito.AsyncEx;
using TemporaryEmploymentCorp.DataAccess;
using TemporaryEmploymentCorp.Helpers.ReportViewer;
using TemporaryEmploymentCorp.Models.Course;
using TemporaryEmploymentCorp.Models.Prerequisites;
using TemporaryEmploymentCorp.Reports.Course;
using TemporaryEmploymentCorp.Views.Course;
using TemporaryEmploymentCorp.Views.Course.Prerequisites;

namespace TemporaryEmploymentCorp.Modules
{
    public class CourseModule : ObservableObject
    {
        public AddCourseWindow _addCourseWindow;

        public AddPrereqWindow _AddPrereqWindow;
        private NewCourseModel _newCourse;
        private NewPrereqModel _newPrereq;
        private readonly IRepository _repository;
        private CourseModel _selectedCourse;
        private PrereqModel _selectedPrereq;


        public CourseModule(IRepository repository)
        {
            _repository = repository;

            CourseLoading = NotifyTaskCompletion.Create(() => LoadCoursesAsync());
        }

        public ObservableCollection<CourseModel> Courses { get; } = new ObservableCollection<CourseModel>();

        public INotifyTaskCompletion CourseLoading { get; set; }


        public CourseModel SelectedCourse
        {
            get { return _selectedCourse; }
            set
            {
                _selectedCourse?.CancelEditCommand.Execute(null);
                _selectedCourse = value;
                if (value != null)
                {
                    _selectedCourse.LoadRelatedInfo();
                    //LoadQualificationDisplay();
                }
                RaisePropertyChanged(nameof(SelectedCourse));
            }
        }


        public NewCourseModel NewCourse
        {
            get { return _newCourse; }
            set
            {
                _newCourse = value;
                RaisePropertyChanged(nameof(NewCourse));
            }
        }


        public ICommand AddCourseCommand => new RelayCommand(AddCourseProc);
        public ICommand CancelCourseCommand => new RelayCommand(CancelCourseProc);
        public ICommand SaveCourseCommand => new RelayCommand(SaveCourseProc, SaveCourseCondition);

        public PrereqModel SelectedPrereq
        {
            get { return _selectedPrereq; }
            set
            {
                _selectedPrereq = value;
                RaisePropertyChanged(nameof(SelectedPrereq));
            }
        }

        public NewPrereqModel NewPrereq
        {
            get { return _newPrereq; }
            set
            {
                _newPrereq = value;
                LoadPrerequisitesList();
                RaisePropertyChanged(nameof(NewPrereq));
            }
        }

        public ObservableCollection<PrereqSelection> PrereqList { get; } =
            new ObservableCollection<PrereqSelection>();


        public void LoadPrerequisitesList()
        {
            PrereqList.Clear();
            var quals = _repository.Qualification.GetRange().Select(c => new PrereqSelection(c));
                //convert qualifications to PrereqSelection type.
            foreach (var prereqSelection in quals)
            {
                PrereqList.Add(prereqSelection);
            }

            foreach (var item in SelectedCourse.Prerequisites)
            {
                var toRemove = PrereqList.FirstOrDefault(c => c.Model.QualificationId == item.Model.QualificationId);
                PrereqList.Remove(toRemove);
            }
        }

        public ICommand DeleteCourseCommand => new RelayCommand(DeleteCourseProc, DeleteCourseCondition);

        private void DeleteCourseProc()
        {
            var result = MessageBox.Show("Are you sure you want to delete selected course?", "Course", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    NotifyTaskCompletion.Create(DeleteCourseAsync);
                }
                catch (Exception)
                {
                    MessageBox.Show("Unable to delete courses.");
                    throw;
                }
            }
        }

        private async Task DeleteCourseAsync()
        {
            await Task.Run(() => _repository.Course.RemoveAsync(SelectedCourse.Model));
            Courses.Remove(SelectedCourse);
        }

        private bool DeleteCourseCondition()
        {
            if (SelectedCourse == null) return false;
            return true;
        }

        public ICommand AddPrereqCommand => new RelayCommand(AddPrereqProc);
        public ICommand CancelPrereqCommand => new RelayCommand(CancelPrereqProc);

        private void CancelPrereqProc()
        {
            NewPrereq?.Dispose();
            _AddPrereqWindow.Close();
        }

        public ICommand SavePrereqCommand => new RelayCommand(SavePrereqProc, SavePrereqCondition);
        public ICommand RemovePrereqCommand => new RelayCommand(RemovePrereqProc, RemovePrereqCondition);

        private void RemovePrereqProc()
        {
            var result = MessageBox.Show("Are you sure you want to remove selected prerquisite?", "Prerequisite",
                MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                NotifyTaskCompletion.Create(RemovePrereqAsync);
            }
        }

        private async Task RemovePrereqAsync()
        {
            await _repository.Prerequisite.RemoveAsync(SelectedPrereq.Model);
            SelectedCourse.Prerequisites.Remove(SelectedPrereq);
        }

        private bool RemovePrereqCondition()
        {
            if (SelectedPrereq == null) return false;
            return true;
        }

        private bool SavePrereqCondition()
        {
            return true;
        }

        private void SavePrereqProc()
        {
            if (NewPrereq == null) return;

            var selectedQuals = PrereqList.Where(c => c.IsSelected).ToList();
            foreach (var item in selectedQuals)
            {
                try
                {
                    NewPrereq.ModelCopy.CourseId = SelectedCourse.Model.CourseId;
                    NewPrereq.ModelCopy.QualificationId = item.Model.QualificationId;

                    _selectedCourse.Prerequisites.Add(new PrereqModel(NewPrereq.ModelCopy, _repository));
                    _repository.Prerequisite.Add(NewPrereq.ModelCopy);
                }
                catch (Exception e)
                {
                    MessageBox.Show("Unable to add. Reason: " + e.Message);
                }
            }

            SelectedCourse = SelectedCourse;
            _AddPrereqWindow.Close();
        }

        private async Task LoadCoursesAsync()
        {
            var courses = await Task.Run(() => _repository.Course.GetRangeAsync());

            foreach (var course in courses)
            {
                var courseModel = new CourseModel(course, _repository);
                courseModel.LoadRelatedInfo();
                Courses.Add(courseModel);
                await Task.Delay(10);
            }
        }

        private bool SaveCourseCondition()
        {
            return (NewCourse != null) && NewCourse.HasChanges && !NewCourse.HasErrors;
        }

        private void SaveCourseProc()
        {
            if (NewCourse == null) return;
            if (!NewCourse.HasChanges) return;
            NotifyTaskCompletion.Create(SaveAddCourseAsync);
        }

        private async Task SaveAddCourseAsync()
        {
            try
            {
                await Task.Run(() => _repository.Course.AddAsync(NewCourse.ModelCopy));
                Courses.Add(new CourseModel(NewCourse.ModelCopy, _repository));
                _addCourseWindow.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show("Unable to save. Reason: " + e.Message);
            }
        }

        private void CancelCourseProc()
        {
            NewCourse?.Dispose();
            _addCourseWindow.Close();
        }

        private void AddCourseProc()
        {
            NewCourse = new NewCourseModel();
            _addCourseWindow = new AddCourseWindow();
            _addCourseWindow.Owner = Application.Current.MainWindow;
            _addCourseWindow.ShowDialog();
        }

        private void AddPrereqProc()
        {
            NewPrereq = new NewPrereqModel();
            _AddPrereqWindow = new AddPrereqWindow();
            _AddPrereqWindow.Owner = Application.Current.MainWindow;
            _AddPrereqWindow.ShowDialog();
        }

        public ICommand PrintCoursesCommand => new RelayCommand(PrintCoursesProc);

        private SingleInstanceWindowViewer<AllCourseReportWindow> _allCoursesWindow =
            new SingleInstanceWindowViewer<AllCourseReportWindow>();

        private void PrintCoursesProc()
        {
            _allCoursesWindow.Show();
        }

        private string _searchCourse;

        public string SearchCourse
        {
            get { return _searchCourse; }
            set
            {
                _searchCourse = value;
                RaisePropertyChanged(nameof(SearchCourse));
                var CourseList = CollectionViewSource.GetDefaultView(Courses);
                if (string.IsNullOrWhiteSpace(SearchCourse))
                {
                    CourseList.Filter = null;
                }
                else
                {
                    CourseList.Filter = obj =>
                    {
                        var courseModel = obj as CourseModel;
                        var sc = SearchCourse.ToLowerInvariant();
                        if (courseModel == null) return false;
                        return courseModel.Model.CourseName.ToLowerInvariant().Contains(sc) ||
                               courseModel.Model.CourseId.ToLowerInvariant().Contains(sc);
                    };
                }
            }
        }
    }
}