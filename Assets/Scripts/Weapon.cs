using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class Weapon : MonoBehaviour {
    
    public GameObject bulletPrefab;
    public float fireRate = 0.5F;//0.5秒实例化一个子弹
    private float nextFire = 0.0F;


    public void Shoot()
    {
        if(Time.time > nextFire)//让子弹发射有间隔
        {
            nextFire = Time.time + fireRate;//Time.time表示从游戏开发到现在的时间，会随着游戏的暂停而停止计算。
            Instantiate(bulletPrefab, transform.position, transform.rotation);
        }
    }
}