﻿using MixItUp.Base;
using MixItUp.Base.ViewModel.User;
using System.Linq;
using System.Windows.Controls;

namespace MixItUp.WPF.Controls.Dialogs
{
    /// <summary>
    /// Interaction logic for UserDialogControl.xaml
    /// </summary>
    public partial class UserDialogControl : UserControl
    {
        private UserViewModel user;

        public UserDialogControl(UserViewModel user)
        {
            this.user = user;

            InitializeComponent();

            this.Loaded += UserDialogControl_Loaded;
        }

        private async void UserDialogControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            await this.user.RefreshDetails(force: true);

            this.UserAvatar.SetSize(100);
            await this.UserAvatar.SetUserAvatarUrl(this.user);

            if (this.user.MixerRoles.Contains(MixerRoleEnum.Banned))
            {
                this.UnbanButton.Visibility = System.Windows.Visibility.Visible;
                this.BanButton.Visibility = System.Windows.Visibility.Collapsed;
            }

            if (this.user.IsOnline)
            {
                this.StreamStatusTextBlock.Text = $"{this.user.CurrentViewerCount} Viewers";
            }
            else
            {
                this.StreamStatusTextBlock.Text = "Offline";
            }

            this.DataContext = this.user;
        }
    }
}
