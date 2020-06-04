// ----------------------------------------------------------------------------
// <copyright file="VoiceDemoUI.cs" company="Exit Games GmbH">
// Photon Voice Demo for PUN - Copyright (C) Exit Games GmbH
// </copyright>
// <summary>
// UI manager class for the PUN Voice Demo
// </summary>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------

using System;
using Photon.Voice.Unity;
using Photon.Voice.PUN;

#pragma warning disable 0649 // Field is never assigned to, and will always have its default value

namespace ExitGames.Demos.DemoPunVoice
{
    using Photon.Pun;
    using UnityEngine;
    using UnityEngine.UI;
    using Client.Photon;

#if !UNITY_EDITOR && UNITY_PS4
    using Sony.NP;
#endif

    public class VoiceDemoUI : MonoBehaviour
    {

        public GameObject teamUi;

        PhotonVoiceNetwork punVoiceNetwork;
       

        
        private int calibrationMilliSeconds = 2000;

        private void Awake()
        {
            this.punVoiceNetwork = PhotonVoiceNetwork.Instance;
        }

        private void OnEnable()
        {
            this.punVoiceNetwork.Client.StateChanged += this.VoiceClientStateChanged;
            PhotonNetwork.NetworkingClient.StateChanged += this.PunClientStateChanged;
        }

        private void OnDisable()
        {
            this.punVoiceNetwork.Client.StateChanged -= this.VoiceClientStateChanged;
            PhotonNetwork.NetworkingClient.StateChanged -= this.PunClientStateChanged;
        }

     
        private void Start()
        {


        }

     

    

        private void PunClientStateChanged(Photon.Realtime.ClientState fromState, Photon.Realtime.ClientState toState)
        {
            switch (toState)
            {
                case Photon.Realtime.ClientState.PeerCreated:
                case Photon.Realtime.ClientState.Disconnected:
                     break;
                case Photon.Realtime.ClientState.Joined:
                     break;
                default:
                    break;
            }
            this.UpdateUiBasedOnVoiceState(this.punVoiceNetwork.ClientState);
        }

        private void VoiceClientStateChanged(Photon.Realtime.ClientState fromState, Photon.Realtime.ClientState toState)
        {
            this.UpdateUiBasedOnVoiceState(toState);
        }

        private void UpdateUiBasedOnVoiceState(Photon.Realtime.ClientState voiceClientState)
        {
            switch (voiceClientState)
            {
                case Photon.Realtime.ClientState.Joined:
                    teamUi.SetActive(true);
                break;
                            case Photon.Realtime.ClientState.PeerCreated:

                case Photon.Realtime.ClientState.Disconnected:
                    if (PhotonNetwork.InRoom)
                    {
                    }
                    else
                    {
                   }
                  break;
                default:
                    break;
            }
        }
    }



}