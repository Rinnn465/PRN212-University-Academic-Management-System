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
using DAL.Entities;

namespace GUI.Views.Student
{
    /// <summary>
    /// Interaction logic for SubjectRegistrationWindow.xaml
    /// </summary>
    public partial class SubjectRegistrationWindow : Window
    {
        private readonly StudentService _studentService;
        private readonly int _studentId;
        public SubjectRegistrationWindow(int studentId, StudentService studentService)
        {
            InitializeComponent();
            _studentId = studentId;
            _studentService = studentService;

            _ = LoadSubjects();
        }
        private async Task LoadSubjects()
        {
            try
            {
                var subjects = await _studentService.GetAvailableSubjects();
                dgSubjects.ItemsSource = subjects;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải môn học: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private async void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var selectedSubject = button?.DataContext as Subject;

            if (selectedSubject != null)
            {
                string message = await _studentService.RegisterSubject(_studentId, selectedSubject.SubjectId);
                MessageBox.Show(message);
            }
        }
    }
}
