using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using System;

public class SlotController : MonoBehaviour
{


    [Header("Sprites")]
    [SerializeField] private Sprite[] iconImages;

    [Header("Slot Images")]
    [SerializeField] private List<SlotImage> slotMatrix;
    [SerializeField] private GlowMatrix[] glowMatrix;
    [SerializeField] private List<SlotIconView> extraIconForAnim;
    [SerializeField] internal GameObject disableIconsPanel;


    [Header("Slots Transforms")]
    [SerializeField] private Transform[] Slot_Transform;



    [Header("tween properties")]
    [SerializeField] private int tweenHeight = 0;
    [SerializeField] private float initialPos;



    private List<Tweener> alltweens = new List<Tweener>();

    private Tweener WinTween = null;

    [SerializeField]
    private List<ImageAnimation> TempList;  //stores the sprites whose animation is running at present 

    [SerializeField] private ImageAnimation[] VHObjectsBlue;
    [SerializeField] private ImageAnimation[] VHObjectsRed;

    [SerializeField] private ImageAnimation[] reel_border;
    [SerializeField] internal List<SlotIconView> animatedIcons = new List<SlotIconView>();

    [SerializeField] internal List<List<int>> freezeIndex = new List<List<int>>();

    internal IEnumerator StartSpin(bool turboMode)
    {

        for (int i = 0; i < Slot_Transform.Length; i++)
        {
            InitializeTweening(Slot_Transform[i], turboMode);
            if (!GameManager.immediateStop)
                yield return new WaitForSeconds(0.1f);
        }
        if (GameManager.immediateStop)
            yield return new WaitForSeconds(0.35f);


    }

