﻿using System;
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
    public partial class PhotoModeWindow : Window
    {
        private const int MAXS = 1000;
        private int StudentCount = 0, CurrentStudent = 0;
        private List<string> StudentPhotoList = new List<string>();
        private bool StuPhotoConfirmed = false;

        public PhotoModeWindow()
        {
            InitializeComponent();
            ModeSelector.SelectedIndex = 2; // Set default mode
            InitAll();
            ClearAndShow();
        }
        // Tools
        private void InitAll()
        {
            StuPhotoConfirmed = false;
            StudentCount = 0;
            CurrentStudent = 0;
            ProgressBar.Value = 0;
        }

        private void Random_Shuffle()
        {
            Random Randotron = new Random();
            for (int i = StudentCount - 1; i >= 0; i--)
            {
                int CurrentStudentIndex = Randotron.Next(i + 1);
                string TempValue = StudentPhotoList[CurrentStudentIndex];
                StudentPhotoList[CurrentStudentIndex] = StudentPhotoList[i];
                StudentPhotoList[i] = TempValue;
            }
        }

        private void ClearAndShow()
        {
            Random_Shuffle();
            CurrentStudent = 0;
            ProgressBar.Value = 0;
        }

        private void UpdatePhoto(int Cur)
        {
            BitmapImage ImageP = new BitmapImage();
            BitmapImage ImageC = new BitmapImage();
            BitmapImage ImageN = new BitmapImage();
            if (Cur > 0)
            {
                ImageP.BeginInit();
                ImageP.UriSource = new Uri(StudentPhotoList[Cur - 1]);
                ImageP.DecodePixelWidth = 720;
                ImageP.EndInit();
                PreImage.Source = ImageP;
            }
            else PreImage.Source = null;
            PreImage.Opacity = 0.5;
            ImageC.BeginInit();
            ImageC.UriSource = new Uri(StudentPhotoList[Cur]);
            ImageC.DecodePixelWidth = 720;
            ImageC.EndInit();
            CurImage.Source = ImageC;
            if (Cur + 1 < StudentCount)
            {
                ImageN.BeginInit();
                ImageN.UriSource = new Uri(StudentPhotoList[Cur + 1]);
                ImageN.DecodePixelWidth = 720;
                ImageN.EndInit();
                NxtImage.Source = ImageN;
            }
            else NxtImage.Source = null;
            NxtImage.Opacity = 0.5;
        }

        private void PopUpBarConfirmed(object sender, RoutedEventArgs e) { PopUpBar.IsActive = false; FocusBox.Focus(); }
        public void PopUp(string Message)
        {
            PopUpBarMessage.Content = Message;
            PopUpBar.IsActive = true;
        }

        private void ModeSelectorChange(object sender, RoutedEventArgs e)
        {
            if (ModeSelector.SelectedIndex == 2) return;
            if (ModeSelector.SelectedIndex == 0)
            {
                StuModeWindow Smw = new StuModeWindow();
                Smw.Show();
                this.Close();
            }
            if (ModeSelector.SelectedIndex == 1)
            {
                FileModeWindow Fmw = new FileModeWindow();
                Fmw.Show();
                this.Close();
            }
        }

        private bool GetComfirmStatus() { return StuPhotoConfirmed; }
        private void RolltoLeft()
        {
            bool HasInited = false;
            if (!GetComfirmStatus()) { PopUp(RandotronWPF.Properties.Resources.InvalidPhotoOperation); return; }
            CurrentStudent -= 1;
            if (CurrentStudent == -1) { PopUp(RandotronWPF.Properties.Resources.AtTheEnd); ClearAndShow(); HasInited = true; }
            UpdatePhoto(CurrentStudent);
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
            if (!GetComfirmStatus()) { PopUp(RandotronWPF.Properties.Resources.InvalidPhotoOperation); return; }
            if (!isFirstTrial) CurrentStudent += 1;
            if (CurrentStudent >= StudentCount) { ClearAndShow(); PopUp(RandotronWPF.Properties.Resources.AtTheEnd); }
            UpdatePhoto(CurrentStudent);
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

        private void ProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void FileModeConfirmed(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog FileDialog = new Microsoft.Win32.OpenFileDialog();
            FileDialog.Filter = RandotronWPF.Properties.Resources.PhotoModeFilter;
            FileDialog.Multiselect = true;
            if (FileDialog.ShowDialog() == true)
            {
                StudentPhotoList.Clear();
                string[] files = null;
                files = FileDialog.FileNames;
                StudentCount = 0;
                for(int i = 0; i < files.Length; ++i)
                {
                    StudentCount++;
                    string StuPhoto = files[i];
                    StudentPhotoList.Add(StuPhoto);
                }
                ClearAndShow();
                UpdatePhoto(CurrentStudent);
                StuPhotoConfirmed = true;
                RolltoRight(true);
                FocusBox.Focus();
            }
        }
    }
}
