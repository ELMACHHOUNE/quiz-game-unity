using UnityEngine;

public class CharacterAnchor : MonoBehaviour
{
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        UpdatePosition();
    }

    void Update()
    {
        // Continuously anchor to bottom-left in case screen resizes
        UpdatePosition();
    }

    void UpdatePosition()
    {
        if (cam == null) return;
        // 0,0 is bottom left of viewport. Z is distance from camera.
        // We use an offset so the character isn't entirely offscreen.
        // x = 0.15 (15% from left), y = 0.2 (20% from bottom)
        Vector3 worldPos = cam.ViewportToWorldPoint(new Vector3(0.15f, 0.2f, Mathf.Abs(cam.transform.position.z)));
        worldPos.z = 0; // Keep it on the 2D plane
        
        // Only update X and Y, keep the same Z.
        transform.position = worldPos;
    }
}
