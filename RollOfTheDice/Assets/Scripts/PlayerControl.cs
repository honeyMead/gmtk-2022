using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
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

    public IList<EnvDieCollider> stickyDice = new List<EnvDieCollider>();
    public IList<RollingDie> rollingDice = new List<RollingDie>();

    void Start()
    {
        sideColliders = GameObject.FindGameObjectsWithTag("SideCollider")
            .Select(s => s.GetComponent<SideCollider>());
        OwnRigidbody = GetComponent<Rigidbody>();
        gameController = GameObject.FindWithTag("GameController")
            .GetComponent<GameController>();
        diceLogic = GetComponent<DiceLogic>();
        SetSideColliders();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            gameController.RestartLevel();
        }
        var isSlowMo = Time.timeScale != 1;
        var isFalling = !OwnRigidbody.isKinematic;
        if (isFalling && !isSlowMo)
        {
            if (transform.position.y < -5f)
            {
                gameController.RestartLevel();
            }
            return;
        }
        if (!diceLogic.IsMoving && !isSlowMo)
        {
            var direction = GetMoveDirectionFromKey();
            diceLogic.MoveIntoDirection(direction, () => MoveRollingDice(), () => FinishMovement());
        }
    }

    private Direction GetMoveDirectionFromKey()
    {
        var wantedDirection = GetWantedDirectionFromKey();
        if (wantedDirection == Direction.None)
        {
            return Direction.None;
        }
        SideCollider ownSide, oppositeSide;
        GetConcernedCollidersForDirection(wantedDirection, out ownSide, out oppositeSide);

        var actualMoveDirection = Direction.None;
        if (ownSide.IsSticking)
        {
            if (!topSide.IsColliding())
            {
                switch (wantedDirection)
                {
                    case Direction.Left:
                        actualMoveDirection = Direction.LeftUp;
                        break;
                    case Direction.Right:
                        actualMoveDirection = Direction.RightUp;
                        break;
                    case Direction.Forward:
                        actualMoveDirection = Direction.ForwardUp;
                        break;
                    case Direction.Back:
                        actualMoveDirection = Direction.BackUp;
                        break;
                }
            }
        }
        else if (!ownSide.IsColliding())
        {
            if (oppositeSide.IsSticking && !bottomSide.IsColliding())
            {
                switch (wantedDirection)
                {
                    case Direction.Left:
                        actualMoveDirection = Direction.LeftDown;
                        break;
                    case Direction.Right:
                        actualMoveDirection = Direction.RightDown;
                        break;
                    case Direction.Forward:
                        actualMoveDirection = Direction.ForwardDown;
                        break;
                    case Direction.Back:
                        actualMoveDirection = Direction.BackDown;
                        break;
                }
            }
            else
            {
                actualMoveDirection = wantedDirection;
            }
        }
        return actualMoveDirection;
    }

    private void GetConcernedCollidersForDirection(Direction wantedDirection, out SideCollider ownSide, out SideCollider oppositeSide)
    {
        ownSide = null;
        oppositeSide = null;
        switch (wantedDirection)
        {
            case Direction.Forward:
                ownSide = frontSide;
                oppositeSide = backSide;
                break;
            case Direction.Back:
                ownSide = backSide;
                oppositeSide = frontSide;
                break;
            case Direction.Left:
                ownSide = leftSide;
                oppositeSide = rightSide;
                break;
            case Direction.Right:
                ownSide = rightSide;
                oppositeSide = leftSide;
                break;
        }
    }

    private static Direction GetWantedDirectionFromKey()
    {
        var wantedDirection = Direction.None;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            wantedDirection = Direction.Forward;
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            wantedDirection = Direction.Back;
        }
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            wantedDirection = Direction.Left;
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            wantedDirection = Direction.Right;
        }

        return wantedDirection;
    }

    public void SideCollided(SideCollider side)
    {
        if (!OwnRigidbody.isKinematic)
        {
            var isBottomCollider = side == bottomSide;

            if (isBottomCollider || side.IsSticking)
            {
                OwnRigidbody.isKinematic = true;
                diceLogic.SettleInPlace();
            }
        }
    }

    private void FinishMovement()
    {
        CheckIfLaysOnGroundOrSticks();
        SetSideColliders();
    }

    private void MoveRollingDice()
    {
        foreach (var die in rollingDice)
        {
            die.Roll();
        }
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

    public void RevertMoveDirection()
    {
        diceLogic.RevertMoveDirection();
    }
}