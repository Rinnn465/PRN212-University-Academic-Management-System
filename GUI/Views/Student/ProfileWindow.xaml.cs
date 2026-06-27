using System;
using System.Windows;
using BUS.DTOs;
using BUS.Services;

namespace GUI.Views.Student
{
    public partial class ProfileWindow : Window
    {
        private readonly AuthenticatedUserDto _currentUser;
        private readonly StudentService _studentService;

        // Hàm tạo chỉ cần nhận vào duy nhất thông tin user đăng nhập
        public ProfileWindow(AuthenticatedUserDto user, StudentService studentService)
        {
            InitializeComponent();
            _currentUser = user;
            _studentService = studentService; // Nhận service từ HomeWindow

            LoadProfileData();
        }

        private async void LoadProfileData()
        {
            try
            {
                int studentId = _currentUser.StudentId ?? 0;

                if (studentId == 0)
                {
                    MessageBox.Show($"Tài khoản {_currentUser.Username} không liên kết với mã sinh viên nào!",
                                    "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Gọi xuống tầng BUS để lấy thông tin chi tiết sinh viên
                var studentInfo = await _studentService.GetStudentById(studentId);

                if (studentInfo != null)
                {
                    // ĐÈ DỮ LIỆU LÊN GIAO DIỆN
                    // Hãy chắc chắn txtStudentCode, txtFullName... khớp với x:Name trong file XAML của bạn
                    txtStudentCode.Text = studentInfo.StudentCode ?? "";
                    txtFullName.Text = studentInfo.FullName ?? "";
                    txtDateOfBirth.Text = studentInfo.DateOfBirth?.ToString("dd/MM/yyyy") ?? "";
                    txtEmail.Text = studentInfo.Email ?? "";
                    txtPhone.Text = studentInfo.Phone ?? "";
                }
                else
                {
                    MessageBox.Show("Tìm thấy Id nhưng không có thông tin chi tiết sinh viên này trong DB!",
                                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                // Bẫy lỗi ngầm từ kết nối Database hoặc Entity Framework
                MessageBox.Show($"Lỗi không thể tải dữ liệu Profile từ SQL Server:\n{ex.Message}\n{ex.InnerException?.Message}",
                                "Lỗi Hệ Thống", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}