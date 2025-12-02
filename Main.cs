#region imports
using QuantConnect.Brokerages;
using QuantConnect.Data;
#endregion
namespace QuantConnect.Algorithm.CSharp
{
    /*
     This is my custom crypto template
     */
    public class MyCustomCryptoTemplate : QCAlgorithm
    {
        //UserVariables
        private string _ticker = "BTCUSD"; //virtual pai - tacks the current USD value of 1 BTC
        private int _startingCash = 1000;
        private decimal _weight = 0.5m;
        private CoinbaseBrokerageModel _coinBaseBrokerageModel = new CoinbaseBrokerageModel(AccountType.Cash);

        //ProgramVariables
        public decimal _price;
        public decimal _holding; //number of BTCs that we hold in portfolio
        public string _baseSymbol; // "BTC"

        public override void Initialize()
        {
            SetStartDate(2017, 1, 1);
            SetEndDate(2018, 1, 1);
            SetCash(_startingCash);

            var _crypto = AddCrypto(_ticker, Resolution.Minute);
            _baseSymbol = _crypto.BaseCurrency.ToString();

            SetBrokerageModel(_coinBaseBrokerageModel);
        }

        /// OnData event is the primary entry point for your algorithm. Each new data point will be pumped in here.
        /// Slice object keyed by symbol containing the stock data
        public override void OnData(Slice data)
        {
            _price = data[_ticker].Price;

            if (!Portfolio.Invested)
            {
                SetHoldings(_ticker, _weight);

            }
        }

    }
}
