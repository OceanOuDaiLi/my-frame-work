using UnityEngine;

namespace GameEngine
{
    public class ChaseStateBehaviour : BaseStateBehaviour
    {
        int frameRate = 0;
        float chaseSpeed = 0;
        bool chaseingBack = false;
        bool chaseingEnemy = false;
        AnimationCurve chaseCurve = null;

        private const int RAND_MAX = 0x7fff;

        public override void OnInitialize()
        {
            chaseSpeed = 350;//owner.Property.MonoProperty.Speed;
            frameRate = (int)(1 / Time.deltaTime);
            chaseCurve = owner.moveCurve;
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            chaseingBack = owner.GetAnimatorBool(AnimCfg.PARAM_BOOL_CHASE_BACK);
            chaseingEnemy = !chaseingBack;

            if (chaseingEnemy && owner.Enemy == null)
            {
                CDebug.LogError($"Current Chase behaviour didn't add enemy");
            }

        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (chaseingEnemy)
            {
                ChaseEnemy(stateInfo);
            }

            if (chaseingBack)
            {
                ChaseBack(stateInfo);
            }
            base.OnStateUpdate(animator, stateInfo, layerIndex);
        }

        public override void OnLocalStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (chaseingEnemy)
            {
                CheckEnemyTransition();
            }

            if (chaseingBack)
            {
                CheckBackTransition();
            }
        }
        #region Chase Enemy

        void CheckEnemyTransition()
        {
            // 若需要攻击过程变更敌人，则 控制索敌时间。
            if (owner.Enemy != null)
            {
                float dist = Mathf.Abs(owner.Enemy.TransformSelf.position.y - owner.TransformSelf.position.y);

                //缓冲距离是使追逐人物时到达的距离不会在最大有效攻击距离的边界
                if (dist <= 150f)  // 可视化攻击距离 或 策划公式计算可攻击距离         150为像素单位
                {
                    // 执行普通攻击， 或则 技能
                    chaseingEnemy = false;
                    owner.SetAnimatorTrigger(AnimCfg.PARAM_TRIGGER_ATTACK);
                    //owner.SetAnimatorBool("Chase", false);
                }
            }
            else
            {
                CDebug.LogError("Current Chase enemy is null");
            }
        }

        void ChaseEnemy(AnimatorStateInfo stateInfo)
        {
            if (!chaseingEnemy /* or other conditions */) return;

            float deltaTime = Time.deltaTime;

            Vector3 ownerPos = owner.TransformSelf.position;
            Vector3 enemyPos = owner.Enemy.TransformSelf.position;

            owner.Chase(GetAnimaChaseDirection(ownerPos, enemyPos));

            ownerPos.y = ownerPos.y.Move(enemyPos.y, chaseSpeed, deltaTime);
            float normalizeTime = stateInfo.normalizedTime - (int)stateInfo.normalizedTime;
            ownerPos.x = ownerPos.x.Move(enemyPos.x, chaseSpeed * chaseCurve.Evaluate(normalizeTime), deltaTime);
            owner.TransformSelf.position = ownerPos;
        }

        #endregion

        #region Chase Back
        void CheckBackTransition()
        {
            float dist = Mathf.Abs(owner.Property.MonoProperty.FightCreatePos.y - owner.TransformSelf.position.y);
            if (dist <= 1f)                         // 检测是否回到了占位点          30为像素单位
            {
                chaseingBack = false;
                owner.SetAnimatorBool(AnimCfg.PARAM_BOOL_CHASE_BACK, false);
                owner.SetAnimatorTrigger(AnimCfg.PARAM_TRIGGER_STAND);
                owner.Play(AnimCfg.STAND, owner.Property.MonoProperty.Dir);
                owner.TransformSelf.localPosition = Vector3.zero;
            }
        }

        void ChaseBack(AnimatorStateInfo stateInfo)
        {
            if (!chaseingBack) return;

            float deltaTime = Time.deltaTime;

            Vector3 ownerPos = owner.TransformSelf.position;
            Vector3 tarPos = owner.Property.MonoProperty.FightCreatePos;

            owner.Chase(GetAnimaChaseDirection(ownerPos, tarPos));

            ownerPos.y = ownerPos.y.Move(tarPos.y, chaseSpeed, deltaTime);
            float normalizeTime = stateInfo.normalizedTime - (int)stateInfo.normalizedTime;
            ownerPos.x = ownerPos.x.Move(tarPos.x, chaseSpeed * chaseCurve.Evaluate(normalizeTime), deltaTime);
            owner.TransformSelf.position = ownerPos;
        }

        #endregion

        private Vector2 GetAnimaChaseDirection(Vector3 ownerPos, Vector3 enemyPos)
        {
            Vector2 dir = Vector2.zero;
            if (ownerPos.x < enemyPos.x)
            {
                //owner.FaceRight();
                dir.x = 1;
            }
            else if (ownerPos.x > enemyPos.x)
            {
                //owner.FaceLeft();
                dir.x = -1;
            }
            if (ownerPos.y < enemyPos.y)
            {
                //owner.FaceUp();
                dir.y = 1;
            }
            else if (ownerPos.y > enemyPos.y)
            {
                //owner.FaceDown();
                dir.y = -1;
            }

            return dir;
        }
    }
}
