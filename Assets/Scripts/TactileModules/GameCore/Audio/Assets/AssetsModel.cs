using System;
using UnityEngine;

namespace TactileModules.GameCore.Audio.Assets
{
	public class AssetsModel : IAssetsModel
	{
		public AudioDatabase AudioDatabase
		{
			get
			{
				return Resources.Load<AudioDatabase>("Audio/AudioDatabase");
			}
		}
	}
}
