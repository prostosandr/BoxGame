using UnityEngine;

public class BoxContentProvider : MonoBehaviour
{
    [Header("Global spawn chance")]
    [Range(0f, 100f)][SerializeField] private float _emptyChance;
    [Range(0f, 100f)][SerializeField] private float _trapChance;
    [Range(0f, 100f)][SerializeField] private float _rewardChance;

    [Header("Rewards")]
    [SerializeField] private BoxContent[] _rewards;

    [Header("Traps")]
    [SerializeField] private BoxContent[] _traps;

    [Header("EmptyObject")]
    [SerializeField] private BoxContent _empty;

    public BoxContent GetRandomBoxContent()
    {
        BoxContentType contentType = GetBoxContent();

        switch(contentType)
        {
            case BoxContentType.Empty:
                return _empty;

            case BoxContentType.Trap:
                return GetRandomBoxContent(_traps);

            case BoxContentType.Reward:
                return GetRandomBoxContent(_rewards);

            default:
                return null;
        }
    }

    private BoxContentType GetBoxContent()
    {
        float totalWeight = _emptyChance + _trapChance + _rewardChance;

        float randomValue = Random.Range(0f, totalWeight);

        if (randomValue <= _emptyChance)
            return BoxContentType.Empty;

        if (randomValue <= _emptyChance + _trapChance)
            return BoxContentType.Trap;

        return BoxContentType.Reward;
    }

    private BoxContent GetRandomBoxContent(BoxContent[] contents)
    {
        if (contents == null || contents.Length == 0)
            return null;

        float totalWeight = 0f;

        foreach (BoxContent content in contents)
        {
            totalWeight += content.SpawnChance;
        }

        if (totalWeight == 0)
            return null;

        float randomValue = Random.Range(0f, totalWeight);

        float currentWeightSum = 0f;

        foreach (BoxContent content in contents)
        {
            currentWeightSum += content.SpawnChance;

            if (randomValue <= currentWeightSum)
            {
                return content;
            }
        }

        return contents[0];
    } 
}