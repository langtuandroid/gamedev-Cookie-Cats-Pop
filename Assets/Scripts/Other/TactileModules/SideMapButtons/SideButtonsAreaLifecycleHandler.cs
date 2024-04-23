using System;
using System.Collections.Generic;
using System.Diagnostics;
using TactileModules.ComponentLifecycle;

namespace TactileModules.SideMapButtons
{
	public class SideButtonsAreaLifecycleHandler : ComponentLifecycleHandler<SideButtonsArea>, ISideButtonsAreaLifecycleHandler
	{
		public SideButtonsAreaLifecycleHandler(SideMapButtonControllerProviderRegistry registry) : base(ComponentLifecycleHandler<SideButtonsArea>.InitializationTiming.Start)
		{
			this.registry = registry;
		}

		////[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<ISideMapButtonController> SideMapButtonControllerCreated = delegate (ISideMapButtonController A_0)
        {
        };



        protected override void InitializeComponent(SideButtonsArea component)
		{
			SideButtonsAreaController sideButtonsAreaController = new SideButtonsAreaController();
			sideButtonsAreaController.SideMapButtonControllerCreated += delegate(ISideMapButtonController buttonController)
			{
				this.SideMapButtonControllerCreated(buttonController);
			};
			sideButtonsAreaController.CreateSideMapButtonControllers(this.registry, component);
			this.controllers[component] = sideButtonsAreaController;
		}

		protected override void TeardownComponent(SideButtonsArea component)
		{
			this.controllers[component].Teardown();
			this.controllers.Remove(component);
			base.TeardownComponent(component);
		}

		private readonly SideMapButtonControllerProviderRegistry registry;

		private readonly Dictionary<SideButtonsArea, SideButtonsAreaController> controllers = new Dictionary<SideButtonsArea, SideButtonsAreaController>();
	}
}
