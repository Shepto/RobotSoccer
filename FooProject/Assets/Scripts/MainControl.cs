using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using Core;
using System.Threading;
using UnityEngine.UI;
using System.Reflection;

public class MainControl : MonoBehaviour
{
    public bool adapted = false;
    
    public GameObject BluePlayer1;
    public GameObject BluePlayer2;
    public GameObject BluePlayer3;
    public GameObject BluePlayer4;
    public GameObject BlueGoal;

    public GameObject RedPlayer1;
    public GameObject RedPlayer2;
    public GameObject RedPlayer3;
    public GameObject RedPlayer4;
    public GameObject RedGoal;

    public GameObject GameBall;

    public Texture2D highLight = null;

    public float GameSpeed = 1;
    int minutes;
    int seconds;

    public string ballControlString = "No one";
    int ballControl = 0;
    
    public int GameTime = 120;
    double gameTimeCounter = 120;
    int leftGoalCount = 0;
    int rightGoalCount = 0;
    int totalLeftGoalCount = 0;
    int totalRightGoalCount = 0;
    int gameCount = 0;
    Unit BP1, BP2, BP3, BP4, BG, RP1, RP2, RP3, RP4, RG;
    List<Unit> units;
    private Vector3 startBallPosition;
    private LogHolder logHolder;
    private bool save = true;
    public bool starting = true;
    public string leftStrategy = @"strategie\MissingDefMROffR.strg";
    public string rightStrategy = @"strategie\StrategyFull.strg";


    //GAME UI
    public Text leftGoal;
    public Text rightGoal;
    public Text timer;
    public Text numberofGames;
    public Text inControl;
    public Text totalGoalRight;
    public Text totalGoalLeft;
    public Text speedOfTheGame;

    private bool paused = false;
    public Button pause;
    public Button play;
    public GameObject menu;
    public Image whiteScreen;


    //ASSEMBLY
    private static Assembly leftDLLassembly;
    private static Assembly rightDLLassembly;
    
    private object leftInstance;
    private object rightInstance;

    private static Type leftType;
    private static Type rightType;

    private static MethodInfo MethodTickLeft;
    private static MethodInfo MethodTickRight;
    private static MethodInfo MethodLeftAdaptStrategy;
    private static MethodInfo MethodRightAdaptStrategy;

    //adaptation
    private object leftStrategyAdaptation;
    private static Type leftAdaptationType;
    private static MethodInfo leftAdaptStrategyMethod;

    private object rightStrategyAdaptation;
    private static Type rightAdaptationType;
    private static MethodInfo rightAdaptStrategyMethod;

    void Awake()
    {
      
        BP1 = (Unit)BluePlayer1.GetComponent(typeof(Unit));
        BP2 = (Unit)BluePlayer2.GetComponent(typeof(Unit));
        BP3 = (Unit)BluePlayer3.GetComponent(typeof(Unit));
        BP4 = (Unit)BluePlayer4.GetComponent(typeof(Unit));
        BG = (Unit)BlueGoal.GetComponent(typeof(Unit));
        BP1.watchBall = true;
        BP2.watchBall = true;
        BP3.watchBall = true;
        BP4.watchBall = true;
        BG.watchBall = true;

        RP1 = (Unit)RedPlayer1.GetComponent(typeof(Unit));
        RP2 = (Unit)RedPlayer2.GetComponent(typeof(Unit));
        RP3 = (Unit)RedPlayer3.GetComponent(typeof(Unit));
        RP4 = (Unit)RedPlayer4.GetComponent(typeof(Unit));
        RG = (Unit)RedGoal.GetComponent(typeof(Unit));
        RP1.watchBall = true;
        RP2.watchBall = true;
        RP3.watchBall = true;
        RP4.watchBall = true;
        RG.watchBall = true;

        units = new List<Unit>();
        units.Add(BP1);
        units.Add(BP2);
        units.Add(BP3);
        units.Add(BP4);
        units.Add(BG);
        units.Add(RP1);
        units.Add(RP2);
        units.Add(RP3);
        units.Add(RP4);
        units.Add(RG);

        startBallPosition = GameBall.transform.position;
        //slightly move the ball
        GameBall.transform.position = new Vector3(
            startBallPosition.x + (float)UnityEngine.Random.Range(0, 5),
            startBallPosition.y,
            startBallPosition.z + (float)UnityEngine.Random.Range(0, 5));
        

        //load assembly
        leftDLLassembly = Assembly.LoadFile(StaticVariable.leftDllStrategy);
        rightDLLassembly = Assembly.LoadFile(StaticVariable.rightDLLStrategy);

        //assembly ..... ziskanie triedy PlayerStrategy z oboch DLL a nasledne metod ThickStrategy
        foreach (Type t in leftDLLassembly.GetExportedTypes())
        {
            if (t.IsPublic && t.Name == "PlayerStrategy")
            {
                leftType = t;
            }

            //adaptation
            if(t.IsPublic && t.Name == "StrategyAdaptation" && StaticVariable.leftAdaptation == true)
            {
                leftAdaptationType = t;
            }
        }

        foreach (Type t in rightDLLassembly.GetExportedTypes())
        {
            if (t.IsPublic && t.Name == "PlayerStrategy")
            {
                rightType = t;
            }

            //adaptation
            if (t.IsPublic && t.Name == "StrategyAdaptation" && StaticVariable.rightAdaptation == true)
            {
                rightAdaptationType = t;
            }
        }

        //ziskanie pozadovanych metod z nacitanych DLL suborov
        MethodTickLeft = leftType.GetMethod("TickStrategy");
        MethodTickRight = rightType.GetMethod("TickStrategy");
       

        //adaptation....ak uzivatel chce pouzivat StrategyAdaptation
        if(StaticVariable.leftAdaptation == true)
        {
            leftAdaptStrategyMethod = leftAdaptationType.GetMethod("AdaptStrategy");
            MethodLeftAdaptStrategy = leftType.GetMethod("AdaptStrategy");
        }

        if(StaticVariable.rightAdaptation == true)
        {
            rightAdaptStrategyMethod = rightAdaptationType.GetMethod("AdaptStrategy");
            MethodRightAdaptStrategy = leftType.GetMethod("AdaptStrategy");
        }    

        Starts();
    }

