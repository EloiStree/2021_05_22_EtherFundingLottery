using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_LotteryComputedResult : MonoBehaviour
{
    public EtherFundingPublicLotteryMono m_source;

    public InputField m_winner;
    public InputField m_winnerHash;
    public InputField m_participantIndex;
    public InputField m_participantCount;

    public void RefreshUI() {

        if (m_source.m_computed) { 
            if (m_winner)
                m_winner.text = m_source.m_currentWinnerAddress;
            if (m_winnerHash)
                m_winnerHash.text = m_source.m_currentComputedHashOfVictory;
            if (m_participantIndex)
                m_participantIndex.text =""+ m_source.m_currentWinnerIndex;
            if (m_participantCount)
                m_participantCount.text =""+ m_source.m_participantsInJoinOrder.Length;
        }


    }

}
