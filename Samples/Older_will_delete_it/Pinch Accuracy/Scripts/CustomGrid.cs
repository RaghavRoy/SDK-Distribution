using System.Collections;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using JioXSDK.Interactions;
using QCHT.Interactions.Core;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Hands.OpenXR;

public class CustomGrid : MonoBehaviour
{
   [SerializeField] GameObject cubePrefab;
    [SerializeField] TextMeshPro positionText;
    [SerializeField] TextMeshPro positionText2;
    private List<List<GridElement>> cubes = new List<List<GridElement>>();
   private int xSize = 20;
   private int ySize = 10;

   private int width = 15;
   private int height = 10;

   private float cellWidth = 0;
   private float cellHeight = 0;

   private int rowRight = -1;
   private int colRight = -1;

   private int rowLeft = -1;
   private int colLeft = -1;

    private void Start()
    {
        HandInteractionEventsBase.OnRightHandPinch += OnRightSelect;
        HandInteractionEventsBase.OnLeftHandPinch += OnLeftSelect;
        //SpawnGrid(10, 10);
    }

    private IEnumerator MoveForward()
    {
        yield return new WaitForSeconds(1f);
        transform.position = new Vector3(0, 0, 20);
    }

   public void SpawnGrid(int _row, int _col)
   {
        StartCoroutine(MoveForward());
        xSize = _row;
        ySize = _col;
        float xDistance = width / xSize;
        float yDistance = height / ySize;
        float startingX = 1.2f - width/2;
        float startingY = 1 - height/2;
        for(int i = 0; i < xSize; i++)
        {
            cubes.Add(new List<GridElement>());
            for(int j = 0; j < ySize; j++)
            {
                GridElement cube = Instantiate(cubePrefab, transform).GetComponent<GridElement>();
                cube.transform.position = new Vector3(startingX + xDistance * i, startingY + yDistance * j, 0);
                
                cubes[i].Add(cube);
            }
        }

        cellWidth = 1.2f / (float)xSize;
        cellHeight = 1f / (float)ySize;
        Debug.Log("cellWidth " + cellWidth);
        Debug.Log("cellHeight " + cellHeight);
        
   }

   private void OnDestroy()
   {
        HandInteractionEventsBase.OnRightHandPinch -= OnRightSelect;
        HandInteractionEventsBase.OnLeftHandPinch -= OnLeftSelect;
   }

   private void Update()
   {
         rowRight = (int)((HandInteractionEventsBase.RightHandPosition.x + 0.5f)/cellWidth);
         colRight = (int)((HandInteractionEventsBase.RightHandPosition.y - 0.5f)/cellHeight);
         rowLeft = (int)((HandInteractionEventsBase.LeftHandPosition.x + 0.5f)/cellWidth);
         colLeft = (int)((HandInteractionEventsBase.LeftHandPosition.y - 0.5f)/cellHeight);
         positionText.text = HandInteractionEventsBase.RightHandPosition.ToString();
         positionText2.text = HandInteractionEventsBase.LeftHandPosition.ToString();
        OnHover();
        //OnHoverExit();
        // Debug.Log("HandInteractionEvents" + HandInteractionEvents.LeftHandPosition + 0.5f);
        // Debug.Log("HandInteractionEvents Row " + row);
        //  Debug.Log("HandInteractionEvents Col " + col);

        //positionText.text = (normalized).ToString();
        //5 650 5 240
        // row = (int)((Input.mousePosition.x - 5f)/cellWidth);
        // col = (int)((Input.mousePosition.y - 5f)/cellHeight);
        // positionText.text = (row).ToString();
        // positionText2.text = (col).ToString();
   }

    public void OnRightSelect()
    {
        cubes[rowRight][colRight].OnSelect();
    }

    public void OnLeftSelect()
    {
        cubes[rowLeft][colLeft].OnSelect();
    }

    public void OnHover()
    {
        if(cubes.Count > 0)
        {
            for(int i = 0; i < cubes.Count; i++)
            {
                for(int j = 0; j < cubes[i].Count; j++)
                {
                    if((rowRight == i && colRight == j) || (rowLeft == i && colLeft == j))
                    {
                        cubes[i][j].OnHover();
                    }
                    else
                    {
                        cubes[i][j].OnHoverExit();
                    }
                }
            }
        }
    }

    public void OnReset()
    {
        for(int i = 0; i < cubes.Count; i++)
        {
            for(int j = 0; j < cubes[i].Count; j++)
            {
                cubes[i][j].Reset();
                Destroy(cubes[i][j].gameObject);
            }
        }
        cubes.Clear();
        transform.position = Vector3.zero;
    }

   
}
