using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp.Interpreter;

public class LuaEntity : MonoBehaviour
{
    void Awake(){

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        LuaCompiler.singleton.SetupLuaEntity(this);
    }
    public string codeInText  ="";
    public string entityName;
    SpriteRenderer spriteRenderer;
    [SerializeField]
    private Sprite entityUp, entityRight, entityLeft,entityDown;
    private const int UP = 0,RIGHT=1,DOWN=2,LEFT=3;
    private int currentDirection = RIGHT;
    [SerializeField]
    private GameObject[] rightLooks, downLooks, upLooks, leftLooks;
    [SerializeField]
    private LayerMask layerMaskForDistances;
    [SerializeField]
    private LayerMask layerMaskForEnemyDetection;
    public int AutoRaycast(Vector2 origin, Vector2 dir)
    {
        return AutoRaycast(origin,dir,layerMaskForDistances);
    }
    public int AutoRaycast(Vector2 origin, Vector2 dir,LayerMask layerMask)
    {

        RaycastHit2D hit = Physics2D.Raycast(origin, dir, Mathf.Infinity, layerMask);

        // If it hits something...
        if (hit.collider != null)
        {
            // Calculate the distance from the surface and the "error" relative
            // to the floating height.
            int distance = (int)Mathf.Round((hit.point - origin).magnitude);
            return distance;
        }
        return 9999;
    }
    public LuaEntity GetRaycastTarget(Vector2 origin, Vector2 dir,LayerMask layerMask){
        RaycastHit2D hit = Physics2D.Raycast(origin, dir, Mathf.Infinity, layerMask);

        // If it hits something...
        if (hit.collider != null)
        {
            // Calculate the distance from the surface and the "error" relative
            // to the floating height.
            return hit.collider.gameObject.GetComponent<LuaEntity>();
        }
        return null;
    }

    int upDist, rightDist, leftDist, downDist;
    int getShortestDistance(GameObject[] looks,Vector2 dir){
        int shortestDistance = 9999;

        foreach(var look in looks){
            int dist = AutoRaycast(look.gameObject.transform.position,dir);
            if (dist < shortestDistance)
                shortestDistance = dist;
        }

        return shortestDistance;
    }
    LuaEntity getClosestObject(GameObject[] looks,Vector2 dir){
        int shortestDistance = 9999;
        LuaEntity obj=null;
        foreach(var look in looks){
            int dist = AutoRaycast(look.gameObject.transform.position,dir);
            LuaEntity tobj = GetRaycastTarget(look.gameObject.transform.position,dir,layerMaskForDistances);
            if (dist < shortestDistance || (tobj==null && dist == shortestDistance)){
                shortestDistance = dist;
                obj=tobj;
            }
        }

       return obj;
    }
    public bool ignoresGravity=false;
    public void SetDistances()
    {

        upDist = getShortestDistance(upLooks,Vector2.up);
        rightDist =getShortestDistance(rightLooks,Vector2.right);
        downDist = getShortestDistance(downLooks,Vector2.down);
        leftDist = getShortestDistance(leftLooks,Vector2.left);
        LuaCompiler.singleton.initDirection(personalScript,upDist, rightDist, downDist, leftDist);
      // Vector4 deb = new Vector4(upDist,rightDist,downDist,leftDist );
      //  if (!isMainHero)
     //     Debug.Log(deb);
    }
    // Start is called before the first frame update
    public bool isMainHero=false;
    void Start()
    {
        
    }

    public Script personalScript;
    private int yOffset = 0;
    void DoGravity()
    {
        yOffset = 0;
        SetDistances();
        if (!ignoresGravity)
            yOffset = -1;
    }
    public float moveSpeed = 1f;
    IEnumerator waitForMovement(Vector3 finishLine){
        float t = 0;
        LuaCompiler.singleton.move.Play();
        Vector3 startLine = transform.position;
        while (t < 1){
            transform.position = Vector3.Lerp(startLine,finishLine,t);
            t+=moveSpeed*Time.deltaTime;
            yield return null;
        }
        transform.position=finishLine;
    }

