using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using System.IO;

public class MatLabPink : MonoBehaviour
{
    private int m_SampleSize = 2500;
    private float m_Mean = 0.0f;
    private float m_StandardDeviation = 1.0f;
    private float m_AlphaValue = 1.0f;

    private List<float> m_BasePinkNoise;
    private GaussianDistribution m_GaussianDistribution;    

    private List<float> m_LeftHalf;
    private List<float> m_RightHalf;
    private List<float> m_Zic;
    private List<double> m_Gammak;
    private List<float> m_ListInd;
    private List<System.Numerics.Complex> m_BasePinkNoiseComplex;
    private List<float> m_ListK;

    private int m_NumberOfCalls = 0;
    private int indexPrev = 0; 

    private void Awake()
    {
        m_BasePinkNoise = new List<float>();
        m_LeftHalf = new List<float>();
        m_RightHalf = new List<float>();
        m_Zic = new List<float>();
        m_ListK = new List<float>();
        m_Gammak = new List<double>();
        m_ListInd = new List<float>();
        m_BasePinkNoiseComplex = new List<System.Numerics.Complex>();

        m_GaussianDistribution = new GaussianDistribution();

        //CalculateBasePinkNoise();
        for( int i = 0; i < 100; i++ )
        {
            m_BasePinkNoise.Clear();
            m_LeftHalf.Clear();
            m_RightHalf.Clear();
            m_Zic.Clear();
            m_ListK.Clear();
            m_Gammak.Clear();
            m_ListInd.Clear();
            m_BasePinkNoiseComplex.Clear();

            CalculateBasePinkNoise();
        }
       
    }

    private void CalculateBasePinkNoise()
    {        
        float multiplier = m_Mean + m_StandardDeviation;

        // Generate GaussianDistribution Random values and multiple each value with the multiplier ( mean + SD )
        for( int i = 0; i < ( 2 * m_SampleSize ); i++ )
        {
            m_BasePinkNoise.Add( multiplier * (float)m_GaussianDistribution.RandomGauss( (double)m_Mean, (double)m_StandardDeviation) );
        }

        // zr
        for( int i = 0; i < m_SampleSize; i++ )
        {
            m_LeftHalf.Add( m_BasePinkNoise[i] );
        }

        // zi and -zic
        for( int i = m_SampleSize; i < ( 2 * m_SampleSize ); i++ )
        {
            float value = m_BasePinkNoise[i];
            m_RightHalf.Add( value );
            m_Zic.Add( -value );
        }

        m_RightHalf[0] = 0;
        m_LeftHalf[0] = m_LeftHalf[0] * Mathf.Sqrt(2.0f);

        m_RightHalf[m_SampleSize - 1] = 0;
        m_LeftHalf[m_SampleSize - 1] = m_LeftHalf[m_SampleSize - 1] * Mathf.Sqrt(2.0f);

        for( int i = ( m_SampleSize - 2 ); i > 0; i-- )
        {
            m_LeftHalf.Add( m_LeftHalf[i] );
        }     

        for( int i = ( m_SampleSize - 2 ); i > 0; i-- )
        {
            m_RightHalf.Add( m_Zic[i] );
        }        

        for ( int i = 0; i < m_RightHalf.Count; i++ )
        {
            Complex myComplex = new Complex( m_LeftHalf[i], m_RightHalf[i] );
            m_BasePinkNoiseComplex.Add( myComplex );
        }        

        for ( int i = 0; i < m_SampleSize; i++ )
        {
            m_ListK.Add( i );            
        }

        for ( int i = 0; i < m_SampleSize; i++ )
        {            
            double first = Math.Pow( ( Mathf.Abs( m_ListK[i] - 1 ) ), ( 2 * m_AlphaValue ) );

            double second = 2 * ( Math.Pow( ( m_ListK[i]) , ( 2 * m_AlphaValue ) ) );

            double third = Math.Pow( ( Mathf.Abs( m_ListK[i] + 1 ) ), ( 2 * m_AlphaValue ) );

            double result = ( first - second + third ) / 2.0;
            m_Gammak.Add(result);
            
        }

        for( int i = 0; i < m_SampleSize; i++ )
        {
            // Just index in dscending order
            m_ListInd.Add( (m_SampleSize-1) - i );
        }

        for( int i = 0; i < m_SampleSize; i++ )
        {
            m_Gammak[i] = m_Gammak[i] * m_BasePinkNoiseComplex.Count;
        }        

        for (int i = (m_SampleSize - 2); i > 0; i--)
        {
            m_Gammak.Add( m_Gammak[i] );
        }        

        Complex[] GammakComplex = new Complex[m_Gammak.Count];

        for (int i = 0; i < m_Gammak.Count; i++)
        {
            Complex gamComp = new Complex( (m_Gammak[i] / m_Gammak.Count), 0.0 );
            GammakComplex[i] = gamComp;
        }

        double[] gksqrt = IDFTReal( GammakComplex );

        bool noZeroFlag = true;

        for( int i = 0; i < gksqrt.Length; i++ )
        {
            if( Mathf.Approximately( (float)gksqrt[i], 0.0f) )
            {
                noZeroFlag = false;
                break;
            }
        }

        Complex[] BasePinkNoiseComplexArr = new Complex[gksqrt.Length];
        /*
        m_BasePinkNoiseComplex[0] = new Complex( 1, 0 );
        m_BasePinkNoiseComplex[1] = new Complex( 2, 0 );
        m_BasePinkNoiseComplex[2] = new Complex( 3, 0 );
        gksqrt[0] = 53.4095452027712;
        gksqrt[1] = 19.1840670596032;
        gksqrt[2] = 13.3328157820935;
        */

        if ( noZeroFlag == true )
        {
            for( int i = 0; i < BasePinkNoiseComplexArr.Length; i++ )
            {
                gksqrt[i] = Math.Sqrt( gksqrt[i] );
                //WriteToFile("sqrt gksqrt " + gksqrt[i]);
                //WriteToFile("m_BasePinkNoiseComplex[" + i + "] " + m_BasePinkNoiseComplex[i] + " * " + gksqrt[i]);
                BasePinkNoiseComplexArr[i] = Complex.Multiply( m_BasePinkNoiseComplex[i], gksqrt[i] );
                //WriteToFile("z.*gksqrt " + BasePinkNoiseComplexArr[i]);
            }

            BasePinkNoiseComplexArr = IDFT( BasePinkNoiseComplexArr );

            for( int i = 0; i < BasePinkNoiseComplexArr.Length; i++ )
            {               
                //BasePinkNoiseComplexArr[i] = Complex.Multiply( BasePinkNoiseComplexArr[i], BasePinkNoiseComplexArr.Length );               
            }

            for( int i = 0; i < BasePinkNoiseComplexArr.Length; i++)
            {
                double power = Math.Pow( ( m_SampleSize - 1 ), (-0.5));
                Complex intermediateResult = Complex.Multiply( power, BasePinkNoiseComplexArr[i] );
                Complex result = Complex.Multiply( intermediateResult, 0.5 );
                WriteToFile("" + result.Real, m_NumberOfCalls);
            }
        }
        m_NumberOfCalls++;
    }

