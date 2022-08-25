using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Opponent : MonoBehaviour, IBoxer
{
    [Header("Core fields")]
    [SerializeField] Player player;
    [SerializeField] HPController hpController;
    [Header("Visuals")]
    [SerializeField] AnimatedGlove leftGlove, rightGlove;
    [SerializeField] CanvasGroup blockState;
    [SerializeField] AnimationClip punchingAnimation;
    [SerializeField] AudioSource punchASource;
    public float punchDamage = 0.15f;
    public int minPunchDuration, maxPunchDuration;
    public int minBlockDuration, maxBlockDuration;
    public Coroutine fightCoroutine;
    private bool knocked = false;
    private bool blockIsEnabled;
    private AnimatedGlove chosenPunch;
    private System.Random rnd;

    public bool Block {
        get {
            return blockIsEnabled;
        }

        set {
            blockIsEnabled = value;

            if (blockIsEnabled) {
                blockState.DOFade(1, 1f);
            } else {
                blockState.DOFade(0, 1f);
            }
        }
    }

    public bool Knocked {
        get {
            return knocked;
        }
    }

    #region Unity Methods
        void Start()
        {
            rnd = new System.Random();
            //fightCoroutine = StartCoroutine(AutoFight());
            StartCoroutine(AutoBlock());
        }
    #endregion
    
    # region Public Methods
        public void ThrowPunch(bool isLeftPunch) {
                chosenPunch = isLeftPunch ? leftGlove : rightGlove;
                chosenPunch.Punch();
                StartCoroutine(PunchDamageDelivery());
            }   


        public void CatchPunch(float receivingDamage) {
            if (!blockIsEnabled) {
                hpController.DecreaseHP(Boxers.Opponent, receivingDamage);
            }
        }

        public void StopFighting() {
            StopAllCoroutines();
        }

        public void RestartFighting() {
            StartCoroutine(AutoBlock());
        }

    #endregion

    #region Private Methods
    
        private IEnumerator AutoFight() {
            while(!knocked && !player.Knocked && !blockIsEnabled) {
                yield return new WaitForSecondsRealtime(rnd.Next(minPunchDuration/2, maxPunchDuration/2));
                Debug.Log("Throwing punch at player");
                ThrowPunch(rnd.Next(0, 2) == 1);
                yield return new WaitForSecondsRealtime(rnd.Next(minPunchDuration, maxPunchDuration));
            }
        }

        private IEnumerator AutoBlock() {
            while(!knocked && !player.Knocked) {
                Block = false;
                fightCoroutine = StartCoroutine(AutoFight());
                yield return new WaitForSecondsRealtime(rnd.Next(minBlockDuration, maxBlockDuration));
                Block = true;
                StopCoroutine(AutoFight());
                yield return new WaitForSecondsRealtime(rnd.Next(minBlockDuration, maxBlockDuration));
            }
        }

        private IEnumerator PunchDamageDelivery() {
            yield return new WaitForSeconds(punchingAnimation.length / 2);
            player.CatchPunch(punchDamage);
            punchASource.Play();
        }
        
    #endregion
}
