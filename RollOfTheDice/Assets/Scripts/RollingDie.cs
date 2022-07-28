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
    }

    public void Roll()
    {
        // TODO rotate only around self
        // TODO rotate faster (currently: var lerpValue = Time.deltaTime * 3;)
        // TODO rotate before player
        diceLogic.MoveIntoDirection(rollDirection, () => { });
    }
}
