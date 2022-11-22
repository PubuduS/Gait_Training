using Microsoft.MixedReality.Toolkit.UI;

/// <summary>
/// 
/// </summary>
public class WhiteNoise : BaseNoiseClass
{
    /// <summary>
    /// 
    /// </summary>
    protected override void Awake()
    {
        base.Awake();        
        PopulateVariablesWithDataFromUI();
        m_DistributionButton.GetComponent<Interactable>().IsEnabled = false;
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
    protected override void CalculateNoise()
    {
        PopulateVariablesWithDataFromUI();
        float value = 0.0f;

        for( int i = 0; i < m_SampleSize; i++ )
        {
            value = (float)m_GaussianDistribution.RandomGauss( (double)m_Mean, (double)m_NoiseSTD );
            m_NoiseValueList.Add( value );
        }
    }
}
