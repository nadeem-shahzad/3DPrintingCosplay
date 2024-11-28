using System;
using System.IO;
using TMPro;
using UnityEngine;
using TriLibCore;
using TriLibCore.General;
using TriLibCore.Utils;
using UnityEngine.Serialization;

public class ExportModelManager : MonoBehaviour
{
    public GameObject m_ObjectToExport;
    public string m_OutputFileName = "ExportedModel.stl";
    public TMP_Text m_OutputText;

    public void ExportSTL()
    {
        if (m_ObjectToExport == null)
        {
            Debug.LogError("No GameObject assigned for export.");
            return;
        }

        MeshFilter meshFilter = m_ObjectToExport.GetComponent<MeshFilter>();
        if (meshFilter == null || meshFilter.mesh == null)
        {
            Debug.LogError("No MeshFilter or Mesh found on the GameObject.");
            return;
        }

        string outputPath = Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "ExportedModels");
        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }

        string filePath = Path.Combine(outputPath, m_OutputFileName);

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.Write(MeshToSTL(meshFilter.mesh));
        }

        Debug.Log($"Exported STL to: {filePath}");
        m_OutputText.text = "File Exported";
        m_OutputText.enabled = true;
    }

    private string MeshToSTL(Mesh mesh)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        sb.AppendLine("solid UnityMesh");
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 normal = Vector3.Cross(vertices[triangles[i + 1]] - vertices[triangles[i]],
                                           vertices[triangles[i + 2]] - vertices[triangles[i]]).normalized;

            sb.AppendLine($"facet normal {normal.x} {normal.y} {normal.z}");
            sb.AppendLine("  outer loop");
            sb.AppendLine($"    vertex {vertices[triangles[i]].x} {vertices[triangles[i]].y} {vertices[triangles[i]].z}");
            sb.AppendLine($"    vertex {vertices[triangles[i + 1]].x} {vertices[triangles[i + 1]].y} {vertices[triangles[i + 1]].z}");
            sb.AppendLine($"    vertex {vertices[triangles[i + 2]].x} {vertices[triangles[i + 2]].y} {vertices[triangles[i + 2]].z}");
            sb.AppendLine("  endloop");
            sb.AppendLine("endfacet");
        }

        sb.AppendLine("endsolid UnityMesh");
        return sb.ToString();
    }
}
