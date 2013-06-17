﻿using BigEgg.Framework.Applications.Commands;
using BigEgg.Framework.Applications.ViewModels;
using BigEgg.Framework.Foundation;
using Bugger.Applications.Models;
using Bugger.Applications.Properties;
using Bugger.Applications.Services;
using Bugger.Applications.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows.Input;

namespace Bugger.Applications.ViewModels
{
    public class SettingsViewModel : ViewModel<ISettingsView>, IDataErrorInfo
    {
        #region Fields
        private const string TeamMemberSplitString = "; ";

        private readonly ObservableCollection<string> teamMembers;
        private readonly ObservableCollection<string> selectedTeamMembers;
        private readonly ObservableCollection<CheckString> statusValues;
        private readonly ReadOnlyCollection<string> proxys;
        private readonly DelegateCommand addNewTeamMemberCommand;
        private readonly DelegateCommand removeTeamMemberCommand;

        private string selectedTeamMember;
        private string activeProxy;
        private string userName;
        private string newTeamMember;
        private int refreshMinutes;
        private bool isFilterCreatedBy;
        private string filterStatusValues;
        #endregion

        public SettingsViewModel(ISettingsView view, IProxyService proxyService, string teamMembers)
            : base(view)
        {
            this.dataErrorInfoSupport = new DataErrorInfoSupport(this);

            this.teamMembers = new ObservableCollection<string>(
                teamMembers.Split(TeamMemberSplitString.ToArray(), StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim()));
            this.proxys = new ReadOnlyCollection<string>(
                proxyService.Proxys.Select(x => x.ProxyName).ToList());
            this.activeProxy = 
                proxyService.ActiveProxy == null ? string.Empty : proxyService.ActiveProxy.ProxyName;

            this.statusValues = new ObservableCollection<CheckString>();

            this.addNewTeamMemberCommand = new DelegateCommand(AddNewTeamMemberCommandExecute, CanAddNewTeamMemberCommandExecute);
            this.removeTeamMemberCommand = new DelegateCommand(RemoveTeamMemberCommandExecute, CanRemoveTeamMemberCommandExecute);

            this.selectedTeamMember = this.teamMembers.FirstOrDefault();
            this.selectedTeamMembers = new ObservableCollection<string>();
            this.selectedTeamMembers.Add(selectedTeamMember);
            this.newTeamMember = string.Empty;
        }

        #region Implement IDataErrorInfo interface
        protected readonly DataErrorInfoSupport dataErrorInfoSupport;

        string IDataErrorInfo.Error { get { return this.dataErrorInfoSupport.Error; } }

        string IDataErrorInfo.this[string columnName] { get { return this.dataErrorInfoSupport[columnName]; } }
        #endregion

        #region Properties
        public ICommand AddNewTeamMemberCommand { get { return this.addNewTeamMemberCommand; } }

        public ICommand RemoveTeamMemberCommand { get { return this.removeTeamMemberCommand; } }

        public ReadOnlyCollection<string> Proxys { get { return this.proxys; } }

        public ObservableCollection<string> TeamMembers { get { return this.teamMembers; } }

        internal string TeamMembersString { get { return String.Join(TeamMemberSplitString, this.teamMembers); } }

        public ObservableCollection<string> SelectedTeamMembers { get { return this.selectedTeamMembers; } }

        public string SelectedTeamMember
        {
            get { return this.selectedTeamMember; }
            set
            {
                if (this.selectedTeamMember != value)
                {
                    this.selectedTeamMember = value;
                    RaisePropertyChanged("SelectedTeamMember");
                }
            }
        }

        [Required(ErrorMessageResourceName = "ActiveProxyMandatory", ErrorMessageResourceType = typeof(Resources))]
        public string ActiveProxy
        {
            get { return this.activeProxy; }
            set
            {
                if (this.activeProxy != value)
                {
                    this.activeProxy = value;
                    RaisePropertyChanged("ActiveProxy");
                }
            }
        }

        [Required(ErrorMessageResourceName = "UserNameMandatory", ErrorMessageResourceType = typeof(Resources))]
        public string UserName
        {
            get { return this.userName; }
            set
            {
                if (this.userName != value)
                {
                    this.userName = value;
                    RaisePropertyChanged("UserName");
                }
            }
        }

        public string NewTeamMember
        {
            get { return this.newTeamMember; }
            set
            {
                if (this.newTeamMember != value)
                {
                    this.newTeamMember = value;
                    UpdateCommands();
                    RaisePropertyChanged("NewTeamMember");
                }
            }
        }

        [Range(1, 720, ErrorMessageResourceName = "RefreshMinutesRange", ErrorMessageResourceType = typeof(Resources))]
        public int RefreshMinutes
        {
            get { return this.refreshMinutes; }
            set
            {
                if (this.refreshMinutes != value)
                {
                    this.refreshMinutes = value;
                    RaisePropertyChanged("RefreshMinutes");
                }
            }
        }

        public bool IsFilterCreatedBy
        {
            get { return this.isFilterCreatedBy; }
            set
            {
                if (this.isFilterCreatedBy != value)
                {
                    this.isFilterCreatedBy = value;
                    RaisePropertyChanged("IsFilterCreatedBy");
                }
            }        
        }

        public string FilterStatusValues
        {
            get { return this.filterStatusValues; }
            set
            {
                if (this.filterStatusValues != value)
                {
                    this.filterStatusValues = value;
                    RaisePropertyChanged("FilterStatusValues");
                }
            }
        }

        public ObservableCollection<CheckString> StatusValues { get { return this.statusValues; } }
        #endregion

        #region Private Methods
        private void AddNewTeamMemberCommandExecute()
        {
            if (!this.TeamMembers.Contains(this.newTeamMember))
            {
                this.TeamMembers.Add(this.newTeamMember);
            }
            this.SelectedTeamMembers.Clear();
            this.SelectedTeamMembers.Add(this.newTeamMember);
            this.SelectedTeamMember = this.newTeamMember;
            this.NewTeamMember = string.Empty;

            UpdateCommands();
        }

        private bool CanAddNewTeamMemberCommandExecute()
        {
            return !string.IsNullOrWhiteSpace(this.NewTeamMember);
        }

        private void RemoveTeamMemberCommandExecute()
        {
            IEnumerable<string> itemsToExclude = this.SelectedTeamMembers.Except(new[] { this.SelectedTeamMember });
            string nextBranch = CollectionHelper.GetNextElementOrDefault(this.TeamMembers.Except(itemsToExclude),
                this.SelectedTeamMember);

            foreach (string item in this.SelectedTeamMembers)
            {
                this.TeamMembers.Remove(item);
            }


            this.SelectedTeamMember = nextBranch ?? this.TeamMembers.LastOrDefault();
            this.SelectedTeamMembers.Clear();
            if (this.SelectedTeamMember != null)
                this.SelectedTeamMembers.Add(this.SelectedTeamMember);

            UpdateCommands();
        }

        private bool CanRemoveTeamMemberCommandExecute()
        {
            return this.SelectedTeamMember != null;
        }

        private void UpdateCommands()
        {
            this.addNewTeamMemberCommand.RaiseCanExecuteChanged();
            this.removeTeamMemberCommand.RaiseCanExecuteChanged();
        }
        #endregion
    }
}
