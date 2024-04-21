using System;
using UnityEngine;

namespace Spine.Unity
{
	public static class SpineMesh
	{
		public static Mesh NewMesh()
		{
			Mesh mesh = new Mesh();
			mesh.MarkDynamic();
			mesh.name = "Skeleton Mesh";
			mesh.hideFlags = (HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild);
			return mesh;
		}

		internal const HideFlags MeshHideflags = HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild;
	}
}
