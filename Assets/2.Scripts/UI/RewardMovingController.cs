using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Keiwando.BigInteger;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct RewardMetaData
{
    public CurrencyType type;
    public Sprite sprite;
    public Vector2 targetPosition;
    public RectTransform targetTransform;

    public RewardMetaData(CurrencyType currencyType, Sprite sprite, Vector2 targetPosition, RectTransform targetTransform)
    {
        type = currencyType;
        this.sprite = sprite;
        this.targetPosition = targetPosition;
        this.targetTransform = targetTransform;
    }
}

public class RewardMovingController : MonoBehaviourSingleton<RewardMovingController>
{
    [SerializeField] private Transform currencyIconPrefab;
    [SerializeField] private Transform currencyIconContainer;
    [SerializeField] private List<RewardMetaData> rewardMetaDatas;
    [SerializeField] private int initializePoolSize = 10;
    [SerializeField] private float speed = 0.5f;

    [SerializeField] private RewardMetaData inGameGoldRewardMetaData;

    private Queue<Transform> currencyIconPool = new Queue<Transform>();
    private Queue<Transform> currentActiveCurrency = new Queue<Transform>();
    private Vector2[] initPos;
    private Quaternion[] initRot;

    private Queue<Coroutine> movingCurrencyQueue = new Queue<Coroutine>();

    private void Awake()
    {
        initializePool();

        for (int i = 0; i < rewardMetaDatas.Count; i++)
        {
            RewardMetaData rewardMetaData = rewardMetaDatas[i];
            RectTransform rectTransform = rewardMetaData.targetTransform;
            Vector2 rectPos = rectTransform.position;
            rewardMetaDatas[i] = new RewardMetaData(rewardMetaData.type, rewardMetaData.sprite, rectPos, rectTransform);
        }
    }

    private void initializePool()
    {
        for (int i = 0; i < initializePoolSize; i++)
        {
            var icon = Instantiate(currencyIconPrefab, currencyIconContainer);
            icon.gameObject.SetActive(false);
            currencyIconPool.Enqueue(icon);
        }

        initPos = new Vector2[initializePoolSize];
        initRot = new Quaternion[initializePoolSize];

        for (int i = 0; i < initializePoolSize; i++)
        {
            Transform iconTransform = currencyIconPool.Dequeue();
            initPos[i] = iconTransform.GetComponent<RectTransform>().anchoredPosition;
            initRot[i] = iconTransform.GetComponent<RectTransform>().rotation;
            currencyIconPool.Enqueue(iconTransform);
        }
    }

    public void Init()
    {
        // while(movingCurrencyQueue.Count > 0)
        // {
        //     StopCoroutine(movingCurrencyQueue.Dequeue());
        // }
    }

    private void RemoveMovingEffects()
    {

    }

    public void RequestMovingCurrency(int count, CurrencyType type, Vector2 transform)
    {
        // MovingCurrency(count, type, transform);
    }

    public void RequestForgeRewardMovingCurrency(int currentCount, CurrencyType type)
    {
        ForgeRewardMovingCurrency(currentCount, type);
    }

    public void MovingCurrency(int currencyCount, Sprite sprite, Vector2 startPos, Vector2 endPos, Action<BigInteger> action, BigInteger amount)
    {
        currencyIconContainer.gameObject.SetActive(true);
        var delay = 0f;
        float maxHeight = 1.0f;  // Maximum height of the jump
        float spreadRadius = 1.0f;  // Radius for x-axis scattering at the ground
        float fallDuration = 1.0f;  // Duration to fall to the ground
        /*float bounceHeight = 0.5f;  // Height of the bounce after hitting the ground
        float bounceDuration = 0.3f;  // Duration of the bounce*/

        for (int i = 0; i < currencyCount; i++)
        {
            var icon = GetIconFromPool();
            icon.GetComponent<Image>().sprite = sprite;

            icon.gameObject.SetActive(true);
            RectTransform rectTransform = icon.GetComponent<RectTransform>();
            rectTransform.position = startPos;  // Start position from the given Transform

            // Random x offset for scattering on the ground
            float xOffset = UnityEngine.Random.Range(-spreadRadius, spreadRadius);

            // Calculate start, peak (highest point of the jump), ground, and final target positions
            Vector2 startPoint = startPos;
            Vector2 peakPosition = new Vector2(startPoint.x + xOffset / 2, startPoint.y + maxHeight);
            Vector2 groundPosition = new Vector2(startPoint.x + xOffset, startPoint.y);
            // Vector2 targetPosition = currencyData.targetPosition;  // Directly use the target position from metadata
            Vector2 targetPosition = endPos;
            // Sequence to animate falling, bouncing, and moving to target
            Sequence sequence = DOTween.Sequence();
            sequence.Append(rectTransform.DOMove(peakPosition, fallDuration / 2)
            .SetEase(Ease.OutQuad));  // Rise to peak
            sequence.Append(rectTransform.DOMove(targetPosition, fallDuration / 2.5f)
                .SetEase(Ease.InQuad));  // Fall to ground
            sequence.AppendInterval(0.5f);  // Pause at ground position
            // sequence.Append(rectTransform.DOMove(targetPosition, 1.0f)
            //     .SetEase(Ease.InOutQuad));  // Move to final target
            sequence.Join(rectTransform.DOScale(0, 0.3f).SetEase(Ease.InBack));  // Scale down and disappear

            sequence.OnComplete(() =>
            {
                icon.gameObject.SetActive(false);
                icon.transform.localScale = Vector3.one;  // Reset scale
                currentActiveCurrency.Dequeue();
                currencyIconPool.Enqueue(icon);
                action?.Invoke(amount / currencyCount);
            });

            delay += 0.1f;  // Staggered start for each icon
        }

        BigInteger rest = amount % currencyCount;
        action?.Invoke(rest);
    }

