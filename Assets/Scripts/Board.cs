using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int numberOfRows = 6;
    public int numberOfCols = 6;

    public Gem[][] board;

    public GameObject coconutPrefab;
    public GameObject applePrefab;
    public GameObject orangePrefab;
    public GameObject crystalPrefab;
    public GameObject broccoliPrefab;
    public GameObject milkPrefab;
    public GameObject breadPrefab;

    public void Awake()
    {
        InitializeBoard();
        RefreshAllBoardSprites();
    }

    public void StartBoard() {
        

    }


    public void MoveGem(Vector2 initialPosition, Vector2 direction)
    {
        Vector2 finalPosition = initialPosition + direction;
        Gem gem = board[(int)initialPosition.x][(int)initialPosition.y];
        board[(int)finalPosition.x][(int)finalPosition.y] = gem;
    }

    private void RefreshAllBoardSprites()
    {
        foreach (GameObject child in this.transform)
        {
            Destroy(child);
        }

        foreach(Gem[] row in board)
        {
            foreach(Gem gem in row)
            {
                GameObject prefab = getPrefabBasedOnGemType(gem.gem);
                GameObject gb = Instantiate(prefab, this.transform, false) as GameObject;
                gb.transform.localPosition = new Vector2(gem.positionX * 64 + 32, gem.positionY * 64 + 32);
                gb.name = "(" + gem.positionY.ToString() + ", " + gem.positionX.ToString() + ")";
            }
        }
        
    }

    private GameObject getPrefabBasedOnGemType(Gem.GemType gemtype) {
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

    private void InitializeBoard()
    {
        board = new Gem[numberOfRows][];
        for (int i = 0; i < board.Length; i++)
        {
            board[i] = new Gem[numberOfCols];
            for (int j = 0; j < board[i].Length; j++)
            {
                board[i][j] = new Gem();
                board[i][j].gem = Gem.getRandomGemType();
                board[i][j].positionX = i;
                board[i][j].positionY = j;                
            }
        }
    }


    void GetRightPrefab(Gem.GemType type)
    {

        switch (type)
        {
            case Gem.GemType.Coconut:
                break;
            case Gem.GemType.Bread:
                break;
            case Gem.GemType.Milk:
                break;
            case Gem.GemType.Crystal:
                break;
            case Gem.GemType.Apple:
                break;
            case Gem.GemType.Orange:
                break;
            case Gem.GemType.Broccoli:
                break;
            default:
                break;
        }
    }

}
