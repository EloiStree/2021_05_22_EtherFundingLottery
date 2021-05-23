using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

public class EtherFundingPublicLottery : MonoBehaviour
{
    [Tooltip("Title of the lottery as Hash")]
    public string m_startingHash = "Bibiliotheque Ferrières";

    [Tooltip("Transaction of the previous lottery end")]
    public string m_startingTransactionHash = "0x000...0000";

    [Tooltip("Participants array allowed in the lottery")]
    public string[] m_participantsInJoinOrder;

    [Tooltip("Transactions history from start transaction to now")]
    public string[] m_allTransactionInReceivedOrder;


    [Header("Debug")]
    public string m_currentComputedHashOfVictory;
    public string m_currentComputedHashOfVictoryAsNumber;


    [Header("Winner")]
    public uint m_currentWinnerIndex;
    public string m_currentWinnerAddress;


    [Header("Details")]
    public bool m_useDetailsLog;
    public bool m_disclaimer;
    [Header("Owner")]
    public bool m_appendOwner;
    public string m_lotteryOwnerContactInformation="";
    public string m_lotteryOwnerInformationToUsers = "";
    public string m_lotteryOwnerHumanityDefense = "";

    public string m_winnerExplained;

    [Header("Event")]
    public UnityEvent m_onComputedEvent;
    private void OnValidate()
    {
        ComputeWinner();
    }

    public void ComputeWinnerWithoutLog()
    {
        if (m_participantsInJoinOrder.Length <= 0)
            return;
        DateTime now = DateTime.Now;
        StringBuilder log = new StringBuilder();
        string logTmp = "";
        string previousHash = "";
        previousHash = ComputeSha256Hash(m_startingHash);
        logTmp = previousHash;
        previousHash = ComputeSha256Hash(previousHash + m_startingTransactionHash);
       
        for (int i = 0; i < m_allTransactionInReceivedOrder.Length; i++)
        {
            logTmp = previousHash;
            previousHash = ComputeSha256Hash(previousHash + m_allTransactionInReceivedOrder[i]);   
        }
        m_currentComputedHashOfVictory = previousHash;
      
        StringBuilder b = new StringBuilder();
        char[] winnerAsChar = m_currentComputedHashOfVictory.ToCharArray();
        int[] winnerAsInt = winnerAsChar.Select(k => (int)k).ToArray();

        string winnerAsStringNumber = string.Join("", winnerAsInt);
        BigInteger.TryParse(winnerAsStringNumber, out BigInteger winnerAsNumber);

        m_currentComputedHashOfVictoryAsNumber = winnerAsStringNumber;
        uint index = (uint)BigInteger.ModPow(winnerAsNumber, new BigInteger(1), new BigInteger(m_participantsInJoinOrder.Length));
        m_currentWinnerIndex = index;
        m_currentWinnerAddress = m_participantsInJoinOrder[m_currentWinnerIndex];
        m_onComputedEvent.Invoke();
    }

