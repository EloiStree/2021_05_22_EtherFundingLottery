using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_SimulationToLotteryFundingComputation : MonoBehaviour
{

    public ComputeEntry m_computationEntryTarget;
    public InputField m_titleHash;
    public InputField m_transactionStartHash;
    public InputField m_minimumWeiEntryInEth;
    public UI_InputFieldToCSVEntry m_transactionsSource;

     string m_format= "{0:0}";
    public string m_weiDebug = "";


    public void PushToComputation() {
        m_computationEntryTarget.m_lotteryIdTitleHash = m_titleHash.text;
        m_computationEntryTarget.m_lotteryStartingTransactionIdHash = m_transactionStartHash.text;
        GetWeiOfEthString(m_minimumWeiEntryInEth.text, out string asWei);
        m_computationEntryTarget.m_minWeiAmount = m_weiDebug= asWei;
        m_computationEntryTarget.SetEntry(TransactionConverter.GetListOfTransactionsWithoutFilterCSV(m_transactionsSource.GetText()), false);
    }


    public void GetWeiOfEthString(string eth, out decimal asWei)
    {
        decimal.TryParse(eth, out decimal result);
        result *= (decimal)1000000000000000000;
        asWei= result;
    }
    public void GetWeiOfEthString(string eth, out string asWei)
    {
        decimal.TryParse(eth, out decimal result);
        result *= (decimal)1000000000000000000;
        asWei = string.Format(m_format, result);
    }
}
