using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int numberOfObjectsInY = 6;
    public int numberOfObjectsInX = 6;

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

    private const int MINIMUN_SWAP_DISTANCE_IN_PIXELS = 64;

    public void Awake()
    {
        InitializeBoard();
        UpdateAllSpritesPositionAndNames();
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            UpdateAllSpritesPositionAndNames();
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            initialClickPosition = Input.mousePosition;
            
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            finalClickPosition = Input.mousePosition;
            if (Vector3.Distance(initialClickPosition, finalClickPosition) < MINIMUN_SWAP_DISTANCE_IN_PIXELS) {
                return;
            }
            
            float angleRad = Mathf.Atan2(finalClickPosition.y - initialClickPosition.y, finalClickPosition.x - initialClickPosition.x);
            float angleDeg = Mathf.Rad2Deg * angleRad;

            int[] el = GetElementOnPixel(initialClickPosition.x, initialClickPosition.y);

            Vector2 moveDirection;
            if (angleDeg > -30 && angleDeg < 30 && el[0] != numberOfObjectsInX-1)
            {
                moveDirection = new Vector2(1, 0);       
            } else if (angleDeg > 60 && angleDeg < 120 && el[1] != numberOfObjectsInY-1)
            {
                moveDirection = new Vector2(0, 1);
            } else if (angleDeg > 150 || angleDeg < -150 && el[0] != 0)
            {
                moveDirection = new Vector2(-1, 0);
            } else if (angleDeg > -120 && angleDeg < -60 && el[1] != 0)
            {
                moveDirection = new Vector2(0, -1);
            } else
            {
                moveDirection = new Vector2(0, 0);
            }

            MoveGem(new Vector2(el[0], el[1]), moveDirection);

            FindAllBoardMatches();
            DestroyAllMatches();
            MoveGemsDown();
            //InstantiateNewGems();
            //UpdateAllSpritesPositionAndNames();


        }
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
                board[iX][iY].GetComponent<Gem>().positionX = iX;
                board[iX][iY].GetComponent<Gem>().positionY = iY;
                MoveGameObjectToPositionDeterminedByGemComponent(board[iX][iY]);
            }
        }


    }

    private void InstantiateNewGems()
    {
        for (int i = 0; i < board.Length; i++)
        {
            for (int j = 0; j < board[i].Length; j++)
            {
                if (board[i][j] == null)
                {
                    board[i][j] = Instantiate(GetRandomPrefab(), this.transform, false) as GameObject;
                    board[i][j].GetComponent<Gem>().positionX = i;
                    board[i][j].GetComponent<Gem>().positionY = j;
                }
            }
        }
    }


    private void InitializeMatchesArrayAsFalse()
    {
        matches = new bool[numberOfObjectsInY][];
        for (int i = 0; i < matches.Length; i++)
        {
            matches[i] = new bool[numberOfObjectsInX];
        }
    }

    private void FindAllBoardMatches()
    {
        InitializeMatchesArrayAsFalse();
        checkAllPossibleVerticalMatches();
        checkAllPossibleHorizontalMatches();
    }

    private void DestroyAllMatches()
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


    private int[] GetElementOnPixel(float px, float py)
    {
        int elementX = (int) Mathf.Floor((px - 32) / 64);
        int elementY = (int) Mathf.Floor((py - 32) / 64);
        return new int[] { elementX, elementY };
    }


    public void MoveGem(Vector2 initialPosition, Vector2 direction)
    {

        Vector2 finalPosition = initialPosition + direction;

        string gemName = getNameOfObjectInPosition((int)initialPosition.x, (int)initialPosition.y);
        GameObject go = GameObject.Find(gemName);

        gemName = getNameOfObjectInPosition((int)finalPosition.x, (int)finalPosition.y);
        GameObject go2 = GameObject.Find(gemName);

        int temp_posx = go.GetComponent<Gem>().positionX;
        int temp_posy = go.GetComponent<Gem>().positionY;
        go.GetComponent<Gem>().positionX = go2.GetComponent<Gem>().positionX;
        go.GetComponent<Gem>().positionY = go2.GetComponent<Gem>().positionY;
        go2.GetComponent<Gem>().positionX = temp_posx;
        go2.GetComponent<Gem>().positionY = temp_posy;

        GameObject temp = board[(int)initialPosition.x][(int)initialPosition.y];
        board[(int)initialPosition.x][(int)initialPosition.y] = board[(int)finalPosition.x][(int)finalPosition.y];
        board[(int)finalPosition.x][(int)finalPosition.y] = temp;

        MoveGameObjectToPositionDeterminedByGemComponent(board[(int)initialPosition.x][(int)initialPosition.y]);
        MoveGameObjectToPositionDeterminedByGemComponent(board[(int)finalPosition.x][(int)finalPosition.y]);
    }




    private void checkAllPossibleVerticalMatches()
    {
        for (int i = 0; i < numberOfObjectsInY; i++)
        {
            for (int j = 0; j < numberOfObjectsInX - 2; j++)
            {
                bool containsAMatch = checkIfPositionContainsAVerticalMatch3(i, j);
                if (containsAMatch)
                {
                    matches[i][j] = true;
                    matches[i][j+1] = true;
                    matches[i][j+2] = true;
                }
            }
        }
    }




    private void checkAllPossibleHorizontalMatches()
    {
        for (int i = 0; i < numberOfObjectsInY - 2; i++)
        {
            for (int j = 0; j < numberOfObjectsInX; j++)
            {
                bool containsAMatch = checkIfPositionContainsAHorizontalMatch3(i, j);
                if (containsAMatch)
                {
                    matches[i][j] = true;
                    matches[i+1][j] = true;
                    matches[i+2][j] = true;
                }
            }
        }
    }


    private bool checkIfPositionContainsAVerticalMatch3(int posX, int posY) {
        if (numberOfObjectsInY - posY <= 2) return false;
        Gem.GemType gemtype = getGemTypeAtPosition(posX, posY);
        Gem.GemType gemtype2 = getGemTypeAtPosition(posX, posY+1);
        Gem.GemType gemtype3 = getGemTypeAtPosition(posX, posY+2);
        if (gemtype == gemtype2 && gemtype == gemtype3)
        {
            Debug.Log(posX + " " + posY);
            return true;
        }
        return false;
    }


    private bool checkIfPositionContainsAHorizontalMatch3(int posX, int posY)
    {
        if (numberOfObjectsInX - posX <= 2) return false;
        Gem.GemType gemtype = getGemTypeAtPosition(posX, posY);
        Gem.GemType gemtype2 = getGemTypeAtPosition(posX + 1, posY);
        Gem.GemType gemtype3 = getGemTypeAtPosition(posX + 2, posY);
        if (gemtype == gemtype2 && gemtype == gemtype3)
        {
            Debug.Log(posX + " " + posY);
            return true;
        }
        return false;
    }


    private Gem.GemType getGemTypeAtPosition(int posX, int posY)
    {
        string gemName = getNameOfObjectInPosition(posX, posY);
        GameObject go = GameObject.Find(gemName);
        return go.GetComponent<Gem>().gem;
    }


    private string getNameOfObjectInPosition(int posX, int posY) {
        return "(" + posX.ToString() + ", " + posY.ToString() + ")";
    }


    private void UpdateAllSpritesPositionAndNames()
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
        go.transform.localPosition = new Vector2(gemComponent.positionX*64 + 32, gemComponent.positionY*64 + 32);
        go.name = getNameOfObjectInPosition(gemComponent.positionX, gemComponent.positionY);
    }


    private void InitializeBoard()
    {
        board = new GameObject[numberOfObjectsInY][];
        for (int i = 0; i < board.Length; i++)
        {
            board[i] = new GameObject[numberOfObjectsInX];
            for (int j = 0; j < board[i].Length; j++)
            {
                board[i][j] = Instantiate(GetRandomPrefab(), this.transform, false) as GameObject;
                board[i][j].GetComponent<Gem>().positionX = i;
                board[i][j].GetComponent<Gem>().positionY = j;
            }
        }
    }


    private GameObject GetRandomPrefab()
    {
        int i = Random.Range(0, 7);
        return getPrefabBasedOnGemType((Gem.GemType)i);
    }


    private GameObject getPrefabBasedOnGemType(Gem.GemType gemtype)
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
