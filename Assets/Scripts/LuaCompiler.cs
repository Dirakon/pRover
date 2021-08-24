using System.Collections;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
public class LuaCompiler : MonoBehaviour
{
    public static LuaCompiler singleton = null;
    [SerializeField]
    public string currentLevel;
    [SerializeField]
    private Text output;
    //const string windowsLibPath = @"\Plugins\Lib\";
    //const string linuxLibPath = @"/Plugins/Lib/";
    const string defaultFile = @"-- Each turn, one of the following functions is called (depending on the state of button):
function onNoButton()
    return goRight
end

function onButton()
    return jump
end


-- You can use following returns:
skip = 0
goRight = 1
goLeft = 2
jump = 3
aimLeft = 4
aimRight = 5
aimUp = 6
shoot = 7
shootLeft = 8
shootRight = 9
shootUp = 10

-- You can also use following variables (they will dynamically change):
distanceDown = 0
distanceUp = 0
distanceRight = 0
distanceLeft = 0
";
    const string ender = @"
	function zzzSetDirections (up,right,down,left)
        distanceDown=down
		distanceUp=up
		distanceRight=right
		distanceLeft=left
    end";

#if UNITY_EDITOR_WIN
        const char fileLine = '\\';
#else
    const char fileLine = '/';
#endif
    public AudioSource shoot,move,death;

    public void SaveMainHeroFile(string level, string rawText)
    {
        Directory.CreateDirectory(Application.dataPath + fileLine + "game" + fileLine + level);
        string levelRoverPath = Application.dataPath + fileLine + "game" + fileLine + level + fileLine + "rover.lua";

        File.WriteAllText(levelRoverPath, rawText);
    }

    public void resetMainHeroCode(){
        lastMainHeroCode="";
    }
    public void SetupLuaEntity(LuaEntity who){
        if (who.isMainHero){
            who.personalScript = new Script();
            who.personalScript.DoString(lastMainHeroCode);
            who.codeInText = lastMainHeroCode;
        }else{
            string level = currentLevel;
            TextAsset txt = (TextAsset)Resources.Load(who.entityName, typeof(TextAsset));
            who.codeInText = txt.text + ender;
            who.personalScript = new Script();
            who.personalScript.DoString(who.codeInText);
        }
    }
    string roverFileName;
    string lastMainHeroCode = "";
    public bool gameIsRunning = false;
    public string UpdateMainHeroCode(string level)
    {
        if (!File.Exists(roverFileName))
        {
            File.WriteAllText(roverFileName, defaultFile);
        }
        string text = File.ReadAllText(roverFileName);


        SaveMainHeroFile(level, text);
        if (output!=null)
            output.text = text;

        lastMainHeroCode = text + ender;
        return text;
    }
    public bool previewMode = false;
    private const string tutorialText = @"                --Your workspace--
    You will build a program for your first rover in this folder. 
    The file name is rover.lua. You will need to make Lua scripts, the default code is already there. You may use any IDE you like, or just use notepad.
    If you need to see the default code again, just delete the file and reload the menu with code.
    After you upload your code in-game, the code will duplicate in the level folder, so you can revisit your old code and copy some things if you need to.
    
                --Menu with code and level choosing--
    Click on the update button (refresh icon) to upload your code to the game.
    You can select level with arrows on the top right.
    You can also preview the level with an eye icon.
    To start the level, click the arrow in the bottom right.

                --Actual level--
    Use arrows (or A/D) to move the camera.
    Press ESC to return to the code menu.
    Click the arrow on the bottom right to start the next turn.
    The function called depends on the state of the button, you can control it by clicking it (updates once per turn). 
    The arrow beside the button indicates your intentions (click or not to click).
    Position of the hand indicates the current state of the button (clicked or not clicked).
    Click on enemies (or yourself) to read the code of the given entity.

That's about it. Good luck! ";
    public bool checkIfTutorialExistsOtherwiseCreateOne(){
        string tutorialPath = Application.dataPath + fileLine + "game" + fileLine + "tutorial.txt";
        if (File.Exists(tutorialPath))
            return true;
        File.WriteAllText(tutorialPath,tutorialText);
        return false;
    }
    public static string ONNOBUTTON = "onNoButton", UPDATE = "update",ONBUTTODHOLD = "onButton", ONBUTTONUP = "onNoButton", ONBUTTONDOWN = "onButton";
    public int callFunction(Script script,string func)
    {
        DynValue val = script.Call(script.Globals[func]);
        return (int)val.Number;
    }
    public void initDirection(Script script,int up, int right, int down, int left)
    {
        script.Call(script.Globals["zzzSetDirections"], up, right, down, left);
    }
    public bool heroScriptLoaded(){
        return lastMainHeroCode != "";
    }
    void Awake()
    {
        if (singleton != null)
        {
            Destroy(gameObject);
            return;
        }
        singleton = this;
        DontDestroyOnLoad(gameObject);
        Directory.CreateDirectory(Application.dataPath + fileLine + "game" + fileLine);
        roverFileName = Application.dataPath + fileLine + "game" + fileLine + @"rover.lua";
        GetComponent<AudioSource>().Play();
    }

    void Start()
    {
    }

    void Update()
    {

    }
}