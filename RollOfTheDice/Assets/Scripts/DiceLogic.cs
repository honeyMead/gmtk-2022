using System;
using UnityEngine;

public class DiceLogic : MonoBehaviour
{
    public bool isSticky = false;
    public bool isFinish = false;

    public int FrontDotValue { get; private set; }
    public int BackDotValue { get; private set; }
    public int TopDotValue { get; private set; }
    public int BottomDotValue { get; private set; }
    public int RightDotValue { get; private set; }
    public int LeftDotValue { get; private set; }

    public bool IsMoving { get; private set; } = false;

    private const float D = 0.5f;
    private static readonly Vector3 sidePointLeft = new(-D, 0, 0);
    private static readonly Vector3 sidePointRight = new(D, 0, 0);
    private static readonly Vector3 sidePointFront = new(0, 0, D);
    private static readonly Vector3 sidePointBack = new(0, 0, -D);
    private static readonly Vector3 sidePointTop = new(0, D, 0);
    private static readonly Vector3 sidePointBottom = new(0, -D, 0);

    private Direction moveDirection;
    private Vector3 absoluteRotationPoint;
    private Vector3 rotationAxis;
    private float angleSign;
    private float rotationApplied;
    private Action actionAfterMovement;

    private const int SumOfOpposingDotValues = 7;

    private const int DefaultFrontValue = 5;
    private const int DefaultTopValue = 1;
    private const int DefaultRightValue = 3;
    private const int DefaultBackValue = SumOfOpposingDotValues - DefaultFrontValue;
    private const int DefaultBottomValue = SumOfOpposingDotValues - DefaultTopValue;
    private const int DefaultLeftValue = SumOfOpposingDotValues - DefaultRightValue;

    void Start()
    {
        SettleInPlace();
    }

    void Update()
    {
        if (IsMoving)
        {
            Roll();
        }
    }

    public void SettleInPlace()
    {
        RoundPositionAndRotation();
        SetDieDotValues();
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

    private void SetDieDotValues()
    {
        ResetDotValues();
        SetDotValuesDependingOnOrientation();
    }

    private void ResetDotValues()
    {
        FrontDotValue = 0;
        BackDotValue = 0;
        TopDotValue = 0;
        BottomDotValue = 0;
        RightDotValue = 0;
        LeftDotValue = 0;
    }

    private void SetDotValuesDependingOnOrientation()
    {
        var forwardDirection = GetVectorDirection(transform.forward);
        var backDirection = GetVectorDirection(-transform.forward);
        var upDirection = GetVectorDirection(transform.up);
        var downDirection = GetVectorDirection(-transform.up);
        var rightDirection = GetVectorDirection(transform.right);
        var leftDirection = GetVectorDirection(-transform.right);

        SetDotValueForDirection(forwardDirection, DefaultFrontValue);
        SetDotValueForDirection(backDirection, DefaultBackValue);
        SetDotValueForDirection(upDirection, DefaultTopValue);
        SetDotValueForDirection(downDirection, DefaultBottomValue);
        SetDotValueForDirection(rightDirection, DefaultRightValue);
        SetDotValueForDirection(leftDirection, DefaultLeftValue);
    }

    private Direction GetVectorDirection(Vector3 vecti)
    {
        var direction = Direction.None;
        var normalized = vecti.normalized;

        if (normalized == Vector3.forward)
        {
            direction = Direction.Forward;
        }
        else if (normalized == Vector3.back)
        {
            direction = Direction.Back;
        }
        else if (normalized == Vector3.up)
        {
            direction = Direction.Up;
        }
        else if (normalized == Vector3.down)
        {
            direction = Direction.Down;
        }
        else if (normalized == Vector3.right)
        {
            direction = Direction.Right;
        }
        else if (normalized == Vector3.left)
        {
            direction = Direction.Left;
        }
        else
        {
            Debug.LogWarning("Could not get die orientation of " + name);
        }
        return direction;
    }

    private void SetDotValueForDirection(Direction direction, int defaultValue)
    {
        switch (direction)
        {
            case Direction.Forward:
                FrontDotValue = defaultValue;
                break;
            case Direction.Back:
                BackDotValue = defaultValue;
                break;
            case Direction.Left:
                LeftDotValue = defaultValue;
                break;
            case Direction.Right:
                RightDotValue = defaultValue;
                break;
            case Direction.Up:
                TopDotValue = defaultValue;
                break;
            case Direction.Down:
                BottomDotValue = defaultValue;
                break;
        }
    }
    public void MoveIntoDirection(Direction direction, Action finishMovementAction)
    {
        if (direction == Direction.None)
        {
            return;
        }
        moveDirection = direction;
        actionAfterMovement = finishMovementAction;

        Vector3? ownSidePoint = null;
        Vector3? oppositeSidePoint = null;

        switch (moveDirection)
        {
            case Direction.Forward:
            case Direction.ForwardUp:
            case Direction.ForwardDown:
                rotationAxis = Vector3.right;
                angleSign = 1f;
                ownSidePoint = sidePointFront;
                oppositeSidePoint = sidePointBack;
                break;
            case Direction.Back:
            case Direction.BackUp:
            case Direction.BackDown:
                rotationAxis = Vector3.right;
                angleSign = -1f;
                ownSidePoint = sidePointBack;
                oppositeSidePoint = sidePointFront;
                break;
            case Direction.Left:
            case Direction.LeftUp:
            case Direction.LeftDown:
                rotationAxis = Vector3.forward;
                angleSign = 1f;
                ownSidePoint = sidePointLeft;
                oppositeSidePoint = sidePointRight;
                break;
            case Direction.Right:
            case Direction.RightUp:
            case Direction.RightDown:
                rotationAxis = Vector3.forward;
                angleSign = -1f;
                ownSidePoint = sidePointRight;
                oppositeSidePoint = sidePointLeft;
                break;
        }
        Vector3? relativeRotationPoint = null;

        switch (direction)
        {
            case Direction.Forward:
            case Direction.Back:
            case Direction.Left:
            case Direction.Right:
                relativeRotationPoint = ownSidePoint.Value + sidePointBottom;
                break;
            case Direction.ForwardUp:
            case Direction.BackUp:
            case Direction.LeftUp:
            case Direction.RightUp:
                relativeRotationPoint = ownSidePoint.Value + sidePointTop;
                break;
            case Direction.ForwardDown:
            case Direction.BackDown:
            case Direction.LeftDown:
            case Direction.RightDown:
                relativeRotationPoint = oppositeSidePoint.Value + sidePointBottom;
                break;
        }
        if (relativeRotationPoint.HasValue)
        {
            absoluteRotationPoint = transform.position + relativeRotationPoint.Value;
            rotationApplied = 0;
            IsMoving = true;
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
            SettleInPlace();
            actionAfterMovement();
            moveDirection = Direction.None;
            IsMoving = false;
        }
    }
}
