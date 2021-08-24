using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public IEnumerator shootABullet(Vector3 origin, Vector3 end, float speed = 1f){
        float t = 0;
        LuaCompiler.singleton.shoot.Play();
        GameObject bullet = Instantiate(PrefabMaster.singleton.bulletPrefab,origin,Quaternion.identity);
        while (t < 1){
            bullet.transform.position = Vector3.Lerp(origin,end,t);
            t+=speed*Time.deltaTime;
            yield return null;
        }
        Destroy(bullet);
    }
    // Update is called once per frame
    
    void Update()
    {
        
    }
}
