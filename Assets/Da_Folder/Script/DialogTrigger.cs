using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    public DialogManager dialogManager;

    public void TriggerDialog()
    {
        dialogManager.StartConversation();
    }
}
