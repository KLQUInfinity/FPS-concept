using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageInfo : MonoBehaviour
{
    public enum WeaponType { firearm, grenade, mine };
    public enum DamageEffect { pan, fly, freeze, root, explotion };

    public WeaponType type;
    public DamageEffect effect;

    public int source_id; //player id
    public int team_id;//
    public float damage_amount;
    public float effect_duration;
    public float radius;
    public bool aoe;//are of effect\
}
