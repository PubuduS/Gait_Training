using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class NoiseController : SingletonMonobehaviour<NoiseController>
{

    private BaseNoiseClass m_BaseNoise;

    public BaseNoiseClass BaseNoise { get => m_BaseNoise; set => m_BaseNoise = value; }

    /// <summary>
    /// Default Constructor
    /// </summary>
    public NoiseController()
    {       
    }

    /// <summary>
    /// Calculate the noise according to the user input.
    /// Mapped to NoiseDataPanel ApplyPattern button.
    /// </summary>
    public void ApplyNoise()
    {
        BaseNoise.ApplyPattern();
    }

    /// <summary>
    /// Cancel the current noise pattern and reset it with default speed.
    /// Mapped to NoiseDataPanel CancelPattern button.
    /// </summary>
    public void CancelNoise()
    {
        BaseNoise.CanclePattern();
    }

    /// <summary>
    /// Generate a new normal(Gaussian) distribution
    /// Mapped to NoiseDataPanel NewDistribution button.
    /// </summary>
    public void SetNewDistribution()
    {
        if ( string.Equals( BaseNoise.CurrentPattern.text, "Noise: Pink" ) )
        {
            BaseNoise.GenerateNewDistribution();
        }
    }

}
