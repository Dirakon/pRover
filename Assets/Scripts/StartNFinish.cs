using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartNFinish : MonoBehaviour
{
    // Start is called before the first frame update
    private static int idGiver = 0;
    private static int maxId = 0;
    public static float start;
    public static float end;
    void Awake(){
        int id = ++idGiver;
        if (id > maxId){
            start = transform.position.x;
            end = transform.position.x;
            maxId = id+1;
        }else{
            if (start >transform.position.x){
                //we're start
                end = start;
                start = transform.position.x;
            }else{
                //we're finish
                end = transform.position.x;
            }
        }
        Destroy(gameObject);

    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
