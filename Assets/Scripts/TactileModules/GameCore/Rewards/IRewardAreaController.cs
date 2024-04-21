using System;
using UnityEngine;

namespace TactileModules.GameCore.Rewards
{
	public interface IRewardAreaController
	{
		void RegisterTargetsFromGameObject(GameObject go);

		void UnregisterTargetsFromGameObject(GameObject go);
	}
}
