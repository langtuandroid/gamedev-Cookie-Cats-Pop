using System;
using System.Collections.Generic;

public static class IMapPopupExtensions
{
	public static MapPopupManager.PopupConfig.PopupConfigData GetConfigData(this MapPopupManager.IMapPopup mapPopup)
	{
		if (IMapPopupExtensions.cache.ContainsKey(mapPopup))
		{
			return IMapPopupExtensions.cache[mapPopup];
		}
		foreach (MapPopupManager.PopupConfig.PopupConfigData popupConfigData in MapPopupManager.Instance.popupManagerProvider.PopupConfig.PopupConfigDatas)
		{
			if (mapPopup.GetType().Name == popupConfigData.PopupClassName)
			{
				IMapPopupExtensions.cache.Add(mapPopup, popupConfigData);
				return popupConfigData;
			}
		}
		foreach (MapPopupManager.PopupConfig.PopupConfigData popupConfigData2 in MapPopupManager.Instance.popupManagerProvider.PopupConfig.PopupConfigDatas)
		{
			Type type = mapPopup.GetType();
			MapPopupIdentifierAttribute[] array = (MapPopupIdentifierAttribute[])type.GetCustomAttributes(typeof(MapPopupIdentifierAttribute), false);
			if (array.Length == 1 && array[0].Identifier == popupConfigData2.PopupClassName)
			{
				IMapPopupExtensions.cache.Add(mapPopup, popupConfigData2);
				return popupConfigData2;
			}
		}
		throw new Exception("No popup config object found for popup data with object type name: " + mapPopup.GetType().Name);
	}

	private static readonly Dictionary<MapPopupManager.IMapPopup, MapPopupManager.PopupConfig.PopupConfigData> cache = new Dictionary<MapPopupManager.IMapPopup, MapPopupManager.PopupConfig.PopupConfigData>();
}
