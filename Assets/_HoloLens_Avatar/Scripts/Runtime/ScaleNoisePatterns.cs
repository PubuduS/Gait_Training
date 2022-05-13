using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

/// <summary>
/// Calculate a Normal(Gaussian) distribution.
/// and if user selected, scaled that and stored it as Pink Noise.
/// Also generated a normal random distribution and calculate Brown and White noises.
/// </summary>
public class ScaleNoisePatterns : MonoBehaviour
{
    #region Private Variables

    /// A List to hold Normal(Gaussian) distribution.
    private List<float> m_StandardNoiseDistribution;

    /// We scale this Normal(Gaussian) distribution and store it as the scaled pink noise.
    private List<float> m_ScaledPinkNoise;

    /// A list to hold brown noise.
    private List<float> m_BrownNoise;

    /// A list to hold white noise.
    private List<float> m_WhiteNoise;

    /// Hold the mean persiod.
    private float m_MeanPeriod = 0.0f;

    /// Hold the standard distribution.
    private float m_NoiseSTD = 1.0f;

    /// Hold the standard distribution period.
    private float m_SDPeriod = 2.0f;

    /// Hold the previous sum od the brown noise.
    private float m_BrownPreviousSum = 0.0f;

    /// Defines the minimum value.
    private int m_MinValue = -2;

    /// Defines the maximum value.
    private int m_MaxValue = 2;

    /// Defines how many samples we want.
    private int m_SampleSize = 1000;

    /// Reference to GaussianDistribution script.
    private GaussianDistribution m_GaussianDistribution;

    /// This flag will indicate we applied or cancel the noise to animations.
    private bool m_NoiseAppliedFlag = false;

    /// We get the value from these text labels.
    [SerializeField] private TextMeshPro m_MeanPeriodLabel;
    [SerializeField] private TextMeshPro m_SDPeriodLabel;
    [SerializeField] private TextMeshPro m_SDLabel;
    [SerializeField] private TextMeshPro m_SampleSizeLabel;
    [SerializeField] private TextMeshPro m_CurrentPattern;
    [SerializeField] private TextMeshPro m_Title;

    #endregion

    #region Public Varibles 
    /// Property to get standard normal(Gaussian) distribution (Read-Only)
    public List<float> NoiseDistribution { get => m_StandardNoiseDistribution; }

    /// Property to get scaled pink noise distribution (Read-Only)
    public List<float> ScaledPinkNoise { get => m_ScaledPinkNoise; }

    /// Property to get brown noise distribution (Read-Only)
    public List<float> BrownNoise { get => m_BrownNoise; }

    /// Property to get white noise distribution (Read-Only)
    public List<float> WhiteNoise { get => m_WhiteNoise; }

    /// Property to get noise applied flag (Read-Only)
    public bool NoiseAppliedFlag { get => m_NoiseAppliedFlag; }


    /// These fields are to get and set noise related values.    
    public float NoiseMean { get => m_MeanPeriod; set => m_MeanPeriod = value; }
    public float NoiseSTD { get => m_NoiseSTD; set => m_NoiseSTD = value; }
    public float SDPeriod { get => m_SDPeriod; set => m_SDPeriod = value; }
    public int MinValue { get => m_MinValue; set => m_MinValue = value; }
    public int MaxValue { get => m_MaxValue; set => m_MaxValue = value; }

    #endregion

    /// <summary>
    /// When invoke the scripts performas all the initializations including,
    /// Lists, scripts, and noise scaling values such as standard distribution and mean.
    /// </summary>
    void Start()
    {
        m_StandardNoiseDistribution = new List<float>();
        m_ScaledPinkNoise = new List<float>();
        m_BrownNoise = new List<float>();
        m_WhiteNoise = new List<float>();

        m_GaussianDistribution = new GaussianDistribution();

        m_MeanPeriod = (float)ExtractDecimalFromUI( m_MeanPeriodLabel.text );       
        m_SDPeriod = (float)ExtractDecimalFromUI( m_SDPeriodLabel.text );
        m_NoiseSTD = (float)ExtractDecimalFromUI(m_SDLabel.text);
        m_SampleSize = (int)ExtractDecimalFromUI( m_SampleSizeLabel.text );       
    }

