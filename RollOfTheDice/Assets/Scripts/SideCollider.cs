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
        var die = GetEnvDieParent(other);
        touchingDie = die;

        player.SideCollided(transform);

        if (die.isSticky)
        {
            var stickyDie = other.GetComponent<EnvDieCollider>();
            if (stickyDie.sideDotValue == dotValue)
            {
                player.stickyDice.Add(stickyDie);
                IsSticking = true;
            }
        }
        if (die.isFinish)
        {
            var finishDie = other.GetComponent<EnvDieCollider>();

            if (finishDie.sideDotValue == dotValue)
            {
                gameController.WinScene();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(CollisionTag))
        {
            return;
        }
        var die = GetEnvDieParent(other);
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

    private static DiceLogic GetEnvDieParent(Collider other)
    {
        return other.transform.parent.parent.GetComponent<DiceLogic>();
    }

    public bool IsColliding()
    {
        return touchingDie != null;
    }
}
