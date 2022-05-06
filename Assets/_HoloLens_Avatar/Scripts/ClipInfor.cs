using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ClipInfor : MonoBehaviour
{
    
    [SerializeField]
    private AnimationClip clip;

    [SerializeField]
    private string m_TimestampFileName;

    //public static Animation m_Anim;


    // Start is called before the first frame update
    void Start()
    {
        
        
        if(clip == null)
        {
            Debug.Log("Clip is Null");
        }
        else
        {
            //GetAllCurves(clip, true);

            //DeleteFileIfExists();
            //GetAllCurveTimestamps(clip);

            DeleteFileIfExists();
            WriteCSV(clip);
        }
        

    }

    // Update is called once per frame
    void Update()
    {        
    }

    public static AnimationClipCurveData[] GetAllCurves(AnimationClip clip, bool includeCurveData)
    {
        EditorCurveBinding[] curveBindings = AnimationUtility.GetCurveBindings(clip);
        AnimationClipCurveData[] dataArray = new AnimationClipCurveData[curveBindings.Length];

        AnimationClip animClip = new AnimationClip();
        animClip.legacy = true;
        

        for (int i = 0; i < dataArray.Length; i++)
        {
            dataArray[i] = new AnimationClipCurveData(curveBindings[i]);
            if (includeCurveData)
            {
                dataArray[i].curve = AnimationUtility.GetEditorCurve(clip, curveBindings[i]);
               // Debug.Log(dataArray[i].curve.keys.Length);
                //Debug.Log(dataArray[i].propertyName);
                //Debug.Log("Time "+dataArray[i].curve.keys[1].time);

                
                if( dataArray[i].propertyName == "Left Foot Up-Down")
                {
                    
                    int len = dataArray[i].curve.keys.Length;
                    //Debug.Log(" Length " + len);
          
                    Keyframe[] keys = dataArray[i].curve.keys;
                    for (int j = 0; j < len; j++)
                    {
                        
                       Keyframe keyFrame = keys[j];                        
                       keys[j].time = 5.0f;

                    }

                    dataArray[i].curve.keys = keys;
                    for (int j = 0; j < len; j++)
                    {
                        Debug.Log("Time " + dataArray[i].curve.keys[j].time+ " i " + j);
                    }

                    animClip.SetCurve("", typeof(float), "Left Foot Up-Down", dataArray[i].curve);
                    //m_Anim.AddClip(animClip, "Test");
                    //m_Anim.Play("Test");
                }
                
            }
        }
        return dataArray;
    }

    private void GetAllCurveTimestamps(AnimationClip clip)
    {
        EditorCurveBinding[] curveBindings = AnimationUtility.GetCurveBindings(clip);
        AnimationClipCurveData[] dataArray = new AnimationClipCurveData[curveBindings.Length];
        string lineToWrite = "";
        int keyFrameLength = 0;
        int timestampCount = 0;

        for (int i = 0; i < dataArray.Length; i++)
        {
            dataArray[i] = new AnimationClipCurveData(curveBindings[i]);

            dataArray[i].curve = AnimationUtility.GetEditorCurve(clip, curveBindings[i]);

            // Write the property name
            lineToWrite = "------------------------------------------------------------------";
            WriteToFile( lineToWrite );

            lineToWrite = dataArray[i].propertyName;
            WriteToFile( lineToWrite );

            keyFrameLength = dataArray[i].curve.keys.Length;

            timestampCount = 0;
            for ( int j = 0; j < keyFrameLength; j++)
            {
                lineToWrite = dataArray[i].curve.keys[j].time.ToString();
                WriteToFile( lineToWrite );
                timestampCount++;
            }

            WriteToFile( "TimeStamp Count = "+timestampCount );
            timestampCount = 0;
        }

    }

    private void WriteToFile( string line )
    {
        string path = Path.Combine(Application.persistentDataPath, m_TimestampFileName);

        using StreamWriter file = new StreamWriter(path, append: true);

        file.WriteLineAsync(line);
    }

    private void DeleteFileIfExists()
    {
        string path = Path.Combine(Application.persistentDataPath, m_TimestampFileName);

        try
        {

            if (File.Exists(path))
            {
                File.Delete(path);
            }

        }

        // Catch exception if the file was already copied.
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    private void WriteCSV( AnimationClip clip )
    {
        EditorCurveBinding[] curveBindings = AnimationUtility.GetCurveBindings(clip);
        AnimationClipCurveData[] dataArray = new AnimationClipCurveData[curveBindings.Length];
        string lineToWrite = "";
        int keyFrameLength = 0;
        int timestampCount = 0;

        string path = Path.Combine(Application.persistentDataPath, m_TimestampFileName);
        TextWriter myWriter = new StreamWriter(path, true);

        for (int i = 0; i < dataArray.Length; i++)
        {
            dataArray[i] = new AnimationClipCurveData(curveBindings[i]);

            dataArray[i].curve = AnimationUtility.GetEditorCurve(clip, curveBindings[i]);

            // Write the property name
            lineToWrite = dataArray[i].propertyName + ", ";            

            keyFrameLength = dataArray[i].curve.keys.Length;

            
            for (int j = 0; j < keyFrameLength; j++)
            {
                lineToWrite += dataArray[i].curve.keys[j].time.ToString() + ", ";                
            }
            
            myWriter.WriteLine(lineToWrite);
            
        }

        myWriter.Close();
    }

}
