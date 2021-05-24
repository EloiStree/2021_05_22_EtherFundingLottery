using System;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class WinnerHashStackTrace
{

    //// START IDEA
    // Title
    // Title >(Hash256)> TitleHash + startTransaction > (Hash256)> InitHash
    // Loop
    // InitHash + Transaction >(Hash256)> NewHash
    // Final Hash
    //// END IDEA


    public string m_title; //Default
    public string m_titleHash256; //0x....
    public string m_startTransaction;//0xstart
    public string m_titleHash256AppendTransaction;
    public string m_titleHash256AppendTransactionHash;
    public List<TransactionHash> m_transactionHash = new List<TransactionHash>();
    public string m_finalHash;
    public string m_finalHashAsChars;
    public string m_finalHashAsNumbers;
    public string m_finalHashAsNumber;
    public int m_participantNumber;
    public int m_finalHashNumberModuloParticipants;
    public int m_winnerIndex;
    public string m_gmdComputerDateWhenComputed;

    [System.Serializable]
    public class TransactionHash
    {
        public string m_initHash;
        public string m_transactionHash;
        public string m_initAppendTranscationHash;
        public string m_newHash;

        public TransactionHash(string initHash, string transactionHash, string newHash)
        {
            m_initHash = initHash;
            m_transactionHash = transactionHash;
            m_initAppendTranscationHash = initHash + transactionHash;
            m_newHash = newHash;
        }
    }

    public void SaveCurrentDateAsGMD() {
        m_gmdComputerDateWhenComputed = DateTime.UtcNow.ToString("yyyy-MM-dd HH mm ss fff");
    }

    
    public string[] GetTransactionsUsedInOrder()
    {
        return m_transactionHash.Select(k=>k.m_transactionHash).ToArray();
    }
}