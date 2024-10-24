using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.IO;
using UnityEngine.InputSystem;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Hands;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using UnityEngine.XR.OpenXR.Input;
using JioXSDK.Utils;
namespace JioXSDK.Interactions
{
    public class UIHandler : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _instructionOne;
        [SerializeField]
        private TMP_Text _timerinstruction;
        [SerializeField]
        private List<TMP_Text> _elapseTimeTracker = new List<TMP_Text>();
        [SerializeField]
        private AudioSource m_MyAudioSource;
        [SerializeField]
        private Image notification;
        [SerializeField]
        private TMP_Text n_NoficationHandDetectionText;

        private List<String> instuctions = new List<String>();

        private float _timer = 5f;

        private bool _timerController;

        private float elapsedTime = 0f;

        private bool isRunning = false;

        private int _elapsedtimetrackercount = 0;

        private float soundLength = 0f;
        private string dataRecord;

        public float fillSpeed = 1f; // Speed at which the radial fill progresses
        public float targetFillAmount = 1f; // Target fill amount (0 to 1)

        [SerializeField]
        private GameObject popUp;

        [SerializeField]
        public TMP_Text averagetimetext;

        private float totalElapsedTime = 0f;

        [SerializeField]
        private GameObject exit_PopUp;
        [SerializeField]
        private TMP_Text avgTime;
        [SerializeField]
        private TMP_Text handsValues;

        bool rightHandTracked, leftHandTracked = false;


        public GameObject retrypopUp;
        //  InputDevice device;
        private void OnDestroy()
        {
            HandInteractionEvents.RightHandTrackingAcquired -= RightHandTracked;
            HandInteractionEvents.LeftHandTrackingAcquired -= LeftHandTracked;
            HandInteractionEvents.RightHandTrackingLost -= RightHandLost;
            HandInteractionEvents.LeftHandTrackingLost -= LeftHandLost;
        }

        // Start is called before the first frame update
        void Start()
        {
            CSVHandler.SetHeaderData("Count", "Average Time");
            m_MyAudioSource = FindObjectOfType<AudioSource>();
            instuctions.Add("Hii Player, please Relax your Hand position and be ready to Take Action when Timer Go's To 0");
            instuctions.Add("Please Ready for the Latency Test Move Your Hands InFront of Your Headset to Record the Value after listening the beep sound");
            _instructionOne.text = instuctions[0];
            ExitPopUpDisable();
            DisablePopUp();
            DisableRetryPopUp();
            HandInteractionEvents.RightHandTrackingAcquired += RightHandTracked;
            HandInteractionEvents.LeftHandTrackingAcquired += LeftHandTracked;
            HandInteractionEvents.RightHandTrackingLost += RightHandLost;
            HandInteractionEvents.LeftHandTrackingLost += LeftHandLost;
            StartCoroutine(ShowInstuction(10));
        }

        void RightHandTracked()
        {
            rightHandTracked = true;
        }
        void LeftHandTracked()
        {
            leftHandTracked = true;
        }

        void RightHandLost()
        {
            rightHandTracked = false;
        }
        void LeftHandLost()
        {
            leftHandTracked = false;
        }

        IEnumerator ShowInstuction(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            _instructionOne.text = instuctions[1];
            StartCoroutine(SetTimer());
        }
        IEnumerator SetTimer()
        {
            yield return new WaitForSecondsRealtime(1);
            _timerinstruction.text = _timer.ToString();
            if (_timer == 0)
            {
                BeepSoundPlay();
                yield return new WaitForSecondsRealtime(soundLength);
                BeepSoundStop();
                _instructionOne.text = "";
                _timerinstruction.text = "";
                StartTimer();
                StopCoroutine(SetTimer());
            }
            else
            {
                StartCoroutine(SetTimer());
            }
            _timer--;
        }