    internal void PopulateSLotMatrix(List<List<int>> resultData)
    {

        for (int j = 0; j < slotMatrix[0].slotImages.Count; j++)
        {
            for (int i = 0; i < slotMatrix.Count; i++)
            {

                slotMatrix[i].slotImages[j].iconImage.sprite = iconImages[resultData[j][i]];
                if (resultData[j][i] == 15 || resultData[j][i] == 12 || resultData[j][i] == 13 || resultData[j][i] == 14)
                {
                    slotMatrix[i].slotImages[j].bgGlow.gameObject.SetActive(true);
                    slotMatrix[i].slotImages[j].bgGlow.StartAnimation();
                    animatedIcons.Add(slotMatrix[i].slotImages[j]);
                }

            }
        }
        for (int i = 0; i < extraIconForAnim.Count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, 10);
            extraIconForAnim[i].iconImage.sprite = iconImages[randomIndex];
        }
    }
    internal IEnumerator StopSpin(float delay1 = 0, float delay2 = 0, bool isFreeSpin = false, bool turboMode = false, Action<bool> playReelGlowSound = null)
    {

        DeActivateReelBorder();
        for (int i = 0; i < Slot_Transform.Length; i++)
        {
            //for glow aniamtion in 2nd reel
            if (delay1 > 0 && i == 2 && !isFreeSpin && !GameManager.immediateStop)
            {
                ActivateReelBorder("red");
                playReelGlowSound?.Invoke(true);
                yield return new WaitForSeconds(delay1);
            }

            //for glow aniamtion in 5th reel
            if (delay2 > 0 && i == 4 && !isFreeSpin && !GameManager.immediateStop)
            {
                playReelGlowSound?.Invoke(false);
                DeActivateReelBorder();
                ActivateReelBorder("blue");
                playReelGlowSound?.Invoke(true);
                yield return new WaitForSeconds(delay2);
            }

            StopTweening(Slot_Transform[i], i, turboMode, GameManager.immediateStop);
            playReelGlowSound?.Invoke(false);

            if (!GameManager.immediateStop)
            {
                if (turboMode)
                    yield return new WaitForSeconds(0.125f);
                else
                    yield return new WaitForSeconds(0.25f);
            }
        }
        if (GameManager.immediateStop)
            yield return new WaitForSeconds(0.25f);

        // playReelGlowSound?.Invoke(false);

        KillAllTweens();
        DeActivateReelBorder();

    }

    internal float[] CalculateDelay(List<List<int>> resultData)
    {
        float[] delay = new float[] { 0, 0 };

        for (int i = 0; i < resultData.Count; i++)
        {
            if (resultData[i][1] == 12)
                delay[0] = 1f;
            else if (resultData[i][3] == 14)
                delay[1] = 1f;

        }

        return delay;
    }


    internal void shuffleInitialMatrix()
    {
        for (int i = 0; i < slotMatrix.Count; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                int randomIndex = UnityEngine.Random.Range(0, 10);
                slotMatrix[i].slotImages[j].iconImage.sprite = iconImages[randomIndex];
                slotMatrix[i].slotImages[j].pos = (i * 10 + j);
            }
        }
    }



    internal void StartIconBlastAnimation(List<int> pos, bool opposite = false)
    {
        // IconController tempIcon; 
        for (int j = 0; j < pos.Count; j++)
        {
            SlotIconView tempIcon;
            // int[] pos = iconPos[j].Split(',').Select(int.Parse).ToArray();
            if (opposite)
                tempIcon = slotMatrix[pos[1]].slotImages[pos[0]];
            else
                tempIcon = slotMatrix[pos[0]].slotImages[pos[1]];

            tempIcon.blastAnim.SetActive(true);
            tempIcon.blastAnim.transform.DOScale(new Vector2(1.1f, 1.1f), 0.35f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                tempIcon.blastAnim.SetActive(false);
                tempIcon.frontBorder.SetActive(true);
            });
            tempIcon.transform.SetParent(disableIconsPanel.transform.parent);
            if (!animatedIcons.Any(icon => icon.pos == tempIcon.pos))
                animatedIcons.Add(tempIcon);
        }

    }
    internal void StartIconBlastAnimation(List<List<int>> pos, bool opposite = false)
    {
        // IconController tempIcon; 
        for (int j = 0; j < pos.Count; j++)
        {
            SlotIconView tempIcon;
            // int[] pos = iconPos[j].Split(',').Select(int.Parse).ToArray();

            if (opposite)
                tempIcon = slotMatrix[pos[j][1]].slotImages[pos[j][0]];
            else
                tempIcon = slotMatrix[pos[j][0]].slotImages[pos[j][1]];

            tempIcon.blastAnim.SetActive(true);
            tempIcon.blastAnim.transform.DOScale(new Vector2(1.1f, 1.1f), 0.35f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                tempIcon.blastAnim.SetActive(false);
                tempIcon.frontBorder.SetActive(true);
            });
            tempIcon.transform.SetParent(disableIconsPanel.transform.parent);
            if (!animatedIcons.Any(icon => icon.pos == tempIcon.pos))
                animatedIcons.Add(tempIcon);
        }

    }
    internal void StartIconBlastAnimation(List<string> iconPos, bool opposite = false)
    {
        // IconController tempIcon; 
        for (int j = 0; j < iconPos.Count; j++)
        {

            SlotIconView tempIcon;
            int[] pos = iconPos[j].Split(',').Select(int.Parse).ToArray();

            if (opposite)
                tempIcon = slotMatrix[pos[1]].slotImages[pos[0]];
            else
                tempIcon = slotMatrix[pos[0]].slotImages[pos[1]];

            tempIcon.blastAnim.SetActive(true);
            tempIcon.blastAnim.transform.DOScale(new Vector2(1.1f, 1.1f), 0.35f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                tempIcon.blastAnim.SetActive(false);
                tempIcon.frontBorder.SetActive(true);
            });
            tempIcon.transform.SetParent(disableIconsPanel.transform.parent);
            if (!animatedIcons.Any(icon => icon.pos == tempIcon.pos))
                animatedIcons.Add(tempIcon);
        }

    }

    internal void ShowOnlyIcons(List<List<int>> pos, bool activateFrontBorder = false)
    {

        for (int j = 0; j < pos.Count; j++)
        {
            // int[] pos = iconPos[j].Split(',').Select(int.Parse).ToArray();
            SlotIconView tempIcon = slotMatrix[pos[j][0]].slotImages[pos[j][1]];
            if (activateFrontBorder)
                tempIcon.frontBorder.SetActive(true);
            tempIcon.transform.SetParent(disableIconsPanel.transform.parent);
            if (!animatedIcons.Any(icon => icon.pos == tempIcon.pos))
                animatedIcons.Add(tempIcon);
        }
    }

    internal void ShowOnlyIcons(List<string> iconPos, bool activateFrontBorder = false)
    {

        for (int j = 0; j < iconPos.Count; j++)
        {
            int[] pos = iconPos[j].Split(',').Select(int.Parse).ToArray();
            SlotIconView tempIcon = slotMatrix[pos[0]].slotImages[pos[1]];
            if (activateFrontBorder)
                tempIcon.frontBorder.SetActive(true);
            tempIcon.transform.SetParent(disableIconsPanel.transform.parent);
            if (!animatedIcons.Any(icon => icon.pos == tempIcon.pos))
                animatedIcons.Add(tempIcon);
        }
    }
    internal void StopIconBlastAnimation()
    {


        foreach (var item in animatedIcons)
        {
            item.blastAnim.SetActive(false);
            item.blastAnim.transform.localScale *= 0;
            // item.frontBorder.SetActive(false);
            item.transform.SetParent(item.parent);

        }
        // animatedIcons.Clear();
    }
    internal void StartIconAnimation(List<string> iconPos)
    {
        SlotIconView tempIcon = null;
        for (int j = 0; j < iconPos.Count; j++)
        {

            int[] pos = iconPos[j].Split(',').Select(int.Parse).ToArray();
            tempIcon = slotMatrix[pos[0]].slotImages[pos[1]];
            tempIcon.frontBorder.SetActive(true);
            tempIcon.transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0), 0.3f, 0, 0.3f);
            tempIcon.transform.SetParent(disableIconsPanel.transform.parent);
            if (!animatedIcons.Any(icon => icon.pos == tempIcon.pos))
                animatedIcons.Add(tempIcon);
        }

    }

    internal void ShowWildAndBloodANimation(List<string> iconPos, bool turbo)
    {

        for (int j = 0; j < iconPos.Count; j++)
        {
            int[] pos = iconPos[j].Split(',').Select(int.Parse).ToArray();
            SlotIconView tempIcon = slotMatrix[pos[1]].slotImages[pos[0]];
            tempIcon.bloodSplatter.transform.localScale = Vector3.zero;
            tempIcon.bloodSplatter.gameObject.SetActive(true);
            tempIcon.bloodSplatter.transform.DOScale(Vector3.one, turbo ? 0.3f : 0.5f).OnComplete(() =>
            {
                tempIcon.wildObject.transform.localScale = Vector3.one;
                tempIcon.wildObject.SetActive(true);
                tempIcon.wildObject.transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0), turbo ? 0.2f : 0.3f, 0, 0.3f);
            }).SetEase(Ease.OutExpo);
            if (!animatedIcons.Any(icon => icon.pos == tempIcon.pos))
                animatedIcons.Add(tempIcon);
        }

    }

    internal void OnlyChangeParent()
    {
        foreach (var item in animatedIcons)
        {
            item.frontBorder.SetActive(false);
            item.transform.localScale = Vector3.one;
            item.transform.SetParent(item.parent);
            item.transform.SetSiblingIndex(item.siblingIndex);
        }
    }
    internal void StopIconAnimation()
    {

        foreach (var item in animatedIcons)
        {
            item.frontBorder.SetActive(false);
            item.transform.localScale = Vector3.one;
            item.transform.SetParent(item.parent);
            item.transform.localPosition = item.defaultPos;
            item.transform.SetSiblingIndex(item.siblingIndex);
            // item.bloodSplatter.transform.localScale = Vector3.one; 
            item.bloodSplatter.gameObject.SetActive(false);
            item.wildObject.SetActive(false);
            item.bgGlow.gameObject.SetActive(false);
            item.bgGlow.StopAnimation();

        }
        animatedIcons.Clear();
    }


    void ActivateReelBorder(string type)
    {
        if (type == "red")
        {
            reel_border[0].gameObject.SetActive(true);
            reel_border[0].StartAnimation();


        }
        else if (type == "blue")
        {
            reel_border[1].gameObject.SetActive(true);
            reel_border[1].StartAnimation();
        }

    }

    internal void DeActivateReelBorder()
    {
        for (int i = 0; i < reel_border.Length; i++)
        {
            reel_border[i].gameObject.SetActive(false);
            reel_border[i].StopAnimation();
        }


    }
    internal void FreeSpinVHAnim(List<string> pos, ref List<ImageAnimation> VHcombo)
    {
        for (int i = 0; i < pos.Count; i++)
        {
            if (i % 2 != 0) continue;

            int[] iconPos = pos[i].Split(',').Select(int.Parse).ToArray();
            if (iconPos[1] == 1)
            {
                VHObjectsRed[iconPos[0]].gameObject.SetActive(true);
                VHObjectsRed[iconPos[0]].StartAnimation();
                VHObjectsRed[iconPos[0]].transform.DOPunchScale(new Vector3(0.4f, 0.4f, 0), 0.3f, 0, 1.2f);
                VHcombo.Add(VHObjectsRed[iconPos[0]]);
            }
            else if (iconPos[1] == 3)
            {
                VHObjectsBlue[iconPos[0]].gameObject.SetActive(true);
                VHObjectsBlue[iconPos[0]].StartAnimation();
                VHObjectsBlue[iconPos[0]].transform.DOPunchScale(new Vector3(0.4f, 0.4f, 0), 0.3f, 0, 1.2f);
                VHcombo.Add(VHObjectsBlue[iconPos[0]]);

            }

        }

    }

    internal void IconShakeAnim(List<string> vhPos)
    {
        int[] pos;
        for (int i = 0; i < vhPos.Count; i++)
        {
            pos = vhPos[i].Split(',').Select(int.Parse).ToArray();
            slotMatrix[pos[1]].slotImages[pos[0]].transform.DOShakePosition(1f, strength: new Vector3(25, 25, 0), vibrato: 20, randomness: 90, fadeOut: true);

        }
    }

    private void InitializeTweening(Transform slotTransform, bool turboMode)
    {
        Tweener tweener = null;
        float delay = 0.3f;
        if (turboMode)
            delay /= 2;
        tweener = slotTransform.DOLocalMoveY(-tweenHeight + 350, delay).SetEase(Ease.InBack).OnComplete(() =>
        {
            // tweener.Pause();
            tweener.Kill();
            slotTransform.localPosition = new Vector2(slotTransform.localPosition.x, -835f);
            tweener = slotTransform.DOLocalMoveY(-tweenHeight + 350, delay).SetLoops(-1, LoopType.Restart).SetDelay(0).SetEase(Ease.Linear);
            alltweens.Add(tweener);

        });
        // tweener.Play();
    }

    private void StopTweening(Transform slotTransform, int index, bool turboMode, bool immediateStop)
    {
        float delay = 0.25f;
        if (turboMode)
            delay /= 2;
        if (immediateStop)
            delay = 0;

        alltweens[index]?.Pause();
        slotTransform.localPosition = new Vector2(slotTransform.localPosition.x, initialPos + 500);
        alltweens[index] = slotTransform.DOLocalMoveY(initialPos, delay).SetEase(Ease.OutQuad);

    }


    private void KillAllTweens()
    {
        for (int i = 0; i < alltweens.Count; i++)
        {
            alltweens[i].Kill();
        }
        alltweens.Clear();

    }

}

[Serializable]
public class SlotImage
{
    public List<SlotIconView> slotImages = new List<SlotIconView>(10);
}

[Serializable]
public class GlowMatrix
{
    public List<ImageAnimation> row = new List<ImageAnimation>();
}

