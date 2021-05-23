using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inspector_CSV2ComputeEntry : MonoBehaviour
{
    public ComputeEntry m_target;
    [TextArea(0,10)]
    public string m_csvText;
    public string m_formatInfo="transaction:origine:destination:wei";
    public TextAsset m_csvTextFile;
    public List<Transaction> m_transactionsLoaded;

    [ContextMenu("Push 2 Test Input")]
    public void PushToTestInput()
    {
        m_transactionsLoaded.Clear();
        string input = m_csvText + "\n";
        
        if(m_csvTextFile!=null)
            input+=m_csvTextFile.text;

        m_transactionsLoaded = TransactionConverter.GetListOfTransactionsWithoutFilterCSV(input);
        m_target.SetEntry(m_transactionsLoaded, true);
    }
    private void OnValidate()
    {
        m_formatInfo = "transaction:origine:destination:wei";
    }
}

public class TransactionConverter {

    public static char[] m_colomnSpliter = new char[] { ';', ':' };
    public static char[] m_lineEnder = new char[] { '\n' };
    public static List<Transaction> GetListOfTransactionsWithoutFilterCSV(string text)
    {
        return GetListOfTransactionsWithoutFilterCSV(text, m_colomnSpliter, m_lineEnder);
    }
        public static List<Transaction> GetListOfTransactionsWithoutFilterCSV(string text,  char [] columnSpliter, char [] lineSpliter) {
        List<Transaction> result = new List<Transaction>();
        result.Clear();

        string[] lines = text.Split(lineSpliter);
        for (int i = 0; i < lines.Length; i++)
        {
            string[] columns = lines[i].Split(columnSpliter);
            if (columns.Length == 4)
            {
                Transaction t = new Transaction(columns[0], columns[1], columns[2], columns[3]);
                result.Add(t);
            }
        }

        return result;
    }

}
