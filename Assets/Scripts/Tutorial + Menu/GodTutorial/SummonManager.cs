﻿using UnityEngine;
using System.Collections;
using System;

public class SummonManager : MonoBehaviour
{

    private float cameraSpeed = 2f;
    private float handSpeed = 2f;

    public GameObject GhostHand;
    public GameObject GhostCrystal;

    private Vector3 currentGhostPos;
    private Vector3 ghostEndPos;

    public GameObject UI;
    public GameObject Instructions;

    private float duration;
    private RectTransform rectTransform;
    private Vector2 textStartPosition, textEndPosition;
    private Coroutine TextCoroutine;

    private bool waitingDone;
    public bool summonOne;
    public bool summonTwo;
    public bool summonThree;

    // Use this for initialization
    void Start()
    {
        UI.SetActive(true); // show main infos what to do in the beginning
        rectTransform = Instructions.GetComponent<RectTransform>();
        textStartPosition = rectTransform.anchoredPosition;
        textEndPosition = new Vector2(textStartPosition.x, -210);
        duration = 4f;
        waitingDone = false;;
        StartCoroutine(Wait());

        ghostEndPos = new Vector3(currentGhostPos.x, 7, currentGhostPos.z);
    }

    // Update is called once per frame
    void Update()
    {
        moveHand();

        if (waitingDone == true)
        {
            MoveCameraForward();
        }

        if (summonOne && summonTwo && summonThree) 
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
        //else
        //{
        //    UItext.SetActive(true);
        //}
    }



    IEnumerator Wait()
    {
        Debug.Log("waiting?");
        yield return new WaitForSeconds(4f);
        waitingDone = true;
        TextCoroutine = StartCoroutine(ShowText());
    }


    private void moveHand()
    {
        currentGhostPos = GhostHand.transform.position;

        if (currentGhostPos.y < ghostEndPos.y)
        { 
            currentGhostPos.y += handSpeed * Time.deltaTime;
            GhostCrystal.SetActive(false);
        }

        
        else if (currentGhostPos.y > ghostEndPos.y)
        {
            currentGhostPos.y = 0;
            GhostCrystal.SetActive(false);
        }
        else
        {

        }

        if (currentGhostPos.y > ghostEndPos.y - 2)
        {
            //currentGhostPos.y = 0;
            GhostCrystal.SetActive(true);
        }
        GhostHand.transform.position = currentGhostPos;

    }
}
