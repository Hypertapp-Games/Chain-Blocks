using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
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
        CreateList();
        grid = LoadArrayFromFile(path);
        LoadGridVisual();

        CreateGruopBlocks(true);
    }

 
    private bool isDrag = false;
    public  void Update()
    {
        SelectedBlocked();
        // if (Input.GetKeyDown(KeyCode.A))
        // {
        //     int[,] clonedMatrix = CloneMatrix(grid);
        //     FindInEachId(3, ref clonedMatrix);
        // }
        if (Input.GetKeyDown(KeyCode.E))
        {
            ExportBtnClick();
        }
    }

    public int numberGruopBlocks = 0;
    
    void SelectedBlocked()
    {
        // Kiểm tra xem người dùng nhấn chuột trái
        if (Input.GetMouseButtonDown(0))
        {
            // Tạo ra một ray từ vị trí chuột
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;

            // Thực hiện raycast
            if (Physics.Raycast(ray, out hitInfo))
            {
                // Kiểm tra xem đối tượng được chọn có thuộc loại đối tượng mong muốn
                if (hitInfo.collider != null)
                {
                    GameObject selectedObject = hitInfo.collider.gameObject;
                    
                    float x = selectedObject.transform.position.x;
                    float y = selectedObject.transform.position.y;

                    int i = 0;
                    int j = (int)x; //cot
                    
                    for (int k = 29; k >= 0; k--)
                    {
                        if (grid[k, j] != 0)
                        {
                            i = k; // hang
                            break;
                        }
                    }

                    if (!IsChain)
                    {
                        DragBlocks(i , j);
                        IsChain = true;
                    }
                    else
                    {
                        DropBlocks(i, j);
                        IsChain = false;
                    }
                    
                }
            }
        }
    }

    private int _numberDragBlocks = 0;
    private int _columDragId = 0;
    private int _colorDragId = 0;
    void DragBlocks(int i, int j)
    {
        List<int> kk = new List<int>();
        kk.Add(i);
        _columDragId = j;
        _colorDragId = grid[i, j];
        
        for (int k = i -1; k >= 0; k--)
        {
            if (grid[k, j] != grid[i, j])
            {
                break;
            }
            else
            {
                kk.Add(k);
            }
        }

        _numberDragBlocks = kk.Count;
        
        for (int k = 0; k < kk.Count; k++)
        {
            int temp = grid[kk[k], j];
            
            grid[kk[k], j] = 0;
            
            ChangeTileColor(kk[k], j);
            
            grid[29 - k, j] = temp;
            
            ChangeTileColor(29 - k, j);
        }
        
    }
    void DropBlocks(int i, int j)
    {
        //ClearColor in Drag
        for (int k = 0; k < _numberDragBlocks; k++)
        {
            grid[29 - k, _columDragId] = 0;
            ChangeTileColor(29 - k, _columDragId);
        }
        //Drop
        for (int k = i + 1; k <= i + _numberDragBlocks; k++)
        {
            grid[k, j] = _colorDragId;
            ChangeTileColor(k,j);
        }

        ClearDataAfterDrop();
        CreateGruopBlocks(false);
    }

    public void ClearDataAfterDrop()
    {
        _numberDragBlocks = 0;
        _columDragId = 0;
        _colorDragId = 0;
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
    void CreateGruopBlocks(bool firstTime)
    {
        numberGruopBlocks = 0;// reset
        
        CloneNewListToCurrentList();
        
        int[,] clonedMatrix = CloneMatrix(grid);
        
        List<int> testID = new List<int>();
        
        for (int i = 2; i < 10; i++)
        {
            FindInEachId(i, ref clonedMatrix , firstTime, ref testID);
        }
        if (!firstTime && testID.Count > 1)
        {
            PushEmptyPosition(ref testID);
            CreateGruopBlocks(false);
        }
    }

    void FindInEachId(int k, ref int[,] clonedMatrix, bool firstTime ,  ref List<int> testID)
    {
        List<GameObject> test = new List<GameObject>();
        
        for (int i = 0; i < currentRows; i++)
        {
            for (int j = 0; j < currentColumns; j++)
            {
                if (clonedMatrix[i, j] == k)
                {
                    FindGroupLoop(k, ref clonedMatrix, i, j , ref test);
                    for (int l = 0; l < test.Count; l++)
                    {
                        newListOfLists[numberGruopBlocks].Add(test[l]);
                    }
                    
                    if (test.Count > 1 && NoOverlapWithAnyPreviousList(newListOfLists[numberGruopBlocks])&&!firstTime)
                    {
                        for (int l = 0; l < test.Count; l++)
                        {
                            float x = test[l].transform.position.x;
                            float y = test[l].transform.position.y;
                            
                            int gridX = currentRows - (int)y;
                            testID.Add(gridX);
                            
                            int gridY = (int)x;
                            testID.Add(gridY);
                        }
                        foreach (GameObject gameObject in newListOfLists[numberGruopBlocks])
                        {
                            Destroy(gameObject); // Hủy bỏ GameObject
                        }
                        newListOfLists[numberGruopBlocks].Clear();
                    }
                   test.Clear();
                   numberGruopBlocks++;

                }
            }
        }
        
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

    bool checkHaveNonEmptyCell(int currentRow, int column)
    {
        for (int i = currentRow; i < currentRows; i++)
        {
            if (grid[i, column] != -1)
            {
                grid[currentRow, column] = grid[i, column];
                
                gridObject[currentRow, column] = gridObject[i, column];
                //new Vector3(j, currentRows - i, 0)
                gridObject[currentRow, column].transform.position = new Vector3(column, currentRows - currentRow, 0);
                
                grid[i, column] = -1;
                return true;
            }
        }
        return false;
    }
    
    
    void FindGroupLoop(int k, ref int[,] clonedMatrix , int i, int j ,  ref List<GameObject> test )
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
                          // Debug.Log("Adjacent value at position (" + neighborX + ", " + neighborY + ") is: " + clonedMatrix[neighborX, neighborY]);
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
            FindGroupLoop(k, ref clonedMatrix, id[l], id[l + 1], ref test);
        }
        
    }

    public bool NoOverlapWithAnyPreviousList(List<GameObject> listCheck)
    {
        for (int i = 0; i < currentListOfLists.Count; i++)
        {
            if (currentListOfLists[i].Count != 0)
            {
                if (CompareLists(listCheck, currentListOfLists[i]))
                {
                    return false;
                }
            }
        }
        return true;
    }
    bool CompareLists(List<GameObject> list1, List<GameObject> list2)
    {
        // Kiểm tra nếu hai list có cùng số lượng phần tử
        if (list1.Count != list2.Count)
            return false;

        // Kiểm tra từng phần tử trong list1 có tồn tại trong list2 không
        foreach (var item in list1)
        {
            if (!list2.Contains(item))
                return false;
        }

        // Kiểm tra từng phần tử trong list2 có tồn tại trong list1 không (đảm bảo cả hai list chứa cùng các phần tử)
        foreach (var item in list2)
        {
            if (!list1.Contains(item))
                return false;
        }

        // Nếu tất cả các phần tử trong cả hai list giống nhau, trả về true
        return true;
    }

    public void CloneNewListToCurrentList()
    {
        for (int i = 0; i < newListOfLists.Count; i++)
        {
            CloneListGameObjects(ref newListOfLists, ref currentListOfLists , i);
        }
    }
    
    public void CloneListGameObjects(ref List<List<GameObject>> originalGameObjects, ref List<List<GameObject>> clonedGameObjects, int index)
    {
        clonedGameObjects[index].Clear();

        foreach (GameObject originalGO in originalGameObjects[index])
        {
            clonedGameObjects[index].Add(originalGO);
        }
        originalGameObjects[index].Clear();
    }
    
    public List<List<GameObject>> currentListOfLists = new List<List<GameObject>>();
    public List<List<GameObject>> newListOfLists = new List<List<GameObject>>();

    void CreateList()
    {
        for (int i = 0; i < 200; i++)
        {
            List<GameObject> currentSublist = new List<GameObject>();
            List<GameObject> newSublist = new List<GameObject>();

            currentListOfLists.Add(currentSublist);
            newListOfLists.Add(newSublist);
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
    public void ExportBtnClick()
    {
        var time = DateTime.Now.ToString("dd_MM_yyyy (HH:mm:ss)");
        string filePath = "Assets/Data/arrayData" + time + ".txt";

        SaveArrayToFile(filePath, grid);

        Debug.Log("Dữ liệu đã được lưu vào tệp văn bản.");
    }
    void SaveArrayToFile(string filePath, int[,] arrayToSave)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            for (int i = 0; i < arrayToSave.GetLength(0); i++)
            {
                for (int j = 0; j < arrayToSave.GetLength(1); j++)
                {
                    writer.Write(arrayToSave[i, j]);

                    if (j < arrayToSave.GetLength(1) - 1)
                    {
                        writer.Write(",");
                    }
                }

                writer.WriteLine();
            }
        }
    }

}
