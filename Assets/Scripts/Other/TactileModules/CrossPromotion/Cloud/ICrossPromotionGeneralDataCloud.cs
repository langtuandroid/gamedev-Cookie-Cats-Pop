using System;
using System.Collections;
using Fibers;
using TactileModules.CrossPromotion.Cloud.Data;

namespace TactileModules.CrossPromotion.Cloud
{
	public interface ICrossPromotionGeneralDataCloud
	{
		IEnumerator GetCrossPromotionGeneralData(EnumeratorResult<CrossPromotionGeneralData> result);
	}
}
