using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEditor;
using Core;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

public class DLLMenuController : MonoBehaviour {

    public Button exit;
    public Button start;
    public Button scan;
    public InputField leftDLL;
    public InputField rightDLL;

    public Image scan1;
    public Image scan2;
    public Sprite ok;
    public Sprite notOk;

    private bool leftDllOK = false;
    private bool rightDllOK = false;

    private bool leftPathChanged = false;
    private bool rightPathChanged = false;

    Assembly leftDLLassembly;
    Assembly rightDLLassembly;

    public GameObject adaptationPanel;
    bool leftAdaptation = false;
    bool rightAdaptation = false;
    public Image whiteScreen;
    public Toggle leftToggle;
    public Toggle rightToggle;

    public GameObject errorLogPanel;
    public Text leftLog;
    public Text rightLog;

    bool scaned = false;
 
    bool leftConstructorOK = false;
    bool rightConstructorOK = false;

    bool leftPlayerStrategy = false;
    bool rightPlayerStrategy = false;

    bool leftmethodIsThere = false;
    bool righttmethodIsThere = false;
    bool leftParameters = false;
    bool rightParameters = false;

    bool leftAdaptationMethod = false;
    bool leftAdaptationParameters = false;
    bool leftAdaptationReturnType = false;
    bool leftAdaptationInPlayerStrategy = false;
    bool leftAdaptationParametersInPlayerStrategy = false;

    bool rightAdaptationMethod = false;
    bool rightAdaptationParameters = false;
    bool rightAdaptationReturnType = false;
    bool rightAdaptationInPlayerStrategy = false;
    bool rightAdaptationParametersInPlayerStrategy = false;

    bool leftRuleNumber = false;
    bool leftRuleNumberReturnType = false;
    bool rightRuleNumber = false;
    bool rightRuleNumberReturnType = false;

    List<string> leftErrorLog = new List<string>();
    List<string> rightErrorLog = new List<string>();

    public void Start()
    {
        adaptationPanel.SetActive(false);
        errorLogPanel.SetActive(false);

        whiteScreen.canvasRenderer.SetAlpha(0.0f);
        leftDLL.text = @"Assets\DLL\StrategiesDLL.dll";
        rightDLL.text = @"Assets\DLL\StrategiesDLL.dll";  
    }

    public void Update()
    {
        if (leftDllOK == false || leftPathChanged == true)
        {
            scan1.sprite = notOk;
        }
        else
        {
            scan1.sprite = ok;
        }

        if (rightDllOK == false)
        {
            scan2.sprite = notOk;

        }
        else
        {
            scan2.sprite = ok;
        } 
    }

    public void OnGUI()
    {
        if (leftDllOK == false || rightDllOK == false)
        {
            start.gameObject.SetActive(false);
            scan.gameObject.SetActive(true);
        }
        else
        {
            start.gameObject.SetActive(true);
            scan.gameObject.SetActive(false);
        }

        if (leftPathChanged == true)
        {
            leftDllOK = false;

            leftConstructorOK = false;
            leftPlayerStrategy = false;
            leftmethodIsThere = false;
            leftParameters = false;
            leftAdaptation = false;
            leftAdaptationMethod = false;
            leftAdaptationReturnType = false;
            leftAdaptationParameters = false;
            leftRuleNumber = false;
            leftRuleNumberReturnType = false;
            leftAdaptationInPlayerStrategy = false;
            leftAdaptationParametersInPlayerStrategy = false;


            start.gameObject.SetActive(false);
            scan.gameObject.SetActive(true);
            leftPathChanged = false;

            StaticVariable.leftAdaptation = false;
            StaticVariable.rightAdaptation = false;

            leftToggle.isOn = false;
            rightToggle.isOn = false;
        }
        if (rightPathChanged == true)
        {
            rightDllOK = false;

            rightConstructorOK = false;
            rightPlayerStrategy = false;
            righttmethodIsThere = false;
            rightParameters = false;
            rightAdaptation = false;
            rightAdaptationMethod = false;
            rightAdaptationReturnType = false;
            rightAdaptationParameters = false;
            rightRuleNumber = false;
            rightRuleNumberReturnType = false;
            rightAdaptationInPlayerStrategy = false;
            rightAdaptationParametersInPlayerStrategy = false;

            start.gameObject.SetActive(false);
            scan.gameObject.SetActive(true);
            rightPathChanged = false;

            StaticVariable.leftAdaptation = false;
            StaticVariable.rightAdaptation = false;

            leftToggle.isOn = false;
            rightToggle.isOn = false;
        }
    }

