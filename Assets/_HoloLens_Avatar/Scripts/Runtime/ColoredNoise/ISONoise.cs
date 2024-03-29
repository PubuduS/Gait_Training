using Microsoft.MixedReality.Toolkit.UI;

/// <summary>
/// Apply a constant speed for the whole walk cycle.
/// </summary>
public class ISONoise : BaseNoiseClass
{

    /// <summary>
    /// This UI only has one field and there is no
    /// need for a Distribution Button. Therefore,
    /// We only display PreferredSpeed field and disable
    /// Distribution Button.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();        
        m_DistributionButton.GetComponent<Interactable>().IsEnabled = false;
    }

    /// <summary>
    /// Change the visibility of the text fields according to the noise patterns.
    /// </summary>  
    protected override void SetUITextVisibility()
    {
        m_MeanPeriodLabel.gameObject.SetActive(false);
        m_SDPeriodLabel.gameObject.SetActive(false);
        m_SampleSizeLabel.gameObject.SetActive(true);
        m_PreferredSpeedLabel.gameObject.SetActive(true);
    }

    /// <summary>
    /// We don't need this method for this class.
    /// This is just a dummy implementation.
    /// </summary>
    protected override void CalculateBaseNoise()
    {
        // No need to implement this method here.
    }

    /// <summary>
    /// Calculate the noise according to the user input.
    /// Mapped to NoiseDataPanel ApplyPattern button.
    /// </summary>
    public override void ApplyPattern()
    {
        CalculateNoise();
        bool noiseAppliedState = ( m_PreferredWalkingSpeed > m_DefaultISOWalkSpeed ) ? true : false;
        SetReadyMessage( noiseAppliedState, "ISO" );
    }

    /// <summary>
    /// Cancel the current noise pattern and reset it with default speed.
    /// Mapped to NoiseDataPanel CancelPattern button.
    /// </summary>
    public override void CanclePattern()
    {
        m_NoiseAppliedFlag = false;
        m_Title.text = "Pattern reset to default speed ";
        m_ApplyButton.GetComponent<Interactable>().IsToggled = false;        
    }

    /// <summary>
    /// Calculate the ISO noise.
    /// </summary>
    protected override void CalculateNoise()
    {
        m_SampleSize = (int)ExtractDecimalFromUI( m_SampleSizeLabel.text );
        m_PreferredWalkingSpeed = (float)ExtractDecimalFromUI( m_PreferredSpeedLabel.text );

        for( int i = 0; i < m_SampleSize; ++i )
        {
            m_NoiseValueList.Add( m_PreferredWalkingSpeed );
        }
    }
}