    public void MovingCurrency(int currencyCount, CurrencyType type, Vector2 startPos, BigInteger amount)
    {
        currencyIconContainer.gameObject.SetActive(true);
        var delay = 0f;
        float maxHeight = 1.0f;  // Maximum height of the jump
        float spreadRadius = 1.0f;  // Radius for x-axis scattering at the ground
        float fallDuration = 1.0f;  // Duration to fall to the ground
        /*float bounceHeight = 0.5f;  // Height of the bounce after hitting the ground
        float bounceDuration = 0.3f;  // Duration of the bounce*/

        for (int i = 0; i < currencyCount; i++)
        {
            var icon = GetIconFromPool();
            var currencyData = rewardMetaDatas.Find(data => data.type == type);
            icon.GetComponent<Image>().sprite = currencyData.sprite;

            icon.gameObject.SetActive(true);
            RectTransform rectTransform = icon.GetComponent<RectTransform>();
            rectTransform.position = startPos;  // Start position from the given Transform

            // Random x offset for scattering on the ground
            float xOffset = UnityEngine.Random.Range(-spreadRadius, spreadRadius);

            // Calculate start, peak (highest point of the jump), ground, and final target positions
            Vector2 startPoint = startPos;
            Vector2 peakPosition = new Vector2(startPoint.x + xOffset / 2, startPoint.y + maxHeight);
            Vector2 groundPosition = new Vector2(startPoint.x + xOffset, startPoint.y);
            // Vector2 targetPosition = currencyData.targetPosition;  // Directly use the target position from metadata
            Vector2 targetPosition = currencyData.targetPosition; 
            // Sequence to animate falling, bouncing, and moving to target
            Sequence sequence = DOTween.Sequence();
            sequence.Append(rectTransform.DOMove(peakPosition, fallDuration / 2)
            .SetEase(Ease.OutQuad));  // Rise to peak
            sequence.Append(rectTransform.DOMove(targetPosition, fallDuration / 2.5f)
                .SetEase(Ease.InQuad));  // Fall to ground
            sequence.AppendInterval(0.5f);  // Pause at ground position
            // sequence.Append(rectTransform.DOMove(targetPosition, 1.0f)
            //     .SetEase(Ease.InOutQuad));  // Move to final target
            sequence.Join(rectTransform.DOScale(0, 0.3f).SetEase(Ease.InBack));  // Scale down and disappear

            sequence.OnComplete(() =>
            {
                icon.gameObject.SetActive(false);
                icon.transform.localScale = Vector3.one;  // Reset scale
                currentActiveCurrency.Dequeue();
                currencyIconPool.Enqueue(icon);
                CurrencyManager.instance.TryUpdateCurrency(type, amount / currencyCount);
            });

            delay += 0.1f;  // Staggered start for each icon
        }

        BigInteger rest = amount % currencyCount;
        CurrencyManager.instance.TryUpdateCurrency(type, rest);
    }

    public void MoveCurrencyByCoroutine(int currencyCount, CurrencyType type, Vector2 startPos)
    {
        currencyIconContainer.gameObject.SetActive(true);

        for (int i = 0; i < currencyCount; i++)
        {
            var icon = GetIconFromPool();
            var currencyData = inGameGoldRewardMetaData;
            icon.GetComponent<Image>().sprite = currencyData.sprite;

            icon.gameObject.SetActive(true);
            RectTransform rectTransform = icon.GetComponent<RectTransform>();
            rectTransform.position = startPos;  // Start position from the given Transform

            Vector2 startPoint = startPos;

            movingCurrencyQueue.Enqueue(StartCoroutine(CoMoveCurrency(icon, rectTransform, currencyData.targetTransform)));
        }
    }