    /// <summary>
    /// Calculates inverse Discrete Fourier Transform of given spectrum X
    /// </summary>
    /// <param name="X">Spectrum complex values</param>
    /// <returns>Signal samples in time domain</returns>
    public Complex[] IDFT(System.Numerics.Complex[] X)
    {
        int N = X.Length; // Number of spectrum elements
        Complex[] x = new Complex[N];

        for (int n = 0; n < N; n++)
        {
            System.Numerics.Complex sum = 0;

            for (int k = 0; k < N; k++)
            {
                sum += X[k] * System.Numerics.Complex.Exp(System.Numerics.Complex.ImaginaryOne * 2 * Math.PI * (k * n) / Convert.ToDouble(N));
            }

            x[n] = sum; // As a result we expect only real values (if our calculations are correct imaginary values should be equal or close to zero)
        }

        return x;
    }

    /// <summary>
    /// Calculates inverse Discrete Fourier Transform of given spectrum X
    /// </summary>
    /// <param name="X">Spectrum complex values</param>
    /// <returns>Return only the real part of the number /returns>
    public double[] IDFTReal(System.Numerics.Complex[] X)
    {
        int N = X.Length; // Number of spectrum elements
        double[] x = new double[N];

        for (int n = 0; n < N; n++)
        {
            System.Numerics.Complex sum = 0;

            for (int k = 0; k < N; k++)
            {
                sum += X[k] * System.Numerics.Complex.Exp(System.Numerics.Complex.ImaginaryOne * 2 * Math.PI * (k * n) / Convert.ToDouble(N));
            }

            x[n] = sum.Real; // As a result we expect only real values (if our calculations are correct imaginary values should be equal or close to zero)
        }

        return x;
    }

    private void WriteToFile( string line, int index )
    {
        string path = "C:\\Users\\Pubudu\\Desktop\\TestData\\Data_H1.0.csv";
        
        using (StreamWriter myStreamWriter = new StreamWriter(path, append: true))
        {
            if( line != " " && ( index == indexPrev ) )
            {                
                myStreamWriter.Write("" + line + ", ");
            }
            else if( line != " " && (index != indexPrev) )
            {
                Debug.Log("index " + index + " prev " + indexPrev );
                myStreamWriter.Write("\n" + line + ", ");
                indexPrev = index;
            }
            else
            {
                myStreamWriter.WriteLine("Error Code: " + line);
            }
        }
    }

}
