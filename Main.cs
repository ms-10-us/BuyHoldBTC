#region imports
using QuantConnect.Brokerages;
using QuantConnect.Data;
using QuantConnect.Orders;
using System;
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
        private string _baseSymbol = "BTC";
        private int _startingCash = 1000;
        private decimal _weight = 0.5m;
        private CoinbaseBrokerageModel _coinBaseBrokerageModel = new CoinbaseBrokerageModel(AccountType.Cash);
        private decimal _targetPrecent = 0.10m; // = +10% sell at that price increase
        private decimal _stopPercent = 0.02m; // = -2% sell at that pricedecrease to minimize losses
        private decimal _trailPercent = 0.03m;

        //ProgramVariables
        public decimal _price;
        public decimal _holding; //number of BTCs that we hold in portfolio
        public decimal _targetPrice;
        public decimal _stopPrice;


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
            _price = data[_ticker].Price;



            if (!Portfolio.Invested)
            {
                SetHoldings(_ticker, _weight);
                Log($"Purchased {_ticker} at {_price}");

                _stopPrice = _price - (_price * _trailPercent);

            }

            _stopPrice = Math.Max(_price - (_price * _trailPercent), _stopPrice);

            if (Portfolio.Invested)
            {
                _holding = Portfolio.CashBook[_baseSymbol].Amount;
                if (_price < _stopPrice)
                {
                    Sell(_ticker, _holding);
                }
            }
        }

        public override void OnOrderEvent(OrderEvent orderEvent)
        {
            Log(orderEvent.ToString());
        }

    }
}
