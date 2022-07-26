using UnityEngine;

public class SideCollider : MonoBehaviour
{
    public int dotValue;
    public bool IsSticking { get; private set; } = false;
    private DiceLogic touchingDie = null;

    private GameController gameController;
    private PlayerControl player;

    void Start()
    {
        gameController = GameObject.FindWithTag("GameController")
            .GetComponent<GameController>();
        player = GameObject.FindWithTag("Player")
            .GetComponent<PlayerControl>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Die"))
        {
            return;
        }
        var die = other.GetComponent<DiceLogic>();
        touchingDie = die;

        player.SideCollided(transform);

        if (die.isFinish)
        {
            var finish = other.GetComponent<Finish>();

            if (finish.winValue == dotValue)
            {
                finish.Win();
                gameController.LoadNextLevel();
            }
        }
        else if (die.isSticky)
        {
            var stickyDie = other.gameObject.GetComponent<StickyDie>();
            if (stickyDie.sideValue == dotValue)
            {
                player.stickyDice.Add(stickyDie);
                IsSticking = true;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Die"))
        {
            return;
        }
        var die = other.GetComponent<DiceLogic>();
        if (die == touchingDie) // when falling, a side collider can touch two dice colliders in parallel
        {
            touchingDie = null;
        }

        if (die.isSticky)
        {
            var stickyDie = other.gameObject.GetComponent<StickyDie>();
            player.stickyDice.Remove(stickyDie);
            IsSticking = false;
        }
    }

    public bool IsColliding()
    {
        return touchingDie != null;
    }
}
