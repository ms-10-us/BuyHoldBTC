#region imports
using QuantConnect.Brokerages;
using QuantConnect.Data;
#endregion
namespace QuantConnect.Algorithm.CSharp
{
    public class WellDressedTanOwlet : QCAlgorithm
    {
        private string _ticker = "BTCUSD";
        private int _startingCash = 1000;
        private decimal _weight = 0.5m;
        private CoinbaseBrokerageModel _coinBaseBrokerageModel = new CoinbaseBrokerageModel(AccountType.Cash);

        public override void Initialize()
        {
            SetStartDate(2017, 1, 1);
            SetEndDate(2018, 1, 1);
            SetCash(_startingCash);

            AddCrypto(_ticker, Resolution.Minute);
            SetBrokerageModel(_coinBaseBrokerageModel);
        }

        /// OnData event is the primary entry point for your algorithm. Each new data point will be pumped in here.
        /// Slice object keyed by symbol containing the stock data
        public override void OnData(Slice data)
        {
            if (!Portfolio.Invested)
            {
                SetHoldings(_ticker, _weight);

            }
        }

    }
}
