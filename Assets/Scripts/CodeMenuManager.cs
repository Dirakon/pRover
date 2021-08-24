using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class CodeMenuManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] thingsToUtilize;
    void Start()
    {
        if (LuaCompiler.singleton.checkIfTutorialExistsOtherwiseCreateOne())
        {
            UtilizeTutorial();
            
        }else{

            LuaCompiler.singleton.UpdateMainHeroCode(scenes[ptr]);
            LuaCompiler.singleton.resetMainHeroCode();
        }
    }
    public void UpdateCode(){
        ScrollPack.singleton.Appear(LuaCompiler.singleton.UpdateMainHeroCode(scenes[ptr]));
    }

    int ptr = 0;
    public string[] scenes;
    public Text sceneCaption;
    void UpdateSceneCaption(){
        sceneCaption.text=scenes[ptr];
        LuaCompiler.singleton.resetMainHeroCode();
        ScrollPack.singleton.Close();
    }
    public void NextScene(){
        ptr+=1;
        if (ptr == scenes.Length)
            ptr=0;
        UpdateSceneCaption();
    }
    public void PreviousScene(){
        if (ptr == 0){
            ptr = scenes.Length;
        }
        ptr-=1;
        UpdateSceneCaption();
    }
    public void GoChosenLevel()
    {
        if (!LuaCompiler.singleton.heroScriptLoaded())
            return;
        LuaCompiler.singleton.previewMode = false;
        LuaCompiler.singleton.gameIsRunning=true;
        LuaCompiler.singleton.currentLevel=scenes[ptr];
        SceneManager.LoadScene(scenes[ptr], LoadSceneMode.Single);
    }
    public void PreviewChosenLevel()
    {
        LuaCompiler.singleton.previewMode = true;
        LuaCompiler.singleton.currentLevel=scenes[ptr];
        SceneManager.LoadScene(scenes[ptr], LoadSceneMode.Single);
    }

    public void UtilizeTutorial()
    {
        foreach (var thing in thingsToUtilize)
        {
            if (thing != null)
                Destroy(thing);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
