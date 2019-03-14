using System;
using System.Collections.Generic;
using System.Linq;
using InterReact.Messages;
using System.IO;
using System.Reactive.Linq;
using InterReact.Extensions;

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
        private readonly Action<object> Output;
        private readonly ResponseReader Reader;
        private IEnumerator<string>? enumerator;

        internal ResponseComposer(Config config, Action<object> output)
        {
            Output = output ?? throw new ArgumentNullException(nameof(output));
            Reader = new ResponseReader(config, ReadString, output);
        }

        internal string ReadString()
        {
            if (enumerator == null)
                throw new Exception("Null enumerator");
            if (enumerator.MoveNext())
                return enumerator.Current;
            throw new IndexOutOfRangeException($"Message shorter than expected.");
        }

        internal void Compose(string[] inputStrings)
        {
            try
            {
                enumerator = inputStrings.AsEnumerable().GetEnumerator();

                var code = ReadString();
                if (code == "1")
                    TickPrice.Send(Reader);
                else
                    Output(Compose(code));

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
                "2" => new TickSize(Reader) as object,
                "3" => new OrderStatusReport(Reader),
                "4" => Alert.Create(Reader),
                "5" => new OpenOrder(Reader),
                "6" => new AccountValue(Reader),
                "7" => new PortfolioValue(Reader),
                "8" => new AccountUpdateTime(Reader),
                "9" => new NextId(Reader),
                "10" => new ContractDetails(Reader, ContractDetailsType.ContractDetails),
                "11" => new Execution(Reader),
                "12" => new MarketDepth(Reader, false),
                "13" => new MarketDepth(Reader, true),
                "14" => new NewsBulletin(Reader),
                "15" => new ManagedAccounts(Reader),
                "16" => new FinancialAdvisor(Reader),
                "17" => new HistoricalBars(Reader),
                "18" => new ContractDetails(Reader, ContractDetailsType.BondContractDetails),
                "19" => new ScannerParameters(Reader),
                "20" => new ScannerData(Reader),
                "21" => new TickOptionComputation(Reader),

                "45" => TickGeneric.Create(Reader),
                "46" => TickString.Create(Reader),
                "47" => new TickExchangeForPhysical(Reader),

                "49" => new CurrentTime(Reader),
                "50" => new RealtimeBar(Reader),
                "51" => new FundamentalData(Reader),
                "52" => new ContractDetailsEnd(Reader),
                "53" => new OpenOrderEnd(Reader),
                "54" => new AccountUpdateEnd(Reader),
                "55" => new ExecutionEnd(Reader),
                "56" => new UnderComp(Reader, false),
                "57" => new TickSnapshotEnd(Reader),
                "58" => new TickMarketDataType(Reader),
                "59" => new CommissionReport(Reader),

                "61" => new AccountPosition(Reader),
                "62" => new AccountPositionEnd(Reader),
                "63" => new AccountSummary(Reader),
                "64" => new AccountSummaryEnd(Reader),
                "65" => new VerifyMessageApi(Reader),
                "66" => new VerifyCompleted(Reader),
                "67" => new DisplayGroups(Reader),
                "68" => new DisplayGroupUpdate(Reader),
                "69" => new VerifyAndAuthorizeMessageApi(Reader),
                "70" => new VerifyAndAuthorizeCompleted(Reader),
                "71" => new PositionMulti(Reader),
                "72" => new PositionMultiEnd(Reader),
                "73" => new AccountUpdateMulti(Reader),
                "74" => new AccountUpdateMultiEnd(Reader),
                "75" => new SecurityDefinitionOptionParameter(Reader),
                "76" => new SecurityDefinitionOptionParameterEnd(Reader),
                "77" => new SoftDollarTiers(Reader),
                "78" => FamilyCode.GetAll(Reader),
                "79" => new ContractDescriptions(Reader),
                "80" => DepthMktDataDescription.GetAll(Reader),
                "81" => new TickReqParams(Reader),
                "82" => new SmartComponents(Reader),
                "83" => new NewsArticle(Reader),
                "84" => new TickNews(Reader),
                "85" => NewsProvider.GetAll(Reader),
                "86" => new HistoricalNews(Reader),
                "87" => new HistoricalNewsEnd(Reader),
                "88" => new HeadTimestamp(Reader),
                "89" => new HistogramData(Reader),
                _ => throw new InvalidDataException($"Undefined code '{code}'.")
            };
        }
    }

}
