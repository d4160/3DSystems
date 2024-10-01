using UnityEngine;

[System.Serializable]
public class AttackState : CharacterStateBase
{
    public int[] statesClipNumber = new int[] { 2, 1, 3 };

    [Header("Params")]
    public string attackParam = "Attack";
    public string attackIndexParam = "AttackIndex";

    [Header("States")]
    public string emptyState = "Empty";

    private int _attackParamHash;
    private int _attackIndexParamHash;
    [SerializeField] private int _attackCounter;
    private bool _attackIndexUpdated;

    public int GetRandomClip(int index)
    {
        return Random.Range(0, statesClipNumber[index]);
    }

    public override void Start()
    {
        _attackParamHash = Animator.StringToHash(attackParam);
        _attackIndexParamHash = Animator.StringToHash(attackIndexParam);
    }

    public override void Attack()
    {
        if (_attackCounter == 0)
        {
            Animator.SetFloat(_attackIndexParamHash, GetRandomClip(_attackCounter));

            _attackCounter++;
        }

        Animator.SetTrigger(_attackParamHash);
    }

    public override void Update()
    {
        AnimatorStateInfo stateInfo = Animator.GetCurrentAnimatorStateInfo(Layer);

        //Debug.Log($"InTransition: {Animator.IsInTransition(Layer)},IsName: {stateInfo.IsName(emptyState)}");

        bool inTransition = Animator.IsInTransition(Layer);
        bool isEmptyState = stateInfo.IsName(emptyState);

        if (!_attackIndexUpdated && inTransition && !isEmptyState)
        {
            _attackCounter %= 3;

            Animator.SetFloat(_attackIndexParamHash, GetRandomClip(_attackCounter));

            _attackIndexUpdated = true;

            _attackCounter++;
        }
        else if (inTransition && isEmptyState)
        {
            _attackCounter = 0;
        }
        else if (inTransition && isEmptyState)
        {
            _attackIndexUpdated = false;
        }
    }
}
