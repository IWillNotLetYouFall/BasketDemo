using TMPro;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    [SerializeField] new BoxCollider collider;
    [SerializeField] TextMeshProUGUI tmp;
    
    int _score;
    private void OnTriggerExit(Collider other)
    {
        if(!(other.TryGetComponent<Rigidbody>(out Rigidbody ballRigidbody)))
        {
            return;
        }

        if (ballRigidbody.velocity.y >= 0f) return;
        
        if(!(other.TryGetComponent<BasketBallController>(out var script)))
        {
            return;
        }

        script.ScorePoint();
        _score++;
        tmp.text = _score.ToString();
    }
}
