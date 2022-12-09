using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

/// <summary>
/// Handle the Random (White) Noise.
/// </summary>
public class WhiteNoise : BaseNoiseClass
{
    /// <summary>
    /// Extend the base class initializations.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();        
        PopulateVariablesWithDataFromUI();        
    }

    /// <summary>
    /// Generate a new normal(Gaussian) distribution
    /// Mapped to NoiseDataPanel NewDistribution button.
    /// </summary>
    public override void GenerateNewDistribution()
    {
        base.GenerateNewDistribution();
        CalculateBaseNoise();
    }

    /// <summary>
    /// Calculate the noise according to the user input.
    /// Mapped to NoiseDataPanel ApplyPattern button.
    /// </summary>
    public override void ApplyPattern()
    {
        CalculateNoise();
        bool noiseAppliedState = ( m_NoiseValueList.Count >= m_SampleSize ) ? true : false;
        SetReadyMessage( noiseAppliedState, "Random" );
    }

    /// <summary>
    /// Calculate the white/random noise.
    /// </summary>
    protected override void CalculateBaseNoise()
    {
        PopulateVariablesWithDataFromUI();
        float value = 0.0f;

        m_Multiplier = m_Mean + m_NoiseSTD;

        for( int i = 0; i < m_SampleSize; i++ )
        {
            value = m_Multiplier * (float)m_GaussianDistribution.RandomGauss( (double)m_Mean, (double)m_NoiseSTD );
            m_StandardNoiseDistribution.Add( value );
        }

        if( m_StandardNoiseDistribution.Count > 0 )
        {
            m_Title.text = "Distribution is Ready";
        }
        else
        {
            m_Title.text = "Distribution is NOT Ready";
        }
    }

    /// <summary>
    /// Scale the base Random/White noise.
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
}
