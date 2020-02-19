using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Hacker : MonoBehaviour
{
    // there is probably a better place to put this
    const System.StringComparison IGNORE_CASE = System.StringComparison.OrdinalIgnoreCase;

    // Game configuration data
    private List<Dictionary<string, string>> keys = new List<Dictionary<string, string>>();
    private Dictionary<string, string> levelOne = new Dictionary<string, string>();
    private Dictionary<string, string> levelTwo = new Dictionary<string, string>();
    private Dictionary<string, string> levelThree = new Dictionary<string, string>();

    private string[] levelOnePasswords = {"password", "qwerty", "dragon", "shadow", "monkey"};
    private string[] levelTwoPasswords = { "opendoor", "runaway", "matchbox", "playstation", "breakaleg" };
    private string[] levelThreePasswords = { "autodidact", "trangam", "imponderabilia", "euchred", "langlauf" };

    private string[] levelOneHints = { "drop saws", "qtyWRE", "Gordan", "do wash", "mn ok ye" };
    private string[] levelTwoHints = { "peon rood", "a awn Ruy", "cat hobmx", "ay Platonist", "Abel Garek" };
    private string[] levelThreeHints = { "Audi do tact", "gnat RAM", "binomial diaper", "hued rec", "flag Luna" };


    // Variables for game difficulty levels
    private string[] difficulty = { "the kindergarten", "the police station", "CIA" };

    // Game state
    private int level;
    private int sublevel;
    private int wrong;
    private List<int> randomIndex = new List<int>();
    private enum Screen {MainMenu, Password, Win, Fail};
    private Screen currentScreen;
    private string password;

    // Start is called before the first frame update
    void Start()
    {
        for (sublevel = 0; sublevel < 5; sublevel++)
        {
            levelOne.Add(levelOnePasswords[sublevel], levelOneHints[sublevel]);
            levelTwo.Add(levelTwoPasswords[sublevel], levelTwoHints[sublevel]);
            levelThree.Add(levelThreePasswords[sublevel], levelThreeHints[sublevel]);

        }

        keys.Add(levelOne);
        keys.Add(levelTwo);
        keys.Add(levelThree);

        ShowMainMenu();
    }

    void ShowMainMenu() 
    {
        Terminal.ClearScreen();
        currentScreen = Screen.MainMenu;
        level = 0;
        sublevel = 0;
        wrong = 0;

        Terminal.WriteLine(@"
 ----------------------------------
| Welcome to The Hacking Simulator™|
 ----------------------------------
");
        for(int i = 0; i < 3; i++) 
        {
            Terminal.WriteLine("Press " + (i+1) +  " for " + difficulty[i]);
        }
        Terminal.WriteLine("Type \"quit\" to exit\n");
        Terminal.WriteLine("Choose wisely:");

    }

    void RunMainMenu(string input)
    {
        // Creating a random number generator that will select passwords and hints in StartGame()
        // Clearing list in case the game was already played once
        randomIndex.Clear();

        while (randomIndex.Count < 5)
        {
            int index = Random.Range(0, 5);
            if (!randomIndex.Contains(index))
            {
                randomIndex.Add(index);
            }
        }

        bool isValidLevelNumber = (input == "1" || input == "2" || input == "3");

        if (isValidLevelNumber)
        {
            level = int.Parse(input);
            StartGame();
        }

        else if (input == "007") //easter egg
        {
            Terminal.WriteLine("Please select a level Mr Bond.");
        }

        else
        {
            Terminal.WriteLine("Please select a valid level.");
        }

    }
    void CheckPassword (string input)
    {
        
        if (input.Equals(password, IGNORE_CASE))
        {
            sublevel += 1;
            wrong = 0;
            WinGame();          
        }
        else
        {
            wrong += 1;
            Terminal.WriteLine("Wrong. Try again.");

            if (wrong == 5)
            {
                FailGame();
            }

        }
    }

    // decides which method handles the input
    void OnUserInput(string input)
    {
        print("User input: " + input);

        if (input == "menu") // we can always go to main menu directly
            {
                ShowMainMenu();
            }

        else if (input == "quit")
        {
            Application.Quit();
        }

        else if (currentScreen == Screen.MainMenu) 
        {
            RunMainMenu(input);
        }

        else if (currentScreen == Screen.Password)
        {
            CheckPassword(input);
        }

    }

    void StartGame()
    {
        Dictionary<string, string> selectedLevel = keys[level - 1];
        currentScreen = Screen.Password;
       
        if (sublevel == 0)
        {
            Terminal.WriteLine("Starting level " + level + ".");
            Terminal.ClearScreen();
            Terminal.WriteLine("Enter \"menu\" to go back.");
            Terminal.WriteLine("Remember that the password will always be one word.\n");
        }

        if (sublevel < 5) {
            Terminal.WriteLine("Hint: " + selectedLevel.ElementAt(randomIndex[sublevel]).Value);
            password = selectedLevel.ElementAt(randomIndex[sublevel]).Key;

            Terminal.WriteLine("Please enter your password:");
        }
        
    }

    void WinGame()
    {
        if (sublevel < 5) {

            Terminal.WriteLine("\nGreat job! Next assignment (" + (sublevel) + "/5 done).\n");
            StartGame();

        } 

        else
        {
            DisplayWinMessage();
        }      
        
    }

    private void DisplayWinMessage()
    {
        Terminal.ClearScreen();
        currentScreen = Screen.Win;

        switch(level)
        {
            case 1:
                Terminal.WriteLine(@"
   __ __         __          ____
  / // /__ _____/ /_____ ___/ / /
 / _  / _ `/ __/  '_/ -_) _  /_/ 
/_//_/\_,_/\__/_/\_\\__/\_,_(_) 
             ");

                Terminal.WriteLine("Congratulations!\n\nYou\'ve hacked " + difficulty[level - 1] + "!\n\nGo to menu to hack " + difficulty[level] + ".");
                break;
            case 2:
                Terminal.WriteLine(@"
  _____             __  __
 / ___/______ ___ _/ /_/ /
/ (_ / __/ -_) _ `/ __/_/ 
\___/_/  \__/\_,_/\__(_)   
             ");

                Terminal.WriteLine("Congratulations!\n\nYou\'ve hacked " + difficulty[level - 1] + "!\n\nGo to menu to hack " + difficulty[level] + ".");
                break;
            case 3:
                Terminal.WriteLine(@"
       __  ___         __          __
      /  |/  /__ ____ / /____ ____/ /
     / /|_/ / _ `(_-</ __/ -_) __/_/ 
    /_/  /_/\_,_/___/\__/\__/_/ (_)
                 ");

                Terminal.WriteLine("Congratulations!\nYou\'re the world's best hacker now!\nYou can now bask in your own glory.\nOr enter \"menu\" to start over.\n");
                Terminal.WriteLine("Thanks for playing!");
                break;
            default:
                Debug.LogError("Invalid level number");
                break;
        } 
    }

    void FailGame()
    {
        currentScreen = Screen.Fail;
        Terminal.ClearScreen();
        Terminal.WriteLine("You've failed 5 times in a row.\nThe police are coming.\n\nEnter \"menu\" to try start over.");
    }

    // Update is called once per frame, not used in this project
    void Update()
    {
        
    }
}
