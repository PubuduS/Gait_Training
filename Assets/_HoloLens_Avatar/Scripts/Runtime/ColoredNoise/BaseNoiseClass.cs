using Microsoft.MixedReality.Toolkit.UI;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;

public abstract class BaseNoiseClass : SingletonMonobehaviour<BaseNoiseClass>
{

    #region Private Varibles

    /// Reference to NoiseDataPanel UI.
    private GameObject m_NoiseDataPanel = null;

    #endregion

    #region Protected Variables

    /// Hold the Mean value.
    protected const float m_Mean = 0.0f;

    /// Hold the standard distribution.
    protected const float m_NoiseSTD = 1.0f;

    /// Hold the Mean Period Value.
    protected float m_MeanPeriod = 1.0f;

    /// Hold the standard distribution period.
    protected float m_SDPeriod = 2.0f;

    /// Defines how many samples we want.
    protected int m_SampleSize = 5000;

    /// Reference to GaussianDistribution script.
    protected GaussianDistribution m_GaussianDistribution;

    /// This flag will indicate we applied or cancel the noise to animations.
    protected bool m_NoiseAppliedFlag = false;

    /// Default ISO preferred walking speed.
    protected const float m_DefaultISOWalkSpeed = -1.0f;

    /// Hold the user's preferred walking speed.
    protected float m_PreferredWalkingSpeed = m_DefaultISOWalkSpeed;

    /// This list stores the calculated colored noise values.
    protected List<float> m_NoiseValueList = null;

    /// We get the value from these text labels.
    protected TextMeshPro m_MeanPeriodLabel;
    protected TextMeshPro m_SDPeriodLabel;
    protected TextMeshPro m_SampleSizeLabel;
    protected TextMeshPro m_PreferredSpeedLabel;
    protected TextMeshPro m_CurrentPattern;
    protected TextMeshPro m_Title;

    protected GameObject m_ApplyButton;
    protected GameObject m_DistributionButton;

    #endregion

    #region Public Varibles 

    /// Property to get colored noise distribution (Read-Only)
    public List<float> NoiseValueList { get => m_NoiseValueList; }

    /// Property to get noise applied flag (Read-Only)
    public bool NoiseAppliedFlag { get => m_NoiseAppliedFlag; }

    /// Property to get the current noise in use (Read-Only)
    public TextMeshPro CurrentPattern { get => m_CurrentPattern; }

    /// Property to get user's preferred walking speed (Read-Only)
    public float PreferredWalkingSpeed { get => m_PreferredWalkingSpeed; }

    #endregion

    /// <summary>
    /// When invoke the scripts performas all the initializations including,
    /// Lists, scripts, and noise scaling values such as standard distribution and mean.
    /// </summary>
    protected virtual void Awake()
    {
        m_NoiseDataPanel = GameObject.FindGameObjectWithTag("NoiseDataPanel");
        m_GaussianDistribution = new GaussianDistribution();
        m_NoiseValueList = new List<float>();       
        InitializeNoiseDataPanelObjects();
        SetUITextVisibility();
    }

    /// <summary>
    /// Cache the references of NoiseDataPanel gameobjects.
    /// </summary>
    protected void InitializeNoiseDataPanelObjects()
    {
        if( m_NoiseDataPanel != null )
        {
            m_MeanPeriodLabel = m_NoiseDataPanel.transform.Find("MeanPeriod").gameObject.GetComponent<TextMeshPro>();
            m_SDPeriodLabel = m_NoiseDataPanel.transform.Find("SD_Period").gameObject.GetComponent<TextMeshPro>();
            m_SampleSizeLabel = m_NoiseDataPanel.transform.Find("SampleSize").gameObject.GetComponent<TextMeshPro>();
            m_PreferredSpeedLabel = m_NoiseDataPanel.transform.Find("Preferred_Speed").gameObject.GetComponent<TextMeshPro>();
            m_CurrentPattern = m_NoiseDataPanel.transform.Find("NoiseLbl").gameObject.GetComponent<TextMeshPro>();
            m_Title = m_NoiseDataPanel.transform.Find("Title").gameObject.GetComponent<TextMeshPro>(); ;

            m_ApplyButton = m_NoiseDataPanel.transform.Find("ApplyPattern").gameObject;
            m_DistributionButton = m_NoiseDataPanel.transform.Find("NewDistribution").gameObject;
        }
        else
        {
            Debug.Log("Cannot Find NoiseDataPanel Object");
        }
    }

    /// <summary>
    /// Calculate the colored noise
    /// </summary>
    protected abstract void CalculateNoise();


    /// <summary>
    /// Calculate the noise according to the user input.
    /// Mapped to NoiseDataPanel ApplyPattern button.
    /// </summary>
    public abstract void ApplyPattern();

    /// <summary>
    /// Generate a new normal(Gaussian) distribution
    /// Mapped to NoiseDataPanel NewDistribution button.
    /// </summary>
    public virtual void GenerateNewDistribution()
    {
        PopulateVariablesWithDataFromUI();        
    }

    /// <summary>
    /// Cancel the current noise pattern and reset it with default speed.
    /// Mapped to NoiseDataPanel CancelPattern button.
    /// </summary>
    public virtual void CanclePattern()
    {
        m_NoiseAppliedFlag = false;
        m_Title.text = "Pattern reset to default speed ";
        m_ApplyButton.GetComponent<Interactable>().IsToggled = false;
        m_DistributionButton.GetComponent<Interactable>().IsToggled = false;

        if( m_NoiseValueList.Count > 0 )
        {
            m_NoiseValueList.Clear();
        }
    }

    /// <summary>
    /// Indicate noise is successgully applied or not.
    /// </summary>
    /// <param name="flag"> Indicate noise applied or not. </param>
    /// <param name="lbl"> Indicate the type of noise. </param>
    protected void SetReadyMessage( bool flag, string lbl )
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
    /// Get the number part from the UI textfields.
    /// </summary>
    /// <param name="textFromUI"></param>
    /// <returns></returns>
    protected double ExtractDecimalFromUI( string textFromUI )
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
    protected virtual void SetUITextVisibility()
    {
        m_MeanPeriodLabel.gameObject.SetActive( true );
        m_SDPeriodLabel.gameObject.SetActive( true );
        m_SampleSizeLabel.gameObject.SetActive( true );
        m_PreferredSpeedLabel.gameObject.SetActive( false );
    }

    /// <summary>
    /// Populate data variables used to alter noise.
    /// The data are gained through UI lables which are set by the keyboard input.
    /// </summary>
    protected virtual void PopulateVariablesWithDataFromUI()
    {
        m_MeanPeriod = (float)ExtractDecimalFromUI(m_MeanPeriodLabel.text);
        m_SDPeriod = (float)ExtractDecimalFromUI(m_SDPeriodLabel.text);
        m_SampleSize = (int)ExtractDecimalFromUI(m_SampleSizeLabel.text);
    }
}
