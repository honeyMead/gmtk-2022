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

    private static readonly Vector3 rotationPointForward = sidePointFront + sidePointBottom;
    private static readonly Vector3 rotationPointBack = sidePointBack + sidePointBottom;
    private static readonly Vector3 rotationPointLeft = sidePointLeft + sidePointBottom;
    private static readonly Vector3 rotationPointRight = sidePointRight + sidePointBottom;

    private static readonly Vector3 rotationPointFrontUp = sidePointFront + sidePointTop;
    private static readonly Vector3 rotationPointBackUp = sidePointBack + sidePointTop;
    private static readonly Vector3 rotationPointLeftUp = sidePointLeft + sidePointTop;
    private static readonly Vector3 rotationPointRightUp = sidePointRight + sidePointTop;

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
    internal bool isLeftSideBlocked; // HACK to prevent rolling through sticky colliders with other side number

    void Start()
    {
        sideColliders = GameObject.FindGameObjectsWithTag("SideCollider")
            .Select(s => s.GetComponent<SideCollider>());
        SetSideColliders();
        OwnRigidbody = GetComponent<Rigidbody>();
        gameController = GameObject.FindWithTag("GameController")
            .GetComponent<GameController>();
        diceLogic = GetComponent<DiceLogic>();
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

    public void SideCollided(Transform sideCollider)
    {
        if (!OwnRigidbody.isKinematic)
        {
            var isBottomCollider = (sideCollider.position - transform.position).y < -0.1f;
            if (isBottomCollider)
            {
                OwnRigidbody.isKinematic = true;
                diceLogic.RoundPositionAndRotation();
            }
        }
    }

    private void SetCurrentKeyIfApplicable(KeyCode key)
    {
        StickyDie leftSticky = null;
        StickyDie rightSticky = null;
        StickyDie frontSticky = null;
        StickyDie backSticky = null;

        foreach (var stickyDie in stickyDice)
        {
            var direction = stickyDie.transform.position - transform.position;
            if (direction.x < 0)
            {
                leftSticky = stickyDie;
                break;
            }
            if (direction.x > 0)
            {
                rightSticky = stickyDie;
                break;
            }
            if (direction.z > 0)
            {
                frontSticky = stickyDie;
                break;
            }
            if (direction.z < 0)
            {
                backSticky = stickyDie;
                break;
            }
        }

        if (moveDirection != Direction.None)
        {
            isMoving = true;
            Vector3? relativeRotationPoint = null;

            switch (moveDirection)
            {
                case Direction.Forward:
                    rotationAxis = Vector3.right;
                    angleSign = 1f;
                    relativeRotationPoint = rotationPointForward;
                    break;
                case Direction.Back:
                    rotationAxis = Vector3.right;
                    angleSign = -1f;
                    relativeRotationPoint = rotationPointBack;
                    break;
                case Direction.Left:
                    rotationAxis = Vector3.forward;
                    angleSign = 1f;
                    if (!isLeftSideBlocked)
                    {
                        relativeRotationPoint = rotationPointLeft;
                        // TODO implement other cases
                        if (leftSticky != null)
                        {
                            relativeRotationPoint = rotationPointLeftUp;
                        }
                    }
                    break;
                case Direction.Right:
                    rotationAxis = Vector3.forward;
                    angleSign = -1f;
                    relativeRotationPoint = rotationPointRight;
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
            FinishMovement();
        }
    }

    private void FinishMovement()
    {
        diceLogic.RoundPositionAndRotation();
        CheckIfLaysOnGround();
        SetSideColliders();
        moveDirection = Direction.None;
        isMoving = false;
    }

    private void CheckIfLaysOnGround()
    {
        var collidingSides = sideColliders.Where(s => s.IsColliding());
        var bottomCollider = collidingSides
            .SingleOrDefault(s => (s.transform.position - transform.position).y < -0.1f);

        var isNotGrounded = bottomCollider == null;

        if (isNotGrounded)
        {
            var isSticking = collidingSides.Any(s => s.IsSticking());

            if (!isSticking)
            {
                OwnRigidbody.isKinematic = false;
            }
        }
    }

    private void SetSideColliders() // TODO use SetDieDotValues from dice logic to first set all dot values and then get the right collider with the number
    {
        foreach (var side in sideColliders)
        {
            var relativeSidePosition = side.transform.position - this.transform.position;
            if (relativeSidePosition == sidePointLeft)
            {
                leftSide = side;
            }
            else if (relativeSidePosition == sidePointRight)
            {
                rightSide = side;
            }
            else if (relativeSidePosition == sidePointFront)
            {
                frontSide = side;
            }
            else if (relativeSidePosition == sidePointBack)
            {
                backSide = side;
            }
            else if (relativeSidePosition == sidePointTop)
            {
                topSide = side;
            }
            else if (relativeSidePosition == sidePointBottom)
            {
                bottomSide = side;
            }
            else
            {
                Debug.LogWarning($"Could not set side collider. Relative position: " +
                    $"x: {relativeSidePosition.x}, y: {relativeSidePosition.y}, z: {relativeSidePosition.z}");
            }
        }
    }
}