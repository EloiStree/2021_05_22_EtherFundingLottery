##  BIG Disclaimer

Before we start, I am explaining the concept and the coding it for the example.     
It does not means that I plan to use it, or used it (outside of checking if it works for science)  
   
If you use this kind of concept, you are in a gray zone of the justice that depending on the mood of your goverment could lead you to prison with heavy consequences.  
Please don't use this concept if you think it is illegal in your country.  

I am the creator of the idea, but I am not responsible of the use of it.

------------------------------------

> "Don't trust, verify"
> 
-------------------------------------

# Current state
Demonstration by examples of the current state
[![image](https://user-images.githubusercontent.com/20149493/120052825-81456b80-c027-11eb-8644-28845925d45f.png)](https://youtu.be/istbsXm4osI)  
https://youtu.be/istbsXm4osI

# Idea

This respositroy is just a concept I through about that is crazy cool.

In Etherieum, we could have anonyme lottery:
- that are thrustable by any body on any machine at any time !
- and where the orginazer can if he want be totaly anonyme and untracable.
  - Meaning that if he want he can do it publicly with the authority and gov authorisation. But can decide to stay private if he fear goverment. Participants addresses are still trackable by the goverment but not the organizer.

You just need a computer somewhere in the world that check every x seconds the balance of a wallet and do the math and the transaction when an amount is reach.
And when the winner is declared. All participants can do the math and confirmed that the winner is the correct address !!!

-----------------------------------------------
Explaination in construction
-----------------------------------------------
# Example of a lottery for funding goal or association

// Set where on the wallet the Lottery start.
Transaction Start Point: 0xsldkjfajmkjmdfs4qs4fd 
// Minimum entry to participate to the lottery (one ticket per wallet max).
Min entry: 0.01 ETH
// The winner is selected based on the last "Hash" of the participants-chain that reach (>=) the lottery aim. 
Lottery Stop: 1 ETH
// How many of the wallet will go to the funding vs the winner
Funding: 40%


## Step by step

### Amount is reach

1. Hash the title of the contest
2. Hash the start transactionID with Title Hash
3. Take the last x transaction of the wallet at the transaction that bring the amount to equal or superior to the lottery max range.
4. Blockchain them with H256  to make a random winner hash
5. Transform the the char as int (based on a public constant array) 
6. make a winner random number of this chars: `aeiou > a e i o u > 10 31 50 46 51 > 1031504651`
7. From late to recent transaction, make a list of all the participants that put the minimum entry amount. And check that he is not already in the list at each entry.
8. Make a modulo of the winner number with the number of participants and apply the result to the array or participants.
9. Ta da !

You have now:
- A winner that is historicaly store indirectly to the blockchain
- A winner that is computable by anyone one any computer at anytime.
- A winner that is decided by pure randomness of H256
- A winner that can't be predicted until the amount is reach. 
- A log indirectly block in the block chain that can't be modify


### Lotterry end with not the amount wanted

1. Wait that a block has a superior timestamp of the contest timestamp.
2. Wait N block to be sure that the blockchain don't find a corrupted fork.
3. Compute the winner based on history from the start block of the contest until the '1.' block.
4. Ta da ! 

You have a now:
- A decentrelized timestamp to finish the lottery and decide of the winner.



### Where to display the public information on the initial step up

If the community thrust the organizer, where ever you want that you are 99.9999% sure it won't be modify or hack for your community to rely on this start condition of the contest.

What I could do with more knowledge of ethereum is to store a compressed initial setup of the contest as a metainformation of the start transaction of in the wallet.
That would help a lot's in the decentralized and thrust of this project. But for the moment I don't know how to do it.



## Code example

This code is what I am using in my second draft of this experiment prototype in C# (2021 May 24).

``` csharp

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
    
//    ----------------------------------------
  
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
    
    

```



---------------------------
## Warning

The main weak point of this concept:
You need to trust organizer that he will send the money at the winner address. 

But the current tool provide some code that allows anyone to verified the transaction from the wallet to the winner address had been done.

**PS: Orginiazed can use automated or manual lottery.**
Don't assume that the organizer is a "scammer" if he don't do the transfert immediately.
But verife until he does that he did. 

If he don't you can't do something about it.
It is why I warn you here.

I would participate to this lottery but:
- I won't participate to a lottery funding pool if I don't have
  - enough information on the organizer
  - thrust on the organizer or history of the organizer beeing thrustfull
- I won't participate to a lottery funding pool if the entry is not "cheap" it don't worse the risk.






