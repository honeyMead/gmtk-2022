using UnityEngine;

public class CamControl : MonoBehaviour
{
    private float initialX;
    private float initialYRotation;

    void Start()
    {
        initialX = transform.position.x;
        initialYRotation = transform.rotation.eulerAngles.y;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            transform.SetPositionAndRotation(new Vector3(-initialX, transform.position.y, transform.position.z),
                Quaternion.Euler(transform.rotation.eulerAngles.x, -initialYRotation, transform.rotation.eulerAngles.z));
        }
        else
        {
            transform.SetPositionAndRotation(new Vector3(initialX, transform.position.y, transform.position.z),
                Quaternion.Euler(transform.rotation.eulerAngles.x, initialYRotation, transform.rotation.eulerAngles.z));
        }
    }
}
