using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System.IO;
using System;
using UnityEngine.InputSystem;
namespace ViveSR
{
    namespace anipal
    {
        namespace Eye
        {

            public class EyeCSV : MonoBehaviour
            {
                public static EyeCSV instance;
                public static int timestamp = 0;
                public static bool isWriting = false;

                public int LengthOfRay = 25;
                //[SerializeField] private LineRenderer GazeRayRenderer;
                private static EyeData_v2 eyeData = new EyeData_v2();
                private bool eye_callback_registered = false;

                //csv
                //public string filename = timestamp.ToString();
                public string filename = "orii_s";
                public static StreamWriter sw;
                public Vector3 GazeOriginCombinedLocalC, GazeDirectionCombinedLocalC;

                public Vector2 lefteye;
                private static VerboseData verboseData;
                private float pupilDiameterLeft, pupilDiameterRight, pupilDiameterCombined;
                private Vector2 pupilPositionLeft, pupilPositionRight, pupilPositionCombined;
                private float eyeOpenLeft, eyeOpenRight, eyeOpenCombined;
                public static Vector3 gaze_direction_right, gaze_direction_left, gaze_direction_combined;
                private Vector3 gaze_origin_right, gaze_origin_left, gaze_origin_combined;
                private float convergence_distance;


                public Ray ray;
                public static FocusInfo focusInfo;
                public float radius = 5.0f;
                public float maxradius = 5.0f;
                private float LeftOpenness, RightOpenness;

                public Vector3 origin;
                public Vector3 direction;
                private bool isKeyDown = false;

                // Use this for initialization
                void Start()
                {

                    //filename = eyeData.timestamp.ToString();
                    filename = "orii_s";
                    Debug.Log(filename);
                    if (!SRanipal_Eye_Framework.Instance.EnableEye)
                    {
                        Debug.Log("test2");

                        enabled = false;
                        return;
                    }
                    focusInfo = new FocusInfo();



                    sw = new StreamWriter(@"" + filename + ".csv", false);
                    isWriting = true;
                    string[] s1 = { "gaze_direction_left_x", "gaze_direction_left_y", "gaze_direction_left_z", "gaze_direction_right_x", "gaze_direction_right_y", "gaze_direction_right_z", "gaze_direction_combined_x", "gaze_direction_combined_y", "gaze_direction_combined_z", "gaze_origin_left_x", "gaze_origin_left_y", "gaze_origin_left_z", "gaze_origin_right_x", "gaze_origin_right_y", "gaze_origin_right_z", "pupil_diameter_left", "pupil_diameter_right", "pupil_position_left_x", "pupil_position_left_y", "pupil_position_right_x", "pupil_position_right_y", "eye_open_left", "eye_open_right", "time", "validity", "emotion"};
                    string s2 = string.Join(",", s1);
                    sw.WriteLine(s2);
                }

                // Update is called once per frame
                void Update()
                {
                    if (Input.GetKey(KeyCode.Space))
                    {
                        isKeyDown = true;
                    }
                    else
                    {
                        isKeyDown = false;
                    }

                    //Debug.Log(eyeData.timestamp);
                    //Debug.Log(SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.WORKING);
                    //Debug.Log(SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.NOT_SUPPORT);
                    if (SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.WORKING) return;

                    if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == true && eye_callback_registered == false)
                    {
                        Debug.Log("test");
                        SRanipal_Eye_v2.WrapperRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
                        eye_callback_registered = true;
                    }
                    else if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == false && eye_callback_registered == true)
                    {
                        SRanipal_Eye_v2.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
                        eye_callback_registered = false;
                    }

                    //Vector3 GazeOriginCombinedLocal, GazeDirectionCombinedLocal;

                    //if (eye_callback_registered)
                    //{
                    //    if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.COMBINE, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal, eyeData)) { }
                    //    else if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.LEFT, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal, eyeData)) { }
                    //    else if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.RIGHT, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal, eyeData)) { }
                    //    else return;
                    //}
                    //else
                    //{
                    //    if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.COMBINE, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal)) { }
                    //    else if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.LEFT, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal)) { }
                    //    else if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.RIGHT, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal)) { }
                    //    else return;
                    //}

