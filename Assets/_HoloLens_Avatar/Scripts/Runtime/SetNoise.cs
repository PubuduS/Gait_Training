using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// This class is used to set noise pattern levels.
/// When user press a button, it will change the pattern.
/// </summary>
public class SetNoise : MonoBehaviour
{
    /// Enum for noise levels 
    public enum NoiseTypes { PinkNoise = 0, IstNoise = 1, WhiteNoise = 2 };

    /// This will hold the user's selected pattern.
    private NoiseTypes m_PatternType = NoiseTypes.PinkNoise;

    /// This will show the user selection of the pattern
    /// under the avatar in avatar panel.
    [SerializeField] private TextMeshPro m_NoiseLable;

    /// <summary>
    /// Gets called from the button's OnClick event.
    /// 0 is for deafult pattern
    /// 1 is for Pink noise pattern.
    /// 2 is for White noise(Random) pattern.
    /// </summary>
    public void SetNoisePattern(int pattern)
    {
        m_PatternType = (NoiseTypes)pattern;
        SetPatternLable();
    }

    /// <summary>
    /// Sets the label according to the user selection.
    /// User can see what they selected in Avatar panel
    /// under the avatar.
    /// </summary>
    private void SetPatternLable()
    {
        switch (m_PatternType)
        {
            case NoiseTypes.PinkNoise:
                m_NoiseLable.text = "Noise: Pink";
                break;

            case NoiseTypes.IstNoise:
                m_NoiseLable.text = "Noise: IST";
                break;

            case NoiseTypes.WhiteNoise:
                m_NoiseLable.text = "Noise: Random";
                break;

            default:
                Debug.Log("Incorect Input. Can't Change Labele");
                m_NoiseLable.text = "!!! Noise: Error !!!";
                break;
        }
    }

    /// <summary>
    /// Gettr to get the user's selection.
    /// </summary>
    public NoiseTypes GetNoisePattern()
    {
        return m_PatternType;
    }
}
