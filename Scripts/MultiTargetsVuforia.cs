using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using Vuforia;

public class MultiTargetsVuforia : MonoBehaviour
{
    [SerializeField] private GameObject startPlane;
    private int planesCount;
    private int indexCurrentPlane;
    private ObserverBehaviour mObserverBehaviour;

    void Start()
    {
        planesCount = transform.childCount;
        indexCurrentPlane = startPlane.transform.GetSiblingIndex();

        // Inicializar todos los planos y detener los videos
        for (int i = 0; i < planesCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
            transform.GetChild(i).GetComponent<VideoPlayer>().Stop();
        }

        mObserverBehaviour = GetComponent<ObserverBehaviour>();
        if (mObserverBehaviour)
        {
            mObserverBehaviour.OnTargetStatusChanged += OnTargetStatusChanged;
        }
    }

    private void OnDestroy()
    {
        if (mObserverBehaviour)
        {
            mObserverBehaviour.OnTargetStatusChanged -= OnTargetStatusChanged;
        }
    }

    private void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus targetStatus)
    {
        if (targetStatus.Status == Status.TRACKED || targetStatus.Status == Status.EXTENDED_TRACKED)
        {
            OnTrackingFound();
        }
        else
        {
            OnTrackingLost();
        }
    }

    private void OnTrackingFound()
    {
        // Activar el plano inicial y reproducir su video
        transform.GetChild(indexCurrentPlane).gameObject.SetActive(true);
        transform.GetChild(indexCurrentPlane).GetComponent<VideoPlayer>().Play();
    }

    private void OnTrackingLost()
    {
        // Detener el video y desactivar todos los planos
        for (int i = 0; i < planesCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
            transform.GetChild(i).GetComponent<VideoPlayer>().Stop();
        }
    }

    public void ChangeARModel(int index)
    {
        // Desactivar el plano actual y detener su video
        GameObject currentPlane = transform.GetChild(indexCurrentPlane).gameObject;
        currentPlane.SetActive(false);
        currentPlane.GetComponent<VideoPlayer>().Stop();

        int newIndex = indexCurrentPlane + index;

        if (newIndex < 0)
        {
            newIndex = planesCount - 1;
        }
        else if (newIndex > planesCount - 1)
        {
            newIndex = 0;
        }

        // Activar el nuevo plano y reproducir su video
        GameObject newPlane = transform.GetChild(newIndex).gameObject;
        newPlane.SetActive(true);
        newPlane.GetComponent<VideoPlayer>().Play();

        indexCurrentPlane = newPlane.transform.GetSiblingIndex();
    }
}
