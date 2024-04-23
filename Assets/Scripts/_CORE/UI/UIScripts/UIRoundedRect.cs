using System;
using UnityEngine;

[ExecuteInEditMode]
public class UIRoundedRect : UIWidget
{
    protected override Material GetMaterial()
    {
        return this.material;
    }

    private Vector2 Rotate(Vector2 v, int angle)
    {
        for (int i = 0; i < angle; i++)
        {
            v = new Vector2(v.y, -v.x);
        }
        return v;
    }

    private void CreateCornerFrame(Vector3 position, MeshData mesh, float length, int angle)
    {
        mesh.verts.Add(position);
        Vector2 v = this.Rotate(new Vector2(-length, 0f), angle);
        mesh.verts.Add(position + (Vector3)v);
        v = this.Rotate(v, 1);
        mesh.verts.Add(position + (Vector3)v);
    }

    protected override void OnFill(MeshData mesh)
    {
        Vector2 size = base.Size;
        this.CreateCornerFrame(new Vector3(0f, 0f), mesh, this.cornerSize, 0);
        this.CreateCornerFrame(new Vector3(size.x, 0f), mesh, this.cornerSize, 1);
        this.CreateCornerFrame(new Vector3(size.x, -size.y), mesh, this.cornerSize, 2);
        this.CreateCornerFrame(new Vector3(0f, -size.y), mesh, this.cornerSize, 3);
        for (int i = 0; i < UIRoundedRect.coreIndicies.Length; i++)
        {
            mesh.indices.Add(UIRoundedRect.coreIndicies[i]);
        }
        this.CreateRoundCorner(mesh, 0, 1, 2, this.cornerSegments, -90f, 1.57079637f);
        this.CreateRoundCorner(mesh, 3, 4, 5, this.cornerSegments, 0f, 1.57079637f);
        this.CreateRoundCorner(mesh, 6, 7, 8, this.cornerSegments, 90f, 1.57079637f);
        this.CreateRoundCorner(mesh, 9, 10, 11, this.cornerSegments, 180f, 1.57079637f);
    }

    private void CreateRoundCorner(MeshData meshData, int centerIndex, int armIndex, int armIndex2, int segments, float startAngle, float angle = 1.57079637f)
    {
        Vector2 vector = meshData.verts[centerIndex];
        Vector2 a = meshData.verts[armIndex];
        float magnitude = (a - vector).magnitude;
        float num = angle / (float)segments;
        startAngle *= 0.0174532924f;
        for (int i = 0; i < segments; i++)
        {
            float f = startAngle + num * (float)(i + 1);
            Vector3 item = vector + new Vector2(Mathf.Sin(f) * magnitude, Mathf.Cos(f) * magnitude);
            int item2;
            if (i == 0)
            {
                item2 = armIndex;
            }
            else
            {
                item2 = meshData.verts.Count - 1;
            }
            int item3;
            if (i < segments - 1)
            {
                item3 = meshData.verts.Count;
                meshData.verts.Add(item);
            }
            else
            {
                item3 = armIndex2;
            }
            meshData.indices.Add(centerIndex);
            meshData.indices.Add(item2);
            meshData.indices.Add(item3);
        }
    }

    public Material material;

    public float cornerSize;

    public int cornerSegments = 1;

    private static int[] coreIndicies = new int[]
    {
        0,
        2,
        4,
        0,
        4,
        3,
        3,
        5,
        7,
        3,
        7,
        6,
        6,
        8,
        10,
        6,
        10,
        9,
        9,
        11,
        1,
        9,
        1,
        0,
        0,
        3,
        6,
        0,
        6,
        9
    };
}
