#region imports
using QuantConnect.Brokerages;
using QuantConnect.Data;
using QuantConnect.Data.Consolidators;
using QuantConnect.Data.Market;
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
        private string ticker = "BTCUSD"; //virtual pai - tacks the current USD value of 1 BTC
        private string baseSymbol = "BTC";
        private int startingCash = 2000;
        private CoinbaseBrokerageModel _coinBaseBrokerageModel = new CoinbaseBrokerageModel(AccountType.Cash);
        private int maxPosition = 1000;
        private int minPosition = 500;
        private int candleSize = 15;

        //ProgramVariables
        public decimal price;
        public decimal holding; //number of BTCs that we hold in portfolio
        public decimal currentLow;
        public decimal previousLow;
        public TradeBar lastTradeBar;
        public decimal usd;



        public override void Initialize()
        {
            SetStartDate(2017, 1, 1);
            SetEndDate(2018, 1, 1);
            SetCash(startingCash);

            AddCrypto(ticker, Resolution.Hour);

            SetBrokerageModel(_coinBaseBrokerageModel);

            var consolidator = new TradeBarConsolidator(candleSize);
            consolidator.DataConsolidated += ConsolidatorHandler;
            SubscriptionManager.AddConsolidator(ticker, consolidator);

        }

        /// OnData event is the primary entry point for your algorithm. Each new data point will be pumped in here.
        /// Slice object keyed by symbol containing the stock data
        public override void OnData(Slice data)
        {
            price = data[ticker].Price;

        }

        private void ConsolidatorHandler(object sender, TradeBar consolidated)
        {
            if (lastTradeBar != null)
            {
                Log($"Open: {consolidated.Open}\n" +
                $"High: {consolidated.High}\n" +
                $"Low: {consolidated.Low}\n" +
                $"Close: {consolidated.Close}");

                previousLow = currentLow;
                currentLow = consolidated.Low;
            }

            if (!Portfolio.Invested)
            {
                usd = Portfolio.CashBook["USD"].Amount;
                if (currentLow > previousLow && previousLow != 0 && usd > minPosition)
                {
                    decimal quantity = Math.Round(Math.Min(usd, maxPosition) / price, 2);
                    MarketOrder(ticker, quantity);
                }
            }

            if (Portfolio.Invested)
            {
                if (currentLow < previousLow)
                {
                    holding = Portfolio.CashBook[baseSymbol].Amount;
                    MarketOrder(ticker, -holding);
                }
            }

            lastTradeBar = consolidated;


        }


        public override void OnOrderEvent(OrderEvent orderEvent)
        {
            Log(orderEvent.ToString());
        }

    }
}
