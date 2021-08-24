using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameController : MonoBehaviour
{
    public static GameController singleton;
    // Start is called before the first frame update
    void Awake()
    {
        singleton=this;
        bulletManager = gameObject.AddComponent<BulletManager>();
    }
    public BulletManager bulletManager;
    public Vector3 startPosition=Vector3.one, endPosition=Vector3.one;
    private float t = 0f;
    private const float speed = 1f;
    void Start()
    {
        startPosition = new Vector3(StartNFinish.start,transform.position.y,transform.position.z);
        endPosition = new Vector3(StartNFinish.end,transform.position.y,transform.position.z);
        if (LuaCompiler.singleton.gameIsRunning && !LuaCompiler.singleton.previewMode)
        {
            ButtonMaster.singleton.realStart();
        }
        Update();
        StartCoroutine(AutoPlay());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
            GoToCodeMenu();
        t = Mathf.Clamp(t + Time.deltaTime*speed*Input.GetAxis("Horizontal"),0f,1f);
        startPosition.y = endPosition.y=mainHero.transform.position.y;
        transform.position = Vector3.Lerp(startPosition,endPosition,t);
    }
    int yOffset;

    public string getButtonState()
    {
        return ButtonMaster.singleton.GetNormalizedState();
    }

    IEnumerator DoRobot()
    {
       yield return mainHero.DoFullTurn();
    }
    public LuaEntity mainHero;
    public LuaEntity[] enemies = new LuaEntity[0];

    IEnumerator DoEnemies()
    {
        foreach(var enemy in enemies){
            if (enemy != null)
                yield return enemy.DoFullTurn();
        }
    }
    public void GoToCodeMenu(){
        StopAllCoroutines();
        LuaCompiler.singleton.resetMainHeroCode();
        SceneManager.LoadScene("Maen",LoadSceneMode.Single);
    }

    IEnumerator AutoPlay()
    {
        while (mainHero.transform.position.x < endPosition.x)
        {

            ButtonMaster.singleton.startOfTheTurn();
            yield return WaitForInput();
         
            yield return Turn();
        }
        GoToCodeMenu();
    }

    IEnumerator WaitForInput()
    {
        ButtonMaster.singleton.changeAllowed = true;
        while (true)
        {
            if (ButtonMaster.singleton.inputFinished)
            {
                break;
            }
            yield return null;
        }


        ButtonMaster.singleton.changeAllowed = false;
    }

    IEnumerator Turn()
    {
        yield return DoRobot();
        yield return DoEnemies();
    }
}
