using UnityEngine;
using System.Collections;
using System;
using DG.Tweening;

public enum Boxers {
    Player,
    Opponent
}

public class HPController : MonoBehaviour
{
    [Header("Visual Components")]
    [SerializeField] UnityEngine.UI.Slider playerHP, opponentHP;
    [SerializeField] TMPro.TextMeshProUGUI countDownTMP;
    [SerializeField] CanvasGroup opponentHeadStars;
    [SerializeField] AnimationClip starsAnimation;
    [SerializeField] AudioSource gameAudio;
    [SerializeField] AudioClip winSound, looseSound;
    [Header("Game Entities")]
    [SerializeField] Player player;
    [SerializeField] Opponent opponent;
    [SerializeField] CanvasGroup countdownUI;
    [Header("Game Controlling Fiedlds")]
    [SerializeField] TMPro.TextMeshProUGUI result;
    [SerializeField] CanvasGroup restartButton;
    
    [Range(0.0f, 1.0f)] public float resurrectionChance;
    [Range(0.0f, 1.0f)] public float hpAmountAfterKO;
    public int stayingAmountOfSeconds;
    private int timerState;
    private System.Random rnd;
    private Coroutine countDownCoroutine;

    #region Unity Methods
        private void Start() {
            rnd = new System.Random();
        }
            
    #endregion

    #region Public Methods
        public void DecreaseHP(Boxers boxer, float damage) {
            switch (boxer) {
                case Boxers.Player: {
                    StartCoroutine(HandleHP(Boxers.Player, playerHP, damage));
                    break;
                }
                case Boxers.Opponent: {
                    StartCoroutine(HandleHP(Boxers.Opponent, opponentHP, damage));
                    break;
                }
            }
        }

        public void RefreshGame() {
            playerHP.value = 1;
            opponentHP.value = 1;
            
            timerState = 0;
            RefreshUI();
            opponent.RestartFighting();
            player.HandleBothHands(true);
        }
    #endregion

    #region Private Methods

        private IEnumerator HandleHP(Boxers pureGuy, UnityEngine.UI.Slider whoWasPunched, float receivedPunchDamage) {
            whoWasPunched.value -= receivedPunchDamage;

            if (whoWasPunched.value <= 0) {
                opponent.StopFighting();
                player.HandleBothHands(false);
                player.StopAllCoroutines();

                if (pureGuy == Boxers.Opponent) HandleHeadStarsAnimation(true);
                yield return new WaitForSecondsRealtime(stayingAmountOfSeconds);
                
                HandleCountDownUI(true);
                countDownCoroutine = StartCoroutine(CountdownKO(pureGuy, rnd.Next(0, 9)));
            }
        }

        private IEnumerator CountdownKO(Boxers forWho, int timeSpot) {
            while(timerState != 10) {
                timerState += 1;
                countDownTMP.SetText(timerState.ToString());
                
                if (timeSpot == timerState && forWho == Boxers.Opponent) {
                    if (StandUpAfterKO()) {
                        RefreshFighter(forWho);
                    }
                }

                yield return new WaitForSeconds(1);
            }

            CallEndGame(forWho);
            timerState = 0;
        }

        private bool StandUpAfterKO() {
            float chance = (float)rnd.Next(0, 100) / 100;
            Debug.Log(chance);

            return chance > resurrectionChance;
        }

        private void CallEndGame(Boxers looser) {
            result.GetComponent<CanvasGroup>().DOFade(1, 1f);
            restartButton.DOFade(1, 1f).OnComplete(() => {
                restartButton.interactable = true;
                restartButton.blocksRaycasts = true;
            });
            result.SetText(looser.ToString() + " Lost!" );
            if (looser == Boxers.Opponent) {
                gameAudio.PlayOneShot(winSound);
            } else {
                gameAudio.PlayOneShot(looseSound);
            }
        }

        private void HandleCountDownUI(bool isShown) {
            if (isShown) {
                countdownUI.DOFade(1, 0.5f).OnComplete(() => {
                    countdownUI.interactable = true;
                    countdownUI.blocksRaycasts = true;
                });
            } else {
                countdownUI.interactable = false;
                countdownUI.blocksRaycasts = false;
                countdownUI.DOFade(0, 0.5f);
            }
        }

        private void RefreshFighter(Boxers boxer) {
            StopAllCoroutines();
            //StopCoroutine(countDownCoroutine);
            HandleCountDownUI(false);
            timerState = 0;
            
            switch(boxer) {
                case Boxers.Player: {
                    playerHP.value = hpAmountAfterKO;
                    
                    break;
                }
                case Boxers.Opponent: {
                    opponentHP.value = hpAmountAfterKO;
                    HandleHeadStarsAnimation(false);
                    break;
                }
            }

            player.HandleBothHands(true);
            opponent.RestartFighting();
        }

        private void RefreshUI() {
            result.GetComponent<CanvasGroup>().DOFade(0, 1f);
            restartButton.DOFade(0, 1f).OnComplete(() => {
                restartButton.interactable = false;
                restartButton.blocksRaycasts = false;
            });

            HandleCountDownUI(false);
        }

        private void HandleHeadStarsAnimation(bool isPLaying) {
            if (isPLaying) {
                opponentHeadStars.GetComponent<Animator>().Play(starsAnimation.name);
                opponentHeadStars.DOFade(1, 1f);
            }
            else {
                opponentHeadStars.DOFade(0, 1f).OnComplete(() => {
                    opponentHeadStars.GetComponent<Animator>().StopPlayback();
                });
            }
        }   

    #endregion
}
