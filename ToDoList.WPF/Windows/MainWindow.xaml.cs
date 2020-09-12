﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
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
using ToDoList.ViewModel;
using ToDoList.ViewModel.ObserverPattern;
using ToDoList.WPF;
using ToDoList.ViewModel.ViewModels;
using ToDoList.WPF.UserControls;

namespace ToDoList.WPF.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IObserver
    {
        private MonthCalendar MonthCalendar { get; }
        public MainWindow(IEventsCalendarViewModel viewModel, IUserControlFactory controlFactory)
        {
            InitializeComponent();

            viewModel.AddObserver(this);
            DataContext = viewModel;
            MonthCalendar = controlFactory.GetMonthCalendar(viewModel);

            Initialize();
        }

        /// <summary>
        /// Allows to drag window with a mouse with its left button down.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RectangleTitleBarBackground_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        /// <summary>
        /// Minimizes the window.
        /// </summary>
        private void Minimize(object sender, EventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// Maximizes the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Maximize(object sender, EventArgs e)
        {
            WindowState = (WindowState != WindowState.Maximized ? WindowState.Maximized : WindowState.Normal);
        }

        /// <summary>
        /// Shuts down the application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Close(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }


        private void AddEvent(object sender, EventArgs e)
        {
            var addEventWindow = new AddEventWindow(DataContext as IEventsCalendarViewModel);
            this.IsEnabled = false;
            addEventWindow.Show();

            addEventWindow.Closed += (s, e) =>  this.IsEnabled = true;
        }

        /// <summary>
        /// Sets text property of MonthTextBlock
        /// </summary>
        private void SetMonth()
        {
            switch((DataContext as IEventsCalendarViewModel).CurrentlyDisplayedMonth.Month)
            {
                case 1: 
                    MonthTextBlock.Text = ToDoList.WPF.Properties.Resources.january;
                    break;
                case 2:
                    MonthTextBlock.Text = ToDoList.WPF.Properties.Resources.february;
                    break;
                case 3:
                    MonthTextBlock.Text = ToDoList.WPF.Properties.Resources.march;
                    break;
                case 4:
                    MonthTextBlock.Text = ToDoList.WPF.Properties.Resources.april;
                    break;
                case 5:
                    MonthTextBlock.Text = ToDoList.WPF.Properties.Resources.may;
                    break;
                case 6:
                    MonthTextBlock.Text = ToDoList.WPF.Properties.Resources.june;
                    break;
                case 7:
                    MonthTextBlock.Text = ToDoList.WPF.Properties.Resources.july;
                    break;
                case 8:
                    MonthTextBlock.Text = ToDoList.WPF.Properties.Resources.august;
                    break;
                case 9:
                    MonthTextBlock.Text = ToDoList.WPF.Properties.Resources.september;
                    break;
                case 10:
                    MonthTextBlock.Text = ToDoList.WPF.Properties.Resources.october;
                    break;
                case 11:
                    MonthTextBlock.Text = ToDoList.WPF.Properties.Resources.november;
                    break;
                case 12:
                    MonthTextBlock.Text = ToDoList.WPF.Properties.Resources.december;
                    break;
            }
        }

        private void Initialize()
        {
            // Add MonthCalendar to the grid
            Grid.SetColumn(MonthCalendar, 0);
            Grid.SetRow(MonthCalendar, 4);
            Grid.SetColumnSpan(MonthCalendar, 2);
            RootGrid.Children.Add(MonthCalendar);

            // Set strings
            SetMonth();
        }

        public void Update()
        {
            SetMonth();
        }
    }
}