    public void Browse1()
    {

        string path = EditorUtility.OpenFilePanel("Select DLL for left players", "Assets/DLL", "");
        leftDLL.text = path;

        if (leftDLL.text.Length != 0)
        {
            if (!leftDLL.text.EndsWith(".dll"))
            {
                EditorUtility.DisplayDialog("Warning", "File have to ends with .dll!", "Ok");
            }

        }
        else
        {
            EditorUtility.DisplayDialog("Warning", "Please, choose file", "Ok");
        }

        leftPathChanged = true;
    }
    
    public void Browse2()
    {
        string path = EditorUtility.OpenFilePanel("Select DLL for right players", "Assets/DLL", "");
        rightDLL.text = path;

        if (rightDLL.text.Length != 0)
        {
            if (!rightDLL.text.EndsWith(".dll"))
            {
                EditorUtility.DisplayDialog("Warning", "File have to ends with .dll!", "Ok");
            }
        }
        else
        {
            EditorUtility.DisplayDialog("Warning", "Please, choose file", "Ok");
        }

        rightPathChanged = true;
    }

    public void StartTheGame()
    {
        StaticVariable.leftDllStrategy = leftDLL.text;
        StaticVariable.rightDLLStrategy = rightDLL.text;
        Application.LoadLevel(2);
    }

    public void Back()
    {
        Application.LoadLevel(0);
    }

