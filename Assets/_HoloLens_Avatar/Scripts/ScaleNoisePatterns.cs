using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class ScaleNoisePatterns : MonoBehaviour
{
    private List<float> m_StandardNoiseDistribution;
    private List<float> m_ScaledPinkNoise;
    private List<float> m_BrownNoise;
    private List<float> m_WhiteNoise;
    private List<float> m_ScaledPinkNoiseCopy;

    private float m_MeanPeriod = 0.0f;
    private float m_NoiseSTD = 1.0f;
    private float m_SDPeriod = 2.0f;
    private float m_BrownPreviousSum = 0.0f;

    // Remove Later
    private float m_PreviousTimeVal = 0.0f;
    private float m_Mean = 0.09046f;

    private int m_MinValue = 0;
    private int m_MaxValue = 2;

    public List<float> NoiseDistribution { get => m_StandardNoiseDistribution; }
    public List<float> ScaledPinkNoise { get => m_ScaledPinkNoise; }
    public List<float> BrownNoise { get => m_BrownNoise; }
    public List<float> WhiteNoise { get => m_WhiteNoise; }

    public float NoiseMean { get => m_MeanPeriod; set => m_MeanPeriod = value; }
    public float NoiseSTD { get => m_NoiseSTD; set => m_NoiseSTD = value; }
    public float SDPeriod { get => m_SDPeriod; set => m_SDPeriod = value; }
    public int MinValue { get => m_MinValue; set => m_MinValue = value; }
    public int MaxValue { get => m_MaxValue; set => m_MaxValue = value; }


    // Start is called before the first frame update
    void Start()
    {
        m_StandardNoiseDistribution = new List<float>();
        m_ScaledPinkNoise = new List<float>();
        m_BrownNoise = new List<float>();
        m_WhiteNoise = new List<float>();
        m_ScaledPinkNoiseCopy = new List<float>();

        GetDistributionFromFile();
        ScalePinkNoise();

        m_ScaledPinkNoiseCopy = m_ScaledPinkNoise.Distinct().ToList();
        m_ScaledPinkNoiseCopy = m_ScaledPinkNoiseCopy.Where(i => i >= 0).ToList();
        m_ScaledPinkNoiseCopy.Sort((a, b) => a.CompareTo(b));

        CalculateBrownNoise();
        CalculateWhiteNoise();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void GetDistributionFromFile()
    {

        string path = Path.Combine( Application.streamingAssetsPath, "RandomDistribution.txt" );
        string line = "";

        // Create an instance of StreamReader to read from a file.
        // The using statement also closes the StreamReader.
        try
        {
            using (StreamReader reader = new StreamReader(path))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    m_StandardNoiseDistribution.Add( float.Parse(line) );                    
                }                
            }
        }
        catch(Exception ex)
        {
            Debug.Log("The file could not read");
            Debug.Log(ex.Message);
        }
    }

    public void ScalePinkNoise()
    {
        float value = 0.0f;

        foreach( float SDValue in m_StandardNoiseDistribution )
        {
            value = m_MeanPeriod + (m_SDPeriod / m_NoiseSTD) * SDValue;
            m_ScaledPinkNoise.Add( value );
        }
    }

    public void CalculateBrownNoise()
    {
        float value = 0.0f;

        for( int i = 0; i < 400; i++ )
        {
            value = GenerateNormalRandom( m_MeanPeriod, m_SDPeriod, m_MinValue, m_MaxValue ) + m_BrownPreviousSum;
            m_BrownNoise.Add( value );
            m_BrownPreviousSum = value;
        }

    }

    public void CalculateWhiteNoise()
    {
        float value = 0.0f;

        for (int i = 0; i < 400; i++)
        {
            value = GenerateNormalRandom( m_MeanPeriod, m_SDPeriod, m_MinValue, m_MaxValue );
            m_WhiteNoise.Add( value );
        }
    }

    private float GenerateNormalRandom( float mean, float sigma, int min, int max )
    {
        float rand1 = UnityEngine.Random.Range(0.0f, 1.0f);
        float rand2 = UnityEngine.Random.Range(0.0f, 1.0f);

        float n = Mathf.Sqrt( -2.0f * Mathf.Log(rand1)) * Mathf.Cos( ( 2.0f * Mathf.PI ) * rand2 );
                
        float generatedNumber = mean + sigma * n;

        generatedNumber = Mathf.Clamp(generatedNumber, min, max);

        return generatedNumber;
    }
 
}
