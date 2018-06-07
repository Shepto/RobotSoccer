# Robot Soccer Simulator

 This program is intended for simulating soccer game played between two robot teams. These robots have defined behavior by tactics and strategies definitions, which defines how they should behave in certain situations in game field like positions of enemy robots, ball and playmates. As was mentioned this program is intended for simulating soccer game and also to allow user to define his own tactics and strategies for robots. These strategies and tactics are loaded dynamically to simulator in dll file and then scanning because dll file have to keep some structure otherwise it wont work properly. This structure is described bellow.  This project also contains yet complete strategy and tactics project as an example for you. So you can see how it all works inside and you can inspire by it.


**This manual consists of two chapters:**

* [**Basic manual**](#basic-manual) - for users who just wants to run this simulator
* [**Advanced manual**](#advanced-manual) – for users who wants to see inside simulator and wants to implement their own strategies and tactics definitions


## Basic manual

How to download and run this robot soccer simulator:
* Download Unity version 5.0.4 and install – other versions are not supported!!!.
* Download this project from github
* Launch Unity and in launcher press „Open Other” button
* Find downloaded file and select FooProject folder and load it to Unity
* Unity will start and just simply play it by clicking on play button on upper center bar

### User interface

Here is step by step descibed user interface and other funcionality of this simulator.

#### Main menu

![Main menu](/images/mainMenu.PNG) 

This menu allows to load defined strategy rules definitions for left and right team. This strategies you can define by our program tool called MENO and simply load them here. This only allow you to load .strg type of files.


**What are strategy rules files** - We have two types of the representation of the game field. The first type is an abstract coordinate system. There the physical coordinates of the real objects on the field are mapped to a standard coordinate system. Because this logical representation is very detailed it is used mainly for the actual robot control and the image analysis which requires the greatest possible accuracy. However this very detailed representation is not suitable for the strategy description. The strategy contains a set of rules that describes how the robots should behave in a given situation. The strategy based on a very detailed coordinate system would have to include a large number of rules and most of these rules  would have been very similar to  describe almost the same situation on the game field. Therefore we introduce the term so called grid coordinates. Grid coordinates have much lower resolution and are used just for the definition of strategies and of underlying rules, see Figure 1. This simplification is sufficient for us because for strategic planning where we do not need to know the exact position of each robot. All we need is the approximate location of the robot represented by the grid coordinates.

<p align="center">
  <img src="/images/grid.PNG">
</p>

As we explained, the strategy is the finite set of the rules that describe the current situation on the game field and thus it says where to move my robots in this situation. Each rule can be easily expressed as a quaternion (M, O, B, D), where M are the grid coordinates of my robots, O are the coordinates of opponent's robots, B are coordinates of the ball and D are coordinates of where we want to move our robots. An example of a rule is described in  table below.

<p align="center">
  <img src="/images/rule.PNG">
</p>

#### Loading Strategies DLLs menu

This menu allows to load dll files which contains definitions of tactics and strategies for left and right team in dll file format. Dll files are then scanned for desired structure. If dll file wont contains errors user will be free to start simulator, otherwise it wont start and user will be informed that he has some errors in his dll file. Informations about errors are in Error Log in this menu and user can see them just by clicking on Error Log button.

**But if you dont want to create your own dll files just use our dll files and run the game (when strategy adaptation menu appear just leave boxes unchecked and run the siumlator).**

![Dll menu](/images/dllMenu.PNG)

![Error log](/images/errorLog.PNG)

If your dll files wont contains errors and you have implemented strategy adaptation in your dll file simple menu popups asking you if you want to use strategy adaptation in game (more about strategy adaptation is described bellow). 

![Adaptation menu](/images/adaptationMenu.PNG)

Then simply choose and hit apply and you will be free to start simulator.


#### Game field UI

Game field UI consists of 3 sections:

1. Left panel contains total score information, speed of the game, play/pause button, restart and reset button.
2. Upper panel contains informations about score of current game, who has ball under control and number of current game.
3. Right panel is refree panel with buttons which represents some game situations as: penalty kick, place kick, goal kick etc.

![Game field](/images/gameField.PNG)

Lastly, if you want to exit game or pause you simply hit escape button. Then game will pause and menu appears which allows you to exit to menu or quit the game.

![Esc menu](/images/escMenu.PNG)


## Advanced manual

**Prerequisites**
* Unity version: 5.0.4 - other versions are not supported!
* Visual Studio 2013 or 2015
* Your dll file have to be implemented on .NET 3.5

**How to download**
* just simply download it here from github repository [here](https://github.com/It4innovations/RobotSoccer)

**The most important files and libraries inside repository:**
* [**CoreStrategyTactic**](https://github.com/It4innovations/RobotSoccer/tree/master/CoreStrategyTactic)– visual studio project which contain projects: 
  * Core - library which contains information about game settings and core functionality of simulator
  * StrategiesDLL – our example project of implementation tactics and strategies
* [**FooProject**](https://github.com/It4innovations/RobotSoccer/tree/master/FooProject) – Unity project
* **RobotSoccerUnity.sln file** – visual studio solution file – run this on version 2013 or 2015 – after running this, visual studio starts and you will see all implemented projects mentioned above as: Core, StrategiesDLL and all unity scripts as you can see on picture down below.

<p align="center">
  <img src="/images/solution.PNG">
</p>

As inspiration for you we have our own strategies and tactics project implemented - StrategiesDLL, as you can see above, so you can change things there and explore it more deeply and you can understand its functionality quicker.

#### Steps how to create your dll file
1. Create your own visual studio library project or use our example project (don't forget to implement it on .NET 3.5 if you are creating your own project)
2. Implement Core library to your project
3. Stick to mandatory structure [**down below**](#structure-of-robot-soccer-simulator) otherwise it wont work!
4. Build your project and simply find your dll and load it in simulator in [**load dll menu**](#loading-strategies-dlls-menu)


#### Structure of robot soccer simulator

If you want to create your own strategies and tactis dll file for this simulator you have to stick to some rules while you‘ll be creating your project otherwise you won´t be able to load your dll file to the simulator.

Simple model of this simulator:

<p align="center">
  <img src="/images/structure.PNG">
</p>

On picture above there is simple model of simulator and some inside structure of dll file loaded to simulator. So as you can see your dll have to contain some Strategy, Tactic and optional class StrategyAdaptation. Dll file is loaded dynamically to simulator and from dll is called main function TickStrategy with parameter StrategyStorage from simulator and then return this StrategyStorage instance back to the simulator.

If you want to be able to create your own dll file for this simulator you have to implement library Core, which is main library with all inside functionality of this simulator. This library contains all informations of robots and ball, especially their positions which you will need most. This information is stored in class StrategyStorage. 


**There are two ways how you can create your project for thist simulator:**

1. Without Strategy Adaptation class – only main structure
2. With Strategy Adaptation class implementation


#### Main Structure (without Strategy Adaptation class)

Your dll have to contain:

* Class PlayerStrategy
*	Constructor of class PlayerStrategy with parameters: GameSetting, String and Bool. GameSetting is class inside core library which contains information about game settings and game field, String is name of file which contains information about process of the game and lastly bool parameter indicates if its left or right team dll.
* In class PlayerStrategy main function TickStrategy, which is called in simulator.
* Function TickStrategy have to take as parameter instance of class StrategyStorage(its class inside Core library and cointain positions of all robots and ball).
* Class PlayerStrategy have to contain function CurrentRuleNumber, which return actual number of using rule.

<p align="center">
  <img src="/images/mainStructure.PNG">
</p>

This is mandatory structure of dll. Other content of dll is just up to you.

#### Main structure with Strategy Adaptation class implementation

If you want to use Strategy Adaptation you have to stick to all rules of Main Structure above and implement this:

* Class StrategyAdaptation
* Constructor of class StrategyAdaptation without parameters
* In StrategyAdaptation class have to be main function AdaptStrategy returning list of type object, where objects are strategies. As parameters this function have to take object of class LogHolder and object of class Strategy
* Class LogHolder, which contains informations about position of robots and game information as time, score etc. This class you can copy from code down below.
* Class PlayerStrategy have to contain function AdaptStrategy, which takes as parameter List<object>

**LogHolder class implementation:**

```cs
public class PositionsHolder
{
        public PositionsHolder(int lr, int rr, Vector2D ball, Vector2D r1, Vector2D r2, Vector2D r3, Vector2D r4, Vector2D r5, Vector2D l1, Vector2D l2, Vector2D l3, Vector2D l4, Vector2D l5, string score, string time, int control) 
        {
            ballPosition = new Vector2D(0.0, 0.0);
            leftPlayerRobots = new Vector2D[5];
            rightPlayerRobots = new Vector2D[5];

            lRule = lr;
            rRule = rr;
            ballPosition = ball;

            leftPlayerRobots[0] = l1;
            leftPlayerRobots[1] = l2;
            leftPlayerRobots[2] = l3;
            leftPlayerRobots[3] = l4;
            leftPlayerRobots[4] = l5;

            rightPlayerRobots[0] = r1;
            rightPlayerRobots[1] = r2;
            rightPlayerRobots[2] = r3;
            rightPlayerRobots[3] = r4;
            rightPlayerRobots[4] = r5;

            this.score = score;
            this.time = time;
            this.control = control;
        }

        public int lRule;
        public int rRule;
        public Vector2D ballPosition;
        public Vector2D[] leftPlayerRobots;
        public Vector2D[] rightPlayerRobots;
        public string score;
        public string time;
        public int control;
}

public class LogHolder
{
    public List<PositionsHolder> positions;


    public LogHolder()
        {
            positions = new List<PositionsHolder>(); 
        }
}
```

As you can see this class contains class PositionHolder which contains constructor with parameters of left and right currently using rule, ball position, positions of left and right robots, score, time and ball control. Lastly this class contains constructor LogHolder() in which is created new instance of PositionHolder class.

<p align="center">
  <img src="/images/strategyStructure.PNG">
</p>

This is mandatory structure of dll. Other content of dll is just up to you.

#### Load your dll to simulator

If you are finished with your own project you simply start simulator and in Load Strategies and Tactics menu click on Browse button and find and load your dll. Then click on button Scan so simulator will scan your dll file. If your dll file wont contains errors you will be free to start simulator, otherwise it wont start and you will be informed that you have some errors in your dll file. Informations about errors are in Error Log in this menu and you can see them just by clicking on Error Log button.
