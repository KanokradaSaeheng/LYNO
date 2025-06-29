using UnityEngine;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class WarpTextEditor : MonoBehaviour
{
    public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public float curveScale = 10;

    TMP_Text m_TextComponent;
    Mesh mesh;
    Vector3[] vertices;

    void Awake()
    {
        m_TextComponent = GetComponent<TMP_Text>();
    }

    void OnEnable()
    {
        UpdateCurve();
    }

    void OnValidate()
    {
        UpdateCurve();
    }

    void Update()
    {
        if (!Application.isPlaying)
            UpdateCurve();
    }

    void UpdateCurve()
    {
        m_TextComponent.ForceMeshUpdate();

        mesh = m_TextComponent.mesh;
        vertices = mesh.vertices;

        int characterCount = m_TextComponent.textInfo.characterCount;
        if (characterCount == 0)
            return;

        for (int i = 0; i < characterCount; i++)
        {
            var charInfo = m_TextComponent.textInfo.characterInfo[i];

            if (!charInfo.isVisible)
                continue;

            int vertexIndex = charInfo.vertexIndex;

            Vector3 offsetToMidBaseline = new Vector2(
                (vertices[vertexIndex + 0].x + vertices[vertexIndex + 2].x) / 2,
                charInfo.baseLine
            );

            float x0 = (offsetToMidBaseline.x - m_TextComponent.bounds.min.x) / m_TextComponent.bounds.size.x;
            float y0 = curve.Evaluate(x0) * curveScale;

            Vector3 offset = new Vector3(0, y0, 0);

            vertices[vertexIndex + 0] += offset;
            vertices[vertexIndex + 1] += offset;
            vertices[vertexIndex + 2] += offset;
            vertices[vertexIndex + 3] += offset;
        }

        mesh.vertices = vertices;
        m_TextComponent.canvasRenderer.SetMesh(mesh);
    }
}
