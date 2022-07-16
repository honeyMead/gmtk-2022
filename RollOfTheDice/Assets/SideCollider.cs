using UnityEngine;

public class SideCollider : MonoBehaviour
{
    public int side;
    private GameController gameController;
    private int touchingColliders = 0;

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
        touchingColliders++;
    }

    void OnTriggerExit(Collider other)
    {
        touchingColliders--;
    }

    public bool IsColliding()
    {
        return touchingColliders > 0;
    }
}
