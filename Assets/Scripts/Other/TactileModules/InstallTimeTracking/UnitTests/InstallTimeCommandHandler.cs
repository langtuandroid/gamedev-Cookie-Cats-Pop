using System;
using JetBrains.Annotations;
using TactileModules.TactilePrefs;

namespace TactileModules.InstallTimeTracking.UnitTests
{
    public class InstallTimeCommandHandler : BaseCommandHandler
    {
        public static void InjectDependencies(ILocalStorageString injectedInstallTimeStore)
        {
            InstallTimeCommandHandler.installTimeStore = injectedInstallTimeStore;
        }

        [UsedImplicitly]
 
        private static void SetTime14DaysBackForFirstInstallTime()
        {
            string data = DateTime.Now.AddDays(-14.0).ToString("yyyy-MM-dd HH:mm:ss");
            InstallTimeCommandHandler.installTimeStore.Save(data);
        }

        [UsedImplicitly]

        private static void SetTimeXDaysBackForFirstInstallTime(int days)
        {
            string data = DateTime.Now.AddDays((double)(-(double)days)).ToString("yyyy-MM-dd HH:mm:ss");
            InstallTimeCommandHandler.installTimeStore.Save(data);
        }

        private static ILocalStorageString installTimeStore;
    }
}