    int GetResponse(){
        if (isMainHero){
            string button = GameController.singleton.getButtonState();
            return  LuaCompiler.singleton.callFunction(personalScript,button);
        }
        return LuaCompiler.singleton.callFunction(personalScript,LuaCompiler.UPDATE);
    }
    private const int RESPONSELIMITFORMAINHERO = 10;
    public int hp = 1;
    public void getHurt(int dmg){
        hp-=dmg;
        LuaCompiler.singleton.death.Play();
        if (hp <= 0){
            if (isMainHero){
                GameController.singleton.GoToCodeMenu();
            }
            Destroy(gameObject);
        }
    }
    public IEnumerator DoAction(){
        int response = GetResponse();
        int xOffset = 0;
        if (isMainHero && response >RESPONSELIMITFORMAINHERO)
            response=0;
        bool hasShot = false;
        Vector3 shootGoal = Vector3.zero;
        Vector3 shootStart = Vector3.zero;
        LuaEntity shootTarget = null;
        switch (response)
        {
            default:
                // skip

                break;
            case 1:
                // goRight
                xOffset += 1;

                break;
            case 2:
                // goLeft
                xOffset -= 1;
                break;
            case 3:
                // jump
                if (upDist>0 && downDist==0)
                    yOffset = 1;
                break;
            case 4:
                // aimLeft
                spriteRenderer.sprite=entityLeft;
                currentDirection = LEFT;
                break;
            case 5:
                // aimRight
                spriteRenderer.sprite=entityRight;
                currentDirection = RIGHT;
                break;
            case 6:
                // aimUp
                spriteRenderer.sprite=entityUp;
                currentDirection = UP;
                break;
            case 7:
                // shoot
                if (currentDirection == UP){
                    goto case 10;
                }else if (currentDirection == RIGHT){
                    goto case 9;
                }else if (currentDirection == DOWN){
                    goto case 11;
                }else{
                    goto case 8;
                }
            case 8:
                // shootLeft
                if (leftDist!=9999 && leftDist !=0){
                    hasShot=true;
                    shootStart=transform.position+new Vector3(0,0,1);
                    shootGoal=transform.position+new Vector3(-leftDist,0,1);
                    shootTarget = getClosestObject(leftLooks,Vector2.left);
                }
                goto case 4;
            case 9:
                // shootRight
                if (rightDist!=9999 && rightDist !=0){
                    hasShot=true;
                    shootStart=transform.position+new Vector3(0,0,1);
                    shootGoal=transform.position+new Vector3(rightDist,0,1);
                    shootTarget = getClosestObject(rightLooks,Vector2.right);
                }
                goto case 5;
            case 10:
                // shootUp
                if (upDist!=9999 && upDist !=0){
                    hasShot=true;
                    shootStart=transform.position+new Vector3(0,0,1);
                    shootGoal=transform.position+new Vector3(0,upDist,1);
                    shootTarget = getClosestObject(upLooks,Vector2.up);
                }
                goto case 6;
            case 11:
                // shootDown
                if (downDist!=9999 && downDist !=0){
                    hasShot=true;
                    shootStart=transform.position+new Vector3(0,0,1);
                    shootGoal=transform.position+new Vector3(0,-downDist,1);
                    shootTarget = getClosestObject(downLooks,Vector2.down);
                }
                goto case 12;
            case 12:
                // aimDown
                spriteRenderer.sprite=entityDown;
                currentDirection = DOWN;
                break;
            case 13:
                //Fall
                if (currentDirection == UP){
                    goto case 14;
                }else if (currentDirection == RIGHT){
                    goto case 15;
                }else if (currentDirection == DOWN){
                    goto case 16;
                }else{
                    goto case 17;
                }
            case 14:
                //FallUp
                    yOffset = upDist;
                    shootTarget = getClosestObject(upLooks,Vector2.up);
                break;
            case 15:
                //FallRight
                    xOffset = rightDist;
                    shootTarget = getClosestObject(rightLooks,Vector2.right);
                break;
            case 16:
                //FallDown
                    yOffset = -downDist;
                    shootTarget = getClosestObject(downLooks,Vector2.down);
                
                break;
            case 17:
                //FallLeft
                    xOffset = -leftDist;
                    shootTarget = getClosestObject(leftLooks,Vector2.left);
                break;
        }
        if (hasShot){
            yield return GameController.singleton.bulletManager.shootABullet(shootStart,shootGoal);
            if (shootTarget != null){
                shootTarget.getHurt(1);
            }
        }
        bool canMove = false;
        if (yOffset>0)
            yield return waitForMovement(transform.position+new Vector3(0, yOffset, 0));
        if (xOffset > 0 )
        {
            canMove = getShortestDistance(rightLooks, Vector2.right) > 0;
        }
        else if (xOffset < 0 )
        {
            canMove = getShortestDistance(leftLooks, Vector2.left) > 0;
        }
        if (canMove)
            yield return waitForMovement(transform.position+new Vector3(xOffset, 0, 0));
        if (yOffset < 0 && getShortestDistance(downLooks, Vector2.down) > 0){
            yield return waitForMovement(transform.position+new Vector3(0, yOffset, 0));
        }
        if (!hasShot && shootTarget != null){
            //fallen onto something
            shootTarget.getHurt(10);
        }
    }
    void OnMouseDown(){
        ScrollPack.singleton.Appear(codeInText);
    }

    public IEnumerator DoFullTurn(){
        if (hp > 0){
            DoGravity();
            yield return DoAction();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
