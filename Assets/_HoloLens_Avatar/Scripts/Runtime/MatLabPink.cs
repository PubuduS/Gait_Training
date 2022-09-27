using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using System.IO;
using System.Linq;

public class MatLabPink : MonoBehaviour
{
    private int m_SampleSize = 2500;
    private float m_Mean = 0.0f;
    private float m_StandardDeviation = 1.0f;
    private float m_AlphaValue = 0.1f;

    private int m_SampleSize2X = 0;
    private float m_SqrtOfTwo = 0.0f;
    private float m_Multiplier = 0.0f;
        
    private GaussianDistribution m_GaussianDistribution;    

    private List<double> m_GKSqrt;
    private List<double> m_FinalBasePinkNoise;

    private int m_NumberOfCalls = 0;
    private int indexPrev = 0; 

    private void Awake()
    {
        m_SampleSize2X = ( 2 * m_SampleSize );
        m_SqrtOfTwo = Mathf.Sqrt( 2.0f );
        m_Multiplier = m_Mean + m_StandardDeviation;                

        m_GKSqrt = new List<double>();
        m_FinalBasePinkNoise = new List<double>();

        m_GaussianDistribution = new GaussianDistribution();

        // CalculateGKSQRT();
        //CalculateBasePinkNoise();

        for( int i = 1; i < 10; i++ )
        {
            for(int j = 0; j < 100; j++)
            {
                CalculateGKSQRT();
                CalculateBasePinkNoise();
            }

            m_AlphaValue += 0.1f;
            indexPrev = 0;
            m_NumberOfCalls = 0;
        }


    }

    private void CalculateGKSQRT()
    {
        int basePinkNoiseArrLen = ( m_SampleSize2X - 2 );
        List<double> gammak = new List<double>();

        if( m_GKSqrt.Any() == true )
        {
            m_GKSqrt.Clear();
        }

        for (int i = 0; i < m_SampleSize; i++)
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

        Complex[] GammakComplex = new Complex[ gammak.Count ];

        for (int i = 0; i < gammak.Count; i++)
        {
            Complex gamComp = new Complex( ( gammak[i] / gammak.Count ), 0.0 );
            GammakComplex[i] = gamComp;
        }

        gammak.Clear();

        IDFTReal( ref GammakComplex );
    }

    private void CalculateBasePinkNoise()
    {
        List<float> basePinkNoise = new List<float>();
        List<float> leftHalf = new List<float>();
        List<float> rightHalf = new List<float>();

        Complex[] basePinkNoiseComplexArr = new Complex[ m_SampleSize2X - 2 ];

        // Generate GaussianDistribution Random values and multiple each value with the multiplier ( mean + SD )
        for ( int i = 0; i < m_SampleSize2X; i++ )
        {
            basePinkNoise.Add( m_Multiplier * (float)m_GaussianDistribution.RandomGauss( (double)m_Mean, (double)m_StandardDeviation) );

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

        for( int i = ( m_SampleSize - 2 ); i > 0; i-- )
        {
            leftHalf.Add( leftHalf[i] );
            rightHalf.Add( -rightHalf[i] );
        }
 
        for ( int i = 0; i < rightHalf.Count; i++ )
        {
            Complex myComplex = new Complex( leftHalf[i], rightHalf[i] );            
            basePinkNoiseComplexArr[i] = myComplex;
        }

        leftHalf.Clear();
        rightHalf.Clear();     

        bool noZeroFlag = true;

        for( int i = 0; i < m_GKSqrt.Count; i++ )
        {
            if( Mathf.Approximately( (float)m_GKSqrt[i], 0.0f) )
            {
                noZeroFlag = false;
                break;
            }
        }

        if ( noZeroFlag == true )
        {
            int basePinkNoiseComplexArrLen = basePinkNoiseComplexArr.Length;

            for ( int i = 0; i < basePinkNoiseComplexArrLen; i++ )
            {
                m_GKSqrt[i] = Math.Sqrt( m_GKSqrt[i] );
                basePinkNoiseComplexArr[i] = Complex.Multiply( basePinkNoiseComplexArr[i], m_GKSqrt[i] );                
            }

            IDFT( ref basePinkNoiseComplexArr );

            for( int i = 0; i < basePinkNoiseComplexArrLen; i++)
            {
                double power = Math.Pow( ( m_SampleSize - 1 ), (-0.5) );
                Complex intermediateResult = Complex.Multiply( power, basePinkNoiseComplexArr[i] );
                Complex result = Complex.Multiply( intermediateResult, 0.5 );
                WriteToFile("" + result.Real, m_NumberOfCalls );
                //WriteToFile("" + result.Real );
            }
        }
        m_NumberOfCalls++;
    }

    /// <summary>
    /// Calculates inverse Discrete Fourier Transform of given spectrum X
    /// </summary>
    /// <param name="X">Spectrum complex values</param>
    /// <returns>Signal samples in time domain</returns>
    public void IDFT( ref Complex[] X )
    {
        int N = X.Length; // Number of spectrum elements
        Complex[] x = new Complex[N];

        for( int n = 0; n < N; n++ )
        {
            Complex sum = 0;

            for( int k = 0; k < N; k++ )
            {
                sum += X[k] * Complex.Exp( Complex.ImaginaryOne * 2 * Math.PI * (k * n) / Convert.ToDouble(N) );
            }

            x[n] = sum; // As a result we expect only real values (if our calculations are correct imaginary values should be equal or close to zero)
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

        for (int n = 0; n < N; n++)
        {
            Complex sum = 0;

            for (int k = 0; k < N; k++)
            {
                sum += X[k] * Complex.Exp( Complex.ImaginaryOne * 2 * Math.PI * (k * n) / Convert.ToDouble(N) );
            }

            // As a result we expect only real values (if our calculations are correct imaginary values should be equal or close to zero)
            m_GKSqrt.Add( sum.Real );
        }        
    }

    private void WriteToFile( string line, int index )
    {
        string path = "C:\\Users\\Pubudu\\Desktop\\TestData\\";
        string postfix = "Data_H" + m_AlphaValue + ".csv";
        path = Path.Combine( path, postfix );
        
        using( StreamWriter myStreamWriter = new StreamWriter( path, append: true) )
        {
            if( line != " " && ( index == indexPrev ) )
            {                
                myStreamWriter.Write("" + line + ", ");
            }
            else if( line != " " && (index != indexPrev) )
            {                
                myStreamWriter.Write("\n" + line + ", ");
                indexPrev = index;
            }
            else
            {
                myStreamWriter.WriteLine("Error Code: " + line);
            }
        }
    }

    private void WriteToFile( string line )
    {
        string path = "C:\\Users\\Pubudu\\Desktop\\UnityIFFT.txt";

        using (StreamWriter myStreamWriter = new StreamWriter(path, append: true))
        {
            if( line != " " )
            {
                myStreamWriter.WriteLine("" + line );
            }
            else
            {
                myStreamWriter.WriteLine("Error Code: " + line);
            }
        }
    }

}
