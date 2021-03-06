﻿using Autofac;
using Planner.Model;
using Planner.Model.Repositories;
using Planner.WPF.Windows;
using Planner.ViewModel.ViewModels;
using Planner.WPF.UserControls;
using Planner.Model.Services;

namespace Planner.WPF.DIContainer
{
    public static class ContainerConfig
    {

        public static IContainer Configure()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<MainWindow>().AsSelf();
            builder.RegisterType<ScheduleViewModel>().As<IScheduleViewModel>();
            builder.RegisterType<UserControlFactory>().As<IUserControlFactory>();
            builder.RegisterType<ScheduleService>().As<IScheduleService>();
            builder.RegisterType<UserService>().As<IUserService>();
            builder.RegisterType<LoginWindow>().AsSelf();
            builder.RegisterType<UserViewModel>().As<IUserViewModel>();

            return builder.Build();
        }
    }
}
