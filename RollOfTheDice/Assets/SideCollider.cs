using UnityEngine;

public class SideCollider : MonoBehaviour
{
    public int side;
    private GameController gameController;

    void Start()
    {
        gameController = GameObject.FindWithTag("GameController")
            .GetComponent<GameController>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Finish"))
        {
            var finish = other.GetComponent<Finish>();

            if (finish.winValue == side)
            {
                gameController.LoadNextLevel();
            }
        }
    }
}
