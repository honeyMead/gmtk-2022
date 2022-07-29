using UnityEngine;

public class RollingDie : MonoBehaviour
{
    public Direction rollDirection;
    private DiceLogic diceLogic;

    void Start()
    {
        var player = GameObject.FindWithTag("Player")
            .GetComponent<PlayerControl>();
        player.rollingDice.Add(this);
        diceLogic = GetComponent<DiceLogic>();
        diceLogic.AllowOnlySelfRotation();
    }

    public void Roll()
    {
        // TODO rotate before player
        diceLogic.MoveIntoDirection(rollDirection, () => { });
    }
}
