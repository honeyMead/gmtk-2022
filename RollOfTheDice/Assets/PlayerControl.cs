using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private Vector3 relativePointForward = new(0, -0.5f, 0.5f);
    private Vector3 relativePointBack = new(0, -0.5f, -0.5f);
    private Vector3 relativePointLeft = new(-0.5f, -0.5f, 0);
    private Vector3 relativePointRight = new(0.5f, -0.5f, 0);

    private KeyCode currentKey;
    private bool isMoving = false;

    private Vector3 absoluteRotationPoint;
    private Vector3 rotationAxis;
    private float angleSign;
    private float rotationApplied;

    void Update()
    {
        if (!isMoving)
        {
            SetCurrentKeyIfApplicable(KeyCode.W);
            SetCurrentKeyIfApplicable(KeyCode.S);
            SetCurrentKeyIfApplicable(KeyCode.A);
            SetCurrentKeyIfApplicable(KeyCode.D);
        }
        if (isMoving)
        {
            Roll();
        }
    }

    private void SetCurrentKeyIfApplicable(KeyCode key)
    {
        if (Input.GetKey(key))
        {
            currentKey = key;
            isMoving = true;
            Vector3? relativeRotationPoint = null;

            switch (currentKey)
            {
                case KeyCode.W:
                    rotationAxis = Vector3.right;
                    angleSign = 1f;
                    relativeRotationPoint = relativePointForward;
                    break;
                case KeyCode.S:
                    rotationAxis = Vector3.right;
                    angleSign = -1f;
                    relativeRotationPoint = relativePointBack;
                    break;
                case KeyCode.A:
                    rotationAxis = Vector3.forward;
                    angleSign = 1f;
                    relativeRotationPoint = relativePointLeft;
                    break;
                case KeyCode.D:
                    rotationAxis = Vector3.forward;
                    angleSign = -1f;
                    relativeRotationPoint = relativePointRight;
                    break;
            }
            if (relativeRotationPoint.HasValue)
            {
                absoluteRotationPoint = transform.position + relativeRotationPoint.Value;
                rotationApplied = 0;
            }
        }
    }

    private void Roll()
    {
        var targetRotationAngle = 90f * angleSign;
        var lerpValue = Time.deltaTime * 3;
        var lerpedAngle = Mathf.LerpAngle(0, targetRotationAngle, lerpValue);
        transform.RotateAround(absoluteRotationPoint, rotationAxis, lerpedAngle);
        rotationApplied += lerpValue;

        if (rotationApplied >= 1)
        {
            currentKey = KeyCode.None;
            isMoving = false;
            RoundPositionAndRotation();
        }
    }

    private void RoundPositionAndRotation()
    {
        var xRotation = RoundToNextOfNinety(transform.rotation.eulerAngles.x);
        var yRotation = RoundToNextOfNinety(transform.rotation.eulerAngles.y);
        var zRotation = RoundToNextOfNinety(transform.rotation.eulerAngles.z);
        var roundedRotation = Quaternion.Euler(xRotation, yRotation, zRotation);

        var x = Mathf.Round(transform.position.x);
        var y = Mathf.Round(transform.position.y);
        var z = Mathf.Round(transform.position.z);
        var roundedPosition = new Vector3(x, y, z);

        transform.SetPositionAndRotation(roundedPosition, roundedRotation);
    }

    private float RoundToNextOfNinety(float a)
    {
        return Mathf.Round(a / 90) * 90;
    }
}