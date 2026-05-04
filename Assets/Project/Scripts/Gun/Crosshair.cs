using UnityEngine;

public class Crosshair : MonoBehaviour
{
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private SpriteRenderer centerDot;
    [SerializeField] private Color dotHighLightColor;

    private Color _originalDotColor;

    private void Start()
    {
        _originalDotColor = centerDot.color;
        Cursor.visible = false;
    }

    private void Update()
    {
        transform.Rotate(Vector3.up * 40f * Time.deltaTime, Space.World);
    }

    public void OnTargetDetect(Ray ray)
    {
        if(Physics.Raycast(ray, 100f, targetLayer))
        {
            centerDot.color = dotHighLightColor;
        }
        else
        {
            centerDot.color = _originalDotColor;
        }
    }
}