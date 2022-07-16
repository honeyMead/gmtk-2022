using UnityEngine;

public class NumberCollider : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Finish"))
        {
            Debug.Log("Finish");
        }
    }
}
