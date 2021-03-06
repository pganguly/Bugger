﻿using BigEgg.Framework.Applications.Collections;
using Bugger.Applications.Models;
using Bugger.Base.Models;
using System;
using System.ComponentModel;

namespace Bugger.Applications.Services
{
    public interface IDataService : INotifyPropertyChanged
    {
        MultiThreadingObservableCollection<Bug> UserBugs { get; }

        MultiThreadingObservableCollection<Bug> TeamBugs { get; }

        DateTime RefreshTime { get; set; }

        QueryStatus UserBugsQueryState { get; set; }

        QueryStatus TeamBugsQueryState { get; set; }

        int UserBugsProgressValue { get; set; }

        int TeamBugsProgressValue { get; set; }

        InitializeStatus InitializeStatus { get; set; }
    }
}
