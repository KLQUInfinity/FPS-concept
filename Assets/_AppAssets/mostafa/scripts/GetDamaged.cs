using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetDamaged : MonoBehaviour
{

    private void OnTriggerEnter(Collider collision)
    {
        DamageInfo dm = collision.GetComponent<DamageInfo>();

        switch (dm.effect)
        {
            case DamageInfo.DamageEffect.freeze:
                print("frozen");
                break;

        }
    }
}
