﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ComputeEntry : MonoBehaviour
{
    public EtherFundingPublicLottery m_target;

    public string m_lotteryIdTitleHash="Default";
    public string m_lotteryStartingTransactionIdHash="";
    public List<Transaction> m_orderedListOfTransaction;
    public List<string> m_participants;

    public string m_minWeiAmount= "10000000000000000";
    public void SetEntry(List<Transaction> transactions, bool computeTheChangeDirectly) {

        m_participants.Clear();
        decimal.TryParse(m_minWeiAmount, out decimal minWei);
        m_orderedListOfTransaction = transactions;
        for (int i = 0; i < m_orderedListOfTransaction.Count; i++)
        {
            decimal.TryParse(m_orderedListOfTransaction[i].m_amountInWei, out decimal minWeiTransaction);

            if (minWeiTransaction >= minWei) {
                string sourceAddress = m_orderedListOfTransaction[i].m_origineAddress;
                if (!m_participants.Contains(sourceAddress)){
                    m_participants.Add(sourceAddress);
                }
            
            }
        }

        m_target.m_startingHash = m_lotteryIdTitleHash;
        m_target.m_startingTransactionHash = m_lotteryStartingTransactionIdHash;
        m_target.m_participantsInJoinOrder = m_participants.ToArray();
        m_target.m_allTransactionInReceivedOrder = transactions.Select(k=>k.m_origineAddress).ToArray();
        if(computeTheChangeDirectly)
        m_target.ComputeWinner();


    }

}

[System.Serializable]
public class Transaction {

    public string m_transactionId;
    public string m_origineAddress;
    public string m_destinationAddress;
    public string m_amountInWei;

    public Transaction(string transactionId, string origineAddress, string destinationAddress, string amountInWei)
    {
        m_transactionId = transactionId;
        m_origineAddress = origineAddress;
        m_destinationAddress = destinationAddress;
        m_amountInWei = amountInWei;
    }
}