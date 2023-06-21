namespace EnumNamespace
{
    public enum SIMPLESTATE { Walk, Attack }
}

public class SimpleStateMachineTemplate : MonoBehaviour
{
    SIMPLESTATE currentState;
    SIMPLESTATE nextState;

    //a general purpose bool to check if we can leave our current state
    bool exitConditionsMet = false;
    //a general purpose bool to check if we are just entering a state
    bool isEntryFrame = false;
    //a general purpose float used to track how long we've been in a state since entry
    float timeInState = 0f;

    //state-specific variables
    [SerializeField] float walkSpeed = 5f;
    float desiredTimeInWalkState = 1.5f;
    float desiredTimeInAttackState = 1.0f;

    //animation variables
    [SerializeField] AnimationReferenceAsset attack, walk;
    SkeletonAnimation skeletonAnimation;
    string currentAnimation; // helps us make sure we only play a new animation if it's different from the current animation playing


    void Start()
    {
        skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();
        currentState = SIMPLESTATE.Walk;
    }

    //STATE MACHINE
    void Update()
    {
        //very simple state machine!
        
        //determine if we need to change state based on if current exit conditions are met.
        if (exitConditionsMet)
        {
            currentState = nextState;
            isEntryFrame = true;
            exitConditionsMet = false;
        }

        //play the appropriate state function based on the current state.
        if (currentState == SIMPLESTATE.Walk)
            Walk();
        else if (currentState == SIMPLESTATE.Attack)
            Attack();

        //Debug.Log($"current state is {currentState}");
    }

    //STATES
    void Walk()
    {
        //ENTRY
        if (isEntryFrame)
        {
            //on entry, trigger the walk animation
            SetBodyAnimation(walk, true, 1f);

            //reset timers and tracking variables
            timeInState = 0f;
            isEntryFrame = false;
        }

        //UPDATE
        timeInState += Time.deltaTime;
        //each frame we are in this state, try to play the walk animation, and move the character.
        SetBodyAnimation(walk, true, 1f);
        Move(1f);


        //EXIT
        //check if the exit conditions were met this frame
        exitConditionsMet = (timeInState >= desiredTimeInWalkState);
        if (exitConditionsMet)
        {
            nextState = SIMPLESTATE.Attack;
        }
    }

    void Attack()
    {
        //ENTRY
        if (isEntryFrame)
        {
            //on entry, play the attack animation
            SetBodyAnimation(attack, false, 1f);

            //reset timers and tracking variables
            timeInState = 0f;
            isEntryFrame = false;
        }

        //UPDATE
        timeInState += Time.deltaTime;

        //EXIT
        //check if the exit conditions were met this frame
        exitConditionsMet = (timeInState >= desiredTimeInAttackState);
        if (exitConditionsMet)
        {
            nextState = SIMPLESTATE.Walk;
        }
    }

    //Move function
    void Move(float input)
    {
        //input expects a 1 for moving right and a -1 for moving left.
        input *= Time.deltaTime * walkSpeed;
        this.transform.Translate(input, 0, 0);
    }

    //Animation helper functions
    void SetBodyAnimation(AnimationReferenceAsset anim, bool loop, float timeScale)
    {
        //try to play animation A -- but if we are already playing animation A, no need to interrupt it.
        if (anim.name.Equals(currentAnimation))
        {
            return;
        }
        skeletonAnimation.state.SetAnimation(0, anim, loop).TimeScale = timeScale;
        currentAnimation = anim.name;
    }
}