    private IEnumerator CoMoveCurrency(Transform icon, RectTransform startRect, RectTransform lastRect)
    {
        float ratio = 0f;
        Vector2 startPos = startRect.position;
        float totalValue = 0f;
        while (ratio < 1f)
        {
            totalValue += Time.deltaTime * speed;
            ratio += totalValue;
            Vector2 tempPos = Vector2.Lerp(startPos, lastRect.position, ratio);
            startRect.position = tempPos;
            yield return null;
        }

        icon.gameObject.SetActive(false);
        icon.transform.localScale = Vector3.one;  // Reset scale
        currentActiveCurrency.Dequeue();
        currencyIconPool.Enqueue(icon);
        StageManager.instance.EarnGold();
        movingCurrencyQueue.Dequeue();
    }

    public void ClearActiveCurrency()
    {
        foreach (var icon in currentActiveCurrency)
        {
            if (icon.gameObject.activeSelf)
            {
                icon.gameObject.SetActive(false);
                currencyIconPool.Enqueue(icon);
            }
        }
        currentActiveCurrency.Clear();
    }

    private void ForgeRewardMovingCurrency(int currencyCount, CurrencyType type)
    {
        currencyIconContainer.gameObject.SetActive(true);
        var delay = 0f;
        float maxHeight = 1.0f;  // Maximum height of the jump
        float spreadRadius = 1.5f;  // Radius for x-axis scattering at the ground
        float bounceHeight = 0.3f;  // Height of the bounce after hitting the ground
        float baseFallDuration = 0.8f;  // Base duration to fall to the ground
        float bounceDuration = 0.3f;  // Duration of the bounce

        for (int i = 0; i < currencyCount; i++)
        {
            var icon = GetIconFromPool();
            var currencyData = rewardMetaDatas.Find(data => data.type == type);
            icon.GetComponent<Image>().sprite = currencyData.sprite;

            icon.gameObject.SetActive(true);
            RectTransform rectTransform = icon.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = currencyIconContainer.position;

            // Random x offset for scattering on the ground
            float xOffset = UnityEngine.Random.Range(-spreadRadius, spreadRadius);
            // Randomize fall duration within a tolerance of 0.3 seconds
            float fallDuration = baseFallDuration + UnityEngine.Random.Range(-0.15f, 0.15f);

            // Calculate start, peak (highest point of the jump), ground, and bounce positions
            Vector2 startPoint = rectTransform.position;
            Vector2 peakPosition = new Vector2(startPoint.x + xOffset / 2, startPoint.y + maxHeight);
            Vector2 groundPosition = new Vector2(startPoint.x + xOffset, startPoint.y - 0.4f);
            Vector2 bouncePosition = new Vector2(startPoint.x + xOffset, startPoint.y + bounceHeight);

            // Sequence to animate falling, bouncing, and settling
            Sequence sequence = DOTween.Sequence();
            sequence.Append(rectTransform.DOMove(peakPosition, fallDuration / 2)
                .SetEase(Ease.OutQuad));  // Rise to peak
            sequence.Append(rectTransform.DOMove(groundPosition, fallDuration / 2.5f)
                .SetEase(Ease.InQuad));  // Fall to ground with randomized duration
            sequence.Append(rectTransform.DOMove(bouncePosition, bounceDuration / 2)
                .SetEase(Ease.OutQuad));  // Bounce up
            sequence.Append(rectTransform.DOMove(groundPosition, bounceDuration / 2)
                .SetEase(Ease.InQuad));  // Settle back to ground
            sequence.AppendInterval(0.3f);  // Pause at the ground
            sequence.Append(rectTransform.DOScale(0, 0.3f).SetEase(Ease.InBack));  // Disappear after landing

            sequence.OnComplete(() =>
            {
                icon.gameObject.SetActive(false);
                icon.transform.localScale = Vector3.one;  // Reset scale
                currentActiveCurrency.Dequeue();
                currencyIconPool.Enqueue(icon);
            });

            delay += 0.1f;  // Staggered start for each icon
        }
    }


    private Transform GetIconFromPool()
    {
        if (currencyIconPool.Count == 0)
        {
            var icon = Instantiate(currencyIconPrefab, currencyIconContainer);
            icon.gameObject.SetActive(false);

            currencyIconPool.Enqueue(icon);
            return icon;
        }

        var pooledIcon = currencyIconPool.Dequeue();

        currentActiveCurrency.Enqueue(pooledIcon);
        pooledIcon.gameObject.SetActive(true);

        return pooledIcon;
    }
}