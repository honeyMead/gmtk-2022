using UnityEngine;

public class CamControl : MonoBehaviour
{
    private Vector3 defaultPosition;
    private Vector3 mirroredPosition;

    private Quaternion defaultRotation;
    private Quaternion mirroredRotation;

    void Start()
    {
        defaultPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        mirroredPosition = new Vector3(-transform.position.x, transform.position.y, transform.position.z);

        defaultRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        mirroredRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, -transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            transform.SetPositionAndRotation(mirroredPosition, mirroredRotation);
        }
        else
        {
            transform.SetPositionAndRotation(defaultPosition, defaultRotation);
        }
    }
}
