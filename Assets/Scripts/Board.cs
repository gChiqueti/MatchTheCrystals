using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    public int numberOfObjectsInY = 6;
    public int numberOfObjectsInX = 6;
    public Score Score;

    public AudioClip clickGem;
    public AudioClip swap;
    public AudioClip match;
    public AudioSource source;

    public GameObject[][] board;
    public bool[][] matches;

    public GameObject coconutPrefab;
    public GameObject applePrefab;
    public GameObject orangePrefab;
    public GameObject crystalPrefab;
    public GameObject broccoliPrefab;
    public GameObject milkPrefab;
    public GameObject breadPrefab;

    private Vector3 initialClickPosition;
    private Vector3 finalClickPosition;

    private const int MINIMUN_SWAP_DISTANCE_IN_PIXELS = 16;
    private const float MAX_SWAP_ANGULATION_IN_DEG = 90.0f; // value between 0 and 90 degrees;
    private const int MAX_SIZE_OF_BOARD = 6;

    private const int FIXED_SCREEN_WIDTH = 448;

    public void Awake()
    {
        InitializeBoardWithRandomGems();
        InitializeMatchesArrayWithFalseValues();
        UpdateAllGemsPositionsAndNames();

    }

    private void Update()
    {
        if (HaveAnyMatchesInActualGame())
        {
            InitializeMatchesArrayWithFalseValues();
            PopulateMatchesArrayWithMatches();
            DestroyAllGemsThatIsInAMatch();
            MoveGemsDown();
            InstantiateNewGemsInOrigin();
            UpdateAllGemsPositionsAndNames();
            return;
        }


        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            initialClickPosition = Input.mousePosition;
            Debug.Log(initialClickPosition);
            initialClickPosition = initialClickPosition * FIXED_SCREEN_WIDTH / Screen.width;

            source.clip = clickGem;
            source.Play();
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            finalClickPosition = Input.mousePosition;
            finalClickPosition = finalClickPosition * FIXED_SCREEN_WIDTH / Screen.width;

            float swap_distance_pixels = Vector3.Distance(initialClickPosition, finalClickPosition);
            if (swap_distance_pixels <= MINIMUN_SWAP_DISTANCE_IN_PIXELS)
            {
                return;
            }
            
            Point clickedPoint = GetElementOnPixel(initialClickPosition.x, initialClickPosition.y);
            if (clickedPoint.x < 0 || clickedPoint.x >= numberOfObjectsInX || 
                clickedPoint.y < 0 || clickedPoint.y >= numberOfObjectsInY)
            {
                return;
            }

            // moveDirection é sempre um desses valores: Point(1, 0); Point(-1, 0); Point(0, 1); Point(0, -1) ou Point(0, 0)
            Point moveDirection = GetMovementDirectionOfGem(initialClickPosition, finalClickPosition);

            source.clip = swap;
            source.Play();

            MoveGem(clickedPoint, moveDirection);

            InitializeMatchesArrayWithFalseValues();
            PopulateMatchesArrayWithMatches();
            CalculateAndUpdateScore();
            DestroyAllGemsThatIsInAMatch();
            MoveGemsDown();
            InstantiateNewGemsInOrigin();
            UpdateAllGemsPositionsAndNames();

        }
    }

    private void CalculateAndUpdateScore()
    {
        int score = 0;
        for (int i = 0; i < matches.Length; i++) {
            for (int j = 0; j < matches[i].Length; j++) {
                if (matches[i][j] == true) {
                    score += 1;
                }
            }
        }

        Score.AddScore(score);
    }


    private Point GetMovementDirectionOfGem(Vector2 initialClickPosition, Vector2 finalClickPosition)
    {
        Point clickedPoint = GetElementOnPixel(initialClickPosition.x, initialClickPosition.y);
        float angleRad = Mathf.Atan2(finalClickPosition.y - initialClickPosition.y, finalClickPosition.x - initialClickPosition.x);
        float angleDeg = Mathf.Rad2Deg * angleRad;

        float half_max_angle = MAX_SWAP_ANGULATION_IN_DEG / 2;
        if (angleDeg > -half_max_angle && 
            angleDeg <  half_max_angle && 
            clickedPoint.x != numberOfObjectsInX-1)
        {
            return new Point(1, 0);
        }
        else if (angleDeg > 90 - half_max_angle && 
                angleDeg <  90 + half_max_angle && 
                clickedPoint.y != numberOfObjectsInY - 1)
        {
            return new Point(0, 1);
        }
        else if (angleDeg >  180 - half_max_angle || 
                 angleDeg < -180 + half_max_angle && 
                 clickedPoint.x != 0)
        {
            return new Point(-1, 0);
        }
        else if (angleDeg > -90 - half_max_angle && 
                 angleDeg < -90 + half_max_angle && 
                 clickedPoint.y != 0)
        {
            return new Point(0, -1);
        }
        else
        {
            return new Point(0, 0);
        }
    }

    private bool GameHaveAPossibleMoveToMake()
    {
        var boardCopy = (GameObject[][]) board.Clone();

        for (int i = 0; i < numberOfObjectsInX-1; i++)
        {
            for (int j = 0; j < numberOfObjectsInY-1; j++)
            {
                InitializeMatchesArrayWithFalseValues();
                board = (GameObject[][]) boardCopy.Clone();
                MoveGem(new Point(i, j), new Point(1, 0));
                PopulateMatchesArrayWithMatches();
                if (HaveAnyMatchesInActualGame())
                {
                    InitializeMatchesArrayWithFalseValues();
                    board = (GameObject[][])boardCopy.Clone();
                    return true;
                }
            }
        }
        InitializeMatchesArrayWithFalseValues();
        board = (GameObject[][])boardCopy.Clone();
        return false;
    }

    private bool HaveAnyMatchesInActualGame() {
        for (int i = 0; i < matches.Length; i++)
        {
            for (int j = 0; j < matches[i].Length; j++)
            {
                if (matches[i][j] == true)
                {
                    return true;
                }
            }
        }
        return false;
    }


    private void MoveGemsDown()
    {
        for (int i = 0; i < numberOfObjectsInX; i++)
        {
            moveAllGemsFromAxisYDown(i);
        }
    }


    private void moveAllGemsFromAxisYDown(int iX) {
        GameObject[] newColumn = new GameObject[numberOfObjectsInY];

        int actualXIndex = 0;
        for (int iY = 0; iY < numberOfObjectsInY; iY++)
        {
            if (board[iX][iY] != null)
            {
                newColumn[actualXIndex] = board[iX][iY];
                actualXIndex += 1;
            }
        }

        for (int iY = 0; iY < numberOfObjectsInY; iY++)
        {
            board[iX][iY] = newColumn[iY];
            if (board[iX][iY] != null)
            {
                board[iX][iY].GetComponent<Gem>().position = new Point(iX, iY);
                MoveGameObjectToPositionDeterminedByGemComponent(board[iX][iY]);
            }
        }


    }

    private void InstantiateNewGemsInOrigin()
    {
        for (int i = 0; i < board.Length; i++)
        {
            for (int j = 0; j < board[i].Length; j++)
            {
                if (board[i][j] == null)
                {
                    board[i][j] = Instantiate(GetRandomPrefab(), this.transform, false) as GameObject;
                    board[i][j].GetComponent<Gem>().position = new Point(i, j);  
                }
            }
        }
    }


    private void InitializeMatchesArrayWithFalseValues()
    {
        matches = new bool[numberOfObjectsInY][];
        for (int i = 0; i < matches.Length; i++)
        {
            matches[i] = new bool[numberOfObjectsInX];
        }
    }

    private void PopulateMatchesArrayWithMatches()
    {
        InitializeMatchesArrayWithFalseValues();
        CheckAllVerticalMatches();
        CheckAllHorizontalMatches();
    }

    private void DestroyAllGemsThatIsInAMatch()
    {
        for (int i = 0; i < numberOfObjectsInY; i++)
        {
            for (int j = 0; j < numberOfObjectsInX; j++)
            {
                if (matches[i][j])
                {
                    Destroy(board[i][j]);
                    board[i][j] = null;
                }
            }
        }
    }


    private Point GetElementOnPixel(float px, float py)
    {
        int x = (int) Mathf.Floor((px - 32) / 64);
        int y = (int) Mathf.Floor((py - 32) / 64);
        return new Point(x, y);
    }


    public void MoveGem(Point initialPosition, Point direction)
    {

        Point finalPosition = initialPosition + direction;

        string gemName = GetNameOfObjectInPosition(initialPosition);
        GameObject go = GameObject.Find(gemName);

        gemName = GetNameOfObjectInPosition(finalPosition);
        GameObject go2 = GameObject.Find(gemName);

        Point tempPoint = go.GetComponent<Gem>().position;
        go.GetComponent<Gem>().position = go2.GetComponent<Gem>().position;
        go2.GetComponent<Gem>().position = tempPoint;

        GameObject temp = board[initialPosition.x][initialPosition.y];
        board[initialPosition.x][initialPosition.y] = board[finalPosition.x][finalPosition.y];
        board[finalPosition.x][finalPosition.y] = temp;

        MoveGameObjectToPositionDeterminedByGemComponent(board[initialPosition.x][initialPosition.y]);
        MoveGameObjectToPositionDeterminedByGemComponent(board[finalPosition.x][finalPosition.y]);
    }




    private void CheckAllVerticalMatches()
    {
        for (int i = 0; i < numberOfObjectsInY; i++)
        {
            for (int j = 0; j < numberOfObjectsInX - 2; j++)
            {
                bool containsAMatch = CheckIfPointStartsAVerticalMatch3(new Point(i, j));
                if (containsAMatch)
                {
                    matches[i][j] = true;
                    matches[i][j+1] = true;
                    matches[i][j+2] = true;
                }
            }
        }
    }




    private void CheckAllHorizontalMatches()
    {
        for (int i = 0; i < numberOfObjectsInY - 2; i++)
        {
            for (int j = 0; j < numberOfObjectsInX; j++)
            {
                bool containsAMatch = CheckIfPointStartsAHorizontalMatch3(new Point(i, j));
                if (containsAMatch)
                {
                    matches[i][j] = true;
                    matches[i+1][j] = true;
                    matches[i+2][j] = true;
                }
            }
        }
    }


    private bool CheckIfPointStartsAVerticalMatch3(Point point) {
        if (numberOfObjectsInY - point.y <= 2) return false;
        Gem.GemType gemtype = GetGemTypeAtPosition(point);
        Gem.GemType gemtype2 = GetGemTypeAtPosition(point + new Point(0,1));
        Gem.GemType gemtype3 = GetGemTypeAtPosition(point + new Point(0,2));
        if (gemtype == gemtype2 && gemtype == gemtype3)
        {
            return true;
        }
        return false;
    }


    private bool CheckIfPointStartsAHorizontalMatch3(Point point)
    {
        if (numberOfObjectsInX - point.x <= 2) return false;
        Gem.GemType gemtype1 = GetGemTypeAtPosition(point);
        Gem.GemType gemtype2 = GetGemTypeAtPosition(point + new Point(1, 0));
        Gem.GemType gemtype3 = GetGemTypeAtPosition(point + new Point(2, 0));
        if (gemtype1 == gemtype2 && gemtype1 == gemtype3)
        {
            return true;
        }
        return false;
    }


    private Gem.GemType GetGemTypeAtPosition(Point point)
    {
        string gemName = GetNameOfObjectInPosition(point);
        GameObject go = GameObject.Find(gemName);
        return go.GetComponent<Gem>().gem;
    }


    private string GetNameOfObjectInPosition(Point point) {
        return "(" + point.x.ToString() + ", " + point.y.ToString() + ")";
    }


    private void UpdateAllGemsPositionsAndNames()
    {
        foreach(GameObject[] row in board)
        {
            foreach(GameObject go in row)
            {
                MoveGameObjectToPositionDeterminedByGemComponent(go);
            }
        }
    }


    private void MoveGameObjectToPositionDeterminedByGemComponent(GameObject go) {
        Gem gemComponent = go.GetComponent<Gem>();
        go.transform.localPosition = new Vector2(gemComponent.position.x*64 + 32, 
                                                 gemComponent.position.y*64 + 32);
        go.name = GetNameOfObjectInPosition(gemComponent.position);
    }


    private void InitializeBoardWithRandomGems()
    {
        board = new GameObject[numberOfObjectsInY][];
        for (int i = 0; i < board.Length; i++)
        {
            board[i] = new GameObject[numberOfObjectsInX];
            for (int j = 0; j < board[i].Length; j++)
            {
                board[i][j] = Instantiate(GetRandomPrefab(), this.transform, false) as GameObject;
                board[i][j].GetComponent<Gem>().position = new Point(i, j);
            }
        }
    }


    private GameObject GetRandomPrefab()
    {
        int i = UnityEngine.Random.Range(0, 7);
        return GetPrefabBasedOnGemType((Gem.GemType)i);
    }


    private GameObject GetPrefabBasedOnGemType(Gem.GemType gemtype)
    {
        switch (gemtype)
        {
            case Gem.GemType.Coconut:
                return coconutPrefab;
            case Gem.GemType.Bread:
                return breadPrefab;
            case Gem.GemType.Milk:
                return milkPrefab;
            case Gem.GemType.Crystal:
                return crystalPrefab;
            case Gem.GemType.Apple:
                return applePrefab;
            case Gem.GemType.Orange:
                return orangePrefab;
            case Gem.GemType.Broccoli:
                return broccoliPrefab;
            default:
                throw new UnityException("prefab nao existe");
        }
    }

}
