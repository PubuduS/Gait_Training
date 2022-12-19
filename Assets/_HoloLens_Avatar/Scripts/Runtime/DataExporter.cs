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

    // Show the last file generated
    [SerializeField] private TextMeshPro m_LastFileLabel;
    [SerializeField] private TextMeshPro m_SignalFileLabel;
    [SerializeField] private TextMeshPro m_AnimLenLabel;    

    /// Need this to access heel strike time stamps
    private AvatarAnimationState m_AvatarAnimationState = null;

    /// Check Coroutine is running or not.
    /// This is to prevent calling for multiple coroutines.
    private bool m_CoroutineIsRunning = false;

    /// Directory location where files stored.
    /// C:\Users\yourname\AppData\LocalLow\DefaultCompany\_BarMetronome
    private string m_DirectoryLocation = null;

    /// <summary>
    /// Initialize persistentDataPath
    /// </summary>
    private void Start()
    {
        m_DirectoryLocation = Application.persistentDataPath + "/";
    }

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

        if( m_AvatarAnimationState == null )
        {
            m_LastFileLabel.text = "ERROR: Avatar needs to walk to get timestamps";
            m_CoroutineIsRunning = false;
            yield return null;
        }

        ExportGenericFiles( "Noise", m_SignalFileLabel, NoiseController.Instance.BaseNoise.NoiseValueList );

        ExportGenericFiles( "AnimationLength", m_AnimLenLabel, m_AvatarAnimationState.AnimationLength );


        m_CoroutineIsRunning = false;
        yield return null;
    }

    /// <summary>
    /// Export the data currently being used in the walking trials.    
    /// </summary>
    private async void ExportGenericFiles( string name, TextMeshPro label, List<float> valList )
    {
        DateTime dob = DateTime.Now;
        string timeStamp = dob.ToString( "MM_dd_yyyyTHH_mm_ss" );
        string type = NoiseController.Instance.BaseNoise.CurrentPattern.text.Split().Last();
        string postfix = type + name + "_" + timeStamp + ".txt";
        string fullPath = Path.Combine( m_DirectoryLocation, postfix );


        if( type.Equals("Pink") || type.Equals("Random") )
        {
            if( valList.Any() )
            {
                foreach( float value in valList )
                {
                    await WriteToFile( fullPath, value );
                }
            }

        }
        else if( type.Equals("ISO") )
        {
            if( name.Equals("Noise") )
            {
                await WriteToFile( fullPath, NoiseController.Instance.BaseNoise.PreferredWalkingSpeed );
            }
            else
            {
                if( valList.Any() )
                {
                    foreach( float value in valList )
                    {
                        await WriteToFile( fullPath, value );
                    }
                }
            }
            
        }
        else
        {
            await WriteToFile( fullPath, -9999 );
        }

        CheckFileExists( fullPath, label );
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
