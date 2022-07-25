using System.Collections.Generic;
using UnityEngine;

public class SideCollider : MonoBehaviour
{
    public int dotValue;
    private GameController gameController;
    private PlayerControl player;
    public bool IsSticking { get; private set; } = false;

    public DiceLogic TouchingDie { get; private set; } = null;

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
            TouchingDie = die;
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
                    player.stickyDice.Add(stickyDie);
                    IsSticking = true;
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Die"))
        {
            TouchingDie = null;
            var die = other.GetComponent<DiceLogic>();

            if (die.isSticky)
            {
                var stickyDie = other.gameObject.GetComponent<StickyDie>();
                player.stickyDice.Remove(stickyDie);
                IsSticking = false;
            }
        }
    }
}
