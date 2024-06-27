﻿using DipesLink.Extensions;
using DipesLink.Models;
using DipesLink.ViewModels;
using DipesLink.Views.Extension;
using DipesLink.Views.SubWindows;
using DipesLink.Views.UserControls.MainUc;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using static DipesLink.Views.Enums.ViewEnums;
using Application = System.Windows.Application;

namespace DipesLink.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static event EventHandler<EventArgs>? MainWindowSizeChangeCustomEvent;
        public static SplashScreenLoading? SplashScreenLoading = new();
       
        public static int currentStation = 0;
        public MainWindow()
        {
            InitializeComponent();
            DataContext = MainViewModel.GetIntance();
            EventRegister();
            if (AuthorizationHelper.IsAdmin())
            {
                ComboBoxSelectView.IsEnabled = false;
            }
        }

        private void EventRegister()
        {
            ViewModelSharedEvents.OnJobDetailChange += JobDetails_OnJobDetailChange;
            SizeChanged += MainWindow_SizeChanged;
            Closing += MainWindow_Closing;
            Closed += MainWindow_Closed;
            AllStationUc.DoneLoadUIEvent += AllStationUc_DoneLoadUIEvent;
            Shared.OnActionLoadingSplashScreen += Shared_OnActionLoadingSplashScreen;
            ListBoxMenu.SelectionChanged += ListBoxMenu_SelectionChanged;
        }

        private void JobDetails_OnJobDetailChange(object? sender, int e)
        {
            var CamIP = ViewModelSharedValues.Settings.SystemParamsList[e].CameraIP;
            var PrinterIP = ViewModelSharedValues.Settings.SystemParamsList[e].PrinterIP;
            var ControllerIP = ViewModelSharedValues.Settings.SystemParamsList[e].ControllerIP;
            TextBlockControllerIP.Text = ControllerIP.ToString();
            TextBlockPrinterIP.Text = PrinterIP.ToString();
            TextBlockCamIP.Text = CamIP.ToString();
            currentStation = e;
        }

        private bool CanApplicationClose()
        {
            var viewModel = CurrentViewModel<MainViewModel>();
            if (viewModel == null) return false;  // Ensure viewModel is not null

            // Count the number of running stations
            int runningPrintersCount = viewModel.PrinterStateList.Count(printerState => printerState.State != "Stopped");

            if (runningPrintersCount > 0)
            {
                CusAlert.Show($"Please stop all running stations!", ImageStyleMessageBox.Warning);
                return false;  // Prevent the window from closing
            }

            // Confirm with the user before closing the application
            bool isExit = CusMsgBox.Show("Do you want to exit the application?", "Exit Application", Enums.ViewEnums.ButtonStyleMessageBox.YesNo, Enums.ViewEnums.ImageStyleMessageBox.Warning);
            return isExit;  // Return true if user confirms to exit, else false
        }


        private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = !CanApplicationClose();
        }

        private void ButtonExitApp_Click(object sender, RoutedEventArgs e)
        {
            if (CanApplicationClose())
            {
                Application.Current.Shutdown();  // Or this.Close() if it's from a window class
            }
        }

        private void ListBoxMenu_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
     
            ViewModelSharedEvents.OnListBoxMenuSelectionChangeHandler(ListBoxMenu.SelectedIndex);
            if (ListBoxMenu.SelectedIndex != -1)
                ComboBoxSelectView.SelectedIndex = 0;
            if (sender is ListBox lb)
            {
                var vm = CurrentViewModel<MainViewModel>();
                switch (lb.SelectedIndex)
                {
                    case -1:
                        vm?.ChangeTitleMainWindow(Enums.ViewEnums.TitleAppContext.Overview);
                        break;
                    case 0:
                        vm?.ChangeTitleMainWindow(Enums.ViewEnums.TitleAppContext.Home);
                        break;
                    case 1:
                        vm?.ChangeTitleMainWindow(Enums.ViewEnums.TitleAppContext.Jobs); break;
                    case 2:
                        vm?.ChangeTitleMainWindow(Enums.ViewEnums.TitleAppContext.Setting);
                        break;
                    case 3:
                        vm?.ChangeTitleMainWindow(Enums.ViewEnums.TitleAppContext.Logs);
                        break;
                }
            }
          


        }

        private void MainWindow_Closed(object? sender, EventArgs e)
        {
            Application.Current?.Shutdown();
        }

        private void AllStationUc_DoneLoadUIEvent(object? sender, EventArgs e)
        {
            MainWindowSizeChangeCustomEvent?.Invoke(ActualWidth, EventArgs.Empty);
        }

        private void Shared_OnActionLoadingSplashScreen(object? sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (sender == null) return;
                var actionType = (DataTypes.ActionSplashScreen)sender;

                if (actionType == DataTypes.ActionSplashScreen.Show)
                {
                    SplashScreenLoading ??= new SplashScreenLoading();
                    SplashScreenLoading.ShowDialog();
                }
                else
                {
                    if (SplashScreenLoading != null)
                    {
                        SplashScreenLoading.Close();
                        SplashScreenLoading = null;
                    }
                }
            });
        }

        public void CallbackCommand(Action<MainViewModel> execute)
        {
            try
            {
                if (DataContext is MainViewModel model)
                {
                    execute?.Invoke(model);
                }
                else
                {
                    return;
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        private T? CurrentViewModel<T>() where T : class
        {
            if (DataContext is T viewModel)
            {
                return viewModel;
            }
            else
            {
                return null;
            }
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double actualWidth = ActualWidth;
            //MainViewModel viewModel = (MainViewModel)DataContext;
            //viewModel.ActualWidthMainWindow = actualWidth;
            //CallbackCommand(vm => vm.MinusWidth());
            MainWindowSizeChangeCustomEvent?.Invoke(ActualWidth, EventArgs.Empty);
        }

       

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeviceStatListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var index = DeviceStatListBox.SelectedIndex;
            CallbackCommand(vm => vm.ChangeJobByDeviceStatSymbol(index));
        }

        private void ComboBoxStationNum_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            //CurrentViewModel<MainViewModel>().CheckStationChange();
        }

        private void ComboBoxSelectView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var cbb = (System.Windows.Controls.ComboBox)sender;
            if (cbb.SelectedIndex == 1)
            {
                ListBoxMenu.SelectedIndex = -1;
            }
            else
            {
                ListBoxMenu.SelectedIndex = 0;
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            // thinh 2
            var menuItem = sender as MenuItem;
            if (menuItem != null)
            {
                switch (menuItem.Header)
                {
                    case "Account Management":
                        UsersManagement um = new();
                        um.Show();
                        break;
                    case "About DP-Link":
                        var aboutPopup = new AboutPopup();
                        aboutPopup.ShowDialog();
                        break;
                    case "System Management":
                        var systemManangement = new SystemManangement();
                        systemManangement.ShowDialog();
                        break;
                    case "Logout": //Restart
                        var res = CusMsgBox.Show("Do you want to logout ?", "Logout", Enums.ViewEnums.ButtonStyleMessageBox.OKCancel, Enums.ViewEnums.ImageStyleMessageBox.Warning);
                        if (res)
                        {
                            Process.Start(Process.GetCurrentProcess().MainModule.FileName);
                            Application.Current.Shutdown();
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private void BorderUser_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Find the button that was clicked
            var userUI = sender as Border;
            if (userUI != null)
            {
                // Find the ContextMenu resource
                ContextMenu contextMenu = this.FindResource("MyContextMenu") as ContextMenu;
                if (contextMenu != null)
                {
                    foreach (var item in contextMenu.Items)
                    {
                        if (item is MenuItem menuItem)
                        {
                            menuItem.Click -= MenuItem_Click;
                            menuItem.Click += MenuItem_Click;
                        }
                    }


                    // Set the placement target and open the context menu
                    contextMenu.PlacementTarget = userUI;
                    contextMenu.IsOpen = true;

                    TextBlock adminTextBlock = contextMenu.Template.FindName("UsernameTextBlock", contextMenu) as TextBlock;
                    if (adminTextBlock != null)
                    {
                        adminTextBlock.Text = (string)Application.Current.Properties["Username"]; // Thay đổi nội dung
                    }
                }
            }
        }
        private void ChangeView()
        {

            ListBoxMenu.SelectedIndex = ListBoxMenu.SelectedIndex != -1 ? -1 : 0;
            if (ListBoxMenu.SelectedIndex != -1)
            {
                StackPanelIPDisplay.Visibility = Visibility.Visible;
                myToggleButton.IsChecked = false; // This will trigger the setter for True

            }
            else
            {
                // or
                StackPanelIPDisplay.Visibility = Visibility.Hidden;
                myToggleButton.IsChecked = true; // This will trigger the setter for False

            }
        }
        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {

            ChangeView();
        }


     
        private void ListBoxItem_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //here
            myToggleButton.IsChecked = false;
            StackPanelIPDisplay.Visibility = Visibility.Visible;
            //var vm = CurrentViewModel<MainViewModel>();
            JobDetails_OnJobDetailChange(sender, currentStation);

            //CurrentViewModel<MainViewModel>().ConnectParamsList[currentStation].LockUISetting;
            // EnableButtons()
            ViewModelSharedEvents.OnMainListBoxMenuChange();
        }

        private void ListBoxItem_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        private void ListBoxItem_PreviewMouseDown_1(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //AboutPopup popup = new AboutPopup();
            var aboutPopup = new AboutPopup();
            aboutPopup.ShowDialog();
        }

        private void Thinh_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ExportResult.ExportNewResult(0);
        }
    }
}