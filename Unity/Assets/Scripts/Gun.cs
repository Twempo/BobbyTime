using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class Gun : ScriptableObject {
    public string name;
    public int ammo;
    public int clipSize;
    public int maxAmmo;
    public int fireRate;
    public float reloadSpeed;

}
