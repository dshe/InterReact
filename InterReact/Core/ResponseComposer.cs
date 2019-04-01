using System;
using System.Collections.Generic;
using System.Linq;
using InterReact.Messages;
using System.IO;
using System.Reactive.Linq;
using InterReact.Extensions;
using StringEnums;
using NodaTime;
using NodaTime.Text;

namespace InterReact.Core
{
    internal static class ToMessagesEx
    {
        internal static IObservable<object> ToMessages(this IObservable<string[]> source, Config config)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            return Observable.Create<object>(observer =>
            {
                var responseComposer = new ResponseComposer(config, observer.OnNext);

                return source.Subscribe(
                    onNext:      responseComposer.Compose,
                    onError:     observer.OnError,
                    onCompleted: observer.OnCompleted);
            });
        }
    }

    public sealed class ResponseComposer
    {
        internal readonly Config Config;
        internal readonly Action<object> Output;
        internal readonly ResponseParser Parser;
        private IEnumerator<string>? enumerator;
        internal ResponseComposer(Config config, Action<object> output)
        {
            Config = config;
            Output = output ?? throw new ArgumentNullException(nameof(output));
            Parser = new ResponseParser(Output);
        }

        internal void Compose(string[] inputStrings)
        {
            try
            {
                enumerator = inputStrings.AsEnumerable().GetEnumerator();
                var code = ReadString(); // read the first string
                if (code == "1")
                    TickPrice.Send(this); // compose the message with the remaining strings
                else
                    Output(Compose(code)); // compose the message with the remaining strings
                if (enumerator.MoveNext())
                    Output(new ResponseWarning($"Message longer than expected: [{inputStrings.JoinStrings(", ")}]."));
            }
            catch (Exception e)
            {
                throw new InvalidDataException($"ComposeMessageError: [{inputStrings.JoinStrings(", ")}].", e);
            }
        }

        private object Compose(string code)
        {
            return code switch
            {
                "2" => new TickSize(this) as object,
                "3" => new OrderStatusReport(this),
                "4" => Alert.Create(this),
                "5" => new OpenOrder(this),
                "6" => new AccountValue(this),
                "7" => new PortfolioValue(this),
                "8" => new AccountUpdateTime(this),
                "9" => new NextId(this),
                "10" => new ContractData(this, ContractDataType.ContractData),
                "11" => new Execution(this),
                "12" => new MarketDepth(this, false),
                "13" => new MarketDepth(this, true),
                "14" => new NewsBulletin(this),
                "15" => new ManagedAccounts(this),
                "16" => new FinancialAdvisor(this),
                "17" => new HistoricalBars(this),
                "18" => new ContractData(this, ContractDataType.BondContractData),
                "19" => new ScannerParameters(this),
                "20" => new ScannerData(this),
                "21" => new TickOptionComputation(this),

                "45" => TickGeneric.Create(this),
                "46" => TickString.Create(this),
                "47" => new TickExchangeForPhysical(this),

                "49" => new CurrentTime(this),
                "50" => new RealtimeBar(this),
                "51" => new FundamentalData(this),
                "52" => new ContractDataEnd(this),
                "53" => new OpenOrderEnd(this),
                "54" => new AccountUpdateEnd(this),
                "55" => new ExecutionEnd(this),
                "56" => new DeltaNeutralContract(this, true),
                "57" => new TickSnapshotEnd(this),
                "58" => new TickMarketDataType(this),
                "59" => new CommissionReport(this),

                "61" => new AccountPosition(this),
                "62" => new AccountPositionEnd(this),
                "63" => new AccountSummary(this),
                "64" => new AccountSummaryEnd(this),
                "65" => new VerifyMessageApi(this),
                "66" => new VerifyCompleted(this),
                "67" => new DisplayGroups(this),
                "68" => new DisplayGroupUpdate(this),
                "69" => new VerifyAndAuthorizeMessageApi(this),
                "70" => new VerifyAndAuthorizeCompleted(this),
                "71" => new AccountPositionMulti(this),
                "72" => new AccountPositionMultiEnd(this),
                "73" => new AccountUpdateMulti(this),
                "74" => new AccountUpdateMultiEnd(this),
                "75" => new SecurityDefinitionOptionParameter(this),
                "76" => new SecurityDefinitionOptionParameterEnd(this),
                "77" => new SoftDollarTiers(this),
                "78" => new FamilyCodes(this),
                "79" => new SymbolSamples(this),
                "80" => new MktDepthExchanges(this),
                "81" => new TickReqParams(this),
                "82" => new SmartComponents(this),
                "83" => new NewsArticle(this),
                "84" => new TickNews(this),
                "85" => new NewsProviders(this),
                "86" => new HistoricalNews(this),
                "87" => new HistoricalNewsEnd(this),
                "88" => new HeadTimestamp(this),
                "89" => new HistogramItems(this),
                "90" => new HistoricalData(this),
                "91" => new RerouteMktData(this),
                "92" => new RerouteMktDepth(this),
                "93" => new MarketRule(this),
                "94" => new PnL(this),
                "95" => new PnLSingle(this),
                "96" => new HistoricalTicks(this),
                "97" => new HistoricalBidAskTicks(this),
                "98" => new HistoricalLastTicks(this),
                "99" => TickByTick.Create(this),
                "100" => new OrderBound(this),
                _ => throw new InvalidDataException($"Undefined code '{code}'.")
            };
        }

        internal string ReadString()
        {
            if (enumerator == null)
                throw new Exception("Null enumerator");
            if (enumerator.MoveNext())
                return enumerator.Current;
            throw new IndexOutOfRangeException($"Message is shorter than expected.");
        }
        internal char ReadChar() => Parser.ParseChar(ReadString());
        internal bool ReadBool() => Parser.ParseBool(ReadString());
        internal int ReadInt() => Parser.ParseInt(ReadString());
        internal long ReadLong() => Parser.ParseLong(ReadString());
        internal double ReadDouble() => Parser.ParseDouble(ReadString());
        internal int? ReadIntNullable() => Parser.ParseIntNullable(ReadString());
        internal double? ReadDoubleNullable() => Parser.ParseDoubleNullable(ReadString());
        internal LocalTime ReadLocalTime(LocalTimePattern p) => p.Parse(ReadString()).GetValueOrThrow();
        internal LocalDateTime ReadLocalDateTime(LocalDateTimePattern p) => p.Parse(ReadString()).GetValueOrThrow();
        internal T ReadEnum<T>() where T:Enum => Parser.ParseEnum<T>(ReadString());
        internal T ReadStringEnum<T>() where T:StringEnum<T>, new() => Parser.ParseStringEnum<T>(ReadString());

        internal void IgnoreVersion() => ReadString();
        internal int GetVersion() => ReadInt();
        internal int RequireVersion(int minimumVersion)
        {
            var v = GetVersion();
            return (v >= minimumVersion) ? v : throw new InvalidDataException($"Invalid response version: {v} < {minimumVersion}.");
        }
        internal void AddStringsToList(IList<string> list)
        {
            var n = ReadInt();
            for (int i = 0; i < n; i++)
                list.Add(ReadString());
        }
        internal void AddTagsToList(IList<Tag> list)
        {
            var n = ReadInt();
            for (int i = 0; i < n; i++)
                list.Add(new Tag(this));
        }
    }
}
