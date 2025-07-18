using System.Collections.Generic;
using UnityEngine;

public class LifeBarHandler : MonoBehaviour
{
    public static LifeBarHandler Instance { get; private set; }

    [SerializeField] private HealthBar lifeBarItemPrefab;

    private List<HealthBar> _lifeBarsList;

    void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        _lifeBarsList = new List<HealthBar>();
    }

    public HealthBar AddLifeBar(Player owner)
    {
        var newLifeBar = Instantiate(lifeBarItemPrefab, transform)
            .SetOwner(owner);

        _lifeBarsList.Add(newLifeBar);

        return newLifeBar;
    }
    
    public void RemoveLifeBar(HealthBar bar)
    {
        if (bar != null && _lifeBarsList.Remove(bar))
        {
            Destroy(bar.gameObject);
        }
    }

    void LateUpdate()
    {
        foreach (var lifeBarItem in _lifeBarsList)
        {
            lifeBarItem.UpdatePosition();
        }
    }
}
