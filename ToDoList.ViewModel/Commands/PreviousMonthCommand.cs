﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace ToDoList.ViewModel.Commands
{
    public class PreviousMonthCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private readonly IEventsCalendarViewModel _viewModel;

        public PreviousMonthCommand(IEventsCalendarViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _viewModel.PreviousMonth();
            _viewModel.LoadScheduleAsync();
            _viewModel.NotifyObservers();
        }
    }
}