    /// <summary>
    /// Generate a new normal(Gaussian) distribution
    /// Mapped to NoiseDataPanel NewDistribution button.
    /// </summary>
    public void GenerateNewDistribution()
    {
        m_MeanPeriod = (float)ExtractDecimalFromUI( m_MeanPeriodLabel.text );
        m_SDPeriod = (float)ExtractDecimalFromUI( m_SDPeriodLabel.text );
        m_NoiseSTD = (float)ExtractDecimalFromUI( m_SDLabel.text );
        m_SampleSize = (int)ExtractDecimalFromUI( m_SampleSizeLabel.text );

        float number = 0.0f;
        for( int i = 0; i < m_SampleSize; i++ )
        {
            number = (float)m_GaussianDistribution.RandomGauss( (double)m_MeanPeriod, (double)m_NoiseSTD );
            m_StandardNoiseDistribution.Add( number );            
        }

        m_Title.text = "Distribution is Ready";
    }

    /// <summary>
    /// Calculate the selected patterns.
    /// Mapped to NoiseDataPanel ApplyPattern button.
    /// </summary>
    public void ApplyPattern()
    {
        string currentPattern = m_CurrentPattern.text.Split().Last();        

        if ( currentPattern.Equals("Pink") )
        {
            ScalePinkNoise();
            m_Title.text = "Pink Noise Ready";
            m_NoiseAppliedFlag = true;
        }
        else if( currentPattern.Equals("IST") )
        {
            CalculateBrownNoise();
            m_Title.text = "Brown Noise Ready";
            m_NoiseAppliedFlag = true;
        }
        else if( currentPattern.Equals("Random") )
        {
            CalculateWhiteNoise();
            m_Title.text = "Random Noise Ready";
            m_NoiseAppliedFlag = true;
        }
        else 
        {
            m_NoiseAppliedFlag = false;
            m_Title.text = " Error Unknown Noise Pattern ";
        }
        
    }

    /// <summary>
    /// Cancel the current noise pattern and reset it with default speed.
    /// Mapped to NoiseDataPanel CancelPattern button.
    /// </summary>
    public void CanclePattern()
    {
        m_NoiseAppliedFlag = false;
        m_Title.text = "Pattern reset to default speed ";
    }

    /// <summary>
    /// We scale each value in normal distribution to get this.
    /// </summary>
    public void ScalePinkNoise()
    {
        float value = 0.0f;

        foreach( float SDValue in m_StandardNoiseDistribution )
        {
            value = m_MeanPeriod + (m_SDPeriod / m_NoiseSTD) * SDValue;            
            m_ScaledPinkNoise.Add( value );
        }
    }

    /// <summary>
    /// We calculate the Brown noise based on random values and previous sum.
    /// Any outliers are assigned to upper or lower bound to prevent large numbers that can mess up the animations.
    /// </summary>
    public void CalculateBrownNoise()
    {
        float value = 0.0f;
        
        for ( int i = 0; i < m_SampleSize; i++ )
        {
            value = GenerateNormalRandom( m_MeanPeriod, m_SDPeriod, m_MinValue, m_MaxValue ) + m_BrownPreviousSum;

            if( value < m_MinValue )
            {
                value = m_MinValue;
            }
            else if( value > m_MaxValue )
            {
                value = m_MaxValue;
            }

            if( value != 0.0f )
            {                
                m_BrownNoise.Add(value);
                m_BrownPreviousSum = value;
            }

        }
    }

    /// <summary>
    /// We calculate the White noise.
    /// This is just random numbers.
    /// </summary>
    public void CalculateWhiteNoise()
    {
        float value = 0.0f;
       
        for (int i = 0; i < m_SampleSize; i++)
        {
            value = GenerateNormalRandom( m_MeanPeriod, m_SDPeriod, m_MinValue, m_MaxValue );           
            m_WhiteNoise.Add( value );
        }
    }

    /// <summary>
    /// Generate a normal random value.
    /// </summary>
    /// <param name="mean"></param>
    /// <param name="sigma"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    private float GenerateNormalRandom( float mean, float sigma, int min, int max )
    {
        float rand1 = UnityEngine.Random.Range(0.0f, 1.0f);
        float rand2 = UnityEngine.Random.Range(0.0f, 1.0f);

        float n = Mathf.Sqrt( -2.0f * Mathf.Log(rand1)) * Mathf.Cos( ( 2.0f * Mathf.PI ) * rand2 );
                
        float generatedNumber = mean + sigma * n;

        generatedNumber = Mathf.Clamp(generatedNumber, min, max);

        return generatedNumber;
    }

    /// <summary>
    /// Get the number part from the UI textfields.
    /// </summary>
    /// <param name="textFromUI"></param>
    /// <returns></returns>
    private double ExtractDecimalFromUI( string textFromUI )
    {
        string numberPart = textFromUI.Split().Last();
        double number = 0.0;
        bool isValid = double.TryParse( numberPart, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out number );

        if( isValid == true )
        {
            return number;
        }

        return 0.0;
    }
 
}
