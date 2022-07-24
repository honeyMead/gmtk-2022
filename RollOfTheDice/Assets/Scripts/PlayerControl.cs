using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private Vector3 relativePointForward = new(0, -0.5f, 0.5f);
    private Vector3 relativePointBack = new(0, -0.5f, -0.5f);
    private Vector3 relativePointLeft = new(-0.5f, -0.5f, 0);
    private Vector3 relativePointRight = new(0.5f, -0.5f, 0);

    private Vector3 relativePointLeftUp = new(-0.5f, 0.5f, 0);

    private Direction moveDirection;
    private bool isMoving = false;

    private Vector3 absoluteRotationPoint;
    private Vector3 rotationAxis;
    private float angleSign;
    private float rotationApplied;

    private IEnumerable<SideCollider> sideColliders;

    public Rigidbody OwnRigidbody { get; private set; }
    private GameController gameController;
    public IList<StickyDie> stickyDice = new List<StickyDie>();
    internal bool isLeftSideBlocked; // HACK to prevent rolling through sticky colliders with other side number

    void Start()
    {
        sideColliders = GameObject.FindGameObjectsWithTag("SideCollider")
            .Select(s => s.GetComponent<SideCollider>());
        OwnRigidbody = GetComponent<Rigidbody>();
        gameController = GameObject.FindWithTag("GameController")
            .GetComponent<GameController>();
    }

    void Update()
    {
        if (!OwnRigidbody.isKinematic)
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
                RoundPositionAndRotation();
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
                    relativeRotationPoint = relativePointForward;
                    break;
                case Direction.Back:
                    rotationAxis = Vector3.right;
                    angleSign = -1f;
                    relativeRotationPoint = relativePointBack;
                    break;
                case Direction.Left:
                    rotationAxis = Vector3.forward;
                    angleSign = 1f;
                    if (!isLeftSideBlocked)
                    {
                        relativeRotationPoint = relativePointLeft;
                        // TODO implement other cases
                        if (leftSticky != null)
                        {
                            relativeRotationPoint = relativePointLeftUp;
                        }
                    }
                    break;
                case Direction.Right:
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
            FinishMovement();
        }
    }

    private void FinishMovement()
    {
        moveDirection = Direction.None;
        isMoving = false;
        RoundPositionAndRotation();
        CheckIfLaysOnGround();
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