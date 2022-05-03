using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;
using System.IO;

public class Controller : MonoBehaviour
{
    public string pythonPath;
    public string scriptPath;

    // Start is called before the first frame update
    void Start()
    {
        // Attempt to run Python script
        ProcessStartInfo psi = new ProcessStartInfo();
        psi.FileName = pythonPath;

        psi.Arguments = $"\"{ scriptPath }\"";
        psi.UseShellExecute = false;
        psi.CreateNoWindow = true;
        psi.RedirectStandardOutput = true;
        psi.RedirectStandardError = true;

        string errors = "";
        string output = "";

        using ( var process = Process.Start( psi ) )
        {
            errors = process.StandardError.ReadToEnd();
            output = process.StandardOutput.ReadToEnd();
        }

        UnityEngine.Debug.Log( "ERRORS:" );
        UnityEngine.Debug.Log( errors );
        UnityEngine.Debug.Log( "RESULTS:" );
        UnityEngine.Debug.Log( output );
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}