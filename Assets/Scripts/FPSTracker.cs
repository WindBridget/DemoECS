using UnityEngine;
using UnityEngine.UI;

public class FPSTracker : MonoBehaviour
{
    public Text FPSText;
    float deltaTime;

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        FPSText.text = $"FPS: {fps:0}";
    }
}