        IEnumerator HandLatencyDetection()
        {
            while (!rightHandTracked || !leftHandTracked)
            {
                elapsedTime += Time.deltaTime;
                _instructionOne.text = "Time: " + FormatTime(elapsedTime);
                yield return null;
            }
            if (_elapsedtimetrackercount <= 9)
            {
                if (elapsedTime == 0.0f)
                {
                    //enable the pop up
                    EnableRetryPopUp();
                    yield return null;
                }
                else
                {
                    RecordTime();
                    PopUpEnable();
                    yield return new WaitForSecondsRealtime(2f);
                    PopUpDisable();
                }
            }
            else
            {
                ExitPopUpEnable();
                SaveData();
                yield return null;
            }

        }
        public void RetryButtonClick()
        {
            DisableRetryPopUp();
            ResetTimer();
        }

        private void EnableRetryPopUp()
        {
            retrypopUp.SetActive(true);
        }
        private void DisableRetryPopUp()
        {
            retrypopUp.SetActive(false);
        }

        private void SaveData()
        {
            string filePath = getPath();
            File.AppendAllText(filePath, dataRecord);
            StopAllCoroutines();
        }

        private string FormatTime(float time)
        {
            int minutes = Mathf.FloorToInt(time / 60);
            int seconds = Mathf.FloorToInt(time % 60);
            int milliseconds = Mathf.FloorToInt((time * 1000) % 1000);

            return string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
        }

        private void BeepSoundPlay()
        {
            m_MyAudioSource.Play();
            soundLength = m_MyAudioSource.clip.length;
            Debug.Log(soundLength);
        }

        private void BeepSoundStop()
        {
            m_MyAudioSource.Stop();
        }
        private void RecordTime()
        {
            _elapseTimeTracker[_elapsedtimetrackercount].text = FormatTime(elapsedTime);
            n_NoficationHandDetectionText.text = "Hand Detection Time " + FormatTime(elapsedTime);
            DataRecord(_elapseTimeTracker[_elapsedtimetrackercount].text);
            _elapsedtimetrackercount++;
            totalElapsedTime += elapsedTime;
            EnablePopUp();
            //  ResetTimer();
        }

        private void DataRecord(string str)
        {
            dataRecord += str + "/n";
        }

        private void StartTimer()
        {
            isRunning = true;
            StartCoroutine(HandLatencyDetection());
        }

        private void StopTimer()
        {
            isRunning = false;
        }

        private void ResetTimer()
        {
            StopTimer();
            elapsedTime = 0f;
            _timer = 5;
            _instructionOne.text = "please Rest the hand Position. " + "The next test starts in 5 sec ";
            StartCoroutine(SetTimer());
        }

        private void PopUpEnable()
        {
            notification.gameObject.SetActive(true);
        }
        private void PopUpDisable()
        {
            notification.gameObject.SetActive(false);
        }
        private string getPath()
        {
            return Application.persistentDataPath + "/data.txt";
        }
        private void EnablePopUp()
        {
            popUp.SetActive(true);
            averagetimetext.text = "Average Time " + FormatAverageTime();
        }
        private void DisablePopUp()
        {
            popUp.SetActive(false);
        }
        public void PopUpsubmitButton()
        {
            //SaveData();
            Application.Quit();
        }
        public void PopUpNextButton()
        {
            DisablePopUp();
            ResetTimer();
        }
        private string FormatAverageTime()
        {
            float averageTime = totalElapsedTime / _elapsedtimetrackercount;
            int minutes = Mathf.FloorToInt(averageTime / 60);
            int seconds = Mathf.FloorToInt(averageTime % 60);
            int milliseconds = Mathf.FloorToInt((averageTime * 1000) % 1000);
            return string.Format("Average Time: {0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
        }

        public void Restartapplication()
        {
            SceneManager.LoadScene("HandLatencyScene");
        }
        public void QuitApplication()
        {
            //  SaveData();
            CSVHandler.SaveCSV("LatencyAvgTimer");
            Application.Quit();
        }

        public void ExitPopUpEnable()
        {
            avgTime.text = "Average Time " + FormatAverageTime();
            CSVHandler.AddLineData(_elapsedtimetrackercount, FormatAverageTime());
            exit_PopUp.SetActive(true);
        }

        public void ExitPopUpDisable()
        {
            exit_PopUp.SetActive(false);
        }

    }
}

