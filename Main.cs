#region imports
using QuantConnect.Brokerages;
using QuantConnect.Data;
using QuantConnect.Data.Consolidators;
using QuantConnect.Data.Market;
using QuantConnect.Indicators;
using QuantConnect.Orders;
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
        private int candleSize = 15; // up untill minutes

        //ProgramVariables
        public decimal price;
        public decimal open;
        public decimal high;
        public decimal low = 1000000m;
        public decimal close;
        public decimal holding; //number of BTCs that we hold in portfolio
        public int momentCounter = 1; // start at 1 minute
        public TradeBar lastTradeBar;


        public ExponentialMovingAverage _fastEMA;
        public ExponentialMovingAverage _slowEMA;


        public override void Initialize()
        {
            SetStartDate(2017, 1, 1);
            SetEndDate(2018, 1, 1);
            SetCash(startingCash);

            AddCrypto(ticker, Resolution.Minute);

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

            //if (momentCounter <= candleSize)
            //{
            //    if (momentCounter == 1)
            //    {
            //        open = price;
            //    }

            //    if (price > high)
            //    {
            //        high = price;
            //    }

            //    if (price < low)
            //    {
            //        low = price;
            //    }

            //    if (momentCounter == candleSize)
            //    {
            //        close = price;
            //        momentCounter = 0;
            //    }
            //}
            //momentCounter++;
            //Log($"Open: {open}\n" +
            //    $"High: {high}\n" +
            //    $"Low: {low}\n" +
            //    $"Close: {close}");


        }

        private void ConsolidatorHandler(object sender, TradeBar consolidated)
        {
            if (lastTradeBar != null)
            {
                Log($"Open: {consolidated.Open}\n" +
                $"High: {consolidated.High}\n" +
                $"Low: {consolidated.Low}\n" +
                $"Close: {consolidated.Close}");
            }
            lastTradeBar = consolidated;
        }

        public override void OnOrderEvent(OrderEvent orderEvent)
        {
            Log(orderEvent.ToString());
        }

    }
}
