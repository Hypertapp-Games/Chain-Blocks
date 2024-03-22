using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;
public class IIDAssetGameManager : MonoBehaviour
{
    private int[,] grid;
    
    private int rows;
    private int columns;
    
    private GameObject[,] gridObject;
    
    public GameObject quad;
    public List<Sprite> Sprites;
    public string path;

    public bool IsChain = false;

    public float timeMove = 0.3f;

    public TMP_InputField speed;
    
    

    private void Start()
    {
        Application.targetFrameRate = 60;
        // timeMove = PlayerPrefs.GetFloat("speed");
        // if (timeMove < 0.2f)
        // {
        //     timeMove = 0.2f;
        //     PlayerPrefs.SetFloat("speed" , timeMove);
        //     
        // }
        // speed.text = timeMove.ToString();
        
        CreateList();
        //grid = LoadArrayFromFile(path);
        grid = new int[30, 7] {
            {0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0},
            {4,0,0,0,0,0,4},
            {4,0,0,0,0,5,5},
            {4,3,3,3,2,2,5},
            {2,2,2,4,4,3,2},
            {3,5,5,5,4,3,2},
            {3,2,4,3,2,5,5},
            {5,2,4,5,2,5,5},
            {5,3,2,4,4,4,2},
            {4,3,2,2,5,3,2},
            {5,5,4,5,3,3,5},
            {2,5,3,3,2,2,2},
            {2,4,2,4,3,5,5},
            {2,4,2,3,4,5,3},
            {3,5,4,5,2,4,3},
            {3,5,3,5,2,5,5},
            {0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0}
        };
        LoadGridVisual();

        CreateGruopBlocks(true);
        //SelectedBlocked();
        _chain.MouseUP += SelectedBlocked;
    }

    public void Replay()
    {
        // PlayerPrefs.SetFloat("speed" , float.Parse(speed.text));
        // Scene currentScene = SceneManager.GetActiveScene();
        // SceneManager.LoadScene(currentScene.name);
    }

 
    private bool isDrag = false;
    public Chain _chain;
    public  void Update()
    {
        //SelectedBlocked();

        if (Input.GetKeyDown(KeyCode.E))
        {
            ExportBtnClick();
        }
    }

    public bool canClick = true;
    void SelectedBlocked()
    {
        Debug.Log("ONclick");
        canClick = false;
        
        // Kiểm tra xem người dùng nhấn chuột trái
        
        int row = 0;
        int col = (int)(_chain.gameObject.transform.position.x + 0.5f);
        Debug.Log("Cot" + col);
               
        for (int k = 0; k <15; k++)
        {
            if (grid[k, col] != 0)
            {
                row = k; // hang
                break;
            }
        }
        if (row != 0)
        {
            if (!IsChain)
            {
                List<int> listIndexRows = LongOfTheChain(row, col);
                //DragBlocks(listIndexRows, col);
                
                var longOfTheChain = row + listIndexRows.Count + 1;
                
                var chainPos = _chain.transform.GetChild(0).transform.position;
                StartCoroutine(0.2f.Tweeng((p) =>  _chain.transform.GetChild(0).transform.position = p, 
                    chainPos
                    , new Vector3(chainPos.x, chainPos.y - longOfTheChain, chainPos.z)));
                StartCoroutine(0.25f.DelayedAction(() =>
                {
                    DragBlocks(listIndexRows , col); // kiểm tra xem khối đang kéo đang thuộc vào khối nào, sau đó chia lại khối đó
                    
                    chainPos =  _chain.transform.GetChild(0).transform.position;
                    StartCoroutine(timeMove.Tweeng((p) =>  _chain.transform.GetChild(0).transform.position = p, 
                        chainPos
                        , new Vector3(chainPos.x, chainPos.y + longOfTheChain, chainPos.z)));
                }));
               
                IsChain = true;
            }
            else
            {
                DropBlocks(row, col);
                IsChain = false;
            }
        }
        else
        {
            if (IsChain)
            {
                {
                    for (int k = 0; k < 19; k++)
                    {
                        if (grid[k, col] != 0)
                        {
                            row = k; // hang
                            break;
                        }
                    }
                    DropBlocks(row, col);
                    IsChain = false;
                }
            }
            else
            {
                canClick = true;
            }
                       
        }
    }

