using System;
using System.Collections;
using UnityEngine;

namespace TactileModules.Foundation
{
	public abstract class BootBase : MonoBehaviour
	{
		private void Awake()
		{

            AndroidSplash androidSplash = AndroidSplash.Create();
			FiberCtrl.Pool.Run(androidSplash.Show(new Action(this.PreSetup)), true);
		}

		protected void PreSetup()
		{
			FiberCtrl.Pool.Run(this.Setup(), false);
		}

		protected virtual IEnumerator Setup()
		{
			this.SetApplicationProperties();
			ApplicationLifeCycleEvents applicationLifeCycleEvents = new ApplicationLifeCycleEvents();
            
            ManagerRepository repository = new ManagerRepository();
			yield return this.RegisterManagers(repository, applicationLifeCycleEvents);
			repository.SetupManagers();
			this.RegisterCloudSynchronizables();
			this.BootCompleted();
			this.NotifyCompleted(applicationLifeCycleEvents);
			yield break;
		}

		protected void NotifyCompleted(ApplicationLifeCycleEvents applicationLifeCycleEvents)
		{

            ApplicationLifeCycle.IsApplicationInitialized = true;
			applicationLifeCycleEvents.BootWasCompleted();
		}

		protected virtual void SetApplicationProperties()
		{
			Application.targetFrameRate = 60;
			Screen.sleepTimeout = -1;
			Input.multiTouchEnabled = false;
		}

		protected abstract IEnumerator RegisterManagers(ManagerRepository repository, IApplicationLifeCycleEvents applicationLifeCycleEvents);

		protected abstract void RegisterCloudSynchronizables();

		protected abstract void BootCompleted();
	}
}
