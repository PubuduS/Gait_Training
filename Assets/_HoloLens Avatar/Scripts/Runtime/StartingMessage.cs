using System.Collections;
using UnityEngine;

public class StartingMessage : MonoBehaviour 
{

    //! Reference to Description GUI
    [SerializeField] private GameObject m_Load;

    //! Display the starting message to the user.
    [SerializeField] private GameObject m_StartMessage;

    //! Display a thank you note at the end of the starting message
    [SerializeField] private GameObject m_ThankMessage;


    //! Get called when the script instance is being loaded
    //! Activate the starting message 
    //! Start the coroutine to display the message to the user.
    private void Awake() 
    {

        m_ThankMessage.SetActive(false);
        m_StartMessage.SetActive(true);
        StartCoroutine(ShowStartingMessage());

    }

    //! Wait 4 seconds and deactivate the starting message
    //! After that it will activate the thank you note.
    //! Wait 3 seconds and destroy the GUI gameobject.
    //! We destroy it because we don't need that anymore.
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
