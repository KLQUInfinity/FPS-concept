using Photon.Voice.Unity;
using UnityEngine;
using UnityEngine.UI;


namespace Photon.Voice.DemoVoiceUI
{
    [RequireComponent(typeof(Speaker))]
    public class RemoteSpeakerUI : MonoBehaviour
    {
        [SerializeField]
        private Text nameText;
        [SerializeField]
        private Image remoteIsMuting;
        [SerializeField]
        private Image remoteIsTalking;

<<<<<<< HEAD
        private Speaker speaker;

        private void Start()
        {
            this.nameText = this.GetComponentInChildren<Text>();
            this.speaker = this.GetComponent<Speaker>();
            string nick = this.speaker.name;
            if (this.speaker.Actor != null)
            {
                nick = this.speaker.Actor.NickName;
                if (string.IsNullOrEmpty(nick))
                {
                    nick = string.Concat("user ", this.speaker.Actor.ActorNumber);
                }
            }
            this.nameText.text = nick;
        }

        void Update()
        {
            object mutedValue;
            if (this.speaker.Actor != null && this.speaker.Actor.CustomProperties.TryGetValue(DemoVoiceUI.MutePropKey, out mutedValue))
            {
                this.remoteIsMuting.enabled = (bool)mutedValue;
            }
            // TODO: It would be nice, if we could show if a user is actually talking right now (Voice Detection)
            this.remoteIsTalking.enabled = this.speaker.IsPlaying;
=======
        private void Start()
        {
            this.nameText = this.GetComponentInChildren<Text>();
        }

        void Update()
        {
            Speaker speaker = this.GetComponent<Speaker>();
            if (speaker.Actor != null)
            {
                string nick = speaker.Actor.NickName;
                if (string.IsNullOrEmpty(nick))
                {
                    nick = string.Concat("user ", speaker.Actor.ActorNumber);
                }
                this.nameText.text = nick;


                if (this.remoteIsMuting != null)
                {
                    bool? muted = speaker.Actor.CustomProperties[DemoVoiceUI.MutePropKey] as bool?;
                    if (muted != null) this.remoteIsMuting.enabled = (bool)muted;
                }

                // TODO: It would be nice, if we could show if a user is actually talking right now (Voice Detection)
                if (this.remoteIsTalking != null)
                {
                    this.remoteIsTalking.enabled = speaker.IsPlaying;
                }
            }
            else
            {
                this.nameText.text = speaker.name;
            }
            //transmitToggle.isOn = speaker.IsPlaying;
>>>>>>> 52cc1095d5af37dd053c569c923f456649f75dfe
        }
    }
}