    public void ComputeWinner()
    {
        if (m_participantsInJoinOrder.Length <= 0)
            return;

        DateTime now = DateTime.Now;
        StringBuilder log = new StringBuilder();
       if(m_useDetailsLog)
            log.Append("# Lottery result Log  \n\n");
        if (m_useDetailsLog)
            log.Append("Date: "+now.ToString()+"  \n");
        if (m_useDetailsLog)
            log.Append("## Initial params  \n\n");
        if (m_useDetailsLog)
            log.Append(string.Format(
            "- Title Hash:{0}  \n - Init Transaction Hash: {1}  \n - Participants Count: {2}  \n - Transactions Count: {3}  \n",
            m_startingHash, m_startingTransactionHash, m_participantsInJoinOrder.Length, m_allTransactionInReceivedOrder.Length));

        string logTmp = "";
        string previousHash = "";

        if (m_useDetailsLog)
            log.Append("## Compute the Winner Hash \n\n");

        previousHash = ComputeSha256Hash(m_startingHash);
        if (m_useDetailsLog)
            log.Append(string.Format("\n> Convert Title To Hash  \n {0} > H256 > {1}  \n ", m_startingHash, previousHash));
        logTmp = previousHash;
        previousHash = ComputeSha256Hash(previousHash + m_startingTransactionHash);
        if (m_useDetailsLog)
            log.Append(string.Format("\n> 'Title Hash' + 'Transaction Start Hash' to an 'initial hash'  \n{0}  \n\t Append {1}  \n\t\t  > H256> {2}  \n ", logTmp, m_startingTransactionHash, previousHash));
        if (m_useDetailsLog)
            log.Append(string.Format("\n\n Previous Hash | Append | Transaction ID | H256 | New Hash  "));
        if (m_useDetailsLog)
            log.Append(string.Format("\n - | - | - | - | -  \n"));

        for (int i = 0; i < m_allTransactionInReceivedOrder.Length; i++)
        {
            logTmp = previousHash;
            previousHash = ComputeSha256Hash(previousHash + m_allTransactionInReceivedOrder[i]);
            if (m_useDetailsLog)
                log.Append(string.Format("{0} | + | {1} | > | {2}  \n", logTmp, m_allTransactionInReceivedOrder[i], previousHash));
        }


        if (m_useDetailsLog)
            log.Append("## Hash to Winner \n\n");
        if (m_useDetailsLog)
            log.Append(string.Format("\nIf the lottery would finish now, the winner is... ?  "));

        m_currentComputedHashOfVictory = previousHash;
        if (m_useDetailsLog)
            log.Append(string.Format("\nHash representing the winner: {0}  ", m_currentComputedHashOfVictory));

        StringBuilder b = new StringBuilder();
        char[] winnerAsChar = m_currentComputedHashOfVictory.ToCharArray();
        int[] winnerAsInt = winnerAsChar.Select(k => (int)k).ToArray();

        string winnerAsStringNumber = string.Join("", winnerAsInt);
        BigInteger.TryParse(winnerAsStringNumber, out BigInteger winnerAsNumber);

        m_currentComputedHashOfVictoryAsNumber = winnerAsStringNumber;
        // m_currentComputedHashOfVictoryAsNumber = string.Format("{0:0}", winnerAsNumber);
        uint index = (uint)BigInteger.ModPow(winnerAsNumber, new BigInteger(1), new BigInteger(m_participantsInJoinOrder.Length));
        m_currentWinnerIndex = index;
        m_currentWinnerAddress = m_participantsInJoinOrder[m_currentWinnerIndex];

        if (m_useDetailsLog)
        {
            log.Append(string.Format("\n**ash as Char array**: {0}  ", string.Join(" ", winnerAsChar)));
            log.Append(string.Format("\n**Hash as Int array**: {0}  ", string.Join(" ", winnerAsInt)));
            log.Append(string.Format("\n**Hash as an number**: {0}  ", winnerAsStringNumber));
            log.Append(string.Format("\n**Number % Participants**:  {0}%{1}={2}  ", winnerAsNumber, m_participantsInJoinOrder.Length, m_currentWinnerIndex));
            log.Append(string.Format("\n> **Winner is the index {0}**:  {1}  ", m_currentWinnerIndex, m_participantsInJoinOrder[m_currentWinnerIndex]));
            log.Append(string.Format("\n\n **Reminder**: The following array of participants is based on the transaction history:" +
                "\n - Ordered by latest transaction to new one that are over the minimum entry value.  " +
                "\n - From the transaction {0} to now {1}  " +
                "\nThe list is not random in aim to let anyone to verify/compute the result of this lottery based on their own code.  " +
                "\nThat basically the main purpose of this log.  \n\n" +
                "\nThis lottery is a 'fair' one. So it don't matter how much you put or how many transaction to inject. One address = one ticket. (See GitHub doc for more information).  ", m_startingTransactionHash, now));



            log.Append("\n\n\n\n");
            log.Append("\n## PARTICIPANTS (ordered):  ");

            for (int i = 0; i < m_participantsInJoinOrder.Length; i++)
            {

                log.Append(string.Format("\n{0}: {1}  ", i, m_participantsInJoinOrder[i]));

            }
            log.Append("\n\n");
            log.Append("\n## ETHER TRANSACTION ID USED (ordered):  ");

            for (int i = 0; i < m_allTransactionInReceivedOrder.Length; i++)
            {

                log.Append(string.Format("\n{0}: {1}  ", i, m_allTransactionInReceivedOrder[i]));

            }
            if (m_disclaimer) { 
                log.Append("\n\n\n\n");
                log.Append("\n-----------------------------\n");
                log.Append("\n## Ether Funding Lottery Developer   ");
                log.Append(string.Format("\nThanks for participating to this lottery.  "));
                log.Append(string.Format("\nFind more on the source project here: https://github.com/EloiStree/2021_05_22_EtherFundingLottery  "));
                log.Append(string.Format("\nDisclaimer: I created this code for the fun and curiosity. I am not the one executing it.  "));
                log.Append(string.Format("\nDisclaimer: All responsability is on the user of my this code.  "));
                log.Append(string.Format("\nDisclaimer: I am not responsible of people modifying this code or scamming people with it. That is not design for that but can be use as such.  "));
                log.Append(string.Format("\nDisclaimer: People can fork this project. I am not responsible of code that is from a fork."));
                log.Append(string.Format("\n  \nA developer that created this code for the good of humanity and , mainly, curiosity.    "));
                log.Append(string.Format("\n  \nKind regards,  \nEloi S.  "));
            }

            if (m_appendOwner) { 
                log.Append("\n\n\n\n");
                log.Append("\n-----------------------------\n");
                log.Append("\n## Organizer information  \n");
                log.Append("\nGovernment HATE lottery, I don't expect organizer to let's contact information or comment...   ");
                log.Append("\nBut if they do, here is what they are providing...  ");
                log.Append("\n### Contact information\n");
                log.Append("\n```  \n");
                if (string.IsNullOrEmpty(m_lotteryOwnerContactInformation))
                {
                    log.Append("\nNot specified  ");
                }
                else
                {
                    log.Append(m_lotteryOwnerContactInformation);
                }
                log.Append("\n```  \n");

                log.Append("\n### Communication to participants\n  ");
                log.Append("\n```  \n");
                if (string.IsNullOrEmpty(m_lotteryOwnerInformationToUsers))
                {
                    log.Append("\nNot specified  ");
                }
                else
                {
                    log.Append(m_lotteryOwnerInformationToUsers);
                }
                log.Append("\n```  \n");
                log.Append("\n### Humanity defense\n  ");
                log.Append("\n*The aim of this project is to fund and help people/association by doing easy to set public lottery.*  ");
                log.Append("\nThe following is the reason why the organizer had set up this lottery...  ");
                log.Append("\n```  \n");
                if (string.IsNullOrEmpty(m_lotteryOwnerHumanityDefense))
                {
                    log.Append("\nNot specified  ");
                }
                else
                {
                    log.Append(m_lotteryOwnerHumanityDefense);
                }
                log.Append("\n```  \n");
            }

            m_winnerExplained = log.ToString();
        }
        m_onComputedEvent.Invoke();
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
