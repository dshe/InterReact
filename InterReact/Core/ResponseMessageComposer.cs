using Stringification;
using System.IO;

namespace InterReact;

public sealed class ResponseMessageComposer
{
    private IClock Clock { get; }
    private ILogger Logger { get; }
    private ResponseReader Reader { get; }

    public ResponseMessageComposer(IClock clock, ILogger<ResponseMessageComposer> logger, ResponseReader reader)
    {
        Clock = clock;
        Logger = logger;
        Reader = reader;
    }

    internal object ComposeMessage(string[] strings)
    {
        string code = "";
        try
        {
            Reader.SetEnumerator(strings);
            code = Reader.ReadString();
            object message = GetResponseMessage(code);
            Reader.VerifyEnumerationEnd();
            return message;
        }
        catch (Exception e)
        {
            string m = $"ResponseComposer error: [{code} => {strings.JoinStrings(", ")}].";
            Logger.LogError(e, "{Message}", m);
            throw new InvalidDataException(m, e);
        }
    }

    private object GetResponseMessage(string code) => code switch
    {
        "1" => PriceTick.Create(Reader),
        "2" => new SizeTick(Reader),
        "3" => new OrderStatusReport(Reader),
        "4" => new AlertMessage(Reader, Clock),
        "5" => new OpenOrder(Reader),
        "6" => new AccountValue(Reader),
        "7" => new PortfolioValue(Reader),
        "8" => new AccountUpdateTime(Reader),
        "9" => new NextOrderId(Reader),
        "10" => new ContractDetails(Reader, ContractDetailsType.GeneralContractType),
        "11" => new Execution(Reader),
        "12" => new MarketDepth(Reader, false),
        "13" => new MarketDepth(Reader, true),
        "14" => new NewsBulletin(Reader),
        "15" => new ManagedAccounts(Reader),
        "16" => new FinancialAdvisor(Reader),
        "17" => new HistoricalData(Reader),
        "18" => new ContractDetails(Reader, ContractDetailsType.BondContractType),
        "19" => new ScannerParameters(Reader),
        "20" => new ScannerData(Reader),
        "21" => new OptionComputationTick(Reader),

        "45" => new GenericTick(Reader),
        "46" => StringTick.Create(Reader),
        "47" => new ExchangeForPhysicalTick(Reader),

        "49" => new CurrentTime(Reader),
        "50" => new RealtimeBar(Reader),
        "51" => new FundamentalData(Reader),
        "52" => new ContractDetailsEnd(Reader),
        "53" => new OpenOrderEnd(Reader),
        "54" => new AccountUpdateEnd(Reader),
        "55" => new ExecutionEnd(Reader),
        "56" => new DeltaNeutralContract(Reader, true),
        "57" => new SnapshotEndTick(Reader),
        "58" => new MarketDataTickType(Reader),
        "59" => new CommissionReport(Reader),

        "61" => new Position(Reader),
        "62" => new PositionEnd(Reader),
        "63" => new AccountSummary(Reader),
        "64" => new AccountSummaryEnd(Reader),
        "65" => new VerifyMessageApi(Reader),
        "66" => new VerifyCompleted(Reader),
        "67" => new DisplayGroups(Reader),
        "68" => new DisplayGroupUpdate(Reader),
        "69" => new VerifyAndAuthorizeMessageApi(Reader),
        "70" => new VerifyAndAuthorizeCompleted(Reader),
        "71" => new AccountPositionMulti(Reader),
        "72" => new AccountPositionMultiEnd(Reader),
        "73" => new AccountUpdateMulti(Reader),
        "74" => new AccountUpdateMultiEnd(Reader),
        "75" => new SecurityDefinitionOptionParameter(Reader),
        "76" => new SecurityDefinitionOptionParameterEnd(Reader),
        "77" => new SoftDollarTiers(Reader),
        "78" => new FamilyCodes(Reader),
        "79" => new SymbolSamples(Reader),
        "80" => new MarketDepthExchanges(Reader),
        "81" => new TickRequestParams(Reader),
        "82" => new SmartComponents(Reader),
        "83" => new NewsArticle(Reader),
        "84" => new TickNews(Reader),
        "85" => new NewsProviders(Reader),
        "86" => new HistoricalNews(Reader),
        "87" => new HistoricalNewsEnd(Reader),
        "88" => new HeadTimestamp(Reader),
        "89" => new HistogramData(Reader),
        "90" => HistoricalDataBar.CreateUpdateBar(Reader),
        "91" => new RerouteMktData(Reader),
        "92" => new RerouteMktDepth(Reader),
        "93" => new MarketRule(Reader),
        "94" => new PnL(Reader),
        "95" => new PnLSingle(Reader),
        "96" => new HistoricalTicks(Reader),
        "97" => new HistoricalBidAskTicks(Reader),
        "98" => new HistoricalLastTicks(Reader),
        "99" => TickByTick.Create(Reader),
        "100" => new OrderBound(Reader),
        "101" => new CompletedOrder(Reader),
        "102" => new CompletedOrdersEnd(),
        "103" => new ReplaceFAEnd(Reader),
        "104" => new WshMetaData(Reader),
        "105" => new WshEventDataReceived(Reader),
        "106" => new HistoricalSchedule(Reader),
        "107" => new UserInfo(Reader),
        _ => throw new InvalidDataException($"Undefined code '{code}'.")
    };
}