                    //Vector3 GazeDirectionCombined = Camera.main.transform.TransformDirection(GazeDirectionCombinedLocal);
                    //GazeRayRenderer.SetPosition(0, Camera.main.transform.position - Camera.main.transform.up * 0.05f);
                    //GazeRayRenderer.SetPosition(0, Camera.main.transform.position + GazeDirectionCombined * LengthOfRay);



                    SRanipal_Eye.GetGazeRay(GazeIndex.COMBINE, out GazeOriginCombinedLocalC, out GazeDirectionCombinedLocalC);

                    Vector3 GazeDirectionCombinedC = Camera.main.transform.TransformDirection(GazeDirectionCombinedLocalC);
                    //RaycastHit hitC;


                    //if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.position + GazeDirectionCombinedC * 500, out hitC))
                    //{
                    //    Debug.Log("test3");
                    //    string[] str = { "" + hitC.point.x, "" + hitC.point.y, "" + hitC.point.z, "" + UnityEngine.Time.time };
                    //    string str2 = string.Join(",", str);



                    //if (iswriting == true)
                    //{
                    //    sw.writeline(str2);
                    //}

                    //}
                    //string[] str =  "" + hitC.point.x, "" + hitC.point.y, "" + hitC.point.z, "" + UnityEngine.Time.time };

                    //string str2 = string.Join(",", str);

                    gaze_direction_left = eyeData.verbose_data.left.gaze_direction_normalized;
                    gaze_direction_right = eyeData.verbose_data.right.gaze_direction_normalized;
                    gaze_direction_combined = eyeData.verbose_data.combined.eye_data.gaze_direction_normalized;
                    eyeOpenLeft = eyeData.verbose_data.left.eye_openness;
                    eyeOpenRight = eyeData.verbose_data.right.eye_openness;
                    //Debug.Log("gaze_direction_combined"+ gaze_direction_combined);
                    pupilDiameterLeft = eyeData.verbose_data.left.pupil_diameter_mm;
                    pupilDiameterRight = eyeData.verbose_data.right.pupil_diameter_mm;
                    pupilPositionLeft = eyeData.verbose_data.left.pupil_position_in_sensor_area;
                    pupilPositionRight = eyeData.verbose_data.right.pupil_position_in_sensor_area;



                    bool eye_validity = eyeData.verbose_data.left.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_EYE_OPENNESS_VALIDITY);
                    float gaze_direction_left_x = gaze_direction_left.x;
                    float gaze_direction_left_y = gaze_direction_left.y;
                    float gaze_direction_left_z = gaze_direction_left.z;
                    float gaze_direction_right_x = gaze_direction_right.x;
                    float gaze_direction_right_y = gaze_direction_right.y;
                    float gaze_direction_right_z = gaze_direction_right.z;
                    float gaze_direction_combined_x = gaze_direction_combined.x;
                    float gaze_direction_combined_y = gaze_direction_combined.y;
                    float gaze_direction_combined_z = gaze_direction_combined.z;
                    float gaze_origin_left_x = gaze_origin_left.x;
                    float gaze_origin_left_y = gaze_origin_left.y;
                    float gaze_origin_left_z = gaze_origin_left.z;
                    float gaze_origin_right_x = gaze_origin_right.x;
                    float gaze_origin_right_y = gaze_origin_right.y;
                    float gaze_origin_right_z = gaze_origin_right.z;

                    
                    string[] str = { "" + gaze_direction_left_x, "" + gaze_direction_left_y, "" + gaze_direction_left_z, "" + gaze_direction_right_x, "" + gaze_direction_right_y, "" + gaze_direction_right_y, "" + gaze_direction_combined_x, "" + gaze_direction_combined_y, "" + gaze_direction_combined_z, "" + gaze_origin_left_x, "" + gaze_origin_left_y, "" + gaze_origin_left_z, "" + gaze_origin_right_x, "" + gaze_origin_right_y, "" + gaze_origin_right_z, "" + pupilDiameterLeft, "" + pupilDiameterRight, "" + pupilPositionLeft.x, "" + pupilPositionLeft.y, "" + pupilPositionRight.x, "" + pupilPositionRight.y, "" + eyeOpenLeft, "" + eyeOpenRight, "" + UnityEngine.Time.time, "" + eye_validity, "" + isKeyDown};
                    string str2 = string.Join(",", str);

