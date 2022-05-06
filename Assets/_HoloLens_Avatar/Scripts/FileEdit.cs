using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

public class FileEdit : MonoBehaviour
{ 

    void Start()
    {     
        DeleteFileIfExists();
        ReadStringAsync();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static async void ReadStringAsync()
    {
        string path = Application.persistentDataPath + "/Joel_Iso1.anim";

        string line = "";
        float timeVal;
        // Create an instance of StreamReader to read from a file.
        // The using statement also closes the StreamReader.
        try
        {
            using ( StreamReader reader = new StreamReader( path ) )
            {
                while( ( line = reader.ReadLine() ) != null )
                {

                    if (line.Contains("time:"))
                    {
                        
                        var match = Regex.Match(line, @"([-+]?[0-9]*\.?[0-9]+)");
                        if (match.Success)
                        {
                            timeVal = Convert.ToSingle(match.Groups[1].Value);
                            timeVal += 0.0004f;
                            // we need exactlt 8 spaces before time
                            line = "        time: " + timeVal;
                        }
                    }

                    await WriteToFile(line);
                    
                }
            }
            Debug.Log("Done Writing");
            //CopyAnimFile("Joel_Iso1Copy.anim");
        }
        catch (Exception ex)
        {
            Debug.Log("The file could not read");
            Debug.Log(ex.Message);
        }

    }

    public static async Task WriteToFile( string line )
    {
        string path = Application.persistentDataPath + "/Joel_Iso1Copy.anim";

        using StreamWriter file = new StreamWriter(path, append: true);

        await file.WriteLineAsync(line);
    }

    private void DeleteFileIfExists()
    {
        string path = Application.persistentDataPath + "/Joel_Iso1Copy.anim";       

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

    public static void CopyAnimFile( string fileName )
    {
        try
        {
            // Will not overwrite if the destination file already exists.
            File.Copy(Path.Combine(Application.persistentDataPath, fileName), Application.dataPath+"/_HoloLens_Avatar/EditedAnimations/", true);
            Debug.Log("Path "+Application.dataPath + "/_HoloLens_Avatar/EditedAnimations/");
        }

        // Catch exception if the file was already copied.
        catch (IOException copyError)
        {
            Debug.Log(copyError.Message);
        }
    }

}