    public void ScanDLL()
    {
        try
        {
            leftDLLassembly = Assembly.LoadFile(leftDLL.text);
            rightDLLassembly = Assembly.LoadFile(rightDLL.text);

            Type[] leftTypes = leftDLLassembly.GetTypes();

            foreach (Type type in leftTypes)
            {
                if (!type.IsPublic)
                {
                    continue;
                }

                MemberInfo[] members = type.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod);

                foreach (MemberInfo member in members)
                {
                    
                    if (type.Name == "PlayerStrategy") {

                        leftPlayerStrategy = true;

                        
                        ConstructorInfo constructorInfoObj = type.GetConstructor(new Type[] { typeof(GameSetting), typeof(String), typeof(bool) });

                        if (constructorInfoObj != null)
                        {
                            leftConstructorOK = true;
                        }

                        
                        if (member.Name == "TickStrategy")
                        {
                            leftmethodIsThere = true;
                        }

                        
                        if (member.Name == "CurrentRuleNumber")
                        {
                            leftRuleNumber = true;
                            MethodInfo Mymethodinfo = type.GetMethod("CurrentRuleNumber");

                            if (Mymethodinfo.ReturnType == typeof(int)) {

                                leftRuleNumberReturnType = true;
                            }
                        }

                        if (leftmethodIsThere == true)
                        {

                            MethodInfo Mymethodinfo = type.GetMethod("TickStrategy");

                            ParameterInfo[] pars = Mymethodinfo.GetParameters();
                            foreach (ParameterInfo p in pars)
                            {
                                if (p.ParameterType.Name == "StrategyStorage")
                                {
                                    leftParameters = true;
                                }
                            }

                        }

                        if (member.Name == "AdaptStrategy")
                        {
                            leftAdaptationInPlayerStrategy = true;
                        }
                        if(leftAdaptationInPlayerStrategy == true){

                            MethodInfo Mymethodinfo = type.GetMethod("AdaptStrategy");

                            ParameterInfo[] pars = Mymethodinfo.GetParameters();
                            foreach (ParameterInfo p in pars)
                            {
                                if (p.ParameterType.GetGenericTypeDefinition() == typeof(List<>)) {

                                    Type typeOfList = p.ParameterType.GetGenericArguments()[0];

                                    if (typeOfList == typeof(object)){
                                        leftAdaptationParametersInPlayerStrategy = true;
                                    }
                                }
                            }
                        }
                    }

                    if (type.Name == "StrategyAdaptation")
                    {
                        leftAdaptation = true;

                        if (member.Name == "AdaptStrategy")
                        {
                            leftAdaptationMethod = true;
                        }
                   
                        if (leftAdaptationMethod == true)
                        {
                            MethodInfo Mymethodinfo = type.GetMethod("AdaptStrategy");
                            ParameterInfo[] pars = Mymethodinfo.GetParameters();

                            if (pars.Length < 3)
                            {
                                if (pars[0].ParameterType.Name == "Object" && pars[1].ParameterType.Name == "Object")
                                {
                                    leftAdaptationParameters = true;
                                }
                            }
                            else
                            {
                                leftAdaptationParameters = false;     
                            }

                            if (Mymethodinfo.ReturnType == typeof(List<object>)) {

                                leftAdaptationReturnType = true;
                            }
                            
                        }
                    } 
                } 
            }
         
           Type[] rightTypes = rightDLLassembly.GetTypes();

            foreach (Type type in rightTypes)
            {
                if (!type.IsPublic)
                {
                    continue;
                }

                MemberInfo[] members = type.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod);

                foreach (MemberInfo member in members)
                {
                    if (type.Name == "PlayerStrategy")
                    {
                        rightPlayerStrategy = true;

                        ConstructorInfo constructorInfoObj = type.GetConstructor(new Type[] { typeof(GameSetting), typeof(String), typeof(bool) });

                        if (constructorInfoObj != null)
                        {
                            rightConstructorOK = true;
                        }

                        if (member.Name == "TickStrategy")
                        {
                            righttmethodIsThere = true;
                        }

                        if (member.Name == "CurrentRuleNumber")
                        {
                            rightRuleNumber = true;
                            MethodInfo Mymethodinfo = type.GetMethod("CurrentRuleNumber");

                            if (Mymethodinfo.ReturnType == typeof(int))
                            {
                                rightRuleNumberReturnType = true;
                            }
                        }

                        if (righttmethodIsThere == true)
                        {

                            MethodInfo Mymethodinfo = type.GetMethod("TickStrategy");

                            ParameterInfo[] pars = Mymethodinfo.GetParameters();
                            foreach (ParameterInfo p in pars)
                            {
                                if (p.ParameterType.Name == "StrategyStorage")
                                {
                                    rightParameters = true;
                                }
                            }

                        }

                        if (member.Name == "AdaptStrategy")
                        {
                            rightAdaptationInPlayerStrategy = true;
                        }
                        if (rightAdaptationInPlayerStrategy == true)
                        {
                            MethodInfo Mymethodinfo = type.GetMethod("AdaptStrategy");

                            ParameterInfo[] pars = Mymethodinfo.GetParameters();
                            foreach (ParameterInfo p in pars)
                            {
                                if (p.ParameterType.GetGenericTypeDefinition() == typeof(List<>))
                                {
                                    Type typeOfList = p.ParameterType.GetGenericArguments()[0];
                                    if (typeOfList == typeof(object))
                                    {
                                        rightAdaptationParametersInPlayerStrategy = true;
                                    }
                                }
                            }
                        }


                    }

                    if (type.Name == "StrategyAdaptation")
                    {
                        rightAdaptation = true;

                        if (member.Name == "AdaptStrategy")
                        {
                            rightAdaptationMethod = true;
                        }

                       
                        if (rightAdaptationMethod == true)
                        {
                            MethodInfo Mymethodinfo = type.GetMethod("AdaptStrategy");

                            ParameterInfo[] pars = Mymethodinfo.GetParameters();

                            if (pars.Length < 3)
                            {
                                if (pars[0].ParameterType.Name == "Object" && pars[1].ParameterType.Name == "Object")
                                {
                                    rightAdaptationParameters = true;
                                }
                            }
                            else
                            {
                                rightAdaptationParameters = false;
                            }
                            if (Mymethodinfo.ReturnType == typeof(List<object>))
                            {
                                rightAdaptationReturnType = true;
                            }
                        }
                    }

                }
            }

