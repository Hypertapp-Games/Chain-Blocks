using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BrokenBlocks : MonoBehaviour
{
    
    private int[,] grid;
    private int rows;
    private int cols;

    private int currentRows;
    private int currentColumns;
    
    private GameObject[,] gridObject;
    
    public GameObject quad;
    public List<Color> tileColors;
    public string path;

    public bool IsChain = false;
    
    

    private void Start()
    {
        grid = LoadArrayFromFile(path);
        LoadGridVisual();
    }

 
    private bool isDrag = false;
    public  void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            int[,] clonedMatrix = CloneMatrix(grid);
            FindInEachId(4, ref clonedMatrix);
        }
    }
    int[,] CloneMatrix(int[,] original)
    {
        int[,] clone = new int[currentRows, currentColumns];

        for (int i = 0; i < currentRows; i++)
        {
            for (int j = 0; j < currentColumns; j++)
            {
                clone[i, j] = original[i, j];
            }
        }

        return clone;
    }

    void FindInEachId(int k, ref int[,] clonedMatrix )
    {
        List<GameObject> test = new List<GameObject>();
        List<int> testID = new List<int>();
        for (int i = 0; i < currentRows; i++)
        {
            for (int j = 0; j < currentColumns; j++)
            {
                if (clonedMatrix[i, j] == k)
                {
                    FindGroupLoop(k, ref clonedMatrix, i, j , ref test, true);
                    
                    if (test.Count > 1)
                    {
                        Debug.Log(test.Count);
                        for (int l = 0; l < test.Count; l++)
                        {
                            float x = test[l].transform.position.x;
                            float y = test[l].transform.position.y;
                            int gridX = currentRows - (int)y;
                            testID.Add(gridX);
                            int gridY = (int)x;
                            testID.Add(gridY);
                            
                            test[l].gameObject.SetActive(false);
                            //Debug.Log("Adjacent value at position (" + gridX + ", " + gridY + ") is: " + grid[gridX, gridY]);

                        }
                    }
                   test.Clear();

                }
            }
        }

        PushEmptyPosition(ref testID);
        //LoadGridVisual();

    }

    void PushEmptyPosition(ref List<int> testID)
    {
        List<int> numberEmptyEachColumns = new List<int>();
        for (int i = 0; i < 10; i++)
        {
            int emptyCount = 0;
            int minRow = int.MaxValue;
            for (int j = 0; j < testID.Count; j+=2)
            {
                if (testID[j + 1] == i)
                {
                    emptyCount++;
                    minRow = Mathf.Min(minRow, testID[j]);
                }
            }

            if (emptyCount > 0)
            {
                numberEmptyEachColumns.Add(i);
                numberEmptyEachColumns.Add(minRow);
            }
        }
        
        for (int j = 0; j < testID.Count; j += 2)
        {
            grid[testID[j], testID[j + 1]] = -1;
        }

        for (int i = 0; i < numberEmptyEachColumns.Count; i += 2)
        {
            int column = numberEmptyEachColumns[i];
            int firstRowToSkip = numberEmptyEachColumns[i + 1];
            
            PushToEmpty(column, firstRowToSkip);
        }
    }

    void PushToEmpty(int column, int firstRow)
    {
        for (int i = firstRow; i < currentRows; i++)
        {
            if (grid[i, column] == -1)
            {
                if (!checkHaveNonEmptyCell(i, column))
                {
                    grid[i, column] = 0;
                    var tile = Instantiate(quad, new Vector3(column, currentRows - i, 0), Quaternion.identity);
                    gridObject[i, column] = tile;
                    tile.transform.SetParent(gameObject.transform);
                    
                    ChangeTileColor(i,column);
                }
            }
        }
    }

    bool checkHaveNonEmptyCell(int current, int column)
    {
        for (int i = current; i < currentRows; i++)
        {
            if (grid[i, column] != -1)
            {
                grid[current, column] = grid[i, column];
                
                gridObject[current, column] = gridObject[i, column];
                //new Vector3(j, currentRows - i, 0)
                gridObject[current, column].transform.position = new Vector3(column, currentRows - current, 0);
                
                grid[i, column] = -1;
                return true;
            }
        }
        return false;
    }
    
    // void PushEmptyPosition(ref List<int> testID)
    // {
    //     List<int> numberEmptyEachColumns = new List<int>();
    //     for (int i = 0; i < 10; i++)
    //     {
    //         int minRow = int.MaxValue;
    //         int maxRow = int.MinValue;
    //         int emptyCount = 0;
    //
    //         for (int j = 0; j < testID.Count; j += 2)
    //         {
    //             if (testID[j + 1] == i) // Kiểm tra cột
    //             {
    //                 emptyCount++;
    //                 minRow = Mathf.Min(minRow, testID[j]);
    //                 maxRow = Mathf.Max(maxRow, testID[j]);
    //             }
    //         }
    //
    //         if (emptyCount > 0)
    //         {
    //             numberEmptyEachColumns.Add(i);
    //             numberEmptyEachColumns.Add(emptyCount);
    //             numberEmptyEachColumns.Add(minRow);
    //             numberEmptyEachColumns.Add(maxRow);
    //         }
    //     }
    //
    //     for (int i = 0; i < numberEmptyEachColumns.Count; i += 4)
    //     {
    //         int column = numberEmptyEachColumns[i];
    //         int emptyCount = numberEmptyEachColumns[i+1];
    //         int firstRowToSkip = numberEmptyEachColumns[i + 2];
    //         int lastRowToSkip = numberEmptyEachColumns[i + 3];
    //
    //         if (emptyCount > 0)
    //         {
    //             Push(column, firstRowToSkip, lastRowToSkip, emptyCount);
    //         }
    //     }
    // }
    //
    // void Push(int column, int firstRowToSkip, int lastRowToSkip, int emptyCount)
    // {
    //     //Debug.Log("colum "+ column + "  num" + emptyCount );
    //     for (int i = firstRowToSkip; i < currentRows; i++)
    //     {
    //         gridObject[i, column].transform.position += new Vector3(0, emptyCount, 0);
    //         if (i <= lastRowToSkip)
    //         {
    //             Destroy(gridObject[i, column].gameObject);
    //         }
    //         if (i + emptyCount < currentRows)
    //         {
    //             grid[i, column] = grid[i + emptyCount, column];
    //             gridObject[i, column] = gridObject[i + emptyCount, column];
    //             
    //         }
    //         else
    //         {
    //             grid[i, column] = 0;
    //             var tile = Instantiate(quad, new Vector3(column, currentRows - i, 0), Quaternion.identity);
    //             gridObject[i, column] = tile;
    //             tile.transform.SetParent(gameObject.transform);
    //             
    //             ChangeTileColor(i,column);
    //         }
    //     }
    //
    //
    // }
    
    
    void FindGroupLoop(int k, ref int[,] clonedMatrix , int i, int j ,  ref List<GameObject> test, bool onlyFirst)
    {
        List<int> id = new List<int>();
   
        
        if (clonedMatrix[i, j] != k)
            return;
        clonedMatrix[i, j] = 0;
        test.Add(gridObject[i, j]);

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                int neighborX = i + x;
                int neighborY = j + y;

                // Kiểm tra xem vị trí của hàng xóm có hợp lệ không
                if (neighborX >= 0 && neighborX < currentRows && neighborY >= 0 && neighborY < currentColumns && (x == 0 || y == 0))
                {
                    // Loại trừ trường hợp là phần tử chính nó
                    if (!(x == 0 && y == 0))
                    {
                        if (clonedMatrix[neighborX, neighborY] == k)
                        {
                            id.Add(neighborX);
                            id.Add(neighborY);
                            Debug.Log("Adjacent value at position (" + neighborX + ", " + neighborY + ") is: " + clonedMatrix[neighborX, neighborY]);
                        }
                    }
                    
                }
            }
        }
        // for (int l = 0; l < id.Count; l++)
        // {
        //     Debug.Log(id[l]);
        // }
        for (int l = 0; l < id.Count; l += 2)
        {
            FindGroupLoop(k, ref clonedMatrix, id[l], id[l + 1], ref test, false);
        }
        
    }

    
    private int[,] LoadArrayFromFile(string filePath)
    {
        string[] lines = File.ReadAllLines(filePath);
    
    
        int numRows = lines.Length;
        int numCols = lines[0].Split(',').Length;
    
    
        grid = new int[numRows, numCols];
    
    
        for (int i = 0; i < numRows; i++)
        {
            string[] values = lines[i].Split(',');
    
            for (int j = 0; j < numCols; j++)
            {
    
                int.TryParse(values[j], out grid[i, j]);
            }
        }
    
        return grid;
    }
    public void LoadGridVisual()
    {
        if (transform.childCount != 0)
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            transform.DetachChildren();
        }

        if (gameObject.transform.childCount == 0)
        {
            currentRows = grid.GetLength(0);
            currentColumns = grid.GetLength(1);

            gridObject = new GameObject[currentRows, currentColumns];
            for (int i = 0; i < currentRows; i++)
            {
                for (int j = 0; j < currentColumns; j++)
                {
                    var tile = Instantiate(quad, new Vector3(j, currentRows - i, 0), Quaternion.identity);
                    gridObject[i, j] = tile;
                    tile.transform.SetParent(gameObject.transform);

                    ChangeTileColor(i,j);
                }
            }
        }
        
    }
    void ChangeTileColor( int i , int j)
    {
        GameObject tile = gridObject[i, j];
        var color = tileColors[grid[i,j]];
        tile.GetComponent<Renderer>().material.color = color;


    }
    
    public void DeBugNumberAray(int[,] number)
    {
        // Debug.Log("       " + number[0, 0] + "       " + number[0, 1] + "       " + number[0, 2] + "       " + number[0, 3] + "       " + number[0, 4]);
        // Debug.Log("       " + number[1, 0] + "       " + number[1, 1] + "       " + number[1, 2] + "       " + number[1, 3] + "       " + number[1, 4]);
        // Debug.Log("       " + number[2, 0] + "       " + number[2, 1] + "       " + number[2, 2] + "       " + number[2, 3] + "       " + number[2, 4]);
        // Debug.Log("       " + number[3, 0] + "       " + number[3, 1] + "       " + number[3, 2] + "       " + number[3, 3] + "       " + number[3, 4]);
        // Debug.Log("       " + number[4, 0] + "       " + number[4, 1] + "       " + number[4, 2] + "       " + number[4, 3] + "       " + number[4, 4]);
        Debug.Log("       " + number[0, 0] + "       " + number[0, 1] + "       " + number[0, 2] + "       " +
                  number[0, 3] + "       " + number[0, 4] + "       " + number[0, 5] + "       " + number[0, 6] +
                  "       " + number[0, 7] + "       " + number[0, 8] + "       " + number[0, 9]);
        Debug.Log("       " + number[1, 0] + "       " + number[1, 1] + "       " + number[1, 2] + "       " +
                  number[1, 3] + "       " + number[1, 4] + "       " + number[1, 5] + "       " + number[1, 6] +
                  "       " + number[1, 7] + "       " + number[1, 8] + "       " + number[1, 9]);
        Debug.Log("       " + number[2, 0] + "       " + number[2, 1] + "       " + number[2, 2] + "       " +
                  number[2, 3] + "       " + number[2, 4] + "       " + number[2, 5] + "       " + number[2, 6] +
                  "       " + number[2, 7] + "       " + number[2, 8] + "       " + number[2, 9]);
        Debug.Log("       " + number[3, 0] + "       " + number[3, 1] + "       " + number[3, 2] + "       " +
                  number[3, 3] + "       " + number[3, 4] + "       " + number[3, 5] + "       " + number[3, 6] +
                  "       " + number[3, 7] + "       " + number[3, 8] + "       " + number[3, 9]);
        Debug.Log("       " + number[4, 0] + "       " + number[4, 1] + "       " + number[4, 2] + "       " +
                  number[4, 3] + "       " + number[4, 4] + "       " + number[4, 5] + "       " + number[4, 6] +
                  "       " + number[4, 7] + "       " + number[4, 8] + "       " + number[4, 9]);
        Debug.Log("       " + number[5, 0] + "       " + number[5, 1] + "       " + number[5, 2] + "       " +
                  number[5, 3] + "       " + number[5, 4] + "       " + number[5, 5] + "       " + number[5, 6] +
                  "       " + number[5, 7] + "       " + number[5, 8] + "       " + number[5, 9]);
        Debug.Log("       " + number[6, 0] + "       " + number[6, 1] + "       " + number[6, 2] + "       " +
                  number[6, 3] + "       " + number[6, 4] + "       " + number[6, 5] + "       " + number[6, 6] +
                  "       " + number[6, 7] + "       " + number[6, 8] + "       " + number[6, 9]);
        Debug.Log("       " + number[7, 0] + "       " + number[7, 1] + "       " + number[7, 2] + "       " +
                  number[7, 3] + "       " + number[7, 4] + "       " + number[7, 5] + "       " + number[7, 6] +
                  "       " + number[7, 7] + "       " + number[7, 8] + "       " + number[7, 9]);
        Debug.Log("       " + number[8, 0] + "       " + number[8, 1] + "       " + number[8, 2] + "       " +
                  number[8, 3] + "       " + number[8, 4] + "       " + number[8, 5] + "       " + number[8, 6] +
                  "       " + number[8, 7] + "       " + number[8, 8] + "       " + number[8, 9]);
        Debug.Log("       " + number[9, 0] + "       " + number[9, 1] + "       " + number[9, 2] + "       " +
                  number[9, 3] + "       " + number[9, 4] + "  ");
    }

    
}
