#region imports
using QuantConnect.Brokerages;
using QuantConnect.Data;
using QuantConnect.Indicators;
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
        private int _startingCash = 2000;
        private CoinbaseBrokerageModel _coinBaseBrokerageModel = new CoinbaseBrokerageModel(AccountType.Cash);
        private int maxPosition = 1000; // USD
        private int minPosition = 100; // USD
        private int fastPeriod = 720;
        private int slowPeriod = 1560;

        //ProgramVariables
        public decimal _price;
        public decimal _holding; //number of BTCs that we hold in portfolio
        public decimal usd;

        public ExponentialMovingAverage _fastEMA;
        public ExponentialMovingAverage _slowEMA;


        public override void Initialize()
        {
            SetStartDate(2017, 1, 1);
            SetEndDate(2018, 1, 1);
            SetCash(_startingCash);

            AddCrypto(_ticker, Resolution.Minute);

            SetBrokerageModel(_coinBaseBrokerageModel);

            _fastEMA = EMA(_ticker, fastPeriod, Resolution.Minute);
            _slowEMA = EMA(_ticker, slowPeriod, Resolution.Minute);
        }

        /// OnData event is the primary entry point for your algorithm. Each new data point will be pumped in here.
        /// Slice object keyed by symbol containing the stock data
        public override void OnData(Slice data)
        {
            _price = data[_ticker].Price;

            usd = Portfolio.CashBook["USD"].Amount;

            if (!Portfolio.Invested && usd > minPosition)
            {
                if (_fastEMA > _slowEMA)
                {
                    decimal quantity = Math.Round(maxPosition / _price);
                    MarketOrder(_ticker, quantity);
                }
            }

            if (Portfolio.Invested)
            {
                _holding = Portfolio.CashBook[_baseSymbol].Amount;

                if (_fastEMA < _slowEMA)
                {
                    Sell(_ticker, _holding);
                }
            }

            Plot("MA", "Fast", _fastEMA);
            Plot("MA", "Slow", _slowEMA);
            Plot("MA", "Price", _price);

        }

        public override void OnOrderEvent(OrderEvent orderEvent)
        {
            Log(orderEvent.ToString());
        }

    }
}
