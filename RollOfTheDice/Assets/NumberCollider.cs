using UnityEngine;

public class NumberCollider : MonoBehaviour
{
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
            Debug.Log("Finish");
            // TODO check if correct side landed on goal. Then load next scene or show winning screen
            gameController.LoadNextLevel();
        }
    }
}
