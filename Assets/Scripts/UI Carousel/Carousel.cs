using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class Carousel : MonoBehaviour, IEndDragHandler
{
    [Header("Parts Setup")]
    [SerializeField] private List<GameObject> entries = new List<GameObject>();

    [Space]
    [SerializeField] private ScrollRect scrollRect;

    [Space]
    [SerializeField] private RectTransform contentBoxHorizontal;
    [SerializeField] private GameObject carouselEntryPrefab;
    [SerializeField] private List<GameObject> classCharactersPanels = new List<GameObject>();

    [Space]
    [SerializeField] private Transform indicatorParent;
    [SerializeField] private CarouselIndicator indicatorPrefab;
    [SerializeField] private List<CarouselIndicator> _indicators = new List<CarouselIndicator>();

    [Header("Animation Setup")]
    [SerializeField, Range(0.25f, 1f)] private float duration = 0.5f;
    [SerializeField] private AnimationCurve easeCurve;

    [Header("Auto Scroll Setup")]
    [SerializeField] private bool autoScroll = false;
    [SerializeField] private float autoScrollInterval = 5f;
    private float _autoScrollTimer;


    [SerializeField] private int _currentIndex = 0;
    private Coroutine _scrollCoroutine;

    private void Reset()
    {
        scrollRect = GetComponentInChildren<ScrollRect>();
    }


    private void Start()
    {
        //foreach (var entry in entries)
        //{
        //    Image carouselEntry = Instantiate(carouselEntryPrefab, contentBoxHorizontal);
        //    carouselEntry.sprite = entry.EntryGraphic;
        //    _imagesForEntries.Add(carouselEntry);

        //    var indicator = Instantiate(indicatorPrefab, indicatorParent);
        //    indicator.Initialize(() => ScrollToSpecificIndex(entries.IndexOf(entry)));
        //    _indicators.Add(indicator);
        //}

        //_indicators[0].Activate(0.1f);

        for (int i = 0; i < _indicators.Count; i++)
        {
            int currentIndicatorIndex = i;
            _indicators[i].Initialize(() => ScrollToSpecificIndex(currentIndicatorIndex));
        }
        _autoScrollTimer = autoScrollInterval;
    }

    public void ActivateCurrentIndicatorByPlayerClass(PlayerClass playerClass)
    {
        if (playerClass == PlayerClass.tank)
        {
            _indicators[0].Activate(0.1f);
            ScrollToSpecificIndex(0); // Scroll to the tank panel if the player class is tank
        }
        else if (playerClass == PlayerClass.adc)
        {
            _indicators[1].Activate(0.1f);
            ScrollToSpecificIndex(1); // Scroll to the ADC panel if the player class is ADC
        }
        else if (playerClass == PlayerClass.mage)
        {
            _indicators[2].Activate(0.1f);
            ScrollToSpecificIndex(2); // Scroll to the mage panel if the player class is mage
        }
        else if (playerClass == PlayerClass.support)
        {
            _indicators[3].Activate(0.1f);
        }
        else if (playerClass == PlayerClass.assassin)
        {
            _indicators[4].Activate(0.1f);
        }
        else if (playerClass == PlayerClass.warrior)
        {
            _indicators[5].Activate(0.1f);
        }
    }

    private void ClearCurrentIndex()
    {
        _indicators[_currentIndex].Deactivate(duration);
    }

    private void ScrollToSpecificIndex(int index)
    {
        Debug.Log($"ScrollToSpecificIndex called with index: {index}");
        ClearCurrentIndex();

        ScrollTo(index);
    }

    public void ScrollToNext()
    {
        Debug.Log("ScrollToNext called");
        ClearCurrentIndex();

        _currentIndex = (_currentIndex + 1) % classCharactersPanels.Count;
        ScrollTo(_currentIndex);
    }

    public void ScrollToPrevious()
    {
        Debug.Log("ScrollToPrevious called");
        ClearCurrentIndex();

        _currentIndex = (_currentIndex - 1 + classCharactersPanels.Count) % classCharactersPanels.Count;
        ScrollTo(_currentIndex);
    }

    private void ScrollTo(int index)
    {
        _currentIndex = index;
        _autoScrollTimer = autoScrollInterval;
        float targetHorizontalPosition = (float)_currentIndex / (classCharactersPanels.Count - 1);

        if (_scrollCoroutine != null)
            StopCoroutine(_scrollCoroutine);

        _scrollCoroutine = StartCoroutine(LerpToPos(targetHorizontalPosition));

        _indicators[_currentIndex].Activate(duration);
    }

    private IEnumerator LerpToPos(float targetHorizontalPosition)
    {
        float elapsedTime = 0f;
        float initialPos = scrollRect.horizontalNormalizedPosition;

        if (duration > 0)
        {
            while (elapsedTime <= duration)
            {
                float easeValue = easeCurve.Evaluate(elapsedTime / duration);

                float newPosition = Mathf.Lerp(initialPos, targetHorizontalPosition, easeValue);

                scrollRect.horizontalNormalizedPosition = newPosition;

                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }

        scrollRect.horizontalNormalizedPosition = targetHorizontalPosition;
    }

    private void Update()
    {
        if (!autoScroll)
            return;

        _autoScrollTimer -= Time.deltaTime;
        if (_autoScrollTimer <= 0)
        {
            ScrollToNext();
            _autoScrollTimer = autoScrollInterval;
        }
    }

    public void OnEndDrag(PointerEventData data)
    {
        Debug.Log($"OnEndDrag called with delta: {data.delta}");
        if (data.delta.x != 0)
        {
            if (data.delta.x > 0)
                ScrollToPrevious();
            else if (data.delta.x < 0)
                ScrollToNext();
        }
        else
            ScrollToSpecificIndex(_currentIndex);
    }
}
