﻿using MixItUp.Base;
using MixItUp.Base.Commands;
using MixItUp.WPF.Controls.Command;
using MixItUp.WPF.Util;
using MixItUp.WPF.Windows.Command;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MixItUp.WPF.Controls.MainControls
{
    /// <summary>
    /// Interaction logic for TimerControl.xaml
    /// </summary>
    public partial class TimerControl : MainControlBase
    {
        private ObservableCollection<TimerCommand> timerCommands = new ObservableCollection<TimerCommand>();

        public TimerControl()
        {
            InitializeComponent();
        }

        protected override Task InitializeInternal()
        {
            this.TimerCommandsListView.ItemsSource = this.timerCommands;
            this.TimerIntervalTextBox.Text = ChannelSession.Settings.TimerCommandsInterval.ToString();
            this.TimerMinimumMessagesTextBox.Text = ChannelSession.Settings.TimerCommandsMinimumMessages.ToString();
            this.DisableAllTimers.IsChecked = ChannelSession.Settings.DisableAllTimers;

            this.RefreshList();

            return base.InitializeInternal();
        }

        private void RefreshList()
        {
            this.TimerCommandsListView.SelectedIndex = -1;

            this.timerCommands.Clear();
            foreach (TimerCommand command in ChannelSession.Settings.TimerCommands.OrderBy(c => c.Name))
            {
                this.timerCommands.Add(command);
            }
        }

        private void CommandButtons_EditClicked(object sender, RoutedEventArgs e)
        {
            CommandButtonsControl commandButtonsControl = (CommandButtonsControl)sender;
            TimerCommand command = commandButtonsControl.GetCommandFromCommandButtons<TimerCommand>(sender);
            if (command != null)
            {
                CommandWindow window = new CommandWindow(new TimerCommandDetailsControl(command));
                window.Closed += Window_Closed;
                window.Show();
            }
        }

        private async void CommandButtons_DeleteClicked(object sender, RoutedEventArgs e)
        {
            await this.Window.RunAsyncOperation(async () =>
            {
                CommandButtonsControl commandButtonsControl = (CommandButtonsControl)sender;
                TimerCommand command = commandButtonsControl.GetCommandFromCommandButtons<TimerCommand>(sender);
                if (command != null)
                {
                    ChannelSession.Settings.TimerCommands.Remove(command);
                    await ChannelSession.SaveSettings();
                    this.RefreshList();
                }
            });
        }

        private void AddCommandButton_Click(object sender, RoutedEventArgs e)
        {
            CommandWindow window = new CommandWindow(new TimerCommandDetailsControl());
            window.Closed += Window_Closed;
            window.Show();
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            this.RefreshList();
        }

        private async void TimerMinimumMessagesTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            int value;
            if (int.TryParse(this.TimerMinimumMessagesTextBox.Text, out value) && value >= 0)
            {
                ChannelSession.Settings.TimerCommandsMinimumMessages = value;
            }
            else
            {
                await MessageBoxHelper.ShowMessageDialog("Minimum Messages must be 0 or greater");
                this.TimerMinimumMessagesTextBox.Text = ChannelSession.Settings.TimerCommandsMinimumMessages.ToString();
            }

            await this.CheckIfMinMessagesAndIntervalAreBothZero();
        }

        private async void TimerIntervalTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            int value;
            if (int.TryParse(this.TimerIntervalTextBox.Text, out value) && value >= 0)
            {
                ChannelSession.Settings.TimerCommandsInterval = value;
            }
            else
            {
                await MessageBoxHelper.ShowMessageDialog("Interval must be 0 or greater");
                this.TimerIntervalTextBox.Text = ChannelSession.Settings.TimerCommandsInterval.ToString();
            }

            await this.CheckIfMinMessagesAndIntervalAreBothZero();
        }

        private async Task CheckIfMinMessagesAndIntervalAreBothZero()
        {
            if (ChannelSession.Settings.TimerCommandsMinimumMessages <= 0 && ChannelSession.Settings.TimerCommandsInterval <= 0)
            {
                await MessageBoxHelper.ShowMessageDialog("Minimum Messages & Interval can not both be 0");
                ChannelSession.Settings.TimerCommandsInterval = 1;
                this.TimerIntervalTextBox.Text = ChannelSession.Settings.TimerCommandsInterval.ToString();
            }
        }

        private void DisableAllTimers_Click(object sender, RoutedEventArgs e)
        {
            ChannelSession.Settings.DisableAllTimers = this.DisableAllTimers.IsChecked.GetValueOrDefault();
        }
    }
}
