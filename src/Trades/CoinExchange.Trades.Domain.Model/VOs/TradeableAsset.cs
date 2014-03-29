using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinExchange.Trades.Domain.Model.VOs
{
    public class TradeableAsset
    {
        public TradeableAsset(int marginStop, int marginCall, string feeVolumeCurrency, List<AssetFee> fees, List<decimal> leverage, decimal lotMultiplier, decimal lotDecimals, decimal pairDecimals, string lot, string quote, string aclassQuote, string aclassBase, string altname)
        {
            MarginStop = marginStop;
            MarginCall = marginCall;
            FeeVolumeCurrency = feeVolumeCurrency;
            FeesSchedule = fees;
            Leverage = leverage;
            LotMultiplier = lotMultiplier;
            LotDecimals = lotDecimals;
            PairDecimals = pairDecimals;
            Lot = lot;
            Quote = quote;
            AclassQuote = aclassQuote;
            AclassBase = aclassBase;
            Altname = altname;
        }

        public string Altname { get; private set; }
        public string AclassBase{ get; private set; }
        public string AclassQuote{ get; private set; }
        public string Quote{ get; private set; }
        public string Lot{ get; private set; }
        public decimal PairDecimals{ get; private set; }
        public decimal LotDecimals{ get; private set; }
        public decimal LotMultiplier{ get; private set; }
        public List<decimal> Leverage{ get; private set; }
        public List<AssetFee> FeesSchedule{ get; private set; }
        public string FeeVolumeCurrency{ get; private set; }
        public int MarginCall{ get; private set; }
        public int MarginStop{ get; private set; }
    }
}
