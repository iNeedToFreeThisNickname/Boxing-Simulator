using UnityEngine;
using DG.Tweening;
using System.Collections;

public class Player : MonoBehaviour, IBoxer
{
    [Header("Core fields")]
    [SerializeField] Opponent opponent;
    [SerializeField] HPController hpController;
    [Header("Visuals")]
    [SerializeField] AnimatedGlove leftGlove, rightGlove;
    [SerializeField] CanvasGroup blockState;
    [SerializeField] AnimationClip punchingAnimation;
    [SerializeField] AudioSource punchASource;
    [Header("Buttons")]
    [SerializeField] UnityEngine.UI.Button block;
    [SerializeField] UnityEngine.UI.Button rPunch;
    [SerializeField] UnityEngine.UI.Button lPunch;
    public float punchDamage = 0.15f;
    private bool knocked;
    private bool blockIsEnabled;
    private AnimatedGlove chosenPunch;

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
        
        }
    #endregion
    
    #region Public Methods
        public void ThrowPunch(bool isLeftPunch) {
            chosenPunch = isLeftPunch ? leftGlove : rightGlove;
            BlockButtonInteractability(false, isLeftPunch);
            chosenPunch.Punch();
            StartCoroutine(PunchDamageDelivery());
        }   


        public void CatchPunch(float receivingDamage) {
            if (!blockIsEnabled) {
                hpController.DecreaseHP(Boxers.Player, receivingDamage);
            }
        }

        public void HandleBothHands(bool isEnabled) {
            lPunch.interactable = isEnabled;
            rPunch.interactable = isEnabled;
        }
    #endregion

    #region Private Methods
        private IEnumerator PunchDamageDelivery() {
            StartCoroutine(DisableRepeatedShot());
            yield return new WaitForSeconds(punchingAnimation.length / 2);
            opponent.CatchPunch(punchDamage);
            punchASource.Play();
        }

        private void BlockButtonInteractability(bool isHandFree, bool blockLeft) {
            if (blockLeft) {
                lPunch.interactable = isHandFree;
            } else {
                rPunch.interactable = isHandFree;
            }
        }

        private IEnumerator DisableRepeatedShot() {
            yield return new WaitForSeconds(punchingAnimation.length);
            HandleBothHands(true);
        }
    #endregion
}

public interface IBoxer {
    public void ThrowPunch(bool isLeftPunch);
    public void CatchPunch(float receivingDamage);
}
