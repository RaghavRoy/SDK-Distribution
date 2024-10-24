using System.Collections.Generic;
using UnityEngine;

namespace JioXSDK.FOVSample
{
    public class DataReportParser : MonoBehaviour
    {
        [SerializeField] private TextAsset _dataSheet;
        [SerializeField] private Transform _camParent;
        [SerializeField] private ReportData[] _reportData;
        [SerializeField] private Transform _point;
        [SerializeField] private Transform _extPoint;

        [Space]
        [SerializeField] private float extension = 2f;
        [SerializeField] private float pointScale = 1f;

        private void Update()
        {
            _camParent.Rotate(Vector3.up, 30f * Time.deltaTime);
        }

        public void ProcessAndDisplayData(ReportData[] reportData)
        {
            ProcessReport(reportData);
            ShowAllPoints();
        }

        [ContextMenu("Calculate FOV")]
        private void CalculateFOV()
        {
            _reportData = GetDataFromJsonFile();
            ProcessAndDisplayData(_reportData);
        }

        private void ProcessReport(ReportData[] reportData)
        {
            List<ReportData> data = new();

            foreach (ReportData reportDataItem in reportData)
            {
                if (reportDataItem.HandPosition.ToVector3() != Vector3.zero)
                    data.Add(reportDataItem);
            }

            _reportData = data.ToArray();
        }

        private ReportData[] GetDataFromJsonFile()
        {
            ReportJson data = JsonUtility.FromJson<ReportJson>(_dataSheet.text);
            return data.dataArray;
        }

        private void ShowAllPoints()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
                DestroyImmediate(transform.GetChild(i).gameObject);

            foreach (var report in _reportData)
            {
                Vector3 pos = report.HandPosition.ToVector3();
                Vector3 extPos = pos * extension;
                Vector3[] positions = new Vector3[] { Vector3.zero, pos, extPos };

                GameObject go = SpawnSphere(pointScale, pos, _point);
                LineRenderer lr = go.GetComponent<LineRenderer>();

                lr.positionCount = positions.Length;
                lr.SetPositions(positions);

                SpawnSphere(pointScale * extension * 0.65f, extPos, _extPoint);
            }

            _camParent.gameObject.SetActive(true);
        }

        private GameObject SpawnSphere(float scale, Vector3 position, Transform prefab)
        {
            Transform sphere = Instantiate(prefab);
            sphere.position = position;
            sphere.localScale = Vector3.one * scale;
            sphere.parent = transform;

            Debug.Log($"{prefab.name} Spawned");

            return sphere.gameObject;
        }
    }
}
