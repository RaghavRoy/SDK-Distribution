using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace JioXSDK
{
    public class MainController : MonoBehaviour
    {
        [SerializeField] private GameObject gridPrefab;
        [SerializeField] private GameObject restartButton;
        [SerializeField] private Camera snapShotCam;

        [SerializeField]private CustomGrid customGrid;
        [SerializeField] private GameObject menu;
        
        private void Awake() {
            //GameObject temp = Instantiate(gridPrefab, Camera.main.transform);
            //customGrid = temp.GetComponent<CustomGrid>();
        }
        private void ShowGrid()
        {
            menu.SetActive(false);
            customGrid.gameObject.SetActive(true);
        }

        public void ShowMenu()
        {
            menu.SetActive(true);
            customGrid.OnReset();
            customGrid.gameObject.SetActive(false);
        }

        public void FiveXFive()
        {
            ShowGrid();
            customGrid.SpawnGrid(5, 5);
        }

        public void TenXTen()
        {
            ShowGrid();
            customGrid.SpawnGrid(10, 10);
        }

        public void TwentyXTen()
        {
            ShowGrid();
            customGrid.SpawnGrid(15, 10);
        }

        public void CamCapture()
        {
            //cam = Camera.main;
            snapShotCam.transform.position = Camera.main.transform.position;
            snapShotCam.transform.rotation = Camera.main.transform.rotation;
    
            RenderTexture currentRT = RenderTexture.active;
            RenderTexture.active = snapShotCam.targetTexture;
    
            snapShotCam.Render();

            
            Texture2D Image = new Texture2D(snapShotCam.targetTexture.width, snapShotCam.targetTexture.height);
            Image.ReadPixels(new Rect(0, 0, snapShotCam.targetTexture.width, snapShotCam.targetTexture.height), 0, 0);
            Image.Apply();
            RenderTexture.active = currentRT;
    
            var Bytes = Image.EncodeToPNG();
            Destroy(Image);
            //string fileName = /PinchAccuracy.png;
            File.WriteAllBytes(Application.persistentDataPath + "/PinchAccuracy" + System.DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".png", Bytes);
            
        }
    }
}
