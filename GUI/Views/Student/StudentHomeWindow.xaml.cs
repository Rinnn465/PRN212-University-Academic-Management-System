using System.Windows;
using BUS.DTOs;
using BUS.Services;
using DAL.Repositories;
using GUI.Views.Authentication;

namespace GUI.Views.Student;

public partial class StudentHomeWindow : Window
{
    private readonly AuthenticatedUserDto _currentUser;
    private readonly UnitOfWork _unitOfWork;
    private readonly StudentService _studentService;
    public StudentHomeWindow(AuthenticatedUserDto user, UnitOfWork unitOfWork)
    {
        InitializeComponent();
        _currentUser = user;
        _unitOfWork = unitOfWork;

        // Kh?i t?o StudentService b?ng cách truy?n UnitOfWork có k?t n?i DB chu?n vào
        _studentService = new StudentService(_unitOfWork);

        WelcomeTextBlock.Text = $"Welcome, {user.FullName}";
    }

    private void BtnProfile_Click(object sender, RoutedEventArgs e)
    {
        // Truy?n Service dùng chung ?ã có k?t n?i DB sang cho c?a s? con
        var profileWindow = new ProfileWindow(_currentUser, _studentService);
        profileWindow.Owner = this;
        profileWindow.ShowDialog();
    }


    private void BtnGrades_Click(object sender, RoutedEventArgs e)
    {
        int studentId = _currentUser.StudentId ?? 0;

        GradeViewWindow gradeWin = new GradeViewWindow(studentId, _studentService);
        gradeWin.Owner = this;
        gradeWin.ShowDialog();
    }

    private void BtnLogout_Click(object sender, RoutedEventArgs e)
    {
        LoginWindow loginWin = new LoginWindow();
        loginWin.Show();

        this.Close();
    }

    private void BtnSubjectList_Click(object sender, RoutedEventArgs e)
    {
        int studentId = _currentUser.StudentId ?? 0;

        SubjectRegistrationWindow subjectWin = new SubjectRegistrationWindow(studentId, _studentService);
        subjectWin.Owner = this;
        subjectWin.ShowDialog();
    }
}

