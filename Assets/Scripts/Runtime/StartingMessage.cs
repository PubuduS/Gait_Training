using System.Collections;
using UnityEngine;

/// <summary>
/// Display a message when starting the app and then destroy it.
/// </summary>
public class StartingMessage : MonoBehaviour 
{

   
    /// Reference to the panel
    [SerializeField] private GameObject m_Load;

    /// Reference to the starting message 
    [SerializeField] private GameObject m_StartMessage;

    /// Reference to the thenk you message 
    [SerializeField] private GameObject m_ThankMessage;

    /// <summary>
    /// Gets called when object is loaded
    /// Activate the start message coroutine
    /// </summary>
    void Awake() 
    {
        m_ThankMessage.SetActive(false);
        m_StartMessage.SetActive(true);
        StartCoroutine(ShowStartingMessage());
    }

    /// <summary>
    /// Show the starting message and thank you message.
    /// Then destroy the entire object because we no longer need that.
    /// </summary>
    /// <returns> IEnumerator </returns>
    private IEnumerator ShowStartingMessage() 
    {
        yield return new WaitForSeconds(4f);
        
        m_StartMessage.SetActive(false);
        m_ThankMessage.SetActive(true);

        yield return new WaitForSeconds(3f);

        Destroy(m_StartMessage);
        Destroy(m_ThankMessage);
        Destroy(m_Load);
    }
}