    void Start()
    {
        Time.timeScale = GameSpeed;
        gameTimeCounter = GameTime;
        logHolder = new LogHolder();

        menu.SetActive(false);
        play.gameObject.SetActive(false);
        whiteScreen.canvasRenderer.SetAlpha(0.0f);
    }


    //GUI 
    void OnGUI()
    {
        if (starting)
        {
            LoadLeftPlayerStrategy(StaticVariable.leftStrategy);
            LoadRightPlayerStrategy(StaticVariable.rightStrategy);
            starting = false;
        }

        leftGoal.text = leftGoalCount.ToString();
        rightGoal.text = rightGoalCount.ToString();
        timer.text = String.Format("{0:00}:{1:00}", minutes, seconds);
        numberofGames.text = gameCount.ToString();

        if (ballControlString == "Left")
        {
            inControl.text = ballControlString;
        }

        if (ballControlString == "Right")
        {
            inControl.text = ballControlString;
        }

        totalGoalLeft.text = totalLeftGoalCount.ToString();
        totalGoalRight.text = totalRightGoalCount.ToString();
        speedOfTheGame.text = GameSpeed.ToString();

    }
    
    public void IncreaseGameSpeed() {

        GameSpeed++;
    }
    
    public void DecreaseGameSpeed(){
        
        if (GameSpeed > 1)
        {
            GameSpeed--;
        }
        else {
        }
    }


    public void PauseGame() {

        pause.gameObject.SetActive(false);
        play.gameObject.SetActive(true);
        GameSpeed = 0;
        
    }

    //odpauzovanie hry
    public void PlayGame() {

        play.gameObject.SetActive(false);
        pause.gameObject.SetActive(true);
        menu.SetActive(false);
        FadeOut();
        GameSpeed = 1;
    }

    //vratenie do menu
    public void ReturnToMenu() {

        Application.LoadLevel(0);
    }

    //ukoncenie programu
    public void QuitGame() {

        Application.Quit();
    }

    //funkcia na zahmlenie obrazovky pri stlaceni esc
    public void FadeIn() {

        whiteScreen.GetComponent<CanvasGroup>().blocksRaycasts = true;
        whiteScreen.CrossFadeAlpha(0.6f, 0.5f, true);
    }

    //funkcia na odhmlenie obrazovky pri stlaceni esc
    public void FadeOut(){

        whiteScreen.CrossFadeAlpha(0.0f, 0.5f, true);
        whiteScreen.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }


    //*********** FUNKCIE PRE ROZHODCU *************** 

    //******* LEFT *******
    //left free kick
    public void LeftFreeKick() {

        InitializeObjects(SituationType.BlueFreeKick);   
    }

    //left goal kick
    public void LeftGoalKick()
    {

        InitializeObjects(SituationType.RedGoalKick);
    }

    //left penalty kick
    public void LeftPenaltyKick()
    {

        InitializeObjects(SituationType.BluePenaltyKick);
    }

    //left place kick
    public void LeftPlaceKick()
    {

        InitializeObjects(SituationType.BluePlaceKick);
    }

    //left right free ball
    public void LeftRightFreeBall()
    {

        InitializeObjects(SituationType.BRFreeBall);
    }

    //left left free ball
    public void LeftLeftFreeBall()
    {

        InitializeObjects(SituationType.BLFreeBall);
    }

    //******* RIGHT *******
    public void RightFreeKick()
    {

        InitializeObjects(SituationType.RedFreeKick);
    }

    //left goal kick
    public void RightGoalKick()
    {

        InitializeObjects(SituationType.BlueGoalKick);
    }

    //left penalty kick
    public void RightPenaltyKick()
    {

        InitializeObjects(SituationType.RedPenaltyKick);
    }

    //left place kick
    public void RightPlaceKick()
    {

        InitializeObjects(SituationType.RedPlaceKick);
    }

    //left right free ball
    public void RightRightFreeBall()
    {

        InitializeObjects(SituationType.TRFreeBall);
    }

    //left left free ball
    public void RightLeftFreeBall()
    {

        InitializeObjects(SituationType.TLFreeBall);
    }



    public void ResetToStartPosition()
    {
        foreach (Unit u in units)
        {
            u.Reset();
        }
        GameBall.transform.position = new Vector3(
            startBallPosition.x + (float)UnityEngine.Random.Range(0, 5),
            startBallPosition.y,
            startBallPosition.z + (float)UnityEngine.Random.Range(0, 5));
        GameBall.GetComponent<Rigidbody>().velocity = Vector3.zero;
        NoOneControl();
    }

    public void GameReset()
    {
        ResetToStartPosition();
        rightGoalCount = 0;
        leftGoalCount = 0;
        gameTimeCounter = GameTime;
        save = true;
    }

    public void LeftGoal()
    {
        leftGoalCount++;
        totalLeftGoalCount++;
        ResetToStartPosition();
    }
    public void RightGoal()
    {
        rightGoalCount++;
        totalRightGoalCount++; 
        ResetToStartPosition();
    }
    public void NoOneControl()
    {
        ballControlString = "No one";
        ballControl = 0;
    }
    public void LeftControl()
    {
        ballControlString = "Left";
        ballControl = 1;
    }
    public void RightControl()
    {
        ballControlString = "Right";
        ballControl = 2;
    }


