using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JioXSDK.Interactions;
using UnityEngine;
using UnityEngine.XR.Hands;

namespace JioXSDK.FOVSample
{
    public class HandTracker : MonoBehaviour
    {

        private const Handedness rightHand = Handedness.Right;
        private const Handedness leftHand = Handedness.Left;

        [SerializeField] private bool isProcessing;
        [SerializeField] private bool isDebug;

        [Space]
        [SerializeField] private DataReportParser _reportParser;
        [SerializeField] private StateManager _stateManager;
        [SerializeField] private Transform _handPointer;
        [SerializeField] private Transform _camTransform;
        [SerializeField] private Popup popupMessage;


        [Space]
        [SerializeField] private List<ReportData> _reportDataList;

        private Vector3 rightHandPosition, leftHandPosition;

        private void Awake()
        {
            _camTransform = Camera.main.transform;
            _stateManager.OnProcessStart += StartProcess;
            // _stateManager.OnProcessStop += StopProcess;
            _reportDataList = new List<ReportData>();

            RightHandSetup();
            LeftHandSetup();
        }


        private void Update()
        {
            if (!isProcessing)
                return;

            rightHandPosition = HandInteractionEvents.RightHandPosition;
            leftHandPosition = HandInteractionEvents.LeftHandPosition;
        }


        private void StartProcess()
        {
            isProcessing = true;
            _stateManager.StartProcessStopTimer(15, "Process ends in: ", StopProcess);
        }

        private void StopProcess()
        {
            isProcessing = false;
            string csvData = GetCSV(_reportDataList);
            ReportData[] reportArray = _reportDataList.ToArray();
            string fileName = $"fovData{DateTime.Now.ToString().Replace('/', '-').Replace(' ', '_')}.csv";
            string jsonFile = $"fovData{DateTime.Now.ToString().Replace('/', '-').Replace(' ', '_')}.json";

            string path = Path.Combine(Application.persistentDataPath, fileName);
            string jsonPath = Path.Combine(Application.persistentDataPath, jsonFile);

            try
            {
                File.WriteAllText(path, csvData);
                File.WriteAllText(jsonPath, new ReportJson { dataArray = reportArray }.GetJsonString());
                Debug.Log($"The file is saved at: {path}, and {jsonPath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"{e.Message}");
            }

            popupMessage.Open("Do you want to restart the test?", () =>
            {
                _stateManager.StartProcessStopTimer(10, "Process will start in: ", StartProcess);
            }, () => Application.Quit());

            _reportParser.ProcessAndDisplayData(_reportDataList.ToArray());
        }

        private void LeftHandSetup()
        {
            HandInteractionEvents.LeftHandTrackingAcquired += OnLeftHandTrackingAcquired;
            HandInteractionEvents.LeftHandTrackingLost += OnLeftHandTrackingLost;
        }

        private void RightHandSetup()
        {
            HandInteractionEvents.RightHandTrackingAcquired += OnRightHandTrackingAcquired;
            HandInteractionEvents.RightHandTrackingLost += OnRightHandTrackingLost;
        }


        private void OnRightHandTrackingAcquired()
        {
            Vector3 positions = rightHandPosition;
            AddReportPoint(positions, rightHand, HandTrackingState.Acquired);
        }

        private void OnRightHandTrackingLost()
        {
            Vector3 positions = rightHandPosition;
            AddReportPoint(positions, rightHand, HandTrackingState.Lost);
        }

        private void OnLeftHandTrackingAcquired()
        {
            Vector3 positions = leftHandPosition;
            AddReportPoint(positions, leftHand, HandTrackingState.Acquired);
        }

        private void OnLeftHandTrackingLost()
        {
            Vector3 positions = leftHandPosition;
            AddReportPoint(positions, leftHand, HandTrackingState.Lost);
        }

        private void AddReportPoint(Vector3 positions, Handedness hand, HandTrackingState trackingState)
        {
            _handPointer.LookAt(positions);
            ReportData data = new()
            {
                Hand = hand,
                TrackingState = trackingState,
                HandPosition = VS3.FromVector3(positions - _camTransform.position),
                EulerAnglesToHandAbs = VS3.FromVector3(_handPointer.eulerAngles),
                EulerAnglesToHandRelative = VS3.FromVector3(_handPointer.localEulerAngles),
            };
            _reportDataList.Add(data);
        }

        private string GetCSV(List<ReportData> data)
        {
            StringBuilder builder = new();

            builder.AppendLine("sep=;");
            builder.AppendLine(ReportData.ToCSVHeader());
            foreach (ReportData item in data)
                builder.AppendLine(item.ToCSVString());

            return builder.ToString();
        }
        internal Transform SetHandPointer
        {
            set
            {
                _handPointer = value;
            }
        }
    }
}
