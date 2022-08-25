using UnityEngine;
using System.Collections;

public class AnimatedGlove : MonoBehaviour
{
    public AnimationClip punchAnimation;
    public AnimationClip idleAnimation;
    private Animator myAnimator;
    private bool isPunching;

    #region Unity Methods

        private void Awake() {
            myAnimator = GetComponent<Animator>();
        }

        private void Start() {
            myAnimator.Play(idleAnimation.name);
        }
    #endregion

    #region Public Methods

    public void Punch() {
        StartCoroutine(StartPunchAnimation());
    }
    #endregion

    #region Private Methods

        private IEnumerator StartPunchAnimation() {
            myAnimator.Play(punchAnimation.name);
            yield return new WaitForSeconds(punchAnimation.length);
            myAnimator.Play(idleAnimation.name);
        }

    #endregion
}
