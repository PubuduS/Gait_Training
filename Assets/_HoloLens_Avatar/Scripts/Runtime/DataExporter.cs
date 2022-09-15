using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.IO;
using System;
using System.Linq;
using TMPro;

/// <summary>
/// This class is a helper class used to export data to files.
/// </summary>
public class DataExporter : MonoBehaviour
{
    // Need this to access signal data.
    [SerializeField] private ScaleNoisePatterns m_ScaleNoisePatterns;

    // Show the last file generated
    [SerializeField] private TextMeshPro m_LastFileLabel;
    [SerializeField] private TextMeshPro m_SignalFileLabel;
    [SerializeField] private TextMeshPro m_LeftFootFileLabel;
    [SerializeField] private TextMeshPro m_RightFootFileLabel;    

    /// Need this to access heel strike time stamps
    private AvatarAnimationState m_AvatarAnimationState = null;

    /// Check Coroutine is running or not.
    /// This is to prevent calling for multiple coroutines.
    private bool m_CoroutineIsRunning = false;

    /// <summary>
    /// This function start a Coroutine to export data
    /// Mapped to export data button.
    /// </summary>
    public void ExportDataFiles()
    {
        if( m_CoroutineIsRunning == false )
        {
            StartCoroutine( ExportFiles() );
        }
        else
        {
            StopCoroutine( ExportFiles() );
        }        
    }

    /// <summary>
    /// This function exports signal data and left/right heel strike time stamps.
    /// </summary>
    IEnumerator ExportFiles()
    {
        m_AvatarAnimationState = GameObject.FindGameObjectWithTag("Avatar").GetComponent<AvatarAnimationState>();
        m_CoroutineIsRunning = true;

        if (m_AvatarAnimationState == null)
        {
            m_LastFileLabel.text = "ERROR: Avatar needs to walk to get timestamps";
            m_CoroutineIsRunning = false;
            yield return null;
        }

        DateTime dob = DateTime.Now;
        string timeStamp = dob.ToString("MM_dd_yyyyTHH_mm_ss");
        string directoryLocation = Application.persistentDataPath + "/";

        ExportNoiseFiles(directoryLocation, timeStamp);

        ExportRightFootTimeStamps(directoryLocation, timeStamp);
        ExportLeftFootTimeStamps(directoryLocation, timeStamp);

        m_CoroutineIsRunning = false;
        yield return null;
    }

    /// <summary>
    /// Export the signal data currently being used in the walking trials.    
    /// </summary>
    private async void ExportNoiseFiles( string path, string timeStamp )
    {
        DateTime dob = DateTime.Now;
        string type = m_ScaleNoisePatterns.CurrentPattern.text.Split().Last();
        string postfix = type + "Noise" + "_" + timeStamp + ".txt";
        string fullPath = Path.Combine( path, postfix );
        

        if ( type.Equals("Pink") )
        {
            foreach( float freq in m_ScaleNoisePatterns.ScaledPinkNoise )
            {
                await WriteToFile( fullPath, freq );
            }
        }
        else if( type.Equals("Random") )
        {
            foreach( float freq in m_ScaleNoisePatterns.WhiteNoise )
            {
                await WriteToFile( fullPath, freq );
            }
        }
        else if( type.Equals("ISO") )
        {
            await WriteToFile( fullPath, m_ScaleNoisePatterns.PreferredWalkingSpeed );
        }
        else
        {
            await WriteToFile( fullPath, -9999 );
        }

        CheckFileExists( fullPath, m_SignalFileLabel );
    }

    /// <summary>
    /// Export the right foot heel strike TimeStamps
    /// </summary>
    private async void ExportRightFootTimeStamps( string path, string timeStamp )
    {
        string postfix = "RightFootTimeStamps" + "_" + timeStamp + ".txt";
        string fullPath = Path.Combine( path, postfix );

        if( m_AvatarAnimationState.RightFootTimeStamps.Count == 0 )
        {
            m_LastFileLabel.text = "ERROR: No Foot TimeStamps Found.";
            return;
        }

        foreach ( float timeVal in m_AvatarAnimationState.RightFootTimeStamps )
        {
            await WriteToFile( fullPath, timeVal );
        }

        CheckFileExists( fullPath, m_RightFootFileLabel );
    }

    /// <summary>
    /// Export the left foot heel strike TimeStamps
    /// </summary>
    private async void ExportLeftFootTimeStamps( string path, string timeStamp )
    {
        string postfix = "LeftFootTimeStamps" + "_" + timeStamp + ".txt";
        string fullPath = Path.Combine( path, postfix );

        if( m_AvatarAnimationState.LeftFootTimeStamps.Count == 0 )
        {
            m_LastFileLabel.text = "ERROR: No Foot TimeStamps Found.";
            return;
        }

        foreach ( float timeVal in m_AvatarAnimationState.LeftFootTimeStamps )
        {
            await WriteToFile( fullPath, timeVal );
        }

        CheckFileExists( fullPath, m_LeftFootFileLabel );
    }

    /// <summary>
    /// Asynchronously write and append line by line to a file.
    /// The using statement before the StreamWrite also closes the StreamReader.
    /// So, no need to close it.
    /// </summary>
    /// <returns></returns>
    private async Task WriteToFile(string path, float line)
    {
        using( StreamWriter myStreamWriter = new StreamWriter( path, append: true ) )
        {
            if( line != -9999 )
            {
                await myStreamWriter.WriteLineAsync( "" + Mathf.Abs(line) );
            }
            else
            {
                await myStreamWriter.WriteLineAsync( "Error Code: " + line );
            }
        } 
    }    

    /// <summary>
    /// Check the file exists and set the name of last file generated by the user.
    /// </summary>
    private void CheckFileExists( string path, TextMeshPro label )
    {
        int idx = path.LastIndexOf('/');
        string fileName = "";

        if( idx != -1 )
        {
            fileName = path.Substring( idx + 1 );
        }

        if ( System.IO.File.Exists( path ) )
        {
            label.text = fileName;
        }
        else
        {
            label.text = "None";
        }
    }
}
