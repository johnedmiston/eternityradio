﻿using System;
using System.Windows.Input;
using Xamarin.Forms;
using EternityRadioApp.Domain.Global;
using EternityRadioApp.Services;
using EternityRadioApp.Styles;
using EternityRadioApp.ViewModel;

namespace EternityRadioApp.Content.Settings
{
    public class SettingsViewModel : NewBaseViewModel
    {
        public ICommand ChangeThemeCommand { get; set; }

        public ICommand ChangeNavigationCommand { get; set; }

        public OSAppTheme SelectedTheme
        {
            get { return selectedTheme; }
            set
            {
                SetProperty(ref selectedTheme, value);
            }
        }

        public SettingsViewModel()
        {
            //ChangeThemeCommand = new Command((x) =>
            //{
            //    if (SelectedTheme.ToLower() == "dark")
            //    {
            //        Application.Current.UserAppTheme = OSAppTheme.Dark;
            //    }
            //    else
            //    {
            //        Application.Current.UserAppTheme = OSAppTheme.Light;
            //    }

            //    App.AppTheme = SelectedTheme.ToLower();
            //});

            ChangeNavigationCommand = new Command<string>((nav) =>
            {
                var am = DependencyService.Resolve<AppModel>();
                if (nav == "flyout")
                {
                    am.NavigationStyle = NavigationStyle.Flyout;
                }
                else
                {
                    am.NavigationStyle = NavigationStyle.Tabs;
                }

                OnPropertyChanged(nameof(UseFlyout));
                OnPropertyChanged(nameof(UseTabs));

                Shell.Current.Navigation.PopModalAsync();
            });
        }

        private bool isVisualMaterial;

        public bool IsVisualMaterial
        {
            get => isVisualMaterial;
            set
            {
                SetProperty(ref isVisualMaterial, value);
                isVisualDefault = !IsVisualMaterial;
                OnPropertyChanged(nameof(IsVisualDefault));
            }
        }
        public bool IsVisualDefault
        {
            get => isVisualDefault;
            set
            {
                SetProperty(ref isVisualDefault, value);
                isVisualMaterial = !isVisualDefault;
                OnPropertyChanged(nameof(IsVisualMaterial));
            }
        }

        public bool UseDarkMode
        {
            get => DependencyService.Get<AppModel>().UseDarkMode;
            set
            {
                DependencyService.Get<AppModel>().UseDarkMode = value;
                UpdateThemeMode();
            }
        }

        public bool UseLightMode
        {
            get => DependencyService.Get<AppModel>().UseDarkMode == false;
            set
            {
                DependencyService.Get<AppModel>().UseDarkMode = !value;
                UpdateThemeMode();
            }
        }

        private void UpdateThemeMode()
        {
            OnPropertyChanged(nameof(UseDarkMode));
            OnPropertyChanged(nameof(UseLightMode));

            if (UseDarkMode && App.AppTheme != "dark")
            {
                Application.Current.UserAppTheme = OSAppTheme.Dark;
                App.AppTheme = "dark";
            }
            else if (!UseDarkMode && App.AppTheme == "dark")
            {
                Application.Current.UserAppTheme = OSAppTheme.Light;
                App.AppTheme = "light";
            }
        }

        public bool UseDeviceThemeSettings
        {
            get => DependencyService.Get<AppModel>().UseDeviceThemeSettings;
            set
            {
                DependencyService.Get<AppModel>().UseDeviceThemeSettings = value;
                Application.Current.UserAppTheme = OSAppTheme.Unspecified;
                OnPropertyChanged(nameof(UseDeviceThemeSettings));
            }
        }

        public int UseFlyout
        {
            get { return (AppModel.NavigationStyle == Domain.Global.NavigationStyle.Flyout) ? 1 : 0; }
        }

        public int UseTabs
        {
            get { return (AppModel.NavigationStyle == Domain.Global.NavigationStyle.Tabs) ? 1 : 0; }
        }

        private bool isVisualDefault;
        private OSAppTheme selectedTheme = App.Current.UserAppTheme;

    }
}
