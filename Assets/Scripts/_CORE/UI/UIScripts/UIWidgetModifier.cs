using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(UIWidget))]
public abstract class UIWidgetModifier : MonoBehaviour
{
    protected UIWidget Widget { get; private set; }

    private void OnEnable()
    {
        this.Widget = base.GetComponent<UIWidget>();
        this.Widget.OnPostFill += this.Modify;
        this.Widget.MarkAsChanged();
    }

    private void OnDisable()
    {
        this.Widget = base.GetComponent<UIWidget>();
        this.Widget.OnPostFill -= this.Modify;
        this.Widget.MarkAsChanged();
    }

    protected void MarkAsChanged()
    {
        if (this.Widget != null)
        {
            this.Widget.MarkAsChanged();
        }
    }

    protected void GetMinMax(List<Vector3> verts, ref Vector2 min, ref Vector2 max)
    {
        if (verts.Count == 0)
        {
            return;
        }
        min = new Vector2(verts[0].x, verts[0].y);
        max = min;
        for (int i = 1; i < verts.Count; i++)
        {
            if (verts[i].x < min.x)
            {
                min.x = verts[i].x;
            }
            if (verts[i].y < min.y)
            {
                min.y = verts[i].y;
            }
            if (verts[i].x > max.x)
            {
                max.x = verts[i].x;
            }
            if (verts[i].y > max.y)
            {
                max.y = verts[i].y;
            }
        }
    }

    protected abstract void Modify(MeshData mesh);
}
