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
using System.IO;

namespace RandotronWPF
{
    public partial class FileModeWindow : Window
    {
        private const int MAXS = 1000;
        private int StudentCount = 0, CurrentStudent = 0;
        private List<string> StudentNameList = new List<string>();
        private bool StuNameConfirmed = false;

        public FileModeWindow()
        {
            InitializeComponent();
            ModeSelector.SelectedIndex = 1; // Set default mode
            InitAll();
            ClearAndShow();
        }
        // Tools
        private void InitAll()
        {
            StuNameConfirmed = false;
            StudentCount = 0;
            CurrentStudent = 0;
            PreText.Content = NxtText.Content = CurText.Content = "";
            ProgressBar.Value = 0;
        }

        private void Random_Shuffle()
        {
            Random Randotron = new Random();
            for (int i = StudentCount - 1; i >= 0; i--)
            {
                int CurrentStudentIndex = Randotron.Next(i + 1);
                string TempValue = StudentNameList[CurrentStudentIndex];
                StudentNameList[CurrentStudentIndex] = StudentNameList[i];
                StudentNameList[i] = TempValue;
            }
        }

        private void ClearAndShow()
        {
            Random_Shuffle();
            CurrentStudent = 0;
            ProgressBar.Value = 0;
        }

        private void UpdateText(int Cur)
        {
            PreText.Content = Cur > 0 ? StudentNameList[Cur - 1] : "";
            CurText.Content = StudentNameList[Cur];
            NxtText.Content = Cur + 1 < StudentCount ? StudentNameList[Cur + 1] : "";
        }

        private void PopUpBarConfirmed(object sender, RoutedEventArgs e) { PopUpBar.IsActive = false; FocusBox.Focus(); }
        public void PopUp(string Message)
        {
            PopUpBarMessage.Content = Message;
            PopUpBar.IsActive = true;
        }

        private void ModeSelectorChange(object sender, RoutedEventArgs e)
        {
            if (ModeSelector.SelectedIndex == 1) return;
            if (ModeSelector.SelectedIndex == 0)
            {
                StuModeWindow Smw = new StuModeWindow();
                Smw.Show();
                this.Close();
            }
        }

        private bool GetComfirmStatus() { return StuNameConfirmed; }
        private void RolltoLeft()
        {
            bool HasInited = false;
            if (!GetComfirmStatus()) { PopUp("请选择文件，格式为：一行一个人名。"); return; }
            CurrentStudent -= 1;
            if (CurrentStudent == -1) { PopUp("到头了 已重新生成"); ClearAndShow(); HasInited = true; }
            UpdateText(CurrentStudent);
            double prev = StudentCount > 0 ? (double)(CurrentStudent + 2) / StudentCount * 100 : 0;
            double goal = StudentCount > 0 ? (double)(CurrentStudent + 1) / StudentCount * 100 : 0;
            if (HasInited) { ProgressBar.Value = goal; return; }
            Thread PBthread = new Thread(new ThreadStart(() =>
            {
                for (int i = (int)System.Math.Ceiling(prev); i >= goal; --i)
                {
                    this.ProgressBar.Dispatcher.BeginInvoke((ThreadStart)delegate { this.ProgressBar.Value = i; });
                    Thread.Sleep(15);
                }
                this.ProgressBar.Dispatcher.BeginInvoke((ThreadStart)delegate { this.ProgressBar.Value = goal; });
            }));
            PBthread.Start();
        }

        private void RolltoRight(bool isFirstTrial)
        {
            if (!GetComfirmStatus()) { PopUp("请选择文件，格式为：一行一个人名。"); return; }
            if (!isFirstTrial) CurrentStudent += 1;
            if (CurrentStudent >= StudentCount) { ClearAndShow(); PopUp("到头了 已重新生成"); }
            UpdateText(CurrentStudent);
            double prev = StudentCount - 1 > 0 ? (double)(CurrentStudent) / StudentCount * 100 : 0;
            double goal = StudentCount > 0 ? (double)(CurrentStudent + 1) / StudentCount * 100 : 0;
            Thread PBthread = new Thread(new ThreadStart(() =>
            {
                for (int i = (int)System.Math.Ceiling(prev); i <= goal; ++i)
                {
                    this.ProgressBar.Dispatcher.BeginInvoke((ThreadStart)delegate { this.ProgressBar.Value = i; });
                    Thread.Sleep(15);
                }
                this.ProgressBar.Dispatcher.BeginInvoke((ThreadStart)delegate { this.ProgressBar.Value = goal; });
            }));
            PBthread.Start();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) RolltoRight(false);
        }

        // Functions
        private void PreBtnClicked(object sender, RoutedEventArgs e) { RolltoLeft(); FocusBox.Focus(); }
        private void NxtBtnClicked(object sender, RoutedEventArgs e) { RolltoRight(false); FocusBox.Focus(); }
        
        private void FileModeConfirmed(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog FileDialog = new Microsoft.Win32.OpenFileDialog();
            FileDialog.Filter = "文本文档 (*.txt) | *.txt";
            FileStream fs;
            if (FileDialog.ShowDialog() == true)
            {
                fs = new FileStream(FileDialog.FileName, System.IO.FileMode.Open, FileAccess.Read);
                using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
                {
                    StudentCount = 0;
                    while(sr.Peek() > 0)
                    {
                        StudentCount++;
                        string StuName = sr.ReadLine();
                        StudentNameList.Add(StuName);
                    }
                }
                ClearAndShow();
                UpdateText(CurrentStudent);
                StuNameConfirmed = true;
                RolltoRight(true);
                FocusBox.Focus();
            }
        }
    }
}
