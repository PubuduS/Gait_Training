using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class PinkNoise : BaseNoiseClass
{
    #region Private Variables
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

    /// A list to hold GKSqrt values.
    /// This is just an intermidiate value use to calculate base Pink noise.
    private List<double> m_GKSqrt;

    /// A List to hold Normal(Gaussian) distribution.
    private List<float> m_StandardNoiseDistribution;

    #endregion

    #region Public Variables
    /// Property to get standard normal(Gaussian) distribution (Read-Only)
    public List<float> NoiseDistribution { get => m_StandardNoiseDistribution; }
    #endregion

    /// <summary>
    /// 
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        m_StandardNoiseDistribution = new List<float>();
        m_GKSqrt = new List<double>();
        m_SqrtOfTwo = Mathf.Sqrt( 2.0f );        
        PopulateVariablesWithDataFromUI();
        m_DistributionButton.GetComponent<Interactable>().IsEnabled = true;
    }

    /// <summary>
    /// Populate data variables used to alter noise.
    /// The data are gained through UI lables which are set by the keyboard input.
    /// </summary>
    protected override void PopulateVariablesWithDataFromUI()
    {
        base.PopulateVariablesWithDataFromUI();
        m_SampleSize2X = m_SampleSize;
        m_SampleSize /= 2;
    }

    /// <summary>
    /// Generate a new normal(Gaussian) distribution
    /// Mapped to NoiseDataPanel NewDistribution button.
    /// </summary>
    public override void GenerateNewDistribution()
    {
        base.GenerateNewDistribution();
        CalculateBasePinkNoise();
    }

    /// <summary>
    /// Calculate the noise according to the user input.
    /// Mapped to NoiseDataPanel ApplyPattern button.
    /// </summary>
    public override void ApplyPattern()
    {
        CalculateNoise();
        bool noiseAppliedState = ( m_NoiseValueList.Count >= ( m_SampleSize2X - 2 ) ) ? true : false;
        SetReadyMessage( noiseAppliedState, "Pink" );
    }

    /// <summary>
    /// Scale the base pink noise.
    /// </summary>
    protected override void CalculateNoise()
    {
        float value = 0.0f;
        ConvertToZScore( ref m_StandardNoiseDistribution );

        foreach( float unscaledSignal in m_StandardNoiseDistribution )
        {
            value = m_MeanPeriod + ( m_SDPeriod / m_NoiseSTD ) * unscaledSignal;
            m_NoiseValueList.Add( value );
        }
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

        for( int i = 0; i < m_SampleSize; i++ )
        {
            double first = Math.Pow( ( Mathf.Abs( i - 1 ) ), ( 2 * m_AlphaValue ) );

            double second = 2 * ( Math.Pow( i, ( 2 * m_AlphaValue ) ) );

            double third = Math.Pow( ( i + 1 ), ( 2 * m_AlphaValue ) );

            double result = ( first - second + third ) / 2.0;
            gammak.Add( result * basePinkNoiseArrLen );
        }

        for( int i = ( m_SampleSize - 2 ); i > 0; i-- )
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
        if( m_StandardNoiseDistribution.Count > 0 )
        {
            m_StandardNoiseDistribution.Clear();
        }

        List<float> basePinkNoise = new List<float>();
        List<float> leftHalf = new List<float>();
        List<float> rightHalf = new List<float>();

        Complex[] basePinkNoiseComplexArr = new Complex[m_SampleSize2X - 2];

        m_Multiplier = m_Mean + m_NoiseSTD;

        // Generate GaussianDistribution Random values and multiple each value with the multiplier ( mean + SD )
        for( int i = 0; i < m_SampleSize2X; i++ )
        {
            basePinkNoise.Add( m_Multiplier * (float)m_GaussianDistribution.RandomGauss( (double)m_Mean, (double)m_NoiseSTD ) );

            if( i < m_SampleSize )
            {
                leftHalf.Add( basePinkNoise[i] );
            }
            else
            {
                rightHalf.Add( basePinkNoise[i] );
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

        for( int i = 0; i < m_GKSqrt.Count; i++ )
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

            for( int i = 0; i < basePinkNoiseComplexArrLen; i++ )
            {
                double power = Math.Pow( ( m_SampleSize - 1 ), ( -0.5 ) );
                Complex intermediateResult = Complex.Multiply( power, basePinkNoiseComplexArr[i] );
                Complex result = Complex.Multiply( intermediateResult, 0.5 );
                m_StandardNoiseDistribution.Add( (float)result.Real );
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
                sum += X[k] * Complex.Exp( Complex.ImaginaryOne * 2 * Math.PI * (k * n) / Convert.ToDouble(N) );
            }

            // As a result we expect only real values (if our calculations are correct imaginary values should be equal or close to zero)
            x[n] = sum;
        }

        Array.Clear( X, 0, N );

        for( int i = 0; i < N; i++ )
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
                sum += X[k] * Complex.Exp( Complex.ImaginaryOne * 2 * Math.PI * (k * n) / Convert.ToDouble(N) );
            }

            // As a result we expect only real values (if our calculations are correct imaginary values should be equal or close to zero)
            m_GKSqrt.Add( sum.Real );
        }
    }

    /// <summary>
    /// This function calculate the sample Standard Deviation value and return it.
    /// </summary>
    /// <param name="basePinkNoiseList"></param>
    /// <returns>Sample Standard Deviation</returns>
    private double GetStandardDeviation( ref List<float> basePinkNoiseList )
    {
        double ret = 0;
        int count = basePinkNoiseList.Count();

        if( count > 1 )
        {
            //Compute the Average
            double avg = basePinkNoiseList.Average();

            //Perform the Sum of (value-avg)^2
            double sum = basePinkNoiseList.Sum(d => (d - avg) * (d - avg));

            //Put it all together
            ret = Math.Sqrt( sum / (count - 1) );
        }

        return ret;
    }

    /// <summary>
    /// This converts Z values to Z Score values.
    /// May get off a small amount due to round error.
    /// </summary>
    /// <param name="basePinkNoiseList"></param>
    private void ConvertToZScore( ref List<float> basePinkNoiseList )
    {
        double mean = basePinkNoiseList.Average();
        double sd = GetStandardDeviation(ref basePinkNoiseList);

        for( int i = 0; i < basePinkNoiseList.Count; i++ )
        {
            float val = (float)( ( basePinkNoiseList[i] - mean ) / sd );
            m_StandardNoiseDistribution[i] = val;
        }
    }
}
