using System.IO;
namespace InterReact;

public class ResponseComposer(ResponseReader reader)
{
    private readonly ResponseReader _reader = reader;

    internal object Compose(string[] strings)
    {
        _reader.SetStrings(strings);
        string code = _reader.ReadString();
        object message = GetResponseMessage(code);
        _reader.VerifyEnumerationEnd();
        return message;
    }

    private object GetResponseMessage(string code) => code switch
    {
        "1" => PriceTick.Create(_reader),
        "2" => new SizeTick(_reader),
        "3" => new OrderStatusReport(_reader),
        "4" => new Alert(_reader),
        "5" => new OpenOrder(_reader),
        "6" => new AccountValue(_reader),
        "7" => new PortfolioValue(_reader),
        "8" => new AccountUpdateTime(_reader),
        "9" => new NextIdMessage(_reader),
        "10" => new ContractDetails(_reader, ContractDetailsType.GeneralContractType),
        "11" => new Execution(_reader),
        "12" => new MarketDepth(_reader, false),
        "13" => new MarketDepth(_reader, true),
        "14" => new NewsBulletin(_reader),
        "15" => new ManagedAccounts(_reader),
        "16" => new FinancialAdvisor(_reader),
        "17" => new HistoricalData(_reader),
        "18" => new ContractDetails(_reader, ContractDetailsType.BondContractType),
        "19" => new ScannerParameters(_reader),
        "20" => new ScannerData(_reader),
        "21" => new OptionComputationTick(_reader), // error here

        "45" => new GenericTick(_reader),
        "46" => StringTick.Create(_reader),
        "47" => new ExchangeForPhysicalTick(_reader),

        "49" => new CurrentTime(_reader),
        "50" => new RealtimeBar(_reader),
        "51" => new FundamentalData(_reader),
        "52" => new ContractDetailsEnd(_reader),
        "53" => new OpenOrderEnd(_reader),
        "54" => new AccountUpdateEnd(_reader),
        "55" => new ExecutionEnd(_reader),
        "56" => new DeltaNeutralContract(_reader, true),
        "57" => new TickSnapshotEnd(_reader),
        "58" => new MarketDataTypeMessage(_reader),
        "59" => new CommissionReport(_reader),

        "61" => new AccountPosition(_reader, false),
        "62" => new AccountPosition(_reader, true),
        "63" => new AccountSummary(_reader),
        "64" => new AccountSummaryEnd(_reader),
        "65" => new VerifyMessageApi(_reader),
        "66" => new VerifyCompleted(_reader),
        "67" => new DisplayGroups(_reader),
        "68" => new DisplayGroupUpdate(_reader),
        "69" => new VerifyAndAuthorizeMessageApi(_reader),
        "70" => new VerifyAndAuthorizeCompleted(_reader),
        "71" => new AccountPositionsMulti(_reader, false),
        "72" => new AccountPositionsMulti(_reader, true),
        "73" => new AccountUpdateMulti(_reader, false),
        "74" => new AccountUpdateMulti(_reader, true),
        "75" => new SecurityDefinitionOptionParameter(_reader),
        "76" => new SecurityDefinitionOptionParameterEnd(_reader),
        "77" => new SoftDollarTiers(_reader),
        "78" => new FamilyCodes(_reader),
        "79" => new SymbolSamples(_reader),
        "80" => new MarketDepthExchanges(_reader),
        "81" => new TickRequestParams(_reader),
        "82" => new SmartComponents(_reader),
        "83" => new NewsArticle(_reader),
        "84" => new TickNews(_reader),
        "85" => new NewsProviders(_reader),
        "86" => new HistoricalNews(_reader),
        "87" => new HistoricalNewsEnd(_reader),
        "88" => new HeadTimestamp(_reader),
        "89" => new HistogramData(_reader),
        "90" => HistoricalDataBar.CreateUpdateBar(_reader),
        "91" => new RerouteMktData(_reader),
        "92" => new RerouteMktDepth(_reader),
        "93" => new MarketRule(_reader),
        "94" => new PnL(_reader),
        "95" => new PnLSingle(_reader),
        "96" => new HistoricalTicks(_reader),
        "97" => new HistoricalBidAskTicks(_reader),
        "98" => new HistoricalLastTicks(_reader),
        "99" => TickByTick.Create(_reader),
        "100" => new OrderBound(_reader),
        "101" => new CompletedOrder(_reader),
        "102" => new CompletedOrdersEnd(),
        "103" => new ReplaceFAEnd(_reader),
        "104" => new WshMetaData(_reader),
        "105" => new WshEventDataReceived(_reader),
        "106" => new HistoricalSchedule(_reader),
        "107" => new UserInfo(_reader),
        _ => throw new InvalidDataException($"Undefined code '{code}'.")
    };
}
