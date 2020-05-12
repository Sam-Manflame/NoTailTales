using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceDiagramGenerator : MonoBehaviour
{
    [SerializeField]
    private RectTransform fullSegment;

    [SerializeField]
    private float segmentOffset = 10;

    [SerializeField]
    private int min = 1;

    [SerializeField]
    private int max = 10;

    [SerializeField]
    private bool smoothGeneration = false;

    [SerializeField]
    private int smoothStep = 2;

    private const int diagramLength = 16;

    private List<RectTransform> segments = new List<RectTransform>();

    private void Start()
    {
        fullSegment.gameObject.SetActive(false);
        generate(min, max, smoothGeneration, smoothStep);
    }

    public void init(int min, int max, bool smoothGeneration, int smoothStep)
    {
        this.min = min;
        this.max = max;
        this.smoothGeneration = smoothGeneration;
        this.smoothStep = smoothStep;
    }

    public void generate()
    {
        generate(min, max, smoothGeneration, smoothStep);
    }

    public void generate(int min, int max, bool smooth, int smoothStep)
    {
        clear();

        int value = max;
        for (int i = 0; i < diagramLength; i++)
        {
            value = smooth ? getSmoothValue(value, min, max, smoothStep) : Random.Range(min, max + 1);
            RectTransform segmentRect = Instantiate(fullSegment, transform) as RectTransform;
            segmentRect.gameObject.SetActive(true);
            
           // сделано для чётного кол-ва сегментов TODO учёт нечётного
            Vector3 segmentPosition = new Vector3(
                (fullSegment.rect.width + segmentOffset) * (i - diagramLength / 2) + (segmentOffset + fullSegment.rect.width) / 2.0f, 
                fullSegment.localPosition.y + fullSegment.rect.height * (value / 10.0f) / 2,
                fullSegment.localPosition.z);
            segmentRect.localPosition = segmentPosition;

            Vector3 segmentScale = segmentRect.localScale;
            segmentScale.y = segmentScale.y * (value / 10.0f);
            segmentRect.localScale = segmentScale;

            segments.Add(segmentRect);
        }
    }

    /**
     * Возвращает "гладкое" значение(отличающееся от предыдущего максимум на smoothStep) в пределах min-max
     */
    private int getSmoothValue(int prevValue, int min, int max, int smoothStep)
    {
        return Random.Range(Mathf.Max(min, prevValue - smoothStep), Mathf.Min(max, prevValue + smoothStep) + 1);
    }

    private void clear()
    {
        for (int i = segments.Count - 1; i >= 0; i--)
        {
            RectTransform segment = segments[i];
            if (segment != fullSegment)
                Destroy(segment.gameObject);
        }

        segments.Clear();
    }
}
