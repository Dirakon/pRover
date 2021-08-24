using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ButtonMaster : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void realStart(){
        GetComponent<Image>().enabled=true;
        GetComponent<Button>().enabled=true;
        handUp.enabled=true;
        arrowUp.enabled=true;
        nextTurnArrow.enabled=true;
        nextTurnArrowButton.enabled=true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public static ButtonMaster singleton;
    void Awake(){
        singleton=this;
    }
    public void startOfTheTurn(){
        inputFinished=false;
        wasPreviusClick=isThisClick;

    }
    public void onNextLevel(){
        inputFinished=true;
        if (isThisClick != wasPreviusClick){
            handDown.enabled = !handDown.enabled;
            handUp.enabled = !handUp.enabled;
        }
    }

    public string GetNormalizedState(){
        if (isThisClick){
            if (wasPreviusClick){
                return LuaCompiler.ONBUTTODHOLD;
            }else{
                return LuaCompiler.ONBUTTONDOWN;
            }
        }else{
            if (wasPreviusClick){
                return LuaCompiler.ONBUTTONUP;
            }else{
                return LuaCompiler.ONNOBUTTON;
            }
        }
    }
    public bool inputFinished=false;
    public bool changeAllowed = true;
    public bool isThisClick=false;
    public bool wasPreviusClick=false;
    [SerializeField] 
    private Image handDown,handUp,arrowDown,arrowUp,nextTurnArrow;
    [SerializeField]
    private Button nextTurnArrowButton;
    public void clicked(){
        if (!changeAllowed)
            return;
        arrowDown.enabled = !arrowDown.enabled;
        arrowUp.enabled = !arrowUp.enabled;
        isThisClick = !isThisClick;
    }
}
