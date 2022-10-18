using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
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

    /// A list to hold GKSqrt values.
    /// This is just an intermidiate value use to calculate base Pink noise.
    private List<double> m_GKSqrt;

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
       
    /// For pink noise 0.99 
    /// For white noise 0.5
    /// In future, we might need to add different values.
    private float m_AlphaValue = 0.99f;

    /// 2 * sample size. 
    /// We calculate 2X with X sample size.
    private int m_SampleSize2X = 5000;

    /// Used this to calculate pink noise
    private float m_SqrtOfTwo = 0.0f;

    /// Used this as a multiplier to calculate pink noise
    private float m_Multiplier = 0.0f;

    /// Defines how many samples we want.
    private int m_SampleSize = 5000;

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
    private void Awake()
    {
        m_GaussianDistribution = new GaussianDistribution();

        m_StandardNoiseDistribution = new List<float>();
        m_ScaledPinkNoise = new List<float>();
        m_BrownNoise = new List<float>();       
        m_WhiteNoise = new List<float>();
        m_GKSqrt = new List<double>();
        m_SqrtOfTwo = Mathf.Sqrt(2.0f);        

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
        CalculateBasePinkNoise();
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
            m_SDPeriodLabel.text = "" + m_ScaledPinkNoise.Count;
            bool noiseAppliedState = ( m_ScaledPinkNoise.Count >= ( m_SampleSize2X - 2 ) ) ? true : false;            
            SetReadyMessage( noiseAppliedState, "Pink" );
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
            bool noiseAppliedState = ( m_WhiteNoise.Count >= m_SampleSize2X ) ? true : false;
            SetReadyMessage( noiseAppliedState, "Random" );
        }
        else 
        {
            m_NoiseAppliedFlag = false;
            m_Title.text = " Error Unknown Noise Pattern ";
        }
        
    }

    /// <summary>
    /// Indicate noise is successgully applied or not.
    /// </summary>
    /// <param name="flag"> Indicate noise applied or not. </param>
    /// <param name="lbl"> Indicate the type of noise. </param>
    private void SetReadyMessage( bool flag, string lbl )
    {
        if( flag == true )
        {
            m_Title.text = lbl + " Noise Ready";
            m_NoiseAppliedFlag = true;
        }
        else
        {
            m_Title.text = lbl + " Noise is NOT Ready";
            m_NoiseAppliedFlag = false;
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

        if( m_ScaledPinkNoise.Count > 0 )
        {
            m_ScaledPinkNoise.Clear();
        }

        if( m_WhiteNoise.Count > 0 )
        {
            m_WhiteNoise.Clear();
        }
    }

    /// <summary>
    /// We scale each value in normal distribution to get this.
    /// </summary>
    public void ScalePinkNoise()
    {
        float value = 0.0f;

        foreach( float SDValue in m_StandardNoiseDistribution )
        {
            value = m_MeanPeriod + ( m_SDPeriod / m_NoiseSTD ) * SDValue;            
            m_ScaledPinkNoise.Add( value );            
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
            value = (float)m_GaussianDistribution.RandomGauss( (double)m_MeanPeriod, (double)m_NoiseSTD );            
            m_WhiteNoise.Add( value );
        }
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
            m_SampleSizeLabel.gameObject.SetActive( true );
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
        m_SampleSize2X = m_SampleSize;
        m_SampleSize /= 2;
    }

    /// <summary>
    /// This value is used to calculate base pink noise
    /// TODO -> We can parellize this in future.
    /// </summary>
    private void CalculateGKSQRT()
    {

        int basePinkNoiseArrLen = ( m_SampleSize2X - 2 );
        List<double> gammak = new List<double>();

        if( m_GKSqrt.Any() == true )
        {
            m_GKSqrt.Clear();
        }

        for(int i = 0; i < m_SampleSize; i++)
        {
            double first = Math.Pow( ( Mathf.Abs(i - 1) ), ( 2 * m_AlphaValue ) );

            double second = 2 * ( Math.Pow( i, ( 2 * m_AlphaValue ) ) );

            double third = Math.Pow( ( i + 1 ), ( 2 * m_AlphaValue ) );

            double result = ( first - second + third ) / 2.0;
            gammak.Add( result * basePinkNoiseArrLen );
        }

        for( int i = (m_SampleSize - 2); i > 0; i-- )
        {
            gammak.Add( gammak[i] );
        }

        Complex[] GammakComplex = new Complex[gammak.Count];

        for( int i = 0; i < gammak.Count; i++ )
        {
            Complex gamComp = new Complex( ( gammak[i] / gammak.Count ), 0.0 );
            GammakComplex[i] = gamComp;
        }

        gammak.Clear();

        IDFTReal( ref GammakComplex );
        
    }

    /// <summary>
    /// Calculate the base pink noise value.
    /// </summary>
    private void CalculateBasePinkNoise()
    {
        if ( m_StandardNoiseDistribution.Count > 0 )
        {
            m_StandardNoiseDistribution.Clear();
        }

        List<float> basePinkNoise = new List<float>();
        List<float> leftHalf = new List<float>();
        List<float> rightHalf = new List<float>();

        Complex[] basePinkNoiseComplexArr = new Complex[m_SampleSize2X - 2];

        m_Multiplier = m_MeanPeriod + m_NoiseSTD;

        // Generate GaussianDistribution Random values and multiple each value with the multiplier ( mean + SD )
        for( int i = 0; i < m_SampleSize2X; i++ )
        {
            basePinkNoise.Add( m_Multiplier * (float)m_GaussianDistribution.RandomGauss( (double)m_MeanPeriod, (double)m_NoiseSTD ) );

            if(i < m_SampleSize )
            {
                leftHalf.Add(basePinkNoise[i]);
            }
            else
            {
                rightHalf.Add(basePinkNoise[i]);
            }
        }

        basePinkNoise.Clear();

        rightHalf[0] = 0;
        leftHalf[0] = leftHalf[0] * m_SqrtOfTwo;

        rightHalf[m_SampleSize - 1] = 0;
        leftHalf[m_SampleSize - 1] = leftHalf[m_SampleSize - 1] * m_SqrtOfTwo;

        for( int i = (m_SampleSize - 2); i > 0; i-- )
        {
            leftHalf.Add(leftHalf[i]);
            rightHalf.Add(-rightHalf[i]);
        }

        for( int i = 0; i < rightHalf.Count; i++ )
        {
            Complex myComplex = new Complex( leftHalf[i], rightHalf[i] );
            basePinkNoiseComplexArr[i] = myComplex;
        }

        leftHalf.Clear();
        rightHalf.Clear();

        bool noZeroFlag = true;

        CalculateGKSQRT();

        for ( int i = 0; i < m_GKSqrt.Count; i++ )
        {
            if( Mathf.Approximately( (float)m_GKSqrt[i], 0.0f ) )
            {
                noZeroFlag = false;
                break;
            }
        }

        if( noZeroFlag == true )
        {
            int basePinkNoiseComplexArrLen = basePinkNoiseComplexArr.Length;

            for( int i = 0; i < basePinkNoiseComplexArrLen; i++ )
            {
                m_GKSqrt[i] = Math.Sqrt( m_GKSqrt[i] );
                basePinkNoiseComplexArr[i] = Complex.Multiply( basePinkNoiseComplexArr[i], m_GKSqrt[i] );
            }

            IDFT( ref basePinkNoiseComplexArr );

            for (int i = 0; i < basePinkNoiseComplexArrLen; i++)
            {
                double power = Math.Pow( ( m_SampleSize - 1 ), ( -0.5 ) );
                Complex intermediateResult = Complex.Multiply( power, basePinkNoiseComplexArr[i] );
                Complex result = Complex.Multiply( intermediateResult, 0.5 );
                m_StandardNoiseDistribution.Add( (float)result.Real);                
            }

            m_Title.text = "Distribution is Ready";
        }
        else 
        {
            m_Title.text = "Distribution is NOT Ready";
        }    
    }

    /// <summary>
    /// Calculates inverse Discrete Fourier Transform of given spectrum X
    /// </summary>
    /// <param name="X">Spectrum complex values</param>
    /// <returns>Signal samples in time domain</returns>
    public void IDFT( ref Complex[] X )
    {
        // Number of spectrum elements
        int N = X.Length; 
        Complex[] x = new Complex[N];

        for( int n = 0; n < N; n++ )
        {
            Complex sum = 0;

            for( int k = 0; k < N; k++ )
            {
                sum += X[k] * Complex.Exp(Complex.ImaginaryOne * 2 * Math.PI * (k * n) / Convert.ToDouble(N));
            }

            // As a result we expect only real values (if our calculations are correct imaginary values should be equal or close to zero)
            x[n] = sum; 
        }

        Array.Clear(X, 0, N);

        for (int i = 0; i < N; i++)
        {
            X[i] = x[i];
        }
    }

    /// <summary>
    /// Calculates inverse Discrete Fourier Transform of given spectrum X
    /// </summary>
    /// <param name="X">Spectrum complex values</param>
    /// <returns>Return only the real part of the number /returns>
    public void IDFTReal( ref Complex[] X )
    {
        int N = X.Length; // Number of spectrum elements        

        for( int n = 0; n < N; n++ )
        {
            Complex sum = 0;

            for( int k = 0; k < N; k++ )
            {
                sum += X[k] * Complex.Exp(Complex.ImaginaryOne * 2 * Math.PI * (k * n) / Convert.ToDouble(N));
            }

            // As a result we expect only real values (if our calculations are correct imaginary values should be equal or close to zero)
            m_GKSqrt.Add( sum.Real );
        }
    }

}
