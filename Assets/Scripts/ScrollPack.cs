using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollPack : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake(){
        singleton=this;
        Close();
    }
    void Start()
    {
        
    }
    [SerializeField]
    Scrollbar scroolbar;
    [SerializeField]
    Button[] buttons;
    [SerializeField]
    Image[] images;
    [SerializeField]
    Text[]texts;
    [SerializeField]
    Text realText;
    public static ScrollPack singleton;

    // Update is called once per frame
    void Update()
    {
        
    }
    void setStateTo(bool state){

        foreach(var el in buttons)
            el.enabled=state;
        foreach(var el in images)
            el.enabled=state;
        foreach(var el in texts)
            el.enabled=state;
        scroolbar.enabled=state;
    }
    public void Appear(string text){
        setStateTo(true);
        realText.text=text;
    }
    public void Close(){
        setStateTo(false);
    }
}
