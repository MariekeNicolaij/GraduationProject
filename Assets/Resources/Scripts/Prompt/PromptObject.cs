using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PromptObject : MonoBehaviour
{
    bool isDraggingToTheLeft, isTurned, isTouchingPrompt;
    Vector3 originalPromptPos;

    float maxDistanceFromOriginalPos = 0.2f;
    Vector3 screenPoint;

    float smooth = 3;

    float endAnimationTime = 0.25f;

    float maxRotationAngle = 45;
    float rotationPercentage = 0;
    float confirmationRotation = 50;

    public bool started, hasTouchedPromptOnce;


    public void CustomStart()
    {
        originalPromptPos = transform.position;
        started = true;
    }

    public void CustomEnd()
    {
        started = true;
    }

    void Update()
    {
        if (!started || Input.touchCount == 0)
            return;

        Touch touch = Input.GetTouch(0);
        if (touch.phase == TouchPhase.Began)
            BeginDrag(touch.position);
        if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
            Drag(touch.position);
        if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            EndDrag();
    }

    public void BeginDrag(Vector3 touchPosition)
    {
        RaycastHit hit;

        // If not touching a prompt then dont continue
        if (Physics.Raycast(Camera.main.ScreenPointToRay(touchPosition), out hit))
            if (hit.collider.tag != "Prompt")
                return;

        if (!hasTouchedPromptOnce)
            GameManager.instance.tutorialObject.SetActive(false); ;
        hasTouchedPromptOnce = true;

        isTouchingPrompt = true;
        screenPoint = Camera.main.WorldToScreenPoint(transform.position);
    }

    public void Drag(Vector3 touchPosition)
    {
        if (!isTouchingPrompt)
            return;
        // - - - - - Drag horizontally - - - - -

        Vector3 touchPoint = new Vector3(touchPosition.x, touchPosition.y, screenPoint.z);
        Vector3 touchPositionInWorld = Camera.main.ScreenToWorldPoint(touchPoint);
        touchPositionInWorld.y = transform.position.y; // Limit user to only drag horizontally

        // Drag
        transform.position = Vector3.Lerp(transform.position, touchPositionInWorld, Time.deltaTime * smooth);
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, (isTurned) ? -1 : 1); // Not allowed to drag in depth

        // Check if it hit maxDistance, if so limit position
        float distanceFromOriginal = Vector3.Distance(transform.position, originalPromptPos);
        if (distanceFromOriginal > maxDistanceFromOriginalPos)
            transform.position += (originalPromptPos - transform.position).normalized * (distanceFromOriginal - maxDistanceFromOriginalPos);

        // - - - - - Drag rotation animation - - - - -

        rotationPercentage = distanceFromOriginal / maxDistanceFromOriginalPos * 100;
        if (rotationPercentage > 100)
            rotationPercentage = 100;

        float rotationPercentageNormalized = rotationPercentage / 100;
        isDraggingToTheLeft = (originalPromptPos - transform.position).normalized.x < 0;

        float angle = (isDraggingToTheLeft) ? Mathf.LerpAngle((isTurned) ? -270 : -90, (isTurned) ? 90 + maxRotationAngle : -180 + maxRotationAngle, rotationPercentageNormalized) :
            Mathf.LerpAngle((isTurned) ? -270 : -90, (isTurned) ? 90 - maxRotationAngle : 0 - maxRotationAngle, rotationPercentageNormalized);
        transform.localEulerAngles = new Vector3((isTurned) ? -angle : angle, 90, (isTurned) ? 90 : -90);

        // Enable/disable option text
        if (rotationPercentage > confirmationRotation && !isTurned)
        {
            PromptManager.instance.option1Text.enabled = isDraggingToTheLeft;
            PromptManager.instance.option2Text.enabled = !isDraggingToTheLeft;
        }
        else
        {
            PromptManager.instance.option1Text.enabled = false;
            PromptManager.instance.option2Text.enabled = false;
        }
    }

    public void EndDrag()
    {
        if (!isTouchingPrompt)
            return;

        isTouchingPrompt = false;

        PromptManager.instance.option1Text.enabled = false;
        PromptManager.instance.option2Text.enabled = false;

        if (isTurned)
        {
            if (rotationPercentage > confirmationRotation)
            {
                PromptManager.instance.NextPrompt();
                isTurned = false;
            }
            return;
        }

        if (isDraggingToTheLeft && rotationPercentage > confirmationRotation)
        {
            StartCoroutine(LerpPromptBackToOriginalPosition(endAnimationTime, true, true));
            isTurned = true;
        }
        else if (!isDraggingToTheLeft && rotationPercentage > confirmationRotation)
        {
            StartCoroutine(LerpPromptBackToOriginalPosition(endAnimationTime, true, false));
            isTurned = true;
        }
        else
            StartCoroutine(LerpPromptBackToOriginalPosition(endAnimationTime, false, false));
    }

    IEnumerator LerpPromptBackToOriginalPosition(float animationTime, bool turned, bool swipedLeft)
    {
        float startTime = 0;

        Vector3 pos = transform.position;
        float currentAngle = transform.localEulerAngles.x;
        float angle = 0;

        while (startTime < animationTime)
        {
            transform.position = Vector3.Lerp(pos, originalPromptPos, (startTime / animationTime));

            angle = Mathf.LerpAngle(currentAngle, -90, (startTime / animationTime));
            transform.localEulerAngles = new Vector3(angle, transform.localEulerAngles.y, transform.localEulerAngles.z);

            startTime += Time.deltaTime;

            yield return null;
        }

        transform.position = originalPromptPos;
        transform.localEulerAngles = new Vector3(-90, transform.localEulerAngles.y, transform.localEulerAngles.z);

        if (turned)
            PromptManager.instance.TurnPrompt(swipedLeft);
    }

    // Cursor input has to on prompt object
    void OnMouseDown()
    {
        BeginDrag(Input.mousePosition);
    }

    void OnMouseDrag()
    {
        Drag(Input.mousePosition);
    }

    void OnMouseUp()
    {
        EndDrag();
    }
}
