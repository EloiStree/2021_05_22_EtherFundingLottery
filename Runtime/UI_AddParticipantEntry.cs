using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_AddParticipantEntry : MonoBehaviour
{
    public UI_InputFieldToCSVEntry m_target;
    public InputField m_transactionId;
    public InputField m_walletOrigineId;
    public InputField m_destinationId;
    public InputField m_amountInWei;

    public void AddToTarget() {
        m_target.AddTransaction(m_transactionId.text, m_walletOrigineId.text, m_destinationId.text, m_amountInWei.text);
    }

}
