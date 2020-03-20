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
            StuNumPanel.Visibility = Visibility.Hidden;
            CurrentStudent = 0;
            ProgressBar.Value = 0;
        }

        private void EnableMode()
        {
            if (ModeSelector.SelectedIndex == 0) StuNumPanel.Visibility = Visibility.Visible;
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

        private int GetStudentNumber(TextBox textBox)
        {
            string content = textBox.Text;
            int t = 0;
            for (int i = 0; i < content.Length; ++i)
            {
                if (content[i] >= '0' && content[i] <= '9')
                    t = (t * 10) + content[i] - '0';
                if (t > MAXS) return t;
            }
            return t;
        }

        private void PopUpBarConfirmed(object sender, RoutedEventArgs e) { PopUpBar.IsActive = false; }
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

        // Functions
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

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (ModeSelector.SelectedIndex == 0 && StuNumConfirmed == false) return;
            if (ModeSelector.SelectedIndex != 0) { PopUp("目前仅支持学号模式"); return;}
            if (e.Key == Key.Enter)
            {
                CurrentStudent += 1;
                if (CurrentStudent == StudentCount) ClearAndShow();
                UpdateText(CurrentStudent);
                double goal = StudentCount > 0 ? (double)(CurrentStudent + 1) / StudentCount * 100 : 0;
                ProgressBar.Value = goal;
            }
        }
    }
}