    private int _numberDragBlocks = 0;
    private int _columDragId = 0;
    private int _rowCountPushDown = 0;
    private int _eatBlockInThisDrop = 0;

    List<int> LongOfTheChain(int i, int j)
    {
        List<int> listIndexRows = new List<int>();
        listIndexRows.Add(i);
        _chain.dragBlocksList.Add(gridObject[i,j]);
        _columDragId = j;
        
        for (int k = i + 1; k < 15; k++)
        {
            if (grid[k, j] == grid[i, j])
            {
                listIndexRows.Add(k);
                _chain.dragBlocksList.Add(gridObject[k,j]);
            }
            else
            {
                break;
            }
        }

        _numberDragBlocks = listIndexRows.Count;
        return listIndexRows;
    }
    void DragBlocks(List<int>listIndexRows , int col)
    {
       
        for (int k = 0; k < listIndexRows.Count; k++)
        {
            var targetRow = k;
            var currentRow = listIndexRows[k];
            MoveBlock(col, currentRow, targetRow,true);
            
        }
        StartCoroutine((timeMove + 0.05f).DelayedAction(() =>
        {
            canClick = true;
        }));

    }

    
   void DropBlocks(int i, int j)
    {
        if (j != _columDragId)
        {
            //bool needPushDown = PushDow();
            int curentChainBlocks = 0;
            List<GameObject> dragObject = new List<GameObject>();
            
            bool needPushDown = PushDown(j);
            
            Debug.Log("needPushDown" + needPushDown);
            // i là dòng đầu tiền 
            
            if (!needPushDown)
            {
                for (int k = i - _numberDragBlocks; k < i ; k++)
                {
                    var targetRow = k;
                    var currentRow = curentChainBlocks;
                
                    MoveBlock(j, currentRow, targetRow, ref dragObject,_rowCountPushDown);
                    curentChainBlocks++;
                }
                StartCoroutine((timeMove + 0.05f).DelayedAction(() =>
                {
                    CheckListContainsInList(dragObject); // kiểm tra xem khối đang kéo đang thuộc vào khối nào, sau đó chia lại khối đó
            
                    CreateGruopBlocks(false);
                    
                    StartCoroutine((timeMove *_eatBlockInThisDrop + 0.05f).DelayedAction(() =>
                    {
                        canClick = true;
                    }));
                }));
            }
            else
            {
                //LoadGridVisual();
                 for (int k = i - _numberDragBlocks; k < i ; k++)
                 {
                     var targetRow = k;
                     var currentRow = curentChainBlocks;
                
                     MoveBlock(j, currentRow + _rowCountPushDown, targetRow  + _rowCountPushDown, ref dragObject,_rowCountPushDown);
                     curentChainBlocks++;
                 }
                //CreateGruopBlocks(true);
                //Debug.Log("_rowCountPushDown" + _rowCountPushDown);
                StartCoroutine((timeMove + 0.05f).DelayedAction(() =>
                {
                    for (int k = 0; k < rows; k++)
                    {
                        for (int l = 0; l < columns; l++)
                        {
                            UpdatePositionOfGameobjectInGirdObject(k,l);
                        }
                    }
                   
                    
                       
                    StartCoroutine((timeMove  + 0.05f).DelayedAction(() =>
                    {
                        CheckListContainsInList(dragObject); // kiểm tra xem khối đang kéo đang thuộc vào khối nào, sau đó chia lại khối đó
                
                        CreateGruopBlocks(false);
                
                        if (_eatBlockInThisDrop == 0)
                        {
                            
                        }
                        
                        StartCoroutine((timeMove * _eatBlockInThisDrop + 0.05f).DelayedAction(() =>
                        {
                            canClick = true;
                        }));
                        
                    }));
                    
                }));

            }
            
           
        }
        else
        {
            Debug.Log(12020);
            for (int k = _numberDragBlocks; k < rows; k++)
            {
                if (grid[k, j] != 0)
                {
                    i = k; // hang
                    break;
                }
            }
            int curentChainBlocks = _numberDragBlocks -1;
            Debug.Log(curentChainBlocks);
            
            for (int k =  i - 1; k >= i - _numberDragBlocks ; k--)
            {
                var targetRow = k;
                var currentRow = curentChainBlocks;
                MoveBlock(j, currentRow, targetRow, false);
                curentChainBlocks--;
                //ChangeTileColor(k,j);
            }
            StartCoroutine((timeMove + 0.05f).DelayedAction(() =>
            {
                canClick = true;
            }));
        }
        ClearDataAfterDrop();
    }

