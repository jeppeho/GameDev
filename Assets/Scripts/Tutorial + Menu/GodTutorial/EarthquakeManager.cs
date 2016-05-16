using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EarthquakeManager : MonoBehaviour {

    private float cameraSpeed = 2f;

    public GameObject UI;
    public GameObject Instructions;
    public Text UICounter;

    private GestureGust gestureGust;

    private float duration;
    private RectTransform rectTransform;
    private Vector2 textStartPosition, textEndPosition;
    private Coroutine TextCoroutine;

    private bool waitingDone;
    private bool prevRelease = false;
    private int earthquakeCounter;

    // Use this for initialization
    void Start()
    {
        UI.SetActive(true); // show main infos what to do in the beginning
        UICounter.gameObject.SetActive(false);
        rectTransform = Instructions.GetComponent<RectTransform>();
        textStartPosition = rectTransform.anchoredPosition;
        textEndPosition = new Vector2(textStartPosition.x, -210);
        duration = 4f;
        waitingDone = false;
        StartCoroutine(Wait());
        earthquakeCounter = 0;
        UICounter.text = "Count: " + earthquakeCounter.ToString();
        GameObject g = GameObject.Find("HandController");
        gestureGust = g.GetComponent<GestureGust>();
        
        
    }

    // Update is called once per frame
    void Update()
    {
        UICounter.text = earthquakeCounter.ToString();
         Debug.Log(earthquakeCounter);
        if (waitingDone == true)
        {
            MoveCameraForward();
        }

        if (gestureGust.released != prevRelease)
        {
            prevRelease = gestureGust.released;
             if(gestureGust.released == true)
               { 
                earthquakeCounter++;
                 }
        }
        if (earthquakeCounter == 3)
        {

            float fadeTime = GameObject.Find("FadingScenes").GetComponent<FadingScenes>().BeginFade(1);
            Invoke("ChangeLevel", 3.0f);
        }

    }

    private void ChangeLevel()
    {
        Application.LoadLevel("TutorialGod");
    }

    private IEnumerator ShowText()
    {
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration; //0 means the animation just started, 1 means it finished
            rectTransform.anchoredPosition = Vector2.Lerp(textStartPosition, textEndPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private void MoveCameraForward()
    {

        Vector3 currentPosition = this.gameObject.transform.position;

        if (currentPosition.z < -0)
        {
            currentPosition.z += cameraSpeed * Time.deltaTime;

            this.gameObject.transform.position = currentPosition;
        }
        else
        {
            UICounter.gameObject.SetActive(true);
        }
    }


    IEnumerator Wait()
    {
        Debug.Log("waiting?");
        yield return new WaitForSeconds(4f);
        waitingDone = true;
        TextCoroutine = StartCoroutine(ShowText());
    }
}
