using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class UI_InputFieldToCSVEntry : MonoBehaviour
{
    public InputField m_targetInputField;
    private string m_csvEntryText;

    public void AddTransaction(string transactionId, string walletOrigine, string walletDestination, string weiAmount) {

        m_csvEntryText = string.Format("{0}:{1}:{2}:{3}\n", transactionId, walletOrigine, walletDestination, weiAmount) + m_csvEntryText;
        if (m_csvEntryText.Length > 500)
        {
            m_targetInputField.text = m_csvEntryText.Substring(0, 500);
        }
        else { 
            m_targetInputField.text = m_csvEntryText;
        }

    }

    private const string m_valideRandomChar = "abcdefABCDEF0123456789";

    public string GetText()
    {
        return m_csvEntryText;
    }

 
    public string GenerateRandomTransaction() {

        StringBuilder address = new StringBuilder();
        address.Append("0x");
        for (int i = 0; i < 40; i++)
        {
            address.Append(m_valideRandomChar[UnityEngine.Random.Range(0, m_valideRandomChar.Length)]);

        }
        return address.ToString();

    }
    public void AddRandomTransaction(int count)
    {
        for (int i = 0; i < count; i++)
        {
            AddRandomTransaction();
        }
    
    }
        public void AddRandomTransaction() { 
    
        AddTransaction(GenerateRandomTransaction(), GenerateRandomTransaction(), GenerateRandomTransaction(),string.Format("{0:0}",((UnityEngine.Random.value*100.0)%100.0)* 10000000000000000.0));

    }
}