   bool PushDown(int columnId)
   {
       int result = FindHighestRow(columnId);
       int highestRowAfterDrop = result - _numberDragBlocks;
       
       Debug.Log("highestRowAfterDrop" +highestRowAfterDrop);
       if (highestRowAfterDrop < 4)
       {
           PushDownTheRowBelow(highestRowAfterDrop);
           return true;
       }

       return false;
   }
   void PushDownTheRowBelow(int highestRowAfterDrop)
   {
       var rowCountPushDown = 4 - highestRowAfterDrop;
       _rowCountPushDown = rowCountPushDown;
       Debug.Log("rowCountPushDown  " +rowCountPushDown);
       for (int i = rows - 1; i >=  rows - rowCountPushDown; i--)
       {
           for (int j = 0; j < columns; j++)
           {
               Destroy(gridObject[i, j].gameObject);
           }
       }
       for (int i = rows - 1; i >= 0 + rowCountPushDown; i--)
       {
           for (int j = 0; j < columns; j++)
           {
               grid[i, j] = grid[i - rowCountPushDown, j];
               
               gridObject[i, j] = gridObject[i - rowCountPushDown, j];
               
           }
       }
       for (int i = 0 + rowCountPushDown - 1; i >= 0 ; i--)
       {
           for (int j = 0; j < columns; j++)
           {
               grid[i, j] = 0;
               
               var tile = Instantiate(quad, new Vector3(j, rows - i, 0), Quaternion.identity);
               gridObject[i, j] = tile;
               tile.transform.SetParent(gameObject.transform);
                    
               ChangeTileColor(i,j);
               
           }
       }
       //CreateGruopBlocks(true);
   }
   int FindHighestRow(int columnId)
   {
       int highestRow = 0;
       for (int i = 0; i < rows; i++)
       {
           if (grid[i, columnId] != 0 && columnId != _columDragId)
           {
               highestRow = i;
               return highestRow;
           }
       }
       return highestRow;
   }

