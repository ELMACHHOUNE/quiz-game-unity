using UnityEngine;

public class CharacterAnchor : MonoBehaviour
{
    [SerializeField] private float viewportX = 0.15f;
    [SerializeField] private float viewportY = 0.2f;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        UpdatePosition();
    }

    void Update()
    {
        UpdatePosition();
    }

    public void SetViewportPosition(float x, float y)
    {
        viewportX = x;
        viewportY = y;
    }

    void UpdatePosition()
    {
        if (cam == null) return;
        Vector3 worldPos = cam.ViewportToWorldPoint(new Vector3(viewportX, viewportY, Mathf.Abs(cam.transform.position.z)));
        worldPos.z = 0;
        transform.position = worldPos;
    }
}
