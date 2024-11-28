using System;
using UnityEngine;

public class GizmoInteraction : MonoBehaviour
{
    private void OnMouseDown()
    {
        GizmoManager.s_Instance.InitializeGizmo(this.transform);
    }
}
