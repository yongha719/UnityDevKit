using System.Collections.Generic;
using UnityEngine;

namespace MyObjectPool
{
    public class GameManager : Singleton<GameManager>
    {
        private Stack<Player> leftPlayerStack = new Stack<Player>();
        private Stack<Player> rightPlayerStack = new Stack<Player>();

        public Transform leftPlayerParent;
        public Transform rightPlayerParent;

        private IGetObjectPool<Player> playerPool;

        private void Start()
        {
            playerPool = ObjectPoolManager.Instance.GetPool<Player>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                foreach (Player leftPlayer in leftPlayerStack)
                {
                    leftPlayer.Attack();
                }
            }

            if (Input.GetKeyDown(KeyCode.RightShift))
            {
                foreach (Player rightPlayer in rightPlayerStack)
                {
                    rightPlayer.Attack();
                }
            }
        }

        // Inspector에서 Button에 OnClick으로 등록할 함수
        public void OnGetPlayer(bool isLeftPlayer)
        {
            // 플레이어 타입에 따른 값들 설정
            var (playerType, playerParent) = GetPlayerTypeAndParent(isLeftPlayer);

            Player player = playerPool.Get(playerParent);
            player.PlayerType = playerType;

            if (player.PlayerType == PlayerType.LeftPlayer)
                leftPlayerStack.Push(player);
            else
                rightPlayerStack.Push(player);

            SetPlayerPosition(player);
        }

        // Inspector에서 Button에 OnClick으로 등록할 함수
        public void OnReleasePlayer(bool isLeftPlayer)
        {
            Player player = null;

            // 플레이어 타입에 맞는 스택 가져오기
            Stack<Player> playerStack = isLeftPlayer ?
                 leftPlayerStack : rightPlayerStack;

            if (playerStack.Count > 0)
                player = playerStack.Pop();

            if (player != null)
                ObjectPoolManager.Instance.ReleaseObject(player);
        }

        private (PlayerType playerType, Transform playerParent) GetPlayerTypeAndParent(bool isLeftPlayer)
        {
            if (isLeftPlayer)
                return (PlayerType.LeftPlayer, leftPlayerParent);

            return (PlayerType.RightPlayer, rightPlayerParent);
        }

        /// <summary>
        /// 플레이어 위치 설정해주는 함수
        /// </summary>
        private void SetPlayerPosition(Player player)
        {
            float playerXPos = player.PlayerType == PlayerType.LeftPlayer ? -7 : 7;

            Stack<Player> playerStack = player.PlayerType == PlayerType.LeftPlayer ?
                 leftPlayerStack : rightPlayerStack;

            // 위로 배치할지 아래로 배치할지
            float playerYPosMultiply = playerStack.Count % 2 == 0 ? 1 : -1;
            float playerYPos = playerYPosMultiply * playerStack.Count / 2;

            player.transform.position = new Vector2(playerXPos, playerYPos);
        }
    }
}
