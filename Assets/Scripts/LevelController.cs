public class LevelController 
{
    public static int actualLevel = 1;
    public static int desiredScore = 20;
    public static bool isScoreBeaten = false;
    public static int elapsedTime = 0;

    public static void setNextLevelVariables() {
        actualLevel += 1;
        desiredScore += 10;
        isScoreBeaten = false;
        elapsedTime = 0;
    }

    public static void ResetVariables()
    {
        actualLevel = 1;
        desiredScore = 20;
        isScoreBeaten = false;
        elapsedTime = 0;
    }
}
