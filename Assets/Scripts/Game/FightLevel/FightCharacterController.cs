using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FightCharacterController : MonoBehaviour
{
    public bool IsHero;

    [SerializeField] private GameObject myEnemy;

    public void StartFight()
    {
        CalculateDistToEnemys();
    }

    private void CalculateDistToEnemys()
    {
        List<GameObject> Enemys = new List<GameObject>();
        
        if (IsHero)
        {
            for (int i = 0; i < FightController.Instance.Enemys.Length; i++)
            {
                Enemys.Add(FightController.Instance.Enemys[i]);
            }
        }
        else
        {
            for (int i = 0; i < FightController.Instance.Characters.Count; i++)
            {
                Enemys.Add(FightController.Instance.Characters[i]);
            }
        }
        
        
        myEnemy = Enemys.OrderBy( enemy => Vector3.Distance(transform.position, enemy.transform.position)).FirstOrDefault();
    }
}
