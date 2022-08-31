using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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

    /// Hold the user's preferred walking speed.
    private float m_PreferredWalkingSpeed = 1.0f;

    /// Defines the minimum value.
    private int m_MinValue = -2;

    /// Defines the maximum value.
    private int m_MaxValue = 2;

    /// Defines how many samples we want.
    private int m_SampleSize = 2000;

    /// Reference to GaussianDistribution script.
    private GaussianDistribution m_GaussianDistribution;

    /// This flag will indicate we applied or cancel the noise to animations.
    private bool m_NoiseAppliedFlag = false;

    /// We get the value from these text labels.
    [SerializeField] private TextMeshPro m_MeanPeriodLabel;
    [SerializeField] private TextMeshPro m_SDPeriodLabel;
    [SerializeField] private TextMeshPro m_SDLabel;
    [SerializeField] private TextMeshPro m_SampleSizeLabel;
    [SerializeField] private TextMeshPro m_PreferredSpeedLabel;
    [SerializeField] private TextMeshPro m_CurrentPattern;
    [SerializeField] private TextMeshPro m_Title;

    [SerializeField] private GameObject m_ApplyButton;
    [SerializeField] private GameObject m_DistributionButton;

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

    /// Property to get user's preferred walking speed (Read-Only)
    public float PreferredWalkingSpeed { get => m_PreferredWalkingSpeed; }

    /// Property to get noise applied flag (Read-Only)
    public bool NoiseAppliedFlag { get => m_NoiseAppliedFlag; }

    /// Property to get the current noise in use (Read-Only)
    public TextMeshPro CurrentPattern { get => m_CurrentPattern; }


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

        PopulateVariablesWithDataFromUI();
    }

    private void Update()
    { 
        SetUITextVisibility( CurrentPattern.text );
    }

    /// <summary>
    /// Generate a new normal(Gaussian) distribution
    /// Mapped to NoiseDataPanel NewDistribution button.
    /// </summary>
    public void GenerateNewDistribution()
    {
        PopulateVariablesWithDataFromUI();

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
        string currentPattern = CurrentPattern.text.Split().Last();        

        if ( currentPattern.Equals("Pink") )
        {
            ScalePinkNoise();
            m_Title.text = "Pink Noise Ready";
            m_NoiseAppliedFlag = true;
        }
        else if( currentPattern.Equals("ISO") )
        {
            StartISOCalculations();
            m_Title.text = "ISO Noise Ready";
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
        m_ApplyButton.GetComponent<Interactable>().IsToggled = false;
        m_DistributionButton.GetComponent<Interactable>().IsToggled = false;
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
    /// We may or may not want this. Just keep it for now for the sake of future.
    /// </summary>
    public void CalculateBrownNoise()
    {
        PopulateVariablesWithDataFromUI();
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
    /// For the ISO noise the time to complete the gait cycle should be a 
    /// constant value and set to preferred speed of the user.
    /// </summary>
    public void StartISOCalculations()
    {
        PopulateVariablesWithDataFromUI( true );
        m_PreferredWalkingSpeed = (float)ExtractDecimalFromUI( m_PreferredSpeedLabel.text );
    }

    /// <summary>
    /// We calculate the White noise.
    /// This is just random numbers.
    /// </summary>
    public void CalculateWhiteNoise()
    {
        PopulateVariablesWithDataFromUI();
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

    /// <summary>
    /// Change the visibility of the text fields according to the noise patterns.
    /// </summary>
    /// <param name="currentPattern"></param>
    private void SetUITextVisibility( string currentPattern )
    {

        if( currentPattern.Equals("Noise: ISO") )
        {
            m_MeanPeriodLabel.gameObject.SetActive( false );
            m_SDPeriodLabel.gameObject.SetActive( false );
            m_SDLabel.gameObject.SetActive( false );
            m_SampleSizeLabel.gameObject.SetActive( false );
            m_PreferredSpeedLabel.gameObject.SetActive( true );
        }
        else
        { 
            m_MeanPeriodLabel.gameObject.SetActive( true );
            m_SDPeriodLabel.gameObject.SetActive( true );
            m_SDLabel.gameObject.SetActive( true );
            m_PreferredSpeedLabel.gameObject.SetActive( false );
        }
    }

    /// <summary>
    /// Populate data variables used to alter noise.
    /// The data are gained through UI lables which are set by the keyboard input.
    /// </summary>
    private void PopulateVariablesWithDataFromUI( bool isISO = false )
    {
        if( isISO )
        {
            m_SampleSize = (int)ExtractDecimalFromUI(m_SampleSizeLabel.text);
            return;
        }

        m_MeanPeriod = (float)ExtractDecimalFromUI(m_MeanPeriodLabel.text);
        m_SDPeriod = (float)ExtractDecimalFromUI(m_SDPeriodLabel.text);
        m_NoiseSTD = (float)ExtractDecimalFromUI(m_SDLabel.text);
        m_SampleSize = (int)ExtractDecimalFromUI(m_SampleSizeLabel.text);
    }
}
