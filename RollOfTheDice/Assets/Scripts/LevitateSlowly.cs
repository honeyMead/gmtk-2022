using UnityEngine;

public class LevitateSlowly : MonoBehaviour
{
    private float startY;
    private float endY;
    private float targetY;
    private float speed;

    void Start()
    {
        startY = transform.position.y;
        var distance = Random.Range(0.1f, 0.175f);
        endY = transform.position.y - distance;
        targetY = endY;
        speed = distance / 8;
    }

    void FixedUpdate()
    {
        var tolerance = 0.01f;
        if (transform.position.y + tolerance >= startY)
        {
            targetY = endY;
        }
        else if (transform.position.y - tolerance <= endY)
        {
            targetY = startY;
        }
        var y = Mathf.Lerp(transform.position.y, targetY, speed);
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }
}
