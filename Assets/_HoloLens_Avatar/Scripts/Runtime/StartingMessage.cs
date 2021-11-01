using System.Collections;
using UnityEngine;

public class StartingMessage : MonoBehaviour 
{

    [SerializeField] private GameObject m_Load;
    [SerializeField] private GameObject m_StartMessage;
    [SerializeField] private GameObject m_ThankMessage;

    void Awake() 
    {
        m_ThankMessage.SetActive(false);
        m_StartMessage.SetActive(true);
        StartCoroutine(ShowStartingMessage());
    }

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
