using UnityEngine;

public class EnemyDamaged : MonoBehaviour
{
    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Creature"))
        {

            Debug.Log("°ø·æ ¸ÂÀ½");
            if (other.TryGetComponent<Creature>(out Creature creature))
            {
                creature.health -= 50;
            }
        }
    }
}
