using Fusion;
using UnityEngine;

namespace Tutorial
{
    public class Player : NetworkBehaviour
    {
        [SerializeField] private Ball _prefabBall;

        [Networked] private TickTimer delay { get; set; }

        private NetworkCharacterController _cc;
        private Vector3 _forward = Vector3.forward;

        private void Awake()
        {
            _cc = GetComponent<NetworkCharacterController>();
        }

        public override void FixedUpdateNetwork()
        {
            if (GetInput(out NetworkInputData data))
            {
                data.direction.Normalize();
                _cc.Move(5 * data.direction * Runner.DeltaTime);

                if (data.direction.sqrMagnitude > 0)
                {
                    _forward = data.direction;
                }

                if (HasStateAuthority && delay.ExpiredOrNotRunning(Runner))
                {
                    if (data.buttons.IsSet(NetworkInputData.MOUSEBUTTON0))
                    {
                        delay = TickTimer.CreateFromSeconds(Runner, 0.5f);
                        Runner.Spawn(_prefabBall,
                            transform.position + _forward,
                            Quaternion.LookRotation(_forward),
                            Object.InputAuthority,
                            (runner, o) =>
                            {
                                // 实例化前初始化
                                o.GetComponent<Ball>().Init();
                            });
                    }
                }
            }
        }
    }
}