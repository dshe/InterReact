using System.IO;
using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using Stringification;

namespace InterReact;

internal static class ToMessagesEx
{
    internal static IObservable<object> ToMessages(this IObservable<string[]> source, InterReactClientConnector connector)
    {
        ResponseComposer composer = new(connector);
        return Observable.Create<object>(observer =>
        {
            return source.Subscribe(
                strings =>
                {
                    object message = composer.Compose(strings);
                    observer.OnNext(message);
                    if (message is PriceTick priceTick && connector.FollowPriceTickWithSizeTick)
                    {
                        TickType type = GetSizeTickType(priceTick.TickType);
                        if (type != TickType.Undefined)
                            observer.OnNext(new SizeTick(priceTick.RequestId, type, priceTick.Size));
                    }
                },
                e => observer.OnError(e),
                observer.OnCompleted);
        });
    }

    private static TickType GetSizeTickType(TickType tickType) => tickType switch
    {
        TickType.BidPrice => TickType.BidSize,
        TickType.AskPrice => TickType.AskSize,
        TickType.LastPrice => TickType.LastSize,
        TickType.DelayedBidPrice => TickType.DelayedBidSize,
        TickType.DelayedAskPrice => TickType.DelayedAskSize,
        TickType.DelayedLastPrice => TickType.DelayedLastSize,
        _ => TickType.Undefined
    };
}

internal sealed class ResponseComposer
{
    private ILogger Logger { get; }
    private ResponseReader Reader { get; }

    internal ResponseComposer(InterReactClientConnector connector)
    {
        Logger = connector.Logger;
        Reader = new ResponseReader(connector);
    }

    internal object Compose(string[] strings)
    {
        try
        {
            Reader.SetEnumerator(strings);
            string code = Reader.ReadString();
            object message = CodeToMessage(code, Reader);
            Reader.VerifyEnumerationEnd();
            return message;
        }
        catch (Exception e)
        {
            string m = $"ResponseComposer error: [{strings.JoinStrings(", ")}].";
            //Logger.LogError(e, "{Message}", m);
            Logger.LogError(e, "{Message}", m);
            throw new InvalidDataException(m, e);
        }
    }

    private static object CodeToMessage(string code, ResponseReader reader) => code switch
    {
        "1" => new PriceTick(reader),
        "2" => new SizeTick(reader),
        "3" => new OrderStatusReport(reader),
        "4" => Alert.Create(reader),
        "5" => new OpenOrder(reader),
        "6" => new AccountValue(reader),
        "7" => new PortfolioValue(reader),
        "8" => new AccountUpdateTime(reader),
        "9" => new NextId(reader),
        "10" => new ContractDetails(reader, ContractDetailsType.GeneralContractType),
        "11" => new Execution(reader),
        "12" => new MarketDepth(reader, false),
        "13" => new MarketDepth(reader, true),
        "14" => new NewsBulletin(reader),
        "15" => new ManagedAccounts(reader),
        "16" => new FinancialAdvisor(reader),
        "17" => new HistoricalData(reader),
        "18" => new ContractDetails(reader, ContractDetailsType.BondContractType),
        "19" => new ScannerParameters(reader),
        "20" => new ScannerData(reader),
        "21" => new OptionComputationTick(reader),

        "45" => GenericTick.Create(reader),
        "46" => StringTick.Create(reader),
        "47" => new ExchangeForPhysicalTick(reader),

        "49" => new CurrentTime(reader),
        "50" => new RealtimeBar(reader),
        "51" => new FundamentalData(reader),
        "52" => new ContractDetailsEnd(reader),
        "53" => new OpenOrderEnd(reader),
        "54" => new AccountUpdateEnd(reader),
        "55" => new ExecutionEnd(reader),
        "56" => new DeltaNeutralContract(reader, true),
        "57" => new SnapshotEndTick(reader),
        "58" => new MarketDataTypeTick(reader),
        "59" => new CommissionReport(reader),

        "61" => new Position(reader),
        "62" => new PositionEnd(reader),
        "63" => new AccountSummary(reader),
        "64" => new AccountSummaryEnd(reader),
        "65" => new VerifyMessageApi(reader),
        "66" => new VerifyCompleted(reader),
        "67" => new DisplayGroups(reader),
        "68" => new DisplayGroupUpdate(reader),
        "69" => new VerifyAndAuthorizeMessageApi(reader),
        "70" => new VerifyAndAuthorizeCompleted(reader),
        "71" => new AccountPositionMulti(reader),
        "72" => new AccountPositionMultiEnd(reader),
        "73" => new AccountUpdateMulti(reader),
        "74" => new AccountUpdateMultiEnd(reader),
        "75" => new SecurityDefinitionOptionParameter(reader),
        "76" => new SecurityDefinitionOptionParameterEnd(reader),
        "77" => new SoftDollarTiers(reader),
        "78" => new FamilyCodes(reader),
        "79" => new SymbolSamples(reader),
        "80" => new MarketDepthExchanges(reader),
        "81" => new ReqParamsTick(reader),
        "82" => new SmartComponents(reader),
        "83" => new NewsArticle(reader),
        "84" => new TickNews(reader),
        "85" => new NewsProviders(reader),
        "86" => new HistoricalNews(reader),
        "87" => new HistoricalNewsEnd(reader),
        "88" => new HeadTimestamp(reader),
        "89" => new HistogramItems(reader),
        "90" => new HistoricalDataUpdate(reader),
        "91" => new RerouteMktData(reader),
        "92" => new RerouteMktDepth(reader),
        "93" => new MarketRule(reader),
        "94" => new PnL(reader),
        "95" => new PnLSingle(reader),
        "96" => new HistoricalTicks(reader),
        "97" => new HistoricalBidAskTicks(reader),
        "98" => new HistoricalLastTicks(reader),
        "99" => TickByTick.Create(reader),
        "100" => new OrderBound(reader),
        "101" => new CompletedOrder(reader),
        "102" => new CompletedOrdersEnd(),
        "103" => new ReplaceFAEnd(reader),
        "104" => new WshMetaData(reader),
        "105" => new WshEventData(reader),
        _ => throw new InvalidDataException($"Undefined code '{code}'.")
    };
}
