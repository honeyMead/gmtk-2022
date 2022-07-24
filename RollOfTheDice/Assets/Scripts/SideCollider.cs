using UnityEngine;

public class SideCollider : MonoBehaviour
{
    public int side;
    private GameController gameController;
    private PlayerControl player;
    private int touchingColliders = 0;
    private bool isSticking = false;

    void Start()
    {
        gameController = GameObject.FindWithTag("GameController")
            .GetComponent<GameController>();
        player = GameObject.FindWithTag("Player")
            .GetComponent<PlayerControl>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            player.SideCollided(transform);
        }

        if (other.CompareTag("Finish"))
        {
            var finish = other.GetComponent<Finish>();

            if (finish.winValue == side)
            {
                finish.Win();
                gameController.LoadNextLevel();
            }
        }
        else if (other.CompareTag("StickyDie"))
        {
            var stickyDie = other.gameObject.GetComponent<StickyDie>();
            if (stickyDie.sideValue == side)
            {
                player.isLeftSideBlocked = false;
                player.stickyDice.Add(stickyDie);
                isSticking = true;
            }
            else
            {
                player.isLeftSideBlocked = true;
            }
        }
        if (!other.CompareTag("Player"))
        {
            touchingColliders++;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("StickyDie"))
        {
            player.isLeftSideBlocked = false;
            var stickyDie = other.gameObject.GetComponent<StickyDie>();
            player.stickyDice.Remove(stickyDie);
            isSticking = false;
        }
        touchingColliders--;
        if (touchingColliders < 0)
        {
            touchingColliders = 0;
        }
    }

    public bool IsColliding()
    {
        return touchingColliders > 0;
    }

    public bool IsSticking()
    {
        return isSticking;
    }
}
