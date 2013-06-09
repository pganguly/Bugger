﻿using BigEgg.Framework.Applications.Collections;
using Bugger.Domain.Models;
using System;
using System.ComponentModel;

namespace Bugger.Applications.Services
{
    public interface IDataService : INotifyPropertyChanged
    {
        MultiThreadingObservableCollection<Bug> UserBugs { get; }

        MultiThreadingObservableCollection<Bug> TeamBugs { get; }

        DateTime RefreshTime { get; set; }
    }
}
