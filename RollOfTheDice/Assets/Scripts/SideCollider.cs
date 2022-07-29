using UnityEngine;

public class SideCollider : MonoBehaviour
{
    public int dotValue;
    public bool IsSticking { get; private set; } = false;
    private DiceLogic touchingDie = null;

    private GameController gameController;
    private PlayerControl player;
    private const string CollisionTag = "EnvDieCollider";

    void Start()
    {
        gameController = GameObject.FindWithTag("GameController")
            .GetComponent<GameController>();
        player = GameObject.FindWithTag("Player")
            .GetComponent<PlayerControl>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(CollisionTag))
        {
            return;
        }
        var die = GetEnvDieLogic(other);
        touchingDie = die;

        player.SideCollided(transform);
        var otherCollider = other.GetComponent<EnvDieCollider>();

        if (die.isSticky)
        {
            if (otherCollider.sideDotValue == dotValue)
            {
                player.stickyDice.Add(otherCollider);
                IsSticking = true;
            }
        }
        if (die.isFinish)
        {
            if (otherCollider.sideDotValue == dotValue)
            {
                gameController.WinScene();
            }
        }
        if (die.isRollingDie)
        {
            var collidingSideDotValue = otherCollider.sideDotValue;

            if (collidingSideDotValue != dotValue)
            {
                player.RevertMoveDirection();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(CollisionTag))
        {
            return;
        }
        var die = GetEnvDieLogic(other);
        if (die == touchingDie) // when falling, a side collider can touch two dice colliders in parallel
        {
            touchingDie = null;
        }

        if (die.isSticky)
        {
            var stickyDie = other.GetComponent<EnvDieCollider>();
            player.stickyDice.Remove(stickyDie);
            IsSticking = false;
        }
    }

    private static DiceLogic GetEnvDieLogic(Collider other)
    {
        return other.transform.parent.parent.GetComponent<DiceLogic>();
    }

    public bool IsColliding()
    {
        return touchingDie != null;
    }
}
