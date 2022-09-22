using System;
using System.Linq;
using Adic;
using Assets.Scripts.Model;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class MessagesController : BaseBehaviour
    {
        #region <| Dependencies |>

        [Inject]
        private IGameState _gameState;

        [Inject("messages-text")]
        private GameObject _messagesText;

        #endregion

        #region <| Start |>

        public override async Task Start()
        {
            await base.Start();

            _gameState.MessagesShouldBeUpdated.AddListener(OnMessagesShouldBeUpdated);
        }

        #endregion

        #region <| Event Handlers |>

        private void OnMessagesShouldBeUpdated(ActiveMessages messages)
        {
            _messagesText.GetComponent<TextMeshProUGUI>().text = string.Join(Environment.NewLine, messages.Messages.Select(x => $"{x.TimeStamp:HH:mm:ss} | {x.Owner} | {x.Text}"));
        }

        #endregion
    }
}