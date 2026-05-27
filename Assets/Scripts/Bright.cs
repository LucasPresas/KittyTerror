using UnityEngine;

public class GlowPulse : MonoBehaviour
{
    private Material mat;

    public float speed = 2f;

    public float minGlow = 0.1f;
    public float maxGlow = 1.2f;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
    }

    void Update()
    {
        float pulse = Mathf.Lerp(
            minGlow,
            maxGlow,
            (Mathf.Sin(Time.time * speed) + 1f) / 2f
        );

        Color finalColor = Color.white * pulse;

        mat.SetColor("_EmissionColor", finalColor);
    }
}