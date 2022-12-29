using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon 
{
    void Shoot();
    IEnumerator Reload(float time);
}
