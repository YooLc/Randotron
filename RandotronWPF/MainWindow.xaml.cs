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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;

namespace RandotronWPF
{
    public partial class MainWindow : Window
    {
        private const int MAXS = 1000;
        private int StudentCount = 0, CurrentStudent = 0;
        private int[] StudentNumList = new int[MAXS];
        private bool StuNumConfirmed = false;
        
        public MainWindow()
        {
            InitializeComponent();
            ModeSelector.SelectedIndex = 0; // Set default mode
            InitAll();
            EnableMode();
            ClearAndShow();
        }
        // Tools
        private void InitAll()
        {
            StuNumConfirmed = false;
            StudentCount = 0;
            StuNumPanel.Visibility = Visibility.Hidden;
            CurrentStudent = 0;
            NumberTextBox.Text = "";
            PreText.Content = NxtText.Content = CurText.Content = "";
            ProgressBar.Value = 0;
        }

        private void EnableMode()
        {
            if (ModeSelector.SelectedIndex == 0) StuNumPanel.Visibility = Visibility.Visible;
            if (ModeSelector.SelectedIndex != 1) FileBtn.Visibility = Visibility.Hidden;
            else FileBtn.Visibility = Visibility.Visible;
        }

        private void Random_Shuffle()
        {
            Random Randotron = new Random();
            for (int i = StudentCount - 1; i >= 0; i--)
            {
                int CurrentStudentIndex = Randotron.Next(i + 1);
                int TempValue = StudentNumList[CurrentStudentIndex];
                StudentNumList[CurrentStudentIndex] = StudentNumList[i];
                StudentNumList[i] = TempValue;
            }
        }

        private void ClearAndShow()
        {
            for (int i = 0; i < MAXS; ++i) StudentNumList[i] = i + 1;
            Random_Shuffle();
            CurrentStudent = 0;
            ProgressBar.Value = 0;
        }

        private void UpdateText(int Cur)
        {
            PreText.Content = Cur > 0 ? StudentNumList[Cur - 1].ToString() : "";
            CurText.Content = StudentNumList[Cur].ToString();
            NxtText.Content = Cur + 1 < StudentCount ? StudentNumList[Cur + 1].ToString() : "";
        }

        private int GetStudentNumber(TextBox TextBox)
        {
            string content = TextBox.Text;
            int t = 0;
            for (int i = 0; i < content.Length; ++i)
            {
                if (content[i] >= '0' && content[i] <= '9')
                    t = (t * 10) + content[i] - '0';
                if (t > MAXS) return t;
            }
            return t;
        }

        private void PopUpBarConfirmed(object sender, RoutedEventArgs e) { PopUpBar.IsActive = false; FocusBox.Focus(); }
        private void PopUp(string Message)
        {
            PopUpBarMessage.Content = Message;
            PopUpBar.IsActive = true;
        }
        
        private void ModeSelectorChange(object sender, RoutedEventArgs e)
        {
            InitAll();
            EnableMode();
        }

        private bool GetComfirmStatus() { return StuNumConfirmed; }
        private void RolltoLeft()
        {
            if (!GetComfirmStatus()) { PopUp("无效操作"); return; }
            CurrentStudent -= 1;
            if (CurrentStudent == -1) { PopUp("到头了 已重新生成"); ClearAndShow(); }
            UpdateText(CurrentStudent);
            double goal = StudentCount > 0 ? (double)(CurrentStudent + 1) / StudentCount * 100 : 0;
            ProgressBar.Value = goal;
        }

        private void RolltoRight()
        {
            if (!GetComfirmStatus()) { PopUp("无效操作"); return; }
            CurrentStudent += 1;
            if (CurrentStudent >= StudentCount) { PopUp("到头了 已重新生成"); ClearAndShow(); }
            UpdateText(CurrentStudent);
            double goal = StudentCount > 0 ? (double)(CurrentStudent + 1) / StudentCount * 100 : 0;
            ProgressBar.Value = goal;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) RolltoRight();
        }

        // Functions
        private void PreBtnClicked(object sender, RoutedEventArgs e) { RolltoLeft(); FocusBox.Focus(); }
        private void NxtBtnClicked(object sender, RoutedEventArgs e) { RolltoRight(); FocusBox.Focus(); }
        private void NumberModeConfirmed(object sender, RoutedEventArgs e)
        {
            int Tnumber = GetStudentNumber(NumberTextBox);
            if (Tnumber <= 0) { PopUp("请输入正确的学生人数，范围为[1, 1000]"); return; }
            if (Tnumber > MAXS) { PopUp("最多仅允许 1000 个学生"); return; }
            NumberTextBox.Text = Tnumber.ToString();
            StudentCount = Tnumber;
            ClearAndShow();
            UpdateText(CurrentStudent);
            StuNumConfirmed = true;
            FocusBox.Focus();
        }

        private void FileModeConfirmed(object sender, RoutedEventArgs e)
        {
            // PopUp("哦哦哦");
            Microsoft.Win32.OpenFileDialog FileDialog = new Microsoft.Win32.OpenFileDialog();
            FileDialog.Filter = "Excel 表格 (*.xlsx , *.xls) | *.xlsx; *.xls";
            if (FileDialog.ShowDialog() == true)
            {
                // DataTable dt = GetData("E:\\test.xls");
                PopUp(FileDialog.FileName); 
            }
        }
    }
}
