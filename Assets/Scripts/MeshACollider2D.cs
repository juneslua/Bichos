using UnityEngine;
using System.Collections;

public class MeshACollider2D : MonoBehaviour
{
    private Mesh _mesh;
    private PolygonCollider2D _col2D;

    void Awake()
    {
        _mesh = GetComponent<MeshFilter>().sharedMesh;
        _col2D = GetComponent<PolygonCollider2D>();

        int[] triangulos = _mesh.triangles;
        Vector3[] vertices = _mesh.vertices;

        _col2D.pathCount = triangulos.Length / 3;

        for (int t = 0, i = 0; t < triangulos.Length; t += 3, i++)
        {
            Vector2[] p = new Vector2[3];
            p[0] = vertices[triangulos[t]];
            p[1] = vertices[triangulos[t + 1]];
            p[2] = vertices[triangulos[t + 2]];

            _col2D.SetPath(i, p);
        }
    }
}