    void Update()
    {
        Time.timeScale = GameSpeed;

        //esc menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
          GameSpeed = 0;
          menu.SetActive(true);
          FadeIn();
          
        }
    }

    void FixedUpdate()
    {
        minutes = (int)(gameTimeCounter / 60);
        seconds = (int)(gameTimeCounter % 60);
        if (!starting)
        {
            if (gameTimeCounter > 0)
            {
                gameTimeCounter -= Time.fixedDeltaTime;
                Tick();
            }
            else if (save)
            {
                DateTime cas = DateTime.Now;
                _logFileCsv = (cas.Month + "-" + cas.Day + "_" + cas.Hour + "-" + cas.Minute + "-" + cas.Second); //nazev logu
                logHolder.LogToTextFileFromHolder(_logFileCsv);
                gameCount++;
                GameReset();            
            }
        }
    }

    Mutex m = SharedMutex.getMutex();

    public static double ROBOT_EDGE_SIZE = 10;                      // velikost hrany robota
    public static double ROBOT_MAX_VEL = 1;                         // maximalni rychlost robota

    public static double BALL_RADIUS_SIZE = 1.6;                    // polomer micku
    public static double BALL_FRICTION = 0.99;                      // treni micku
    public static double BALL_ELASTICITY = 1.1;                     // pruznost micku
    public static double BALL_MIN_VEL = 0.02;                       // minimalni rychlsot micku (pri nizsi uz stoji) 
    public static double BALL_MAX_VEL = 2;                          // maximalni rychlost micku

    public static double AREA_X = 220;                              // sirka hriste
    public static double AREA_Y = 180;                              // vyska hriste
    public static double AREA_NET_X = 15;                           // hloubka branky
    public static double AREA_NET_Y = 40;                           // delka branky
    public static double AREA_CORNER = 7;                           // rozmer zkoseni rohu hriste
    public static double AREA_BORDER = 2.9;                         // sirka okraje hriste

    public static float GRAPHICS_SCALE = 2;                         // meritko zobrazeni


    public static GameSetting SimulatorGameSetting;
    private Storage storage;

    //nove instancie tried StrategyStorage kvoli mirrored Storage
    private StrategyStorage leftStorage;
    private StrategyStorage rightStorage;


    public static Color COLOR_BALL = Color.yellow;                  // barva micku
    public static Color COLOR_RIGHT_PLAYER_ROBOT = Color.blue;      // barva robotu praveho hrace
    public static Color COLOR_LEFT_PLAYER_ROBOT = Color.red;        // barva robotu leveho hrace
    public static Color COLOR_AREA = Color.gray;                    // barva mantinelu
    public static Color COLOR_LINES = Color.gray;                   // barva car na plose hriste
    public static Color COLOR_GRID = Color.green;                   // barva mrizky

    public static int TIME_GAME_LENGTH = 120;                       // delka zapasu (v sekundach)

    public enum RobotTeam { RightPlayer, LeftPlayer };

    private RobotSoccerSimulator.RealSimulator.Ball ball;                                              // micek
    private List<RobotSoccerSimulator.RealSimulator.Robot> rightPlayerRobots;                          // roboti praveho tymu
    private List<RobotSoccerSimulator.RealSimulator.Robot> leftPlayerRobots;                           // roboti leveho tymu

    private bool isGoal;                                            // flag rikajici zdali padl gol a ma se ukoncit zapas
    private int currentTime;                                        // aktualni cas v milisekundach
    private Graphics g;                                             // instance pro vykresleni aktualni situace
    private SituationType situation;                                // aktualne zvolena situace
    
    private string _logFile = "log.xml";
    private string _logFileCsv = "log.txt";

    internal RobotSoccerSimulator.RealSimulator.Ball Ball
    {
        get { return ball; }
        set { ball = value; }
    }
    internal List<RobotSoccerSimulator.RealSimulator.Robot> RightPlayerRobots
    {
        get { return rightPlayerRobots; }
        set { rightPlayerRobots = value; }
    }
    internal List<RobotSoccerSimulator.RealSimulator.Robot> LeftPlayerRobots
    {
        get { return leftPlayerRobots; }
        set { leftPlayerRobots = value; }
    }
    
    public String LogFile
    {
        get { return _logFileCsv; }
    }

    void Starts()
    {
        this.currentTime = 0;
        this.isGoal = false;
        rightPlayerRobots = new List<RobotSoccerSimulator.RealSimulator.Robot>();
        leftPlayerRobots = new List<RobotSoccerSimulator.RealSimulator.Robot>();

        for (int i = 0; i < 5; i++)
        {
            RobotSoccerSimulator.RealSimulator.Robot r1 = new RobotSoccerSimulator.RealSimulator.Robot(RobotSoccerSimulator.RealSimulator.Simulator.RobotTeam.RightPlayer);
            RobotSoccerSimulator.RealSimulator.Robot r2 = new RobotSoccerSimulator.RealSimulator.Robot(RobotSoccerSimulator.RealSimulator.Simulator.RobotTeam.LeftPlayer);
            rightPlayerRobots.Add(r1);
            leftPlayerRobots.Add(r2);
        }
        ball = new RobotSoccerSimulator.RealSimulator.Ball();

        //vytvoreni logu
        DateTime cas = DateTime.Now;  //aktualni cas
        _logFileCsv = (cas.Month + "-" + cas.Day + "_" + cas.Hour + ":" + cas.Minute + ":" + cas.Second); //nazev logu                                                                                                              

        //GameSettingu
        SimulatorGameSetting = new GameSetting();
        SimulatorGameSetting.AREA_NET_X = AREA_NET_X;
        SimulatorGameSetting.AREA_NET_Y = AREA_NET_Y;
        SimulatorGameSetting.AREA_X = AREA_X;
        SimulatorGameSetting.AREA_Y = AREA_Y;
        SimulatorGameSetting.BALL_FRICTION = BALL_FRICTION;
        SimulatorGameSetting.NUMBER_OF_ROBOTS = 5;
        SimulatorGameSetting.ROBOT_EDGE_SIZE = ROBOT_EDGE_SIZE;
        SimulatorGameSetting.ROBOT_MAX_VEL = ROBOT_MAX_VEL;

        storage = new Storage();

        storage.Ball.Position = this.ball.Position;
        storage.Ball.Velocity = this.ball.Velocity;

        storage.LeftRobots[0].Position = leftPlayerRobots[0].Position;
        storage.LeftRobots[1].Position = leftPlayerRobots[1].Position;
        storage.LeftRobots[2].Position = leftPlayerRobots[2].Position;
        storage.LeftRobots[3].Position = leftPlayerRobots[3].Position;
        storage.LeftRobots[4].Position = leftPlayerRobots[4].Position;
        storage.RightRobots[0].Position = rightPlayerRobots[0].Position;
        storage.RightRobots[1].Position = rightPlayerRobots[1].Position;
        storage.RightRobots[2].Position = rightPlayerRobots[2].Position;
        storage.RightRobots[3].Position = rightPlayerRobots[3].Position;
        storage.RightRobots[4].Position = rightPlayerRobots[4].Position;

        storage.LeftRobots[0].PositionMove = leftPlayerRobots[0].Position;
        storage.LeftRobots[1].PositionMove = leftPlayerRobots[1].Position;
        storage.LeftRobots[2].PositionMove = leftPlayerRobots[2].Position;
        storage.LeftRobots[3].PositionMove = leftPlayerRobots[3].Position;
        storage.LeftRobots[4].PositionMove = leftPlayerRobots[4].Position;
        storage.RightRobots[0].PositionMove = rightPlayerRobots[0].Position;
        storage.RightRobots[1].PositionMove = rightPlayerRobots[1].Position;
        storage.RightRobots[2].PositionMove = rightPlayerRobots[2].Position;
        storage.RightRobots[3].PositionMove = rightPlayerRobots[3].Position;
        storage.RightRobots[4].PositionMove = rightPlayerRobots[4].Position;

        storage.LeftRobots[0].Velocity = leftPlayerRobots[0].Velocity;
        storage.LeftRobots[1].Velocity = leftPlayerRobots[1].Velocity;
        storage.LeftRobots[2].Velocity = leftPlayerRobots[2].Velocity;
        storage.LeftRobots[3].Velocity = leftPlayerRobots[3].Velocity;
        storage.LeftRobots[4].Velocity = leftPlayerRobots[4].Velocity;
        storage.RightRobots[0].Velocity = rightPlayerRobots[0].Velocity;
        storage.RightRobots[1].Velocity = rightPlayerRobots[1].Velocity;
        storage.RightRobots[2].Velocity = rightPlayerRobots[2].Velocity;
        storage.RightRobots[3].Velocity = rightPlayerRobots[3].Velocity;
        storage.RightRobots[4].Velocity = rightPlayerRobots[4].Velocity;

        //vytvorenie instancii pre left a right storage
        leftStorage = new StrategyStorage();
        rightStorage = new StrategyStorage();
    }

    int regCounter = 0;

    // metoda zjistujici nove pokyny pro pohyb robotu ze strategii, zjistuje zdali nepadl gol, updatuje cas a pohybuje scenou
    public void Tick()
    {
        //save actual situation to storage
        storage.Ball.Position = From3Dto2D(GameBall.transform.position);
        storage.LeftRobots[0].Position = From3Dto2D(BluePlayer1.transform.position);
        storage.LeftRobots[1].Position = From3Dto2D(BluePlayer2.transform.position);
        storage.LeftRobots[2].Position = From3Dto2D(BluePlayer3.transform.position);
        storage.LeftRobots[3].Position = From3Dto2D(BluePlayer4.transform.position);
        storage.LeftRobots[4].Position = From3Dto2D(BlueGoal.transform.position);
        storage.RightRobots[0].Position = From3Dto2D(RedPlayer1.transform.position);
        storage.RightRobots[1].Position = From3Dto2D(RedPlayer2.transform.position);
        storage.RightRobots[2].Position = From3Dto2D(RedPlayer3.transform.position);
        storage.RightRobots[3].Position = From3Dto2D(RedPlayer4.transform.position);
        storage.RightRobots[4].Position = From3Dto2D(RedGoal.transform.position);

        //*********************  LEFT ****************************
        leftStorage.Ball.Position = storage.Ball.Position;
        leftStorage.myRobots[0].Position = storage.LeftRobots[0].Position;
        leftStorage.myRobots[1].Position = storage.LeftRobots[1].Position;
        leftStorage.myRobots[2].Position = storage.LeftRobots[2].Position;
        leftStorage.myRobots[3].Position = storage.LeftRobots[3].Position;
        leftStorage.myRobots[4].Position = storage.LeftRobots[4].Position;
        leftStorage.oppntRobots[0].Position = storage.RightRobots[0].Position;
        leftStorage.oppntRobots[1].Position = storage.RightRobots[1].Position;
        leftStorage.oppntRobots[2].Position = storage.RightRobots[2].Position;
        leftStorage.oppntRobots[3].Position = storage.RightRobots[3].Position;
        leftStorage.oppntRobots[4].Position = storage.RightRobots[4].Position;

        //*********************  RIGHT ****************************
        rightStorage.Ball.Position = storage.Ball.Position;
        rightStorage.myRobots[0].Position = storage.RightRobots[0].Position;
        rightStorage.myRobots[1].Position = storage.RightRobots[1].Position;
        rightStorage.myRobots[2].Position = storage.RightRobots[2].Position;
        rightStorage.myRobots[3].Position = storage.RightRobots[3].Position;
        rightStorage.myRobots[4].Position = storage.RightRobots[4].Position;
        rightStorage.oppntRobots[0].Position = storage.LeftRobots[0].Position;
        rightStorage.oppntRobots[1].Position = storage.LeftRobots[1].Position;
        rightStorage.oppntRobots[2].Position = storage.LeftRobots[2].Position;
        rightStorage.oppntRobots[3].Position = storage.LeftRobots[3].Position;
        rightStorage.oppntRobots[4].Position = storage.LeftRobots[4].Position;

        //volanie funkci TickStrategy z DLL
        object[] leftSt = new object[] { leftStorage };
        object[] rightSt = new object[] { rightStorage };

        MethodTickLeft.Invoke(leftInstance, leftSt);
        MethodTickRight.Invoke(rightInstance, rightSt);

        // ***********     LEFT        ***************
        //load new situation from storage and move robots
        BP1.MoveTo(From2Dto2D(leftStorage.myRobots[0].PositionMove));
        BP2.MoveTo(From2Dto2D(leftStorage.myRobots[1].PositionMove));
        BP3.MoveTo(From2Dto2D(leftStorage.myRobots[2].PositionMove));
        BP4.MoveTo(From2Dto2D(leftStorage.myRobots[3].PositionMove));
        BG.MoveTo(From2Dto2D(leftStorage.myRobots[4].PositionMove));
        //load boal position to robots
        BP1.ballPosition = From2Dto2D(leftStorage.Ball.Position);
        BP2.ballPosition = From2Dto2D(leftStorage.Ball.Position);
        BP3.ballPosition = From2Dto2D(leftStorage.Ball.Position);
        BP4.ballPosition = From2Dto2D(leftStorage.Ball.Position);
        BG.ballPosition = From2Dto2D(leftStorage.Ball.Position);


        // ***********     RIGHT        ***************
        //load new situation from storage and move robots
        RP1.MoveTo(From2Dto2D(rightStorage.myRobots[0].PositionMove));
        RP2.MoveTo(From2Dto2D(rightStorage.myRobots[1].PositionMove));
        RP3.MoveTo(From2Dto2D(rightStorage.myRobots[2].PositionMove));
        RP4.MoveTo(From2Dto2D(rightStorage.myRobots[3].PositionMove));
        Vector2D a = new Vector2D(-rightStorage.myRobots[4].PositionMove.X, rightStorage.myRobots[4].PositionMove.Y);
        RG.MoveTo(From2Dto2D(rightStorage.myRobots[4].PositionMove));
        //load boal position to robots
        RP1.ballPosition = From2Dto2D(rightStorage.Ball.Position);
        RP2.ballPosition = From2Dto2D(rightStorage.Ball.Position);
        RP3.ballPosition = From2Dto2D(rightStorage.Ball.Position);
        RP4.ballPosition = From2Dto2D(rightStorage.Ball.Position);
        RG.ballPosition = From2Dto2D(rightStorage.Ball.Position);


        LogToHolder();

        //************ adaptStrategy ***************
        //ak uzivatel ma v DLL adapt strategy a chce ho vyuzivat
        //left
        if(StaticVariable.leftAdaptation == true)
        {
            List<object> newRules = (List<object>)leftAdaptStrategyMethod.Invoke(leftStrategyAdaptation, new object[] { logHolder, leftInstance });

            if (newRules.Count > 0)
            {
                MethodLeftAdaptStrategy.Invoke(leftInstance, new object[] { newRules });
                Debug.Log("Added rules: " + newRules.Count);
            }
        }

        //right
        if (StaticVariable.rightAdaptation == true)
        {
            List<object> newRules = (List<object>)rightAdaptStrategyMethod.Invoke(rightStrategyAdaptation, new object[] { logHolder, rightInstance });

            if (newRules.Count > 0)
            {
                MethodRightAdaptStrategy.Invoke(rightInstance, new object[] { newRules });
                Debug.Log("Added rules: " + newRules.Count);
            }
        }
    }


    private void LogToHolder()
    {
        bool grid = true;

        //nacitanie metod ktore som pridal do PlayerStrategy v DLL aby vracalo cislo prave vyuzivaneho Rule
        MethodInfo MethodLeft = leftType.GetMethod("CurrentRuleNumber");
        MethodInfo MethodRight = rightType.GetMethod("CurrentRuleNumber");
        int leftRuleNumber;
        int rightRuleNumber;

        if (grid)
        {
            leftRuleNumber = (int)MethodLeft.Invoke(leftInstance, null);
            rightRuleNumber = (int)MethodRight.Invoke(rightInstance, null);

            logHolder.positions.Add(new PositionsHolder(
                leftRuleNumber,
                rightRuleNumber,
                RealToGrid(From3Dto2D(GameBall.transform.position)),
                RealToGrid(From3Dto2D(RedPlayer1.transform.position)),
                RealToGrid(From3Dto2D(RedPlayer2.transform.position)),
                RealToGrid(From3Dto2D(RedPlayer3.transform.position)),
                RealToGrid(From3Dto2D(RedPlayer4.transform.position)),
                RealToGrid(From3Dto2D(RedGoal.transform.position)),
                RealToGrid(From3Dto2D(BluePlayer1.transform.position)),
                RealToGrid(From3Dto2D(BluePlayer2.transform.position)),
                RealToGrid(From3Dto2D(BluePlayer3.transform.position)),
                RealToGrid(From3Dto2D(BluePlayer4.transform.position)),
                RealToGrid(From3Dto2D(BlueGoal.transform.position)),
                leftGoalCount + ":" + rightGoalCount,
                String.Format("{0:00}:{1:00}", minutes, seconds),
                ballControl
            ));

        }
        else
        {
            leftRuleNumber = (int)MethodLeft.Invoke(leftInstance, null);
            rightRuleNumber = (int)MethodRight.Invoke(rightInstance, null);

            logHolder.positions.Add(new PositionsHolder(
                    leftRuleNumber,
                    rightRuleNumber,
                    RealToGrid(From3Dto2D(GameBall.transform.position)),
                    RealToGrid(From3Dto2D(RedPlayer1.transform.position)),
                    RealToGrid(From3Dto2D(RedPlayer2.transform.position)),
                    RealToGrid(From3Dto2D(RedPlayer3.transform.position)),
                    RealToGrid(From3Dto2D(RedPlayer4.transform.position)),
                    RealToGrid(From3Dto2D(RedGoal.transform.position)),
                    RealToGrid(From3Dto2D(BluePlayer1.transform.position)),
                    RealToGrid(From3Dto2D(BluePlayer2.transform.position)),
                    RealToGrid(From3Dto2D(BluePlayer3.transform.position)),
                    RealToGrid(From3Dto2D(BluePlayer4.transform.position)),
                    RealToGrid(From3Dto2D(BlueGoal.transform.position)),
                    leftGoalCount + ":" + rightGoalCount,
                    String.Format("{0:00}:{1:00}", minutes, seconds),
                    ballControl
                ));


        }
    }
    private Vector2D RealToGrid(Vector2D realPoint)
    {
        int sizeX = 6;
        int sizeY = 4;

        return new Vector2D(
            (int)((realPoint.X + AREA_X / 2 + AREA_NET_X) / ((AREA_X + 2 * AREA_NET_X) / sizeX)) + 1,
            (int)((realPoint.Y + AREA_Y / 2) / ((AREA_Y) / sizeY)) + 1);
    }
    private Vector2D From3Dto2D(Vector3 from)
    {
        return new Vector2D(from.x, from.z);
    }
    private Vector2 From2Dto2D(Vector2D from)
    {
        return new Vector2((float)from.X, (float)from.Y);
    }

    private Vector2 From2Dto3D(Vector2D from)
    {
        return new Vector3((float)from.X, (float)from.Y, 35.0f);
    }

    private void WriteVector(StreamWriter sw, Vector2D vector2D)
    {
        sw.WriteLine("\t<x>" + vector2D.X + "</x>");
        sw.WriteLine("\t<y>" + vector2D.Y + "</y>");
    }

    // nacteni strategie leveho hrace z daneho souboru
    public void LoadLeftPlayerStrategy(String file)
    {
        leftInstance = Activator.CreateInstance(leftType, SimulatorGameSetting, file, false);  //vytvorenie instancie z DLL

        if(StaticVariable.leftAdaptation == true)
        {
            leftStrategyAdaptation = Activator.CreateInstance(leftAdaptationType, null);
        }
    }

    // nacteni strategie praveho hrace z daneho souboru
    public void LoadRightPlayerStrategy(String file)
    {
        rightInstance = Activator.CreateInstance(rightType, SimulatorGameSetting, file, true);  //vytvorenie instancie z DLL


        if (StaticVariable.rightAdaptation == true)
        {
            rightStrategyAdaptation = Activator.CreateInstance(rightAdaptationType, null);
        }
    }

    // rozmisteni robotu a micku podle zvoleneho typu situace
    public void InitializeObjects(SituationType situationType)
    {
        situation = situationType;
 
        GameBall.GetComponent<Rigidbody>().velocity = new Vector2(0, 0);

        foreach (RobotSoccerSimulator.RealSimulator.Robot r in rightPlayerRobots)
            r.Velocity = new Vector2D(0, 0);
        foreach (RobotSoccerSimulator.RealSimulator.Robot r in leftPlayerRobots)
            r.Velocity = new Vector2D(0, 0);

        if (situationType == SituationType.Start)
        {
            GameBall.transform.position = new Vector3(0, 5, 0);

            BluePlayer1.transform.position = new Vector3((float)-AREA_X / 6, 5, (float)-AREA_Y / 6);
            BluePlayer2.transform.position = new Vector3((float) -AREA_X / 6, 5, (float) AREA_Y / 6);
            BluePlayer3.transform.position = new Vector3((float) -AREA_X / 3, 5, (float) -AREA_Y / 3);
            BluePlayer4.transform.position = new Vector3((float) -AREA_X / 3, 5, (float) AREA_Y / 3);
            BlueGoal.transform.position = new Vector3((float)-AREA_X / 2 + (float)ROBOT_EDGE_SIZE / 2, 5, 0);

            RedPlayer1.transform.position = new Vector3((float)AREA_X / 6, 5, (float)-AREA_Y / 6);
            RedPlayer2.transform.position = new Vector3((float)AREA_X / 6, 5, (float)AREA_Y / 6);
            RedPlayer3.transform.position = new Vector3((float)AREA_X / 3, 5, (float)-AREA_Y / 3);
            RedPlayer4.transform.position = new Vector3((float)AREA_X / 3, 5, (float)AREA_Y / 3);
            RedGoal.transform.position = new Vector3((float)AREA_X / 2 - (float)ROBOT_EDGE_SIZE / 2, 5, 0);


        }
        else if (situationType == SituationType.BluePlaceKick)
        {
            GameBall.transform.position = new Vector3(0, 5, 0);

            RedPlayer1.transform.position = new Vector3(-12, 5, -6);
            RedPlayer2.transform.position = new Vector3(18, 5, 6);
            RedPlayer3.transform.position = new Vector3(32, 5, -18);
            RedPlayer4.transform.position = new Vector3(23, 5, 28);
            RedGoal.transform.position = new Vector3((float)AREA_X / 2 - (float)ROBOT_EDGE_SIZE / 2, 5, 0);

            BluePlayer1.transform.position = new Vector3((float)-31.5, 5, 0);
            BluePlayer2.transform.position = new Vector3((float)-22.5, 5, -22);
            BluePlayer3.transform.position = new Vector3((float)-24.5, 5, 20);
            BluePlayer4.transform.position = new Vector3((float)-62.5, 5, 0);
            BlueGoal.transform.position = new Vector3((float)-AREA_X / 2 + (float)ROBOT_EDGE_SIZE / 2, 5, 0);

        }
        else if (situationType == SituationType.BluePenaltyKick)
        {
            GameBall.transform.position = new Vector3((float)72.5, 5, 0);

            RedPlayer1.transform.position = new Vector3(-6, 5, -34);
            RedPlayer2.transform.position = new Vector3(-6, 5, 34);
            RedPlayer3.transform.position = new Vector3(-6, 5, -47);
            RedPlayer4.transform.position = new Vector3(-6, 5, 47);
            RedGoal.transform.position = new Vector3((float)AREA_X / 2 - (float)ROBOT_EDGE_SIZE / 2, 5, 0);

            BluePlayer1.transform.position = new Vector3(54, 5, 0);
            BluePlayer2.transform.position = new Vector3(-6, 5, -19);
            BluePlayer3.transform.position = new Vector3(-6, 5, 19);
            BluePlayer4.transform.position = new Vector3(-6, 5, 0);
            BlueGoal.transform.position = new Vector3((float)-AREA_X / 2 + (float)ROBOT_EDGE_SIZE / 2, 5, 0);

        }
        else if (situationType == SituationType.BlueFreeKick)
        {
            GameBall.transform.position = new Vector3(72.5f, 5, 0);

            BluePlayer1.transform.position = new Vector3(25, 5, -30);
            BluePlayer2.transform.position = new Vector3(56, 5, 0);
            BluePlayer3.transform.position = new Vector3(-41, 5, 0);
            BluePlayer4.transform.position = new Vector3(25, 5, 30);
            BlueGoal.transform.position = new Vector3((float)-AREA_X / 2 + (float)ROBOT_EDGE_SIZE / 2, 5, 0);

            RedPlayer1.transform.position = new Vector3(88, 5, -20);
            RedPlayer2.transform.position = new Vector3(88, 5, 20);
            RedPlayer3.transform.position = new Vector3(88, 5, -47);
            RedPlayer4.transform.position = new Vector3(88, 5, 47);
            RedGoal.transform.position = new Vector3((float)AREA_X / 2 - (float)ROBOT_EDGE_SIZE / 2, 5, 0);

        }
        else if (situationType == SituationType.TRFreeBall)
        {
            GameBall.transform.position = new Vector3(55, 5, -60);

            RedPlayer1.transform.position = new Vector3(35, 5, 6);
            RedPlayer2.transform.position = new Vector3(80, 5, 6);
            RedPlayer3.transform.position = new Vector3(74, 5, -60);
            RedPlayer4.transform.position = new Vector3(60, 5, 6);
            RedGoal.transform.position = new Vector3(103, 5, -17);

            BluePlayer1.transform.position = new Vector3(35, 5, -60);
            BluePlayer2.transform.position = new Vector3(0, 5, -30);
            BluePlayer3.transform.position = new Vector3(0, 5, -70);
            BluePlayer4.transform.position = new Vector3(0, 5, 6);
            BlueGoal.transform.position = new Vector3((float)-AREA_X / 2 + (float)ROBOT_EDGE_SIZE / 2, 5, 0);

        }
        else if (situationType == SituationType.BlueGoalKick)
        {
            GameBall.transform.position = new Vector3(94, 5, 0);

            RedPlayer1.transform.position = new Vector3(68, 5, -20);
            RedPlayer2.transform.position = new Vector3(68, 5, 20);
            RedPlayer3.transform.position = new Vector3(100, 5, -33);
            RedPlayer4.transform.position = new Vector3(100, 5, 33);
            RedGoal.transform.position = new Vector3((float)AREA_X / 2 - (float)ROBOT_EDGE_SIZE / 2, 5, 0);

            BluePlayer1.transform.position = new Vector3(-6, 5, -30);
            BluePlayer2.transform.position = new Vector3(-6, 5, 0);
            BluePlayer3.transform.position = new Vector3(-6, 5, 30);
            BluePlayer4.transform.position = new Vector3(-50, 5, 0);
            BlueGoal.transform.position = new Vector3((float)-AREA_X / 2 + (float)ROBOT_EDGE_SIZE / 2, 5, 0);

        }
        else if (situationType == SituationType.RedPlaceKick)
        {
            InitializeObjects(SituationType.BluePlaceKick);
            yMirrorRobotsPositions();
        }
        else if (situationType == SituationType.RedPenaltyKick)
        {
            InitializeObjects(SituationType.BluePenaltyKick);
            yMirrorRobotsPositions();
        }
        else if (situationType == SituationType.RedFreeKick)
        {
            InitializeObjects(SituationType.BlueFreeKick);
            yMirrorRobotsPositions();
        }
        else if (situationType == SituationType.TLFreeBall)
        {
            InitializeObjects(SituationType.TRFreeBall);
            yMirrorRobotsPositions();
        }
        else if (situationType == SituationType.BRFreeBall)
        {
            InitializeObjects(SituationType.TRFreeBall);
            xMirrorRobotsPositions();
        }
        else if (situationType == SituationType.BLFreeBall)
        {
            InitializeObjects(SituationType.TRFreeBall);
            xMirrorRobotsPositions();
            yMirrorRobotsPositions();
        }
        else if (situationType == SituationType.RedGoalKick)
        {
            InitializeObjects(SituationType.BlueGoalKick);
            yMirrorRobotsPositions();
        }
    }

    // funkce prevracejici y-ovou souradnici robotu a micku
    private void yMirrorRobotsPositions()
    {
        Vector3 tmp1 = RedPlayer1.transform.position;
        Vector3 tmp2 = RedPlayer2.transform.position;
        Vector3 tmp3 = RedPlayer3.transform.position;
        Vector3 tmp4 = RedPlayer4.transform.position;
        Vector3 tmp5 = RedGoal.transform.position;

        GameBall.transform.position = Vector3.Scale(GameBall.transform.position, new Vector3(-1, 1, 1));

        RedPlayer1.transform.position = Vector3.Scale(BluePlayer1.transform.position, new Vector3(-1, 1, 1));
        RedPlayer2.transform.position = Vector3.Scale(BluePlayer2.transform.position, new Vector3(-1, 1, 1));
        RedPlayer3.transform.position = Vector3.Scale(BluePlayer3.transform.position, new Vector3(-1, 1, 1));
        RedPlayer4.transform.position = Vector3.Scale(BluePlayer4.transform.position, new Vector3(-1, 1, 1));
        RedGoal.transform.position = Vector3.Scale(BlueGoal.transform.position, new Vector3(-1, 1, 1));

        BluePlayer1.transform.position = Vector3.Scale(tmp1, new Vector3(-1, 1, 1));
        BluePlayer2.transform.position = Vector3.Scale(tmp2, new Vector3(-1, 1, 1));
        BluePlayer3.transform.position = Vector3.Scale(tmp3, new Vector3(-1, 1, 1));
        BluePlayer4.transform.position = Vector3.Scale(tmp4, new Vector3(-1, 1, 1));
        BlueGoal.transform.position = Vector3.Scale(tmp5, new Vector3(-1, 1, 1));

    }

    // funkce prevracejici x-ovou souradnici robotu a micku
    private void xMirrorRobotsPositions()
    {
        GameBall.transform.position = Vector3.Scale(GameBall.transform.position, new Vector3(1, 1, -1));

        RedPlayer1.transform.position = Vector3.Scale(RedPlayer1.transform.position, new Vector3(1, 1, -1));
        RedPlayer2.transform.position = Vector3.Scale(RedPlayer2.transform.position, new Vector3(1, 1, -1));
        RedPlayer3.transform.position = Vector3.Scale(RedPlayer3.transform.position, new Vector3(1, 1, -1));
        RedPlayer4.transform.position = Vector3.Scale(RedPlayer4.transform.position, new Vector3(1, 1, -1));
        RedGoal.transform.position = Vector3.Scale(RedGoal.transform.position, new Vector3(1, 1, -1));

        BluePlayer1.transform.position = Vector3.Scale(BluePlayer1.transform.position, new Vector3(1, 1, -1));
        BluePlayer2.transform.position = Vector3.Scale(BluePlayer2.transform.position, new Vector3(1, 1, -1));
        BluePlayer3.transform.position = Vector3.Scale(BluePlayer3.transform.position, new Vector3(1, 1, -1));
        BluePlayer4.transform.position = Vector3.Scale(BluePlayer4.transform.position, new Vector3(1, 1, -1));
        BlueGoal.transform.position = Vector3.Scale(BlueGoal.transform.position, new Vector3(1, 1, -1));

    }

    private static void CopyPositionRobotList(IList<RobotSoccerSimulator.RealSimulator.Robot> src, IList<RobotSoccerSimulator.RealSimulator.Robot> dst)
    {
        for (int i = 0; i < src.Count; i++)
        {
            dst[i].Position = new Vector2D(src[i].Position.X, src[i].Position.Y);
        }
    }

    private Vector2D getPosition(String[] line, String entita, String[] systemEntit)
    {
        int index = -1;
        for (int i = 0; i < systemEntit.Length; i++)
        {
            if (systemEntit[i] == entita)
            {
                index = i;
                break;
            }
        }
        double x = Convert.ToDouble(line[index]);
        double y = Convert.ToDouble(line[index + 1]);

        return new Vector2D(x, y);
    }

    public void Log()
    {
        _logFileCsv = _logFileCsv + ".log";
        using (StreamWriter sw = File.AppendText(_logFile))
        {

            sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            sw.WriteLine("<ticks>");
        }
        using (StreamWriter sw = File.AppendText(_logFileCsv))
        {
            sw.Write("leftRule ");
            sw.Write("rightRule ;");

            sw.Write("ball.real.x;");
            sw.Write("ball.real.y;");
            sw.Write("ball.grid.x;");
            sw.Write("ball.grid.y;");
            for (int i = 0; i < leftPlayerRobots.Count; i++)
            {
                sw.Write("lr" + i + ".real.x;");
                sw.Write("lr" + i + ".real.y;");
                sw.Write("lr" + i + ".grid.x;");
                sw.Write("lr" + i + ".grid.y;");
            }
            for (int i = 0; i < rightPlayerRobots.Count; i++)
            {
                sw.Write("rr" + i + ".real.x;");
                sw.Write("rr" + i + ".real.y;");
                sw.Write("rr" + i + ".grid.x;");
                sw.Write("rr" + i + ".grid.y;");
            }
            sw.WriteLine();
        }
    }

}
