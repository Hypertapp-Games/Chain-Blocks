using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chain : MonoBehaviour
{
    private Vector3 mOffset;
    private float mZCoord;
    private bool mDragging = false;
    private bool mMoving = false;
    public event Action MouseUP;
    
    public List<GameObject> dragBlocksList = new List<GameObject>();
    public IIDAssetGameManager gameManager;
    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mZCoord;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    private void Update()
    {
        if (gameManager.canClick)
        {
            if (Input.GetMouseButtonDown(0))
            {
                HandleMouseDown();
            }

            HandleMouseDrag();

            if (Input.GetMouseButtonUp(0))
            {
                HandleMouseUp();
            }
        }
        // if (gameManager.canClick)
        // {
        //     if (Input.GetMouseButtonDown(0))
        // {
        //         mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        //         for (int i = 0; i < dragBlocksList.Count; i++)
        //         {
        //             dragBlockListmOffset.Add(dragBlocksList[i].transform.position - GetMouseWorldPos());
        //         }
        //     
        //         mDragging = true;
        //         int col = (int)(GetMouseWorldPos().x + 0.5f);
        //         float newX = Mathf.Clamp(col, 0f, 6f);
        //         StartCoroutine(0.2f.Tweeng((p) => gameObject.transform.position = p, 
        //             gameObject.transform.position
        //             , new Vector3(newX, gameObject.transform.position.y,gameObject.transform.position.z)));
        //         
        //         var offset = col - transform.position.x;
        //         
        //         for (int i = 0; i < dragBlocksList.Count; i++)
        //         {
        //             GameObject dragObject = dragBlocksList[i];
        //             StartCoroutine(0.2f.Tweeng((d) => dragObject.transform.position = d,
        //                 dragObject.transform.position,
        //                 dragObject.transform.position + new Vector3(offset, 0, 0)));
        //         }
        //         
        //         StartCoroutine(0.21f.DelayedAction(() =>
        //         {
        //             mMoving = true;
        //         }));
        //         
        //    
        // }
        // if (mDragging)
        // {
        //     for (int i = 0; i < dragBlocksList.Count; i++)
        //     {
        //         Vector3 newBlockPos = GetMouseWorldPos() + dragBlockListmOffset[i];
        //         float newBlockX = Mathf.Clamp(newBlockPos.x, 0f, 6f); // Giới hạn giá trị của newX trong khoảng từ 0 đến 6
        //         dragBlocksList[i].transform.position = new Vector3(newBlockX, dragBlocksList[i].transform.position.y, dragBlocksList[i].transform.position.z);
        //     }
        //     
        // }
        //
        // if (mMoving)
        // {
        //      Vector3 newPos = GetMouseWorldPos();
        //      float newX = Mathf.Clamp(newPos.x, 0f, 6f); // Giới hạn giá trị của newX trong khoảng từ 0 đến 6
        //      transform.position = new Vector3(newX, transform.position.y, transform.position.z);
        // }
        //
        // if (Input.GetMouseButtonUp(0))
        // {
        //         if (mDragging)
        //         {
        //             
        //             mDragging = false;
        //             float time = 0.1f;
        //             if (!mMoving)
        //             {
        //                 int col = (int)(gameObject.transform.position.x + 0.5f);
        //                 var offset = col - transform.position.x;
        //                 StartCoroutine(time.Tweeng((p) => gameObject.transform.position = p, 
        //                     gameObject.transform.position
        //                     , new Vector3(col, gameObject.transform.position.y,gameObject.transform.position.z)));
        //         
        //                 for (int i = 0; i < dragBlocksList.Count; i++)
        //                 {
        //                     GameObject dragObject = dragBlocksList[i];
        //                     StartCoroutine(time.Tweeng((d) => dragObject.transform.position = d,
        //                         dragObject.transform.position,
        //                         dragObject.transform.position + new Vector3(offset, 0, 0)));
        //                 }
        //         
        //                 StartCoroutine(time.DelayedAction(() =>
        //                 {
        //                     dragBlocksList.Clear();
        //                     MouseUP?.Invoke();
        //                 }));
        //             }
        //             else
        //             {
        //                 StartCoroutine(0.2f.DelayedAction(() =>
        //                 {
        //                     dragBlocksList.Clear();
        //                     MouseUP?.Invoke();
        //                 }));
        //             }
        //             mMoving = false;
        //             
        //     }
        //    
        // }
        // }
        //
        // if (Input.GetMouseButtonUp(0))
        // {
        //     mDragging = false;
        // }
        
    }

    private bool moveFree = true;
    private void HandleMouseDown()
    {
        moveFree = false;
        
        mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;

        MoveChainWithBlock(0.1f);
        
        StartCoroutine(0.105f.DelayedAction(() =>
            {
                if (!moveFree) // chưa thả chuột
                {
                    mDragging = true;
                    
                }
                else // đã thả chuột 
                {
                    HandleMouseUpBeforeDelayedAction();
                }
           
            }));
     
    }

    void HandleMouseDrag()
    {
        if (mDragging)
        {
            Vector3 newPos = GetMouseWorldPos();
            float newX = Mathf.Clamp(newPos.x, 0f, 6f); // Giới hạn giá trị của newX trong khoảng từ 0 đến 6
            transform.position = new Vector3(newX, transform.position.y, transform.position.z);
            
            for (int i = 0; i < dragBlocksList.Count; i++)
            {
                dragBlocksList[i].transform.position = new Vector3(newX, dragBlocksList[i].transform.position.y, dragBlocksList[i].transform.position.z);
            }
        }
    }
    void HandleMouseUp()
    {
        if (!moveFree) // đã nhận sự kiện click xuống 
        {
            moveFree = true;
            if (mDragging) // đã hết thời gian và chain đã đến vị trí
            {
                
                mDragging = false;
                
                MoveChainWithBlock(0.05f);
                
                StartCoroutine(0.055f.DelayedAction(() =>
                {
                    dragBlocksList.Clear();
                    MouseUP?.Invoke();
                    UpdateSavePosition();
                
                }));
            }
        }
        
    }

    void HandleMouseUpBeforeDelayedAction() // nếu trước khi chain đến vị trí click mà đã thả chuột thì sau khi chain đến vị trí sẽ thực hiện kéo 
    {
        UpdateSavePosition();
        dragBlocksList.Clear();
        MouseUP?.Invoke();
    }

    void UpdateSavePosition()
    {
        for (int i = 0; i < dragBlocksList.Count; i++)
        {
            var newX = (int)(dragBlocksList[i].transform.position.x+ 0.5f);
            var savePos = dragBlocksList[i].GetComponent<SavePosition>();
            savePos.pos = new Vector3(newX, savePos.pos.y, savePos.pos.z);
        }
    }

    void MoveChainWithBlock(float time)
    {
        int col = (int)(GetMouseWorldPos().x + 0.5f);
        float newX = Mathf.Clamp(col, 0f, 6f);
        StartCoroutine(time.Tweeng((p) => gameObject.transform.position = p, 
            gameObject.transform.position
            , new Vector3(newX, gameObject.transform.position.y,gameObject.transform.position.z)));
        
        
        //cả khối sẽ đi chuyển cùng với chain
        if (dragBlocksList.Count > 0)
        {
            for (int i = 0; i < dragBlocksList.Count; i++)
            {
                GameObject dragObject = dragBlocksList[i];
                StartCoroutine(time.Tweeng((d) => dragObject.transform.position = d,
                    dragObject.transform.position,
                    new Vector3(newX, dragObject.transform.position.y, dragObject.transform.position.z)));
            }  
        }
    }

}
