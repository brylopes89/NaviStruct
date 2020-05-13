using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace PolyPerfect
{
    public class AnimationDemo : MonoBehaviour
    {
        string currentAnimation = "";

        [SerializeField]
        private Animator avatarAnim;        

        private void Start()
        {
            avatarAnim = PuppetController.pc.avatarPlayer.GetComponent<Animator>();            
        }
        public void SetAnimation(string animationName)
        {
            if (currentAnimation != "")
            {
                avatarAnim.SetBool(currentAnimation, false);
            }
            avatarAnim.SetBool(animationName, true);
            currentAnimation = animationName;
        }

        public void SetAnimationIdle()
        {
            if (currentAnimation != "")
            {
                avatarAnim.SetBool(currentAnimation, false);
            }
        }
        public void SetDeathAnimation(int numOfClips)
        {
            int clipIndex = Random.Range(0, numOfClips);
            string animationName = "Death";
            Debug.Log(clipIndex);

            avatarAnim.SetInteger(animationName, clipIndex);
        }
    }
}