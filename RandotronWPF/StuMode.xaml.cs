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
    public partial class StuModeWindow : Window
    {
        private const int MAXS = 1000;
        private int StudentCount = 0, CurrentStudent = 0;
        private int[] StudentNumList = new int[MAXS];
        private bool StuNumConfirmed = false;
        
        public StuModeWindow()
        {
            InitializeComponent();
            ModeSelector.SelectedIndex = 0; // Set Student Number Mode
            InitAll();
            ClearAndShow();
        }

        // Tools
        private void InitAll()
        {
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            StuNumConfirmed = false;
            StudentCount = 0;
            CurrentStudent = 0;
            NumberTextBox.Text = "";
            PreText.Content = NxtText.Content = CurText.Content = "";
            ProgressBar.Value = 0;
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
        public void PopUp(string Message)
        {
            PopUpBarMessage.Content = Message;
            PopUpBar.IsActive = true;
        }
        
        private void ModeSelectorChange(object sender, RoutedEventArgs e)
        {
            if (ModeSelector.SelectedIndex == 0) return;
            if (ModeSelector.SelectedIndex == 1)
            {
                FileModeWindow Fmw = new FileModeWindow();
                Fmw.Show();
                this.Close();
            }
            if (ModeSelector.SelectedIndex == 2)
            {
                PhotoModeWindow Pmw = new PhotoModeWindow();
                Pmw.Show();
                this.Close();
            }
        }

        private bool GetComfirmStatus() { return StuNumConfirmed; }
        private void RolltoLeft()
        {
            bool HasInited = false;
            if (!GetComfirmStatus()) { PopUp(RandotronWPF.Properties.Resources.InvalidOperation); return; }
            CurrentStudent -= 1;
            if (CurrentStudent == -1) { PopUp(RandotronWPF.Properties.Resources.AtTheEnd); ClearAndShow(); HasInited = true; }
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
            if (!GetComfirmStatus()) { PopUp(RandotronWPF.Properties.Resources.InvalidOperation); return; }
            if (!isFirstTrial) CurrentStudent += 1;
            if (CurrentStudent >= StudentCount) { ClearAndShow(); PopUp(RandotronWPF.Properties.Resources.AtTheEnd); }
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
        private void NumberModeConfirmed(object sender, RoutedEventArgs e)
        {
            int Tnumber = GetStudentNumber(NumberTextBox);
            if (Tnumber <= 0) { PopUp(RandotronWPF.Properties.Resources.InvalidNumber); return; }
            if (Tnumber > MAXS) { PopUp(RandotronWPF.Properties.Resources.HugeNumber); return; }
            NumberTextBox.Text = Tnumber.ToString();
            StudentCount = Tnumber;
            ClearAndShow();
            UpdateText(CurrentStudent);
            StuNumConfirmed = true;
            RolltoRight(true);
            FocusBox.Focus();
        }
    }
}
