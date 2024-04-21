using System;

namespace Spine.Unity.MeshGeneration
{
	public interface ISubmeshSetMeshGenerator
	{
		MeshAndMaterials GenerateMesh(ExposedList<SubmeshInstruction> instructions, int startSubmesh, int endSubmesh);

		float ZSpacing { get; set; }

		bool AddNormals { get; set; }

		bool AddTangents { get; set; }
	}
}
