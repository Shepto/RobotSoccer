using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Core;
using System.IO;
using UnityEditor;

public class MenuUIController : MonoBehaviour {

    public Button exit;
    public Button start;
    public InputField tactic1;
    public InputField tactic2;


    public void Start()
    {
        tactic1.text = @"strategie\MissingDefMROffR.strg";
        tactic2.text = @"strategie\StrategyFull.strg";
    }

    public void Browse1() {

        string path = EditorUtility.OpenFilePanel("Select strategy", "strategie", "");
        tactic1.text = path;

        if (tactic1.text.Length != 0)
        {
            if (!tactic1.text.EndsWith(".strg"))
            {
                EditorUtility.DisplayDialog("Warning", "File have to ends with .strg!", "Ok");
                start.enabled = false;
            }
            else
            {

                start.enabled = true;
            }
        }
        else {

            EditorUtility.DisplayDialog("Warning", "Please, choose file", "Ok");
            start.enabled = false;
        }
    }

    public void Browse2()
    {
        string path = EditorUtility.OpenFilePanel("Select strategy", "strategie", "");
        tactic2.text = path;

        if (tactic2.text.Length != 0)
        {
            if (!tactic2.text.EndsWith(".strg"))
            {
                EditorUtility.DisplayDialog("Warning", "File have to ends with .strg!", "Ok");
                start.enabled = false;
            }
            else
            {
                start.enabled = true;
            }
        }
        else
        {
            EditorUtility.DisplayDialog("Warning", "Please, choose file", "Ok");
            start.enabled = false;
        }

    }

    public void OnGUI()
    {
        if (tactic1.text.Length == 0 || tactic2.text.Length == 0)
        {
            start.enabled = false;
        }
        else {
            start.enabled = true;    
        }
    }


    public void StartTheGame()
    {
        StaticVariable.leftStrategy = tactic1.text;
        StaticVariable.rightStrategy = tactic2.text;
        Application.LoadLevel(1);
    }

    public void Exit() {

        Application.Quit();
    }
}
