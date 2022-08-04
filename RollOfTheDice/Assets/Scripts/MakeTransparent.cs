using UnityEngine;

public class MakeTransparent : MonoBehaviour
{
    private Material dieMat;
    private Color defaultColor;
    private Color transparentColor;

    void Start()
    {
        dieMat = GetComponent<Renderer>().material;
        defaultColor = dieMat.color;
        transparentColor = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 0.02f);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.X))
        {
            dieMat.color = transparentColor;
        }
        else
        {
            dieMat.color = defaultColor;
        }
    }
}