    public void ClearDataAfterDrop()
    {
        _numberDragBlocks = 0;
        _columDragId = 0;
        _rowCountPushDown = 0;
        _eatBlockInThisDrop = 0;
    }
    void MoveBlock(int column,int currentRow, int targetRow, bool drag)
    {
        GameObject objectTemp = gridObject[targetRow, column];
        //dragObject.Add(objectTemp);
        grid[targetRow, column] = grid[currentRow, column];   // thay đổi từ không màu thành có màu 
        gridObject[targetRow, column] = gridObject[currentRow, column];
        if (!drag || _numberDragBlocks < 1)
        {
            UpdatePositionOfGameobjectInGirdObject(targetRow, column);
        }
        else
        {
            int luilai = _numberDragBlocks - 1;
            UpdatePositionOfGameobjectInGirdObject(targetRow , column, luilai, true);
        }
        
            
        grid[currentRow, column] = 0;
        gridObject[currentRow, column] = objectTemp;  // thay đổi từ có màu thành không màu
        UpdatePositionOfGameobjectInGirdObject(currentRow, column);
        
    }
    void MoveBlock(int column,int currentRow, int targetRow, ref List<GameObject> dragObject, int number)
    {
        GameObject objectTemp = gridObject[targetRow, column];
        //dragObject.Add(objectTemp);
            
        grid[targetRow, column] = grid[currentRow, _columDragId];   // thay đổi từ không màu thành có màu (vị trí trên cùng)
        gridObject[targetRow, column] = gridObject[currentRow, _columDragId];
        
        dragObject.Add(gridObject[targetRow, column]);
        
        UpdatePositionOfGameobjectInGirdObject(targetRow, column , number);
            
        grid[currentRow, _columDragId] = 0;
        gridObject[currentRow, _columDragId] = objectTemp;  // vị trí trên ma trận
        UpdatePositionOfGameobjectInGirdObject(currentRow, _columDragId);
        
    }
    int[,] CloneMatrix(int[,] original)
    {
        int[,] clone = new int[rows, columns];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                clone[i, j] = original[i, j];
            }
        }

        return clone;
    }
    public int numberGruopBlocks = 0;
    void CreateGruopBlocks(bool firstTime)
    {
        numberGruopBlocks = 0;// reset
        
        CloneNewListToCurrentList();
        
        int[,] clonedMatrix = CloneMatrix(grid);
        
        List<int> groupID = new List<int>();
        
        for (int i = 2; i < 10; i++)
        {
            FindGroupWithEachID(i, ref clonedMatrix , firstTime, ref groupID);
        }
        if (!firstTime && groupID.Count > 1)
        {
            _eatBlockInThisDrop++;
            PushEmptyPosition(ref groupID);
            
            StartCoroutine((timeMove + 0.05f).DelayedAction(() =>
            {
                CreateGruopBlocks(false);
            }));
        }
        else
        {
            PusUp();
        }
    }

    void PusUp()
    {
        int result = FindHighestRow();
        if (result > 4)
        {
            PushUpTheRowBelow(result);
        }
        
    }

    int FindHighestRow()
    {
        int highestRow = 0;
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                if (grid[i, j] != 0)
                {
                    highestRow = i;
                    return highestRow;
                }
            }
        }
        return highestRow;
    }

    void PushUpTheRowBelow(int highestRow)
    {
        var rowCountPushUp = highestRow - 4;
        for (int i = highestRow; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                grid[i - rowCountPushUp , j] = grid[i, j];
                gridObject[i - rowCountPushUp , j]  = gridObject[i, j];
                UpdatePositionOfGameobjectInGirdObject(i - rowCountPushUp, j);
            }
        }
        for (int i = rows - rowCountPushUp; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                grid[i, j] = 0;
                var tile = Instantiate(quad, new Vector3(j, rows - i, 0), Quaternion.identity);
                gridObject[i, j] = tile;
                tile.transform.SetParent(gameObject.transform);
                    
                ChangeTileColor(i,j);
            }
        }

        if (FindLastRow() < 19)
        {
            for (int i = 19 - rowCountPushUp ; i < 19; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    grid[i, j] = Random.Range(2, 6);
                    Destroy(gridObject[i, j].gameObject);
                    
                    var tile = Instantiate(quad, new Vector3(j, rows - i - rowCountPushUp, 0), Quaternion.identity);
                    gridObject[i, j] = tile;
                    tile.transform.SetParent(gameObject.transform);
                        
                    ChangeTileColor(i,j);
                }
            }
        }

        
        CreateGruopBlocks(true);
    }
    int FindLastRow()
    {
        int lastRow = 0;
        for (int i = rows -1; i >=0; i--)
        {
            for (int j = 0; j < columns; j++)
            {
                if (grid[i, j] != 0)
                {
                    lastRow = i;
                    return lastRow;
                }
            }
        }
        return lastRow;
    }
    
    void FindGroupWithEachID (int id, ref int[,] clonedMatrix, bool firstTime ,  ref List<int> groupID)
    {
        List<GameObject> groupObject = new List<GameObject>();

        int lastRowToCheck = 15; // dòng cuối cùng còn nằm trong màn hình
        
        for (int i = 0; i < lastRowToCheck; i++) // for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                if (clonedMatrix[i, j] == id)
                {
                    FindGroup_Loop(id, ref clonedMatrix, i, j , ref groupObject , lastRowToCheck);
                    
                    for (int l = 0; l < groupObject.Count; l++)
                    {
                        newListOfLists[numberGruopBlocks].Add(groupObject[l]);
                    }
                    
                    if (groupObject.Count > 1 && NoOverlapWithAnyPreviousList(newListOfLists[numberGruopBlocks])&&!firstTime)
                    {
                        for (int l = 0; l < groupObject.Count; l++)
                        {
                            var savePos = groupObject[l].GetComponent<SavePosition>();
                            float x = savePos.pos.x;
                            float y = savePos.pos.y;
                            
                            int gridX = rows - (int)y;
                            groupID.Add(gridX); // i
                            
                            int gridY = (int)x;
                            groupID.Add(gridY); // j
                        }
                        foreach (GameObject gameObject in newListOfLists[numberGruopBlocks])
                        {
                            StartCoroutine(timeMove.Tweeng((s) => gameObject.transform.localScale = s,
                                gameObject.transform.localScale, 
                                new Vector3(0, 0, 0)));
                            StartCoroutine(timeMove.Tweeng((p) => gameObject.transform.position = p,
                                gameObject.transform.position, 
                                gameObject.transform.GetChild(1).localToWorldMatrix.GetPosition()));
                            Destroy(gameObject, timeMove + 0.05f); // Hủy bỏ GameObject
                        }
                        newListOfLists[numberGruopBlocks].Clear();
                    }
                    groupObject.Clear();
                   numberGruopBlocks++;

                }
            }
        }
        
    }

    
   void PushEmptyPosition(ref List<int> testID)
    {
        List<int> numberEmptyEachColumns = new List<int>();
        
        for (int i = 0; i < columns; i++) /// số cột
        {
            int emptyCount = 0;
            
            int maxRow = int.MinValue;
            
            for (int j = 0; j < testID.Count; j+=2) // testID, số chẵn là chỉ số dòng, còn số lẻ là chỉ số cột 
            {
                if (testID[j + 1] == i)
                {
                    emptyCount++;
                    maxRow = Mathf.Max(maxRow, testID[j]);
                }
            }

            if (emptyCount > 0) // nếu trong 1 cột có số ô trống lớn hơn 1 thì sẽ làm gì đó
            {
                numberEmptyEachColumns.Add(i);
                numberEmptyEachColumns.Add(maxRow);
            }
        }
        
        for (int j = 0; j < testID.Count; j += 2)
        {
            grid[testID[j], testID[j + 1]] = -1; // thay id bằng số -1 
        }

        for (int i = 0; i < numberEmptyEachColumns.Count; i += 2) // sét theo từng dòng 
        {
            int column = numberEmptyEachColumns[i];
            int lastRow = numberEmptyEachColumns[i + 1];
            
            PushToEmpty_Loop(column, lastRow);
        }
    }

    void PushToEmpty_Loop(int column, int lastRow)
    {
        for (int i = lastRow; i >= 0; i--)
        {
            if (grid[i, column] == -1)
            {
                if (!checkHaveNonEmptyCell(i, column))
                {
                    grid[i, column] = 0;
                    var tile = Instantiate(quad, new Vector3(column, rows - i, 0), Quaternion.identity);
                    gridObject[i, column] = tile;
                    tile.transform.SetParent(gameObject.transform);
                    
                    ChangeTileColor(i,column);
                }
            }
        }
    }

    bool checkHaveNonEmptyCell(int currentRow, int column)
    {
        for (int i = currentRow; i >= 0; i--)
        {
            if (grid[i, column] != -1)
            {
                grid[currentRow, column] = grid[i, column];
                
                gridObject[currentRow, column] = gridObject[i, column];
                //new Vector3(j, rows - i, 0)
                UpdatePositionOfGameobjectInGirdObject(currentRow, column);
                
                grid[i, column] = -1;
                return true;
            }
        }
        return false;
    }

    public void UpdatePositionOfGameobjectInGirdObject(int i, int j, int luilai, bool drag)
    {
        GameObject obj = gridObject[i, j];
        Transform root = obj.transform.GetChild(1);
        var savePos = obj.GetComponent<SavePosition>();
        
        var change = new Vector3(j, rows - i + luilai, 0) - savePos.pos;
        savePos.pos = new Vector3(j, rows - i + luilai, 0);
        
        StartCoroutine(timeMove.Tweeng((p) => root.position = p, 
            root.position
            , root.position + change));
        
        //gridObject[i, j].transform.position = new Vector3(j, rows - i, 0);
        
        // StartCoroutine(timeMove.Tweeng((p) => gridObject[i, j].transform.position = p, 
        //     gridObject[i, j].transform.position
        //     , new Vector3(j, rows - i + luilai, 0)));
    }
    public void UpdatePositionOfGameobjectInGirdObject(int i, int j)
    {
        GameObject obj = gridObject[i, j];
        Transform root = obj.transform.GetChild(1);
        var savePos = obj.GetComponent<SavePosition>();
        
        var change = new Vector3(j, rows - i , 0) - savePos.pos;
        savePos.pos = new Vector3(j, rows - i , 0);
        
        StartCoroutine(timeMove.Tweeng((p) => root.position = p, 
            root.position
            , root.position + change));
        //gridObject[i, j].transform.position = new Vector3(j, rows - i, 0);
        
        // StartCoroutine(timeMove.Tweeng((p) => gridObject[i, j].transform.position = p, 
        //     gridObject[i, j].transform.position
        //     , new Vector3(j, rows - i, 0)));
    }
    public void UpdatePositionOfGameobjectInGirdObject(int i, int j, int number)
    {
        GameObject obj = gridObject[i, j];
        Transform root = obj.transform.GetChild(1);
        var savePos = obj.GetComponent<SavePosition>();
        
        var change = new Vector3(j, savePos.pos.y, 0) - savePos.pos;
        float changex = j - savePos.pos.x;
        savePos.pos = new Vector3(j, savePos.pos.y, 0);
        
        // StartCoroutine(0.1f.Tweeng((p) => gridObject[i,j].transform.position = p, 
        //     gridObject[i,j].transform.position
        //     , new Vector3(gridObject[i,j].transform.position.x + changex, gridObject[i,j].transform.position.y,0)));
        
        //gridObject[i, j].transform.position = new Vector3(j, rows - i, 0);
            // StartCoroutine(0.1f.Tweeng((p) => gridObject[i, j].transform.position = p, 
            //     gridObject[i, j].transform.position
            //     , new Vector3(j, gridObject[i, j].transform.position.y, 0)));
            
            change = new Vector3(savePos.pos.x, rows - i +number, 0) - savePos.pos;
            savePos.pos =  new Vector3(savePos.pos.x, rows - i +number, 0);
        
            StartCoroutine((0.1f).DelayedAction(() =>
            {
                StartCoroutine((timeMove - 0.1f).Tweeng((p) => root.position = p, 
                    root.position
                    , root.position + change));
                 // StartCoroutine((timeMove - 0.1f).Tweeng((p) => gridObject[i, j].transform.position = p, 
                 //     gridObject[i, j].transform.position
                 //     , new Vector3(gridObject[i, j].transform.position.x, rows - i +number, 0)));
            }));
        
    }

    // public float iScale = 1;
    // Vector3 ScaleVector(int colums, int row)
    // {
    //     return new Vector3(colums, rows - row, 0) * iScale;
    // }
    //
    // int GetRowID(GameObject tile)
    // {
    //     return 0;
    // }

    void FindSmallerGroup(ref List<GameObject> test, ref List<GameObject> origin)
    {
        GameObject firstObject = origin[0];
        test.Add(origin[0]);
        origin.Remove(firstObject);
        
        List<GameObject> keepChecking = new List<GameObject>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                var neighborXPos = firstObject.GetComponent<SavePosition>();
                int neighborX = (int)neighborXPos.pos.x + x;
                int neighborY = (int)neighborXPos.pos.y + y;
                

                // Kiểm tra xem vị trí của hàng xóm có hợp lệ không
                if (x == 0 || y == 0)
                {
                    // Loại trừ trường hợp là phần tử chính nó
                    if (!(x == 0 && y == 0))
                    {
                        for (int i = 0; i < origin.Count; i++)
                        {
                            var originPos = origin[i].GetComponent<SavePosition>();
                            if (originPos.pos.x == neighborX &&
                                originPos.pos.y == neighborY)
                            {
                                keepChecking.Add(origin[i]);
                            }
                        }
                    }
                }
            }
        }

        for (int l = 0; l < keepChecking.Count; l++)
        {
            if (origin.Count > 0)
            {
                FindSmallerGroup(ref test, ref origin);
            }
           
        }
    }

    void FindGroup_Loop(int k, ref int[,] clonedMatrix , int i, int j ,  ref List<GameObject> test , int lastRowToCheck )
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
                if (neighborX >= 0 && neighborX < lastRowToCheck && neighborY >= 0 && neighborY < columns && (x == 0 || y == 0)) // if (neighborX >= 0 && neighborX < rows && neighborY >= 0 && neighborY < columns && (x == 0 || y == 0))
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
            FindGroup_Loop(k, ref clonedMatrix, id[l], id[l + 1], ref test, lastRowToCheck);
        }
        
    }

    public bool NoOverlapWithAnyPreviousList(List<GameObject> listCheck)
    {
        for (int i = 0; i < currentListOfLists.Count; i++)
        {
            if (currentListOfLists[i].Count != 0)
            {
                if (CompareLists(listCheck, currentListOfLists[i])) // điều kiện nếu nó trùng với 1 list đã tồn tại thì sai
                {
                    return false;
                }
                else if (ContainLists(listCheck, currentListOfLists[i])) // nếu không thì kiểm tra nó nằm trong list đã tồn tại 
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
    
    // Khi kéo đi những block thì sẽ chia khối gốc ra thành 2 phần, 1 phần là những block được kéo và phần còn lại.
    public bool CheckListContainsInList(List<GameObject> listCheck)
    {
        for (int i = 0; i < newListOfLists.Count; i++)
        {
            if (newListOfLists[i].Count != 0)
            {
                if (ContainLists(listCheck, newListOfLists[i]))
                {
                   // Debug.Log(newListOfLists[i].Count + "  " +listCheck.Count);
                    
                    List<GameObject> remainingObjects = new List<GameObject>();
                    
                    foreach (GameObject obj1 in newListOfLists[i])
                    {
                        if (!listCheck.Contains(obj1))
                        {
                            remainingObjects.Add(obj1);
                        }
                    }
                    newListOfLists[i].Clear(); // clear list cũ
                    newListOfLists[i].AddRange(listCheck); // thay luôn list cũ vào list mới để không ảnh hướng đến thứ tự phía sau
                    for (int j = 0; j < newListOfLists.Count; j++)
                    {
                        if (newListOfLists[j].Count == 0)
                        {
                            FindSmallestGroup(ref remainingObjects, j);
                            //newListOfLists[j+1].AddRange(remainingObjects);
                            break;
                        }
                        
                    }
                    return true;
                }
            }
        }
        return false;
    }
    // Chia phần còn lại thành những khối liền kề nhau
    void FindSmallestGroup( ref List<GameObject> remainingObjects, int continuevalue)
    {
        Debug.Log("remind " + remainingObjects.Count);
        
        List<GameObject> test = new List<GameObject>();
        if (remainingObjects.Count > 0)
        {
           FindSmallerGroup(ref test, ref remainingObjects);
            
           newListOfLists[continuevalue].AddRange(test);
           //Debug.Log("new " + continuevalue + " new x"+ test[i].transform.position.x + " new y"+test[i].transform.position.y);
           //Debug.Log("new " + continuevalue + "new " + newListOfLists[continuevalue].Count);
           
           if (remainingObjects.Count > 0)
           {
               FindSmallestGroup(ref remainingObjects, continuevalue+1);
           }
        }
    }
    bool ContainLists(List<GameObject> list1, List<GameObject> list2)
    {

        if (list1.Count >= list2.Count)
            return false;
        
        foreach (var item in list1)
        {
            if (!list2.Contains(item))
                return false;
        }
        return true;
    }
    // Sau khi sử dụng xong sẽ xoá new list và lưu dữ liệu vào current list
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

    // Tạo danh sách trống để lưu những list chứa group block để so sánh chúng với nhau
    void CreateList()
    {
        for (int i = 0; i < 100; i++)
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
            rows = grid.GetLength(0);
            columns = grid.GetLength(1);

            gridObject = new GameObject[rows, columns];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    var tile = Instantiate(quad, new Vector3(j, rows - i, 0), Quaternion.identity);
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
        var rend = tile.GetComponent<SpriteRenderer>();
        
        if (grid[i, j] == 0 || grid[i, j] == 1)
        {
            rend.enabled = false;
        }
        else
        {
            var sprite = Sprites[grid[i,j] - 2];
            rend.sprite = sprite;
        }


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
