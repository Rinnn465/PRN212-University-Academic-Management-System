using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BUS.Services;

namespace GUI.Views.Student
{
    /// <summary>
    /// Interaction logic for GradeViewWindow.xaml
    /// </summary>
    public partial class GradeViewWindow : Window
    {
        private readonly StudentService _studentService;
        private readonly int _studentId;

        public GradeViewWindow(int studentId, StudentService studentService)
        {
            InitializeComponent();
            _studentId = studentId;
            _studentService = studentService;

            _ = LoadGrades();
        }

        private async Task LoadGrades()
        {
            try
            {
                var enrollments = await _studentService.GetStudentGrades(_studentId);

                if (enrollments != null)
                {
                    foreach (var item in enrollments)
                    {
                        if (item.Semester == "Spring 2026")
                        {
                            item.Semester = "SP26";
                        }
                    }

                    dgGrades.ItemsSource = enrollments;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải điểm: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
