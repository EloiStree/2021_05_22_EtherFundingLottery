using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

public class EtherFundingPublicLotteryMono : MonoBehaviour
{
    [Tooltip("Title of the lottery as Hash")]
    public string m_titleHash = "Bibiliotheque Ferrières";

    [Tooltip("Transaction of the previous lottery end")]
    public string m_startingTransactionHash = "0x000...0000";

    [Tooltip("Participants array allowed in the lottery")]
    public string[] m_participantsInJoinOrder;

    [Tooltip("Transactions history from start transaction to now")]
    public string[] m_allTransactionInReceivedOrder;
    public string[] m_usedTransactionInReceivedOrder;


    [Header("Debug")]
    public string m_currentComputedHashOfVictory;
    public string m_currentComputedHashOfVictoryAsNumber;
    public string m_endTransactionHash = "0x000...0000";


    [Header("Winner")]
    public bool m_computed;
    public uint m_currentWinnerIndex;
    public string m_currentWinnerAddress;


    [Header("Details")]
    public bool m_useDetailsLog;
    public bool m_disclaimer;
    [Header("Owner")]
    public bool m_appendOwner;
    public string m_lotteryOwnerContactInformation = "";
    public string m_lotteryOwnerInformationToUsers = "";
    public string m_lotteryOwnerHumanityDefense = "";

    public string m_winnerExplained;

    [Header("Event")]
    public UnityEvent m_onComputedEvent;
    public bool m_shitHappen;
    private void OnValidate()
    {
        ComputeWinnerBasedWithStackTrace();
    }

    public void ComputeWinnerBasedWithStackTrace()
    {
        try
        {

            m_usedTransactionInReceivedOrder = new string[m_minTransactionToComputeTheWin];
            int ti = 0;
            for (int i = m_allTransactionInReceivedOrder.Length - m_minTransactionToComputeTheWin; i < m_allTransactionInReceivedOrder.Length; i++)
            {
                m_usedTransactionInReceivedOrder[ti] = m_allTransactionInReceivedOrder[i];
                ti++;

            }

            CryptoDontTrustVerifyWinnerAlogrithm.ComputeWinnerIndexOf(m_titleHash, m_startingTransactionHash, m_participantsInJoinOrder.Length,
                m_usedTransactionInReceivedOrder, out m_currentWinnerIndex, out m_computeStack);
        }
        catch (Exception e) { m_shitHappen = true; Debug.LogWarning(e); }
        m_onComputedEvent.Invoke();
    }

    public int m_minParticipantsToWin = 10;
    public int m_minTransactionToComputeTheWin = 10;
    public WinnerHashStackTrace m_computeStack = new WinnerHashStackTrace();




}


public class CryptoDontTrustVerifyWinnerAlogrithm
{
    public static void ComputeWinnerIndexOf(
       string title,
       string transactionId,
       int participantCount,
       string[] decisionTransactionInReceivedOrder,
       out uint winnerIndex,
       out WinnerHashStackTrace computeStack)
    {
        if (title == null || transactionId == null)
            throw new NullReferenceException("Title or transaction can be empty but not null");
        if (participantCount <= 0)
            throw new Exception("Need participants");
        if (decisionTransactionInReceivedOrder == null || decisionTransactionInReceivedOrder.Length <= 0)
            throw new Exception("Need transactions");
        for (int i = 0; i < decisionTransactionInReceivedOrder.Length; i++)
        {
            if (string.IsNullOrEmpty(decisionTransactionInReceivedOrder[i]))
                throw new NullReferenceException("On transaction is null or empty.");
        }


        computeStack = new WinnerHashStackTrace();
        computeStack.SaveCurrentDateAsGMD();

        string logDebugHolder = "";
        string previousHash = "";

        computeStack.m_title = title;

        //CORE
        previousHash = ComputeSha256Hash(title);

        computeStack.m_titleHash256 = previousHash;
        logDebugHolder = previousHash;
        computeStack.m_startTransaction = transactionId;
        computeStack.m_titleHash256AppendTransaction = previousHash + transactionId;

        //CORE
        previousHash = ComputeSha256Hash(previousHash + transactionId);

        computeStack.m_startTransaction = transactionId;
        computeStack.m_titleHash256AppendTransactionHash = previousHash;

        //CORE
        for (int i = 0; i < decisionTransactionInReceivedOrder.Length; i++)
        {
            logDebugHolder = previousHash;

            //CORE
            previousHash = ComputeSha256Hash(previousHash + decisionTransactionInReceivedOrder[i]);

            computeStack.m_transactionHash.Add(
                new WinnerHashStackTrace.TransactionHash(logDebugHolder, decisionTransactionInReceivedOrder[i], previousHash));
        }

        //CORE
        string currentComputedHashOfVictory = previousHash;

        computeStack.m_finalHash = previousHash;


        //CORE
        char[] winnerAsChar = currentComputedHashOfVictory.ToCharArray();
        //CORE
        int[] winnerAsInt = winnerAsChar.Select(k => (int)k).ToArray();
        //CORE
        string winnerAsStringNumber = string.Join("", winnerAsInt);

        computeStack.m_finalHashAsChars = string.Join(" ", winnerAsChar);
        computeStack.m_finalHashAsNumbers = string.Join(" ", winnerAsInt);
        computeStack.m_finalHashAsNumber = winnerAsStringNumber;

        //CORE
        BigInteger.TryParse(winnerAsStringNumber, out BigInteger winnerAsNumber);

        //CORE    
        int index = (int)BigInteger.ModPow(winnerAsNumber, new BigInteger(1), new BigInteger(participantCount));


        computeStack.m_finalHashNumberModuloParticipants = index;
        computeStack.m_winnerIndex = index;
        computeStack.m_participantNumber = (int)participantCount;

        //CORE   
        winnerIndex = (uint)index;

    }
    static string ComputeSha256Hash(string rawData)
    {
        //source: https://www.c-sharpcorner.com/article/compute-sha256-hash-in-c-sharp/#:~:text=The%20HashAlgorithm%20class%20is%20the,byte%20array%20of%20256%20bits.
        // Create a SHA256   
        using (SHA256 sha256Hash = SHA256.Create())
        {
            // ComputeHash - returns byte array  
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

            // Convert byte array to a string   
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }
}