using UnityEngine;

public abstract class BoxContent : MonoBehaviour
{
    [SerializeField] private BoxContentType _contentType;
    [Range(0f,100f)][SerializeField] private float _spawnChance;

    public BoxContentType ContentType => _contentType;
    public float SpawnChance => _spawnChance;

    public virtual void Activate()
    {
        gameObject.SetActive(true);
    }

    public virtual void Interact()
    {

    }

    public virtual void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public Trap TryGetTrap()
    {
        if (this is Trap trap)
            return trap;

        return null;
    }

    public Reward TryGetReward()
    {
        if (this is Reward reward)
            return reward;

        return null;
    }
}