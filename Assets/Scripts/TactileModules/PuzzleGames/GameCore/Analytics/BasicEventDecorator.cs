using System;
using JetBrains.Annotations;
using Tactile;
using TactileModules.Analytics.Interfaces;
using TactileModules.FeatureManager;
using TactileModules.PuzzleGames.Configuration;

namespace TactileModules.PuzzleGames.GameCore.Analytics
{
    public class BasicEventDecorator : IEventDecorator<BasicEvent>, IEventDecorator
    {
        public BasicEventDecorator(IFlowStack flowStack, IConfigurationManager configurationManager, InventoryManager inventoryManager, [CanBeNull] TactileModules.FeatureManager.FeatureManager featureManager, [CanBeNull] IAnalyticsContextProvider contextProvider)
        {
            this.flowStack = flowStack;
            this.configurationManager = configurationManager;
            this.inventoryManager = inventoryManager;
            this.featureManager = featureManager;
            this.contextProvider = contextProvider;
        }

        public void Decorate(BasicEvent basicEvent)
        {
            int version = this.configurationManager.GetVersion();
            basicEvent.SetPuzzleCoreCommonProperties(this.MakeFlowStackAsPathSafely(this.flowStack), version, (this.contextProvider == null) ? string.Empty : this.contextProvider.CreateContextString());
        }

        private string MakeFlowStackAsPathSafely(IFlowStack flowStack)
        {
            string result;
            try
            {
                string text = string.Empty;
                foreach (IFlow flow in flowStack.TraverseStack())
                {
                    if (flow != null)
                    {
                        if (text.Length > 0)
                        {
                            text += "/";
                        }
                        text += flow.GetType().Name;
                    }
                }
                result = text;
            }
            catch (Exception ex)
            {
                result = "EXCEPTION";
            }
            return result;
        }

        private readonly IFlowStack flowStack;

        private readonly IConfigurationManager configurationManager;

        private readonly InventoryManager inventoryManager;

        private readonly TactileModules.FeatureManager.FeatureManager featureManager;

        private readonly IAnalyticsContextProvider contextProvider;
    }
}
