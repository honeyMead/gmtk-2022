using UnityEngine;

public class CamControl : MonoBehaviour
{
    private Vector3 defaultPosition;
    private Vector3 otherPosition;

    private Quaternion defaultRotation;
    private Quaternion otherRotation;

    void Start()
    {
        defaultPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        otherPosition = new Vector3(transform.position.x * 2, transform.position.y, transform.position.z);

        defaultRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        otherRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y * 2, transform.rotation.eulerAngles.z);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            transform.SetPositionAndRotation(otherPosition, otherRotation);
        }
        else
        {
            transform.SetPositionAndRotation(defaultPosition, defaultRotation);
        }
    }
}