                    if (isWriting == true)
                    {
                        sw.WriteLine(str2);
                    }

                    //string[] str = { "" + gaze_direction_left_x, "" + gaze_direction_left_y, "" + gaze_direction_left_z, "" + gaze_direction_right_x, "" + gaze_direction_right_y, "" + gaze_direction_right_y, "" + gaze_direction_combined_x, "" + gaze_direction_combined_y, "" + gaze_direction_combined_z, "" + gaze_origin_left_x, "" + gaze_origin_left_y, "" + gaze_origin_left_z, "" + gaze_origin_right_x, "" + gaze_origin_right_y, "" + gaze_origin_right_z, "" + pupilDiameterLeft, "" + pupilDiameterRight, "" + pupilPositionLeft.x, "" + pupilPositionLeft.y, "" + pupilPositionRight.x, "" + pupilPositionRight.y, "" + eyeOpenLeft, "" + eyeOpenRight, "" + UnityEngine.Time.time, "" + eye_validity };
                    //string str2 = string.Join(",", str);

                    //if (isWriting == true )
                    //{
                    //    sw.WriteLine(str2);
                    //}

                    //Vector3 GazeOriginCombineLocal, GazeDirectionCombinedLocal;

                    //SRanipal_Eye.GetGazeRay(GazeIndex.COMBINE, out GazeOriginCombineLocal, out GazeDirectionCombinedLocal);



                    //Vector3 GazeDirectionCombined = Camera.main.transform.TransformDirection(GazeDirectionCombinedLocal);
                    RaycastHit hit;
                    //Debug.Log(Camera.main.transform.position + GazeDirectionCombined * LengthOfRay);

                    //Debug.Log("GazeDirectionCombinedLocal:" + GazeDirectionCombinedLocal);
                    //Debug.Log("GazeOriginCombineLocal:" + GazeOriginCombineLocal);



                    //var forward = new Vector3(0f, 0f, 1.0f);
                    //var positions = new Vector3[]{
                    //    Camera.main.transform.position + forward,                // 開始点
                    //    Camera.main.transform.position   + gaze_direction_combined * LengthOfRay,               // 終了点
                    //};

                    ////GazeRayRenderer.SetPositions(Camera.main.transform.position, Camera.main.transform.position + gaze_direction_combined * LengthOfRay);
                    //GazeRayRenderer.SetPositions(positions);
                    ////GazeRayRenderer.SetPosition(0, Camera.main.transform.position + gaze_direction_combined * LengthOfRay);
                    //GazeRayRenderer.SetPosition(0, Camera.main.transform.position - Camera.main.transform.up * 0.05f);
                    //GazeRayRenderer.SetPosition(1, Camera.main.transform.position + gaze_direction_combined * LengthOfRay);

                    if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.position + gaze_direction_combined * LengthOfRay, out hit))
                    {
                        string objectName = hit.collider.gameObject.name;
                        //Debug.Log(objectName + ":" + hit.point.ToString("F2"));
                    }

                    //if (isWriting == true)
                    //{
                    //    sw.WriteLine(eyeData.timestamp);
                    //}
                    //if (input.getkeydown(keycode.f))
                    //{
                    //    sw.close();
                    //    isWriting = false;
                    //}

                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        sw.Close();
                        isWriting = false;
                    }


                }


                private void Release()
                {
                    if (eye_callback_registered == true)
                    {
                        SRanipal_Eye_v2.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
                        eye_callback_registered = false;
                        isWriting = false;
                        Debug.Log("end");
                    }
                    sw.Close();
                }


                private static void EyeCallback(ref EyeData_v2 eye_data)
                {
                    // Debug.Log(eyeData.timestamp);
                    eyeData = eye_data;
                    timestamp = eye_data.timestamp;
                    //Debug.Log(eye_data);

                    if (isWriting == true)
                    {
                        //Debug.Log(eyeData.timestamp);
                        //sw.WriteLine(eyeData.timestamp);
                    }

                    if (isWriting == false)
                    {
                        sw.Close();
                        isWriting = false;
                    }


                    //sw.WriteLine((string)eye_data.timestamp);
                }
            }
        }
    }
}