            if (leftPlayerStrategy == true && leftmethodIsThere == true && leftParameters == true && leftConstructorOK == true && leftRuleNumber == true && leftRuleNumberReturnType == true)
            {
                leftDllOK = true;
            }
            else
            {
                leftDllOK = false;
                EditorUtility.DisplayDialog("DLL for left players", "Your dll file contains errors. Check Error Log!", "Ok");
            }

            if (rightPlayerStrategy == true && righttmethodIsThere == true && rightParameters == true && rightConstructorOK == true && rightRuleNumber == true && rightRuleNumberReturnType == true)
            {
                rightDllOK = true;
            }
            else
            {
                rightDllOK = false;
                EditorUtility.DisplayDialog("DLL for right players", "Your dll file contains errors. Check Error Log!", "Ok");
            }

            if(leftDllOK == true && rightDllOK == true)
            {
                if(leftAdaptation == true && leftAdaptationMethod == true && leftAdaptationParameters == true && leftAdaptationReturnType == true && leftAdaptationInPlayerStrategy == true && leftAdaptationParametersInPlayerStrategy == true)
                {
                    leftToggle.interactable = true;
                    adaptationPanel.SetActive(true);
                    FadeIn();
                }
                else
                {
                    leftToggle.interactable = false;
                }

                if(rightAdaptation == true && rightAdaptationMethod == true && rightAdaptationParameters == true && rightAdaptationReturnType == true && rightAdaptationInPlayerStrategy == true && rightAdaptationParametersInPlayerStrategy == true)
                { 
                    rightToggle.interactable = true;
                    adaptationPanel.SetActive(true);
                    FadeIn();
                }
                else
                {
                    rightToggle.interactable = false;
                }
            }

        }
        catch {

            leftDllOK = false;
            rightDllOK = false;
            EditorUtility.DisplayDialog("Error", "One or both DLLs cannot be loaded. Make sure you loaded right file", "Ok");
        }

        scaned = true;
        leftLog.text = "";
        rightLog.text = "";
        leftErrorLog.Clear();
        rightErrorLog.Clear();

    }

    public void FadeIn()
    {
        whiteScreen.GetComponent<CanvasGroup>().blocksRaycasts = true;
        whiteScreen.CrossFadeAlpha(0.6f, 0.5f, true);
        
    }

    public void FadeOut()
    {
        whiteScreen.CrossFadeAlpha(0.0f, 0.5f, true);
        whiteScreen.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void ApplyAdaptation()
    {
        if(leftToggle.isOn == true)
        {
            StaticVariable.leftAdaptation = true;
        }
        if (rightToggle.isOn == true)
        {
            StaticVariable.rightAdaptation = true;
        }

        adaptationPanel.SetActive(false);
        FadeOut();
    }

    public void ShowErrorLog() {

        if (scaned == true)
        {
            leftLog.text = "";
            rightLog.text = "";
            leftErrorLog.Clear();
            rightErrorLog.Clear();

            if (leftPlayerStrategy == false)
            {
                leftErrorLog.Add("Your DLL doesnt contain class PlayerStrategy.");
            }
            if (leftPlayerStrategy == true && leftConstructorOK == false)
            {
                leftErrorLog.Add("PlayerStrategy class doesnt contain right constructor. The right form of constructor is: PlayerStrategy(GameSetting g, String s, bool b).");
            }
            if (leftPlayerStrategy == true && leftmethodIsThere == false)
            {
                leftErrorLog.Add("PlayerStrategy class doesnt contain method TickStrategy(StratrgyStorage storage)");
            }
            if (leftmethodIsThere == true && leftParameters == false)
            {
                leftErrorLog.Add("TickStrategy method doesnt take right parameters. The right form of TickStrategy method is: TickStrategy(Storage storage)");
            }
            if (leftPlayerStrategy == true && leftRuleNumber == false)
            {
                leftErrorLog.Add("Your PlayerStrategy doesnt contain method public int CurrentRuleNumber() which return current number of rule");
            }
            if (leftRuleNumber == true && leftRuleNumberReturnType == false)
            {
                leftErrorLog.Add("Your method CurrentRuleNumber() in PlayerStrategy class doesnt return right type. Right return type is int");
            }
            if (leftAdaptation == true && leftAdaptationMethod == false)
            {
                leftErrorLog.Add("Class StrategyAdaptation doesnt contain method AdaptStrategy(object logholder, object strategy)");
            }
            if (leftAdaptation == true && leftAdaptationParameters == false)
            {
                leftErrorLog.Add("Method AdaptStrategy doesnt take right parameters. The right form of AdaptStrategy is: AdaptStrategy(object logholder, object strategy)");
            }
            if (leftAdaptation == true && leftAdaptationInPlayerStrategy == false) {

                leftErrorLog.Add("If you want to use StrategyAdaptation your PlayerStrategy class has to contain method public void AdaptStrategy(List<object>)");
            }
            if(leftAdaptationInPlayerStrategy == true && leftAdaptationParametersInPlayerStrategy == false)
            {
                leftErrorLog.Add("If you want to use StrategyAdaptation your method AdaptStrategy doesen't take right parameters. The right form of this method is public void AdaptStrategy(List<object>)");
            }




            if (rightPlayerStrategy == false)
            {
                rightErrorLog.Add("Your DLL doesnt contain class PlayerStrategy.");
            }
            if (rightPlayerStrategy == true && rightConstructorOK == false)
            {
                rightErrorLog.Add("PlayerStrategy class doesnt contain right constructor. The right form of constructor is: PlayerStrategy(GameSetting g, String s, bool b).");
            }
            if (rightPlayerStrategy == true && righttmethodIsThere == false)
            {
                rightErrorLog.Add("PlayerStrategy class doesnt contain method TickStrategy(StrategyStorage storage)");
            }
            if (righttmethodIsThere == true && rightParameters == false)
            {
                rightErrorLog.Add("TickStrategy method doesnt take right parameters. The right form of TickStrategy method is: TickStrategy(Storage storage)");
            }
            if (rightPlayerStrategy == true && rightRuleNumber == false)
            {
                leftErrorLog.Add("Your PlayerStrategy doesnt contain method public int CurrentRuleNumber() which return current number of rule");
            }
            if (rightRuleNumber == true && rightRuleNumberReturnType == false)
            {
                leftErrorLog.Add("Your method CurrentRuleNumber() in PlayerStrategy class doesnt return right type. Right return type is int");
            }
            if (rightAdaptation == true && rightAdaptationMethod == false)
            {
                rightErrorLog.Add("Class StrategyAdaptation doesnt contain method AdaptStrategy(object logholder, object strategy)");
            }
            if (rightAdaptation == true && rightAdaptationParameters == false)
            {
                rightErrorLog.Add("Method AdaptStrategy doesnt take right parameters. The right form of AdaptStrategy is: AdaptStrategy(object logholder, object strategy)");
            }
            if (rightAdaptation == true && rightAdaptationInPlayerStrategy == false)
            {

                leftErrorLog.Add("If you want to use StrategyAdaptation your PlayerStrategy class has to contain method public void AdaptStrategy(List<object>)");
            }
            if (rightAdaptationInPlayerStrategy == true && rightAdaptationParametersInPlayerStrategy == false)
            {
                leftErrorLog.Add("If you want to use StrategyAdaptation your method AdaptStrategy doesen't take right parameters. The right form of this method is public void AdaptStrategy(List<object>)");
            }

            string leftFinal = "";
            string rightFinal = "";

            if(leftErrorLog.Count != 0)
            {
                foreach (string a in leftErrorLog)
                {
                    leftFinal += (a + "\n" + "\n");
                }
                leftLog.text = leftFinal;
            }
            else
            {
                leftLog.text = "No Errors.";
            }
            
            if(rightErrorLog.Count != 0)
            {
                foreach (string a in rightErrorLog)
                {
                    rightFinal += (a + "\n" + "\n");
                }
                rightLog.text = rightFinal;
            }
            else
            {
                rightLog.text = "No Errors.";
            }

        }
        else
        {
            leftLog.text = "DLL hasn't been scaned yet.";
            rightLog.text = "DLL hasn't been scaned yet.";
        }

        errorLogPanel.SetActive(true);
        FadeIn();
    }

    public void HideErrorLog() {

        errorLogPanel.SetActive(false);
        FadeOut();
    }

}
