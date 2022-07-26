using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private const float D = 0.5f;
    private static readonly Vector3 sidePointLeft = new(-D, 0, 0);
    private static readonly Vector3 sidePointRight = new(D, 0, 0);
    private static readonly Vector3 sidePointFront = new(0, 0, D);
    private static readonly Vector3 sidePointBack = new(0, 0, -D);
    private static readonly Vector3 sidePointTop = new(0, D, 0);
    private static readonly Vector3 sidePointBottom = new(0, -D, 0);

    private Direction moveDirection;
    private bool isMoving = false;

    private Vector3 absoluteRotationPoint;
    private Vector3 rotationAxis;
    private float angleSign;
    private float rotationApplied;

    private IEnumerable<SideCollider> sideColliders;
    private SideCollider leftSide;
    private SideCollider rightSide;
    private SideCollider frontSide;
    private SideCollider backSide;
    private SideCollider topSide;
    private SideCollider bottomSide;

    public Rigidbody OwnRigidbody { get; private set; }
    private GameController gameController;
    private DiceLogic diceLogic;
    public IList<StickyDie> stickyDice = new List<StickyDie>();

    void Start()
    {
        sideColliders = GameObject.FindGameObjectsWithTag("SideCollider")
            .Select(s => s.GetComponent<SideCollider>());
        OwnRigidbody = GetComponent<Rigidbody>();
        gameController = GameObject.FindWithTag("GameController")
            .GetComponent<GameController>();
        diceLogic = GetComponent<DiceLogic>();
        diceLogic.SettleInPlace();
        SetSideColliders();
    }

    void Update()
    {
        var isFalling = !OwnRigidbody.isKinematic;
        if (isFalling)
        {
            if (transform.position.y < -5f)
            {
                gameController.RestartLevel();
            }
            return;
        }
        if (!isMoving)
        {
            SetMoveDirectionFromPressedKey();
            SetMovementRotationPoint();
        }
        if (isMoving)
        {
            Roll();
        }
    }

    private void SetMoveDirectionFromPressedKey()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            moveDirection = Direction.Forward;
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            moveDirection = Direction.Back;
        }
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            moveDirection = Direction.Left;
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            moveDirection = Direction.Right;
        }
    }

    public void SideCollided(Transform sideCollider) // TODO handle falling without unity physic engine
    {
        if (!OwnRigidbody.isKinematic)
        {
            var side = sideCollider.GetComponent<SideCollider>();
            var isBottomCollider = side == bottomSide;

            if (isBottomCollider)
            {
                OwnRigidbody.isKinematic = true;
                diceLogic.SettleInPlace();
            }
        }
    }

    private void SetMovementRotationPoint()
    {
        if (moveDirection == Direction.None)
        {
            return;
        }

        Vector3? ownSidePoint = null;
        Vector3? oppositeSidePoint = null;
        SideCollider ownSide = null;
        SideCollider oppositeSide = null;

        switch (moveDirection)
        {
            case Direction.Forward:
                rotationAxis = Vector3.right;
                angleSign = 1f;
                ownSidePoint = sidePointFront;
                oppositeSidePoint = sidePointBack;
                ownSide = frontSide;
                oppositeSide = backSide;
                break;
            case Direction.Back:
                rotationAxis = Vector3.right;
                angleSign = -1f;
                ownSidePoint = sidePointBack;
                oppositeSidePoint = sidePointFront;
                ownSide = backSide;
                oppositeSide = frontSide;
                break;
            case Direction.Left:
                rotationAxis = Vector3.forward;
                angleSign = 1f;
                ownSidePoint = sidePointLeft;
                oppositeSidePoint = sidePointRight;
                ownSide = leftSide;
                oppositeSide = rightSide;
                break;
            case Direction.Right:
                rotationAxis = Vector3.forward;
                angleSign = -1f;
                ownSidePoint = sidePointRight;
                oppositeSidePoint = sidePointLeft;
                ownSide = rightSide;
                oppositeSide = leftSide;
                break;
        }
        Vector3? relativeRotationPoint = null;
        if (ownSide.IsSticking)
        {
            relativeRotationPoint = ownSidePoint.Value + sidePointTop;
        }
        else if (!ownSide.IsColliding())
        {
            if (oppositeSide.IsSticking)
            {
                relativeRotationPoint = oppositeSidePoint.Value + sidePointBottom;
            }
            else
            {
                relativeRotationPoint = ownSidePoint.Value + sidePointBottom;
            }
        }
        if (relativeRotationPoint.HasValue)
        {
            absoluteRotationPoint = transform.position + relativeRotationPoint.Value;
            rotationApplied = 0;
            isMoving = true;
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
            FinishMovement();
        }
    }

    private void FinishMovement()
    {
        diceLogic.SettleInPlace();
        CheckIfLaysOnGroundOrSticks();
        SetSideColliders();
        moveDirection = Direction.None;
        isMoving = false;
    }

    private void CheckIfLaysOnGroundOrSticks()
    {
        var collidingSides = sideColliders.Where(s => s.IsColliding());
        var bottomCollider = collidingSides
            .SingleOrDefault(s => (s.transform.position - transform.position).y < -0.1f);

        var isNotGrounded = bottomCollider == null;

        if (isNotGrounded)
        {
            var isSticking = collidingSides.Any(s => s.IsSticking);

            if (!isSticking)
            {
                OwnRigidbody.isKinematic = false;
            }
        }
    }

    private void SetSideColliders()
    {
        foreach (var side in sideColliders)
        {
            if (side.dotValue == diceLogic.TopDotValue)
            {
                topSide = side;
            }
            else if (side.dotValue == diceLogic.BottomDotValue)
            {
                bottomSide = side;
            }
            else if (side.dotValue == diceLogic.LeftDotValue)
            {
                leftSide = side;
            }
            else if (side.dotValue == diceLogic.RightDotValue)
            {
                rightSide = side;
            }
            else if (side.dotValue == diceLogic.FrontDotValue)
            {
                frontSide = side;
            }
            else if (side.dotValue == diceLogic.BackDotValue)
            {
                backSide = side;
            }
            else
            {
                Debug.LogWarning($"Could not set side collider.");
            }
        }
    }
}