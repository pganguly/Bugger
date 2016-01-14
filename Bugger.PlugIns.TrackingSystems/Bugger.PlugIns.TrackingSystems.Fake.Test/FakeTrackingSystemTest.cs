﻿using Bugger.PlugIns.TrackingSystem;
using Bugger.PlugIns.TrackingSystems.Fake.Services;
using Bugger.PlugIns.TrackingSystems.Fake.Test.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

namespace Bugger.PlugIns.TrackingSystems.Fake.Test
{
    [TestClass]
    public class FakeTrackingSystemTest
    {
        private readonly CompositionContainer container;

        public FakeTrackingSystemTest()
        {
            AggregateCatalog catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new TypeCatalog(
                typeof(FakeTrackingSystem)
            ));
            catalog.Catalogs.Add(new TypeCatalog(
                typeof(MockDataService)
            ));
            container = new CompositionContainer(catalog);
            CompositionBatch batch = new CompositionBatch();
            batch.AddExportedValue(container);
            container.Compose(batch);
        }


        [TestMethod]
        public void InjectionTest()
        {
            var plugIn = container.GetExportedValue<IPlugIn>();
            Assert.IsNotNull(plugIn);

            var trackingSystemPlugIn = container.GetExportedValue<ITrackingSystemPlugIn>();
            Assert.IsNotNull(trackingSystemPlugIn);
        }

        [TestMethod]
        public void GeneralTest()
        {
            var fakeTrackingSystem = container.GetExportedValue<ITrackingSystemPlugIn>();
            Assert.AreEqual("41090009-10c1-447f-9189-a42cd9657c29", fakeTrackingSystem.Guid.ToString());
            Assert.AreEqual(PlugInType.TrackingSystem, fakeTrackingSystem.PlugInType);

            Assert.AreEqual(TrackingSystemStatus.Unknown, fakeTrackingSystem.GetStatus());
        }

        [TestMethod]
        public void GetBugsTest()
        {
            var fakeTrackingSystem = container.GetExportedValue<ITrackingSystemPlugIn>();
            var dataService = (MockDataService)container.GetExportedValue<IDataService>();

            dataService.Clear();

            Assert.IsFalse(dataService.GetBugsCalled);
            fakeTrackingSystem.QueryAsync("username")
                .ContinueWith(task =>
                {
                    Assert.IsNull(task.Result);
                    Assert.IsTrue(dataService.GetBugsCalled);
                    Assert.AreEqual(TrackingSystemStatus.CanConnect, fakeTrackingSystem.GetStatus());
                });
            Assert.AreEqual(TrackingSystemStatus.Querying, fakeTrackingSystem.GetStatus());
        }

        [TestMethod]
        public void GetTeamBugsTest()
        {
            var fakeTrackingSystem = container.GetExportedValue<ITrackingSystemPlugIn>();
            var dataService = (MockDataService)container.GetExportedValue<IDataService>();

            dataService.Clear();

            Assert.IsFalse(dataService.GetTeamBugsCalled);
            fakeTrackingSystem.QueryAsync(new List<string> { "username" })
                .ContinueWith(task =>
                 {
                     Assert.IsNull(task.Result);
                     Assert.IsTrue(dataService.GetTeamBugsCalled);
                     Assert.AreEqual(TrackingSystemStatus.CanConnect, fakeTrackingSystem.GetStatus());
                 });
            Assert.AreEqual(TrackingSystemStatus.Querying, fakeTrackingSystem.GetStatus());
        }
    }
}
