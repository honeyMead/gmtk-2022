using System.Collections.Generic;
using UnityEngine;

public class SideCollider : MonoBehaviour
{
    public int dotValue;
    private GameController gameController;
    private PlayerControl player;
    public bool IsSticking { get; private set; } = false;

    public IList<DiceLogic> TouchingDice { get; private set; } = new List<DiceLogic>();

    void Start()
    {
        gameController = GameObject.FindWithTag("GameController")
            .GetComponent<GameController>();
        player = GameObject.FindWithTag("Player")
            .GetComponent<PlayerControl>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Die"))
        {
            var die = other.GetComponent<DiceLogic>();
            TouchingDice.Add(die);
        }

        if (!other.CompareTag("Player"))
        {
            player.SideCollided(transform);
        }

        if (other.CompareTag("Die"))
        {
            var die = other.GetComponent<DiceLogic>();
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
                    player.isLeftSideBlocked = false;
                    player.stickyDice.Add(stickyDie);
                    IsSticking = true;
                }
                else
                {
                    player.isLeftSideBlocked = true;
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Die"))
        {
            var die = other.GetComponent<DiceLogic>();
            TouchingDice.Remove(die);

            if (die.isSticky)
            {
                player.isLeftSideBlocked = false;
                var stickyDie = other.gameObject.GetComponent<StickyDie>();
                player.stickyDice.Remove(stickyDie);
                IsSticking = false;
            }
        }
    }

    public bool IsColliding()
    {
        return TouchingDice.Count > 0;
    }
}