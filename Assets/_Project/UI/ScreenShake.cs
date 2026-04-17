using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Shakes the Canvas by offsetting its RectTransform position.
/// </summary>
public class ScreenShake : MonoBehaviour
{
    [SerializeField] private RectTransform _target;
    private Vector2 _originalPosition;
    private Coroutine _shakeCoroutine;

    void Awake()
    {
        Debug.Assert(_target != null, "ScreenShake: Canvas RectTransform reference is missing. Please assign it in the Inspector.");
        if (_target == null) return;
        _originalPosition = _target.localPosition;
    }

    public float Shake(float duration, float magnitude)
    {
        if (_shakeCoroutine != null)
            StopCoroutine(_shakeCoroutine);
        _shakeCoroutine = StartCoroutine(ShakeRoutine(duration, magnitude));
        return duration;
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        float elapsed = 0f;
        float frequency = 35f;

        //initial kick
        _target.anchoredPosition = _originalPosition + Random.insideUnitCircle * magnitude;
        yield return null;

        while (elapsed < duration)
        {
            float progress = elapsed / duration;

            // fast decay
            float damper = 1f - Mathf.Pow(progress, 2f);

            // smooth pseudo-random motion
            float x = (Mathf.PerlinNoise(Time.time * frequency, 0f) * 2f - 1f);
            float y = (Mathf.PerlinNoise(0f, Time.time * frequency) * 2f - 1f);

            Vector2 offset = new Vector2(x, y) * magnitude * damper;

            _target.anchoredPosition = _originalPosition + offset;

            elapsed += Time.deltaTime;
            yield return null;
        }

        _target.anchoredPosition = _originalPosition;
    }
}