using System;

namespace TactileModules.SagaCore
{
	public interface IMapPlugin
	{
		void ViewsCreated(MapIdentifier mapId, MapContentController mapContent, MapFlow mapFlow);

		void ViewsDestroyed(MapIdentifier mapId, MapContentController mapContent);
	}
}
