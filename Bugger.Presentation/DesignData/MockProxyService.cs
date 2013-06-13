﻿using BigEgg.Framework.Applications.ViewModels;
using Bugger.Applications.Services;
using Bugger.Proxy;
using System.Collections.Generic;
using System.Linq;

namespace Bugger.Presentation.DesignData
{
    public class MockProxyService : DataModel, IProxyService
    {
        #region Fields
        private readonly IEnumerable<ISourceControlProxy> proxys;
        private ISourceControlProxy activeProxy;
        #endregion

        public MockProxyService()
        {
            this.proxys = new List<ISourceControlProxy> { new FakeProxy() };
            this.activeProxy = this.proxys.First();
        }

        #region Properties
        public IEnumerable<ISourceControlProxy> Proxys { get { return this.proxys; } }

        public ISourceControlProxy ActiveProxy
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
        #endregion
    }
}