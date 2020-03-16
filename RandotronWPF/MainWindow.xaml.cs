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
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        const int MAXS = 1000;
        private int totStudent = 0, current = 0;
        private int[] studentNumList = new int[MAXS];
        public MainWindow()
        {
            InitializeComponent();
            clearAndShow();
        }
        public int GetStudentNumber(TextBox textBox)
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
        private void SnackbarConfirmed(object sender, RoutedEventArgs e)
        {
            SnackbarTwo.IsActive = false;
        }
        private void popUp(string Message)
        {
            SnackbarTwoMessage.Content = Message;
            SnackbarTwo.IsActive = true;
        }
        private void numberModeConfirmed(object sender, RoutedEventArgs e)
        {
            int number = GetStudentNumber(numberTextBox);
            if (number <= 0) { popUp("请输入正确的学生人数，范围为[1, 1000]"); return; }
            if (number > MAXS) { popUp("最多仅允许 1000 个学生"); return; }
            numberTextBox.Text = number.ToString();
            totStudent = number;
            clearAndShow();
            preText.Content = current > 0 ? studentNumList[current - 1].ToString() : "";
            curText.Content = studentNumList[current].ToString();
            nxtText.Content = current + 1 < totStudent ? studentNumList[current + 1].ToString() : "";
            virtualTextBox.Focus();
        }
        private void random_Shuffle()
        {
            int currentIndex;
            int tempValue;
            Random r = new Random();
            for (int i = totStudent - 1; i >= 0; i--)
            {
                currentIndex = r.Next(i + 1);
                tempValue = studentNumList[currentIndex];
                studentNumList[currentIndex] = studentNumList[i];
                studentNumList[i] = tempValue;
            }
        }
        private void clearAndShow()
        {
            ModeSelect.SelectedIndex = 0;
            for (int i = 0; i < MAXS; ++i) studentNumList[i] = i + 1;
            random_Shuffle();
            current = 0;
            ProgressBar.Value = 0;
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                current += 1;
                if (current == totStudent) clearAndShow();
                preText.Content = current > 0 ? studentNumList[current - 1].ToString() : "";
                curText.Content = studentNumList[current].ToString();
                nxtText.Content = current + 1 < totStudent ? studentNumList[current + 1].ToString() : "";
                double goal = totStudent > 0 ? (double)(current + 1) / totStudent * 100 : 0;
                ProgressBar.Value = goal;
            }
        }
    }
}
