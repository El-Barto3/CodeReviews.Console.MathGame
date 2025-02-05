

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Net.Sockets;

int[] maxValueOfNumber = { 10, 30, 100 };
int[] minValueOfNumber = { 1, 10, 30 };
int[] minValueOfNumberForDivide = { 2, 2, 3 };
string[] standardGames = ["ADDITION", "SUBSTRACTION", "MULTIPLICATION", "DIVISION"];
string[] mainMenuStrings = ["STANDARD", "RANDOMIZER", "CHOOSE DIFFICULTY", "GAMES HISTORY"];
string[] difficultiesLevel = ["EASY", "NORMAL", "HARD"];
char[] opperands = ['+', '-', '*', '/'];
var randomVar = new Random();
int gameLength = 5;
int difficultyLevel = 0;
string[] gamesHistory = new string[10];
Stopwatch gameTimer = new Stopwatch();


void dividingHandler(ref int firstNumber,ref int secondNumber)
{
    //can be prime, 0 or smaller than divider
    while (isPrime(firstNumber))
        firstNumber = randomVar.Next(minValueOfNumber[difficultyLevel], maxValueOfNumber[difficultyLevel]);

    //find divider that will give int outcome
    while (firstNumber % secondNumber != 0 || firstNumber == secondNumber || secondNumber == 1)
        secondNumber = randomVar.Next(minValueOfNumberForDivide[difficultyLevel], firstNumber - 1);
}
int enterWholeGame(int option)
{
    var pointsSum = 0;
    var mode = 0;
    if (option == 0)
    {
        mode = showMenu(standardGames, "Choose your game:");
        if (mode == -1)
            return -1;
    }

    gameTimer.Restart();
    for (int i = 0; i < gameLength; i++)
    {
        if (option == 0)
            pointsSum += startBasicGame(mode, (i + 1), gameLength);
        else if (option == 1)
            pointsSum += startBasicGame((randomVar.Next() % standardGames.Length), (i + 1), gameLength);
    }

    return pointsSum;
}

int findLastNonEmpty(string[] array)
{
    int lastIndex = -1;
    for (int i = array.Length - 1; i >= 0; i--)
    {
        if (array[i] != null)
        {
            lastIndex = i;
            break; // Stop at the first non-empty element
        }
    }
    return lastIndex;
}
bool isPrime(int number)
{
    if(number <= 2)
        return true;

    var limit = Math.Ceiling(Math.Sqrt(number));
    for (int i = 2;i <= limit; i++)
    {
        if (number % i == 0)
            return false;
    }

    return true;
}

int startBasicGame(int selectedGame, int currentRound, int maxRounds)
{
    Console.Clear();
    Console.WriteLine($"Calculate this equation ({currentRound}/{maxRounds}):");
    gameTimer.Start();

    int correctAnswer = 0;
    //need to fix making numbers
    int firstNumber = randomVar.Next(minValueOfNumber[difficultyLevel], maxValueOfNumber[difficultyLevel]);
    int secondNumber = randomVar.Next(minValueOfNumber[difficultyLevel], maxValueOfNumber[difficultyLevel]);

    switch (selectedGame)
    {
        case 0:
            correctAnswer = firstNumber + secondNumber;
            break;
        case 1:
            correctAnswer = firstNumber - secondNumber;
            break;
        case 2:
            correctAnswer = firstNumber * secondNumber;
            break;
        case 3:
            secondNumber = randomVar.Next(minValueOfNumberForDivide[difficultyLevel], maxValueOfNumber[difficultyLevel]);
            dividingHandler(ref firstNumber, ref secondNumber);
            correctAnswer = firstNumber / secondNumber;
            break;

    }

    Console.Write($"{firstNumber} {opperands[selectedGame]} {secondNumber} = ");
    var userAnswer = Console.ReadLine();
    var point = 0;

    if (userAnswer != null && userAnswer.Equals(correctAnswer.ToString()))
    {
        Console.WriteLine("Good job!");
        point = 1;
    }
    else
        Console.WriteLine("Wrong answer!");

    gameTimer.Stop();
    Console.WriteLine("Press any key to " + (currentRound == maxRounds ? "exit" : "continue"));
    Console.ReadKey();

    return point;

}

int showMenu(string[] namesTable, string menuHeader, int selectedOption = 0)
{
    ConsoleKey clickedKey;
    do
    {
        Console.Clear();
        if(menuHeader != null)
            Console.WriteLine(menuHeader);

        for (int i = 0; i < namesTable.Length; i++)
        {
            if (selectedOption == i)
                Console.Write("--> ");
            Console.WriteLine(namesTable[i]);
        }

        clickedKey = Console.ReadKey().Key;

        //UP click
        if (clickedKey == ConsoleKey.UpArrow)
        {
            if (selectedOption <= 0)
                selectedOption = namesTable.Length - 1;
            else
                selectedOption -= 1;
        }

        //DOWN click
        if (clickedKey == ConsoleKey.DownArrow)
        {
            if (selectedOption >= (namesTable.Length-1))
                selectedOption = 0;
            else
                selectedOption += 1;
        }       
        
        //ESC click
        if (clickedKey == ConsoleKey.Escape)
        {
            return -1;
        }

    } while (clickedKey != ConsoleKey.Enter);

    return selectedOption;
}


//menu for choosing mode of game
void main()
{
    int outcome;
    //making game in loop until user quits
    do
    {
        //show main menu
        outcome = showMenu(mainMenuStrings, "Navigate with arrows and use ESC to go back");

        //randomizer and standard game and saving to history handler
        if (outcome == 0 || outcome == 1)
        {
            var playerPoints = enterWholeGame(outcome);

            if (playerPoints == -1)
                continue;

            var indexOfEmpty = Array.IndexOf(gamesHistory, null);
            if (indexOfEmpty == -1)
            {
                indexOfEmpty = gamesHistory.Length;
                Array.Resize(ref gamesHistory, (gamesHistory.Length + 10));
            }

            var tempNumber = (indexOfEmpty + 1).ToString().PadRight(8);
            var tempDifficulty = (difficultiesLevel[difficultyLevel]).PadRight(16);
            var tempMode = mainMenuStrings[outcome].PadRight(16);
            var tempPoints = (playerPoints + "/" + gameLength).ToString().PadRight(16);
            var tempTime = String.Format("{0:00}:{1:00}:{2:00}", gameTimer.Elapsed.Hours, gameTimer.Elapsed.Minutes, gameTimer.Elapsed.Seconds);

            gamesHistory[indexOfEmpty] = tempNumber + tempDifficulty + tempMode + tempPoints + tempTime;
        }

        //difficulty level
        if (outcome == 2)
        {
            var newDifficulty = showMenu(difficultiesLevel, "Choose difficulty", difficultyLevel);
            difficultyLevel = newDifficulty != -1 ? newDifficulty : difficultyLevel;

        }   
        
        //games history panel
        if (outcome == 3)
        {
            var lastValid = findLastNonEmpty(gamesHistory);
            var historyMenuTitle = "Games history\n" + "No.".PadRight(8) + "Difficulty".PadRight(16) + "Mode".PadRight(16) + "Score".PadRight(16) + "Time";

            showMenu(gamesHistory[0..(lastValid + 1)], historyMenuTitle, -1); //dont show choosing arrow and null elements when loaded
        }
    } while (outcome != -1);
